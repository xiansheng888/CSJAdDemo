---
name: unity-unit-test
description: Write, update, refactor, and run Unity Test Framework tests (NUnit, EditMode, PlayMode, UnityTest, async Task). Use this skill when requests involve Unity unit tests, coroutine tests, edge-case coverage, flaky test diagnosis, or execution strategy when UnityEditor is already running.
---

# Unity Unit Test Authoring

## Goal
- Produce maintainable and reproducible Unity tests with strong edge-case coverage.
- Prefer `[Test]` by default. Use `[UnityTest]` only when frame stepping or special yield instructions are required.
- Do not add demo/example-only code unless explicitly requested.

## Workflow
1. Decide test mode.
- Pure logic without frame progression: EditMode + `[Test]`.
- MonoBehaviour lifecycle, physics, coroutine, frame progression: PlayMode + `[UnityTest]`.
- Any `UnityEditor` API dependency: EditMode with Editor-only asmdef.

2. Decide run strategy (critical).
- Run `scripts/detect_unity_editor.ps1 -ProjectPath <UnityProjectPath>` first.
- If `editorRunningForProject = true`:
- Do not start another `Unity.exe -batchmode -runTests` for the same project.
- Run tests in the currently open Editor (Test Runner window or `TestRunnerApi`).
- For automation in this repo, prefer `UnityProject/Tools/unity_eval.py` and call `TestRunnerApi.Execute(...)`.
- If `editorRunningForProject = false`:
- Command-line test run is allowed.

3. Place tests in the correct assemblies.
- Keep test scripts in the same folder tree as the target test asmdef.
- EditMode asmdef: `includePlatforms: ["Editor"]`.
- PlayMode asmdef: include `optionalUnityReferences: ["TestAssemblies"]` and reference tested assemblies.
- For this repository:
- Unity API and Editor tests belong under `UnityProject/Assets/HotFix` related test folders.
- `UnityProject/Assets/HotFixBattle` is deterministic logic. Do not add Unity API usage there.

4. Write focused tests.
- Use `Arrange-Act-Assert`.
- Test names must describe condition, behavior, and expected result.
- Keep one core behavior per test.
- Remove randomness or lock random seed.

5. Cover edge cases (minimum baseline).
- Input edges: `null`, empty string, empty collection, min, max, out-of-range, invalid enum value, duplicate input.
- State edges: first call, repeated call, uninitialized state, disposed object, state-transition boundaries.
- Timing edges: same-frame vs next-frame, `Update` vs `FixedUpdate`, `deltaTime = 0`, timeout path.
- Failure edges: expected exception path, unexpected exception guard, error/warning log path.
- Async edges: cancellation, timeout, task failure, main-thread blocking risk.
- Platform edges: add `[UnityPlatform]` or `[ConditionalIgnore]` only when needed.

6. Handle logs and failures explicitly.
- For strict log checking, use `[TestMustExpectAllLogs]` or `LogAssert.NoUnexpectedReceived()`.
- For async exception assertions, avoid `Assert.ThrowsAsync` in Unity editor context. Prefer `try/catch` + explicit assertion.

7. Execute and verify.
- In-editor run: Test Runner window or `TestRunnerApi` with `Filter` (`assemblyNames`, `testNames`, `testMode`).
- CLI run (when same project is not already open in Editor):
- `Unity.exe -runTests -batchmode -projectPath <path> -testPlatform EditMode|PlayMode -testResults <xml>`
- Use `-runSynchronously` only for EditMode. It filters out multi-frame tests (`UnityTest`, `UnitySetUp`, `UnityTearDown`).

## Templates
### Standard unit test
```csharp
[Test]
public void Method_WhenCondition_ShouldExpectedResult()
{
    // Arrange
    // Act
    // Assert
}
```

### Coroutine or frame-based test
```csharp
[UnityTest]
public IEnumerator Method_WhenNextFrame_ShouldExpectedResult()
{
    // Arrange
    yield return null;
    // Assert
}
```

### Async test without editor freeze risk
```csharp
[Test]
public async Task Method_WhenAsyncFails_ShouldThrow()
{
    var caught = false;
    try
    {
        await SomeAsync();
    }
    catch (ExpectedException)
    {
        caught = true;
    }

    Assert.That(caught, Is.True);
}
```

## References
- Framework rules: `references/unity-test-framework-rules.md`
- Edge-case checklist: `references/boundary-checklist.md`
