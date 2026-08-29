# Unity Test Framework Rules

## 1. EditMode vs PlayMode
- EditMode runs inside Unity Editor and can access UnityEditor code.
- PlayMode can run in Editor or Player. `[UnityTest]` runs as coroutine.
- Prefer `[Test]` unless frame stepping or yield instructions are required.

## 2. Core Attributes
- `[UnityTest]`: allows `yield return null`, `WaitForFixedUpdate`, `WaitForSeconds`, and other yield instructions.
- `[UnitySetUp]` / `[UnityTearDown]`: `IEnumerator` setup and teardown for multi-frame prep/cleanup.
- `[UnityPlatform]`: include or exclude runtime platforms.
- `[ConditionalIgnore]`: conditionally skip tests via mapping id.
- `[TestMustExpectAllLogs]`: strict log mode; warning and info logs fail unless expected.

## 3. Async Task Tests
- `async Task` test methods are supported.
- Unity evaluates completion on update loops.
- Avoid `Assert.ThrowsAsync` in Unity editor context. Use `try/catch` with explicit assert.

## 4. Run Methods
- CLI:
- `Unity.exe -runTests -batchmode -projectPath <Path> -testPlatform EditMode|PlayMode -testResults <xml>`
- `-runSynchronously` is EditMode-only and removes multi-frame tests (`UnityTest`, `UnitySetUp`, `UnityTearDown`).
- Programmatic run: `TestRunnerApi.Execute(new ExecutionSettings(filter))`.

## 5. Filter Behavior
- Multiple fields in one filter are AND logic.
- Multiple filters passed to `ExecutionSettings` are OR logic.
- Common fields: `testMode`, `assemblyNames`, `testNames`.

## 6. If UnityEditor Is Already Running
- For the same project, do not start another `-batchmode` test run.
- Run inside the open Editor using Test Runner UI or `TestRunnerApi`.
- If available, use `UnityProject/Tools/unity_eval.py` to execute C# in the open Editor.

## 7. Stability Notes
- For float and vector assertions, use equality comparers (for example `FloatEqualityComparer`, `Vector2EqualityComparer`).
- Keep waiting time minimal to reduce flaky tests.
- Lock random seed for deterministic behavior.
- Re-register callbacks after domain reload.
