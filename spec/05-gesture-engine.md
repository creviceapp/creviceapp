## 5. Gesture Engine

### 5.1 Input Model

The gesture engine processes discrete input tokens. Tokens represent mouse buttons, keyboard keys, mouse wheel directions, and stroke directions.

The engine tracks currently active trigger paths and advances gesture candidates as input tokens arrive.

Wheel gesture paths and stroke gesture paths stay independent. Wheel input does not execute stroke handlers. Stroke input does not execute wheel handlers.

### 5.2 Stroke Recognition

Stroke recognition converts pointer movement into directional stroke tokens. Direction detection uses movement thresholds to distinguish intentional strokes from small pointer noise.

Below-threshold movement does not establish a stroke. A release after below-threshold movement cancels the stroke path and does not execute a stroke handler.

Generated positive stroke scenarios cover direction, start coordinate, distance, point density, and jitter variation. Generated negative stroke scenarios cover below-threshold movement, unknown stroke sequences, and stable noisy non-matches.

### 5.3 Sequence Matching

A configured gesture sequence matches only in the configured token order.

Partial matches remain candidates until the sequence completes, a token invalidates the candidate, or the gesture path is canceled.

Unknown stroke sequences consume the active trigger release path without executing a handler.

False context predicates prevent executor invocation for the matching gesture branch.

### 5.4 Executor Invocation

An executor runs exactly once for each completed matching gesture.

A replay that executes one handler does not execute extra handlers. The recorder-based tests assert both per-label counts and total execution counts.

### 5.5 Scheduler and Point Processing

`PointProcessor` processes pointer samples and dispatches stroke observation work. A positive watch interval starts background processing. A zero or negative watch interval keeps processing synchronous and does not start a background task.

`PointProcessor.Dispose` is idempotent. Repeated disposal does not cancel or dispose the same background resources multiple times.

### 5.6 Coverage Contract

The default gesture inventory and generated scenario matrix remain covered by deterministic tests. New default gestures add corresponding fixture entries and replay coverage.
