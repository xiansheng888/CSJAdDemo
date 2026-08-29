from __future__ import annotations

import asyncio
import time
import uuid
from dataclasses import dataclass
from datetime import datetime, timezone
from typing import Optional

from models import (
    AgentPullTaskResponse,
    DoCodeResponse,
    GatewayTaskPayload,
    StatusResponse,
)


def utc_now_iso() -> str:
    return datetime.now(timezone.utc).isoformat()


@dataclass
class AgentSession:
    session_id: str
    state: str
    detail: str
    last_heartbeat_monotonic: float
    last_heartbeat_utc: str


@dataclass
class QueueTicket:
    payload: GatewayTaskPayload
    future: asyncio.Future
    enqueued_monotonic: float
    timed_out: bool = False


class GatewayState:
    def __init__(self, offline_timeout_sec: float = 3.0, max_queue_size: int = 1) -> None:
        self._offline_timeout_sec = offline_timeout_sec
        self._queue: asyncio.Queue[QueueTicket] = asyncio.Queue(maxsize=max_queue_size)
        self._pending: dict[str, QueueTicket] = {}
        self._tickets: dict[str, QueueTicket] = {}
        self._agent: Optional[AgentSession] = None
        self._lock = asyncio.Lock()

    def _is_offline_unlocked(self) -> bool:
        if self._agent is None:
            return True
        elapsed = time.monotonic() - self._agent.last_heartbeat_monotonic
        return elapsed > self._offline_timeout_sec

    async def get_status(self) -> StatusResponse:
        async with self._lock:
            if self._is_offline_unlocked():
                return StatusResponse(
                    state="Offline",
                    detail="Unity agent is not connected",
                    unitySessionId=None if self._agent is None else self._agent.session_id,
                    lastHeartbeatUtc=None if self._agent is None else self._agent.last_heartbeat_utc,
                )

            if self._agent is None:
                return StatusResponse(state="Offline", detail="Unity agent is not connected")

            if self._agent.state != "Ready" or self._pending or not self._queue.empty():
                return StatusResponse(
                    state="Busy",
                    detail=self._agent.detail or "Unity agent is busy",
                    unitySessionId=self._agent.session_id,
                    lastHeartbeatUtc=self._agent.last_heartbeat_utc,
                )

            return StatusResponse(
                state="Ready",
                detail=self._agent.detail or "Unity agent is ready",
                unitySessionId=self._agent.session_id,
                lastHeartbeatUtc=self._agent.last_heartbeat_utc,
            )

    async def register_agent(self) -> str:
        async with self._lock:
            session_id = str(uuid.uuid4())
            self._agent = AgentSession(
                session_id=session_id,
                state="Ready",
                detail="Registered",
                last_heartbeat_monotonic=time.monotonic(),
                last_heartbeat_utc=utc_now_iso(),
            )

            while not self._queue.empty():
                try:
                    queued = self._queue.get_nowait()
                    if not queued.future.done():
                        queued.future.set_exception(RuntimeError("Unity agent re-registered"))
                except asyncio.QueueEmpty:
                    break

            self._pending.clear()
            self._tickets.clear()
            return session_id

    async def heartbeat(self, session_id: str, state: str, detail: str) -> bool:
        async with self._lock:
            if self._agent is None or self._agent.session_id != session_id:
                return False

            self._agent.state = state
            self._agent.detail = detail
            self._agent.last_heartbeat_monotonic = time.monotonic()
            self._agent.last_heartbeat_utc = utc_now_iso()
            return True

    async def enqueue_do_code(
        self,
        request_id: str,
        code: str,
        timeout_sec: int,
    ) -> tuple[bool, str, str, Optional[QueueTicket]]:
        loop = asyncio.get_running_loop()

        async with self._lock:
            if self._is_offline_unlocked():
                return False, "Offline", "Unity agent is offline", None

            if self._queue.full():
                return False, "BusyRejected", "Gateway queue is full", None

            ticket = QueueTicket(
                payload=GatewayTaskPayload(
                    requestId=request_id,
                    code=code,
                    timeoutSec=timeout_sec,
                    createdAtUtc=utc_now_iso(),
                ),
                future=loop.create_future(),
                enqueued_monotonic=time.monotonic(),
            )

            self._tickets[request_id] = ticket
            self._queue.put_nowait(ticket)
            return True, "", "", ticket

    async def pull_task(self, session_id: str, max_wait_ms: int) -> AgentPullTaskResponse:
        async with self._lock:
            if self._agent is None or self._agent.session_id != session_id:
                return AgentPullTaskResponse(accepted=False, message="Invalid session")

        timeout_sec = max_wait_ms / 1000.0
        end_time = time.monotonic() + timeout_sec

        while True:
            remaining = end_time - time.monotonic()
            if remaining <= 0:
                return AgentPullTaskResponse(accepted=True, hasTask=False)

            try:
                ticket = await asyncio.wait_for(self._queue.get(), timeout=remaining)
            except asyncio.TimeoutError:
                return AgentPullTaskResponse(accepted=True, hasTask=False)

            if ticket.timed_out:
                async with self._lock:
                    self._tickets.pop(ticket.payload.requestId, None)
                continue

            async with self._lock:
                if self._agent is None or self._agent.session_id != session_id:
                    return AgentPullTaskResponse(accepted=False, message="Invalid session")

                self._pending[ticket.payload.requestId] = ticket
                self._agent.state = "Busy"
                self._agent.detail = f"Executing {ticket.payload.requestId}"

            return AgentPullTaskResponse(accepted=True, hasTask=True, task=ticket.payload)

    async def push_result(self, session_id: str, result: DoCodeResponse) -> bool:
        async with self._lock:
            if self._agent is None or self._agent.session_id != session_id:
                return False

            ticket = self._pending.pop(result.requestId, None)
            self._tickets.pop(result.requestId, None)

            if ticket is None:
                return False

            self._agent.state = "Ready"
            self._agent.detail = "Idle"

        if not ticket.future.done() and not ticket.timed_out:
            ticket.future.set_result(result)
        return True

    async def mark_timeout(self, request_id: str) -> None:
        async with self._lock:
            ticket = self._tickets.get(request_id)
            if ticket is None:
                return
            ticket.timed_out = True
