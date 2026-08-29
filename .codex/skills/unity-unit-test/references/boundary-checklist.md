# Unity Test Edge Case Checklist

How to use: list candidate edge cases by category, then keep only cases that match the requested behavior.

## Input Boundaries
- `null` input object.
- Empty string, empty collection, default struct.
- Minimum, maximum, and zero values.
- Out-of-range values.
- Invalid enum values.
- Duplicate input and idempotency behavior.

## State Boundaries
- Call before initialization.
- First call vs repeated call behavior.
- Access after dispose or destroy.
- State-machine transition boundaries.
- Retry after failure.

## Timing Boundaries (Unity-specific)
- Same-frame assert vs next-frame assert.
- `Update` vs `FixedUpdate` boundary.
- `deltaTime = 0` behavior.
- State after `WaitForFixedUpdate`.
- Timeout path for coroutine or async operation.

## Exception and Log Boundaries
- Expected exception type and message key.
- Guard against unexpected exception.
- Expected error, warning, and info logs.
- Strict log mode behavior with `TestMustExpectAllLogs`.

## Async Boundaries
- Success path.
- Cancellation path.
- Timeout path.
- Child task failure aggregation.
- Main-thread blocking risk.

## Platform Boundaries
- Editor vs Player behavior differences.
- Platform-specific behavior with `UnityPlatform`.
- Conditional skip with `ConditionalIgnore` and clear reason.

## Assertion Quality
- Assert behavior, not implementation details.
- Failure message is diagnosable.
- Test is independent and isolated.
- Cleanup is complete (objects, static state, subscriptions).
