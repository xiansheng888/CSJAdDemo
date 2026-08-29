# Unity Roslyn Gateway

## 目录说明
- `gateway_server.py`: 外部常驻网关服务（FastAPI）
- `ai_gateway_client.py`: AIAgent调用脚本，支持 `status` 与 `do-code`
- `models.py`: 网关协议模型
- `state_store.py`: 网关会话与任务状态管理

## 启动网关
```bash
cd Tools/UnityRoslynGateway
python3 -m pip install -r requirements.txt
python3 gateway_server.py
```

默认监听地址：`http://127.0.0.1:19090`

## AIAgent调用
```bash
python3 Tools/UnityRoslynGateway/ai_gateway_client.py status
python3 Tools/UnityRoslynGateway/ai_gateway_client.py do-code --code "<UTF-8代码字符串>"
```

包含引号或多行代码时，建议优先使用 `--code-file` 或 `--code-stdin`，避免 shell 转义导致代码内容变化。

## Unity Editor 控制窗口
- 菜单路径：`Tools/Unity Roslyn Gateway/Control Window`
- 支持能力：
  - 启动网关（先检测 `fastapi/uvicorn/pydantic`，缺失时自动安装依赖）
  - 停止网关（优先请求网关优雅关闭，再结束本地记录进程）
  - 实时查看网关 HTTP 状态、Unity Agent 连接状态、Session、心跳时间
  - Trusted Mode 开关（开启后执行使用 `EnsureLoad`，可调用 `UnityEditor` API）
- 可配置项：
  - Python 可执行路径（默认 `python3`，可在窗口保存）

## 环境变量
- `UNITY_ROSLYN_GATEWAY_URL`: Python CLI 默认网关地址
- `UNITY_ROSLYN_GATEWAY_HOST`: 网关监听地址（默认 `127.0.0.1`）
- `UNITY_ROSLYN_GATEWAY_PORT`: 网关监听端口（默认 `19090`）
- `UNITY_ROSLYN_GATEWAY_LOG_LEVEL`: 网关日志级别（默认 `warning`）
- `UNITY_ROSLYN_GATEWAY_ACCESS_LOG`: 是否打印每个HTTP请求访问日志（默认 `0`，关闭）
- `UNITY_ROSLYN_GATEWAY_PYTHON`: Unity Editor 控制窗口默认 Python 可执行路径（未在窗口保存时生效）
