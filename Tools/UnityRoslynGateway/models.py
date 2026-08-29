from __future__ import annotations

from typing import Literal, Optional

from pydantic import BaseModel, Field


GatewayStateLiteral = Literal["Ready", "Busy", "Offline"]
DoCodeStateLiteral = Literal[
    "Success",
    "CompileError",
    "SecurityCheck",
    "RuntimeError",
    "Timeout",
    "Offline",
    "BusyRejected",
    "ClientError",
]


class StatusResponse(BaseModel):
    state: GatewayStateLiteral
    detail: str = ""
    unitySessionId: Optional[str] = None
    lastHeartbeatUtc: Optional[str] = None


class DoCodeRequest(BaseModel):
    code: str
    timeoutSec: int = Field(default=30, ge=1, le=300)
    requestId: Optional[str] = None


class DiagnosticEntry(BaseModel):
    severity: Literal["Info", "Warning", "Error"]
    code: str = ""
    message: str = ""
    line: int = 0
    column: int = 0


class CompileInfo(BaseModel):
    usedMode: Literal["fullSource", "wrappedSnippet", "none"] = "none"
    diagnostics: list[DiagnosticEntry] = Field(default_factory=list)


class ResultInfo(BaseModel):
    resultJson: Optional[str] = None
    resultText: Optional[str] = None
    resultType: Optional[str] = None


class ErrorInfo(BaseModel):
    type: Optional[str] = None
    message: Optional[str] = None
    stackTrace: Optional[str] = None


class TimingInfo(BaseModel):
    queue: int = 0
    compile: int = 0
    execute: int = 0
    total: int = 0


class DoCodeResponse(BaseModel):
    requestId: str
    success: bool
    state: DoCodeStateLiteral
    result: ResultInfo = Field(default_factory=ResultInfo)
    compile: CompileInfo = Field(default_factory=CompileInfo)
    error: ErrorInfo = Field(default_factory=ErrorInfo)
    timingMs: TimingInfo = Field(default_factory=TimingInfo)


class AgentRegisterRequest(BaseModel):
    agentName: str = "unity-editor"


class AgentRegisterResponse(BaseModel):
    accepted: bool
    sessionId: Optional[str] = None
    heartbeatIntervalSec: float = 1.0
    pollIntervalSec: float = 0.2
    message: str = ""


class AgentHeartbeatRequest(BaseModel):
    sessionId: str
    state: Literal["Ready", "Busy", "Reloading"]
    detail: str = ""


class AgentHeartbeatResponse(BaseModel):
    accepted: bool
    message: str = ""


class GatewayTaskPayload(BaseModel):
    requestId: str
    code: str
    timeoutSec: int
    createdAtUtc: str


class AgentPullTaskRequest(BaseModel):
    sessionId: str
    maxWaitMs: int = Field(default=200, ge=10, le=5000)


class AgentPullTaskResponse(BaseModel):
    accepted: bool
    hasTask: bool = False
    task: Optional[GatewayTaskPayload] = None
    message: str = ""


class AgentPushResultRequest(BaseModel):
    sessionId: str
    requestId: str
    result: DoCodeResponse


class AgentPushResultResponse(BaseModel):
    accepted: bool
    message: str = ""
