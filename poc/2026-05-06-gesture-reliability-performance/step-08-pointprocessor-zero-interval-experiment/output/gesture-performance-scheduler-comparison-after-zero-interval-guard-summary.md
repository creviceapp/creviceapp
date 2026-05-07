# Gesture Performance Scheduler Comparison Summary

Step 08 focused scheduler comparison after the PointProcessor zero-interval guard; no hard pass/fail timing threshold is applied.

Step: 08-pointprocessor-zero-interval-experiment. This run compares the default test replay scheduler with a production-like low-latency replay scheduler using the same deterministic attribution scenarios.

- Started UTC: 2026-05-06T15:50:52.1889637+00:00
- Started JST: 2026-05-07T00:50:52.1889637+09:00
- Warmup iterations per run: 2
- Measured iterations per run: 5
- Repeat runs per scheduler: 1
- Scenario count: 336
- Events per measured iteration: 2620
- Total measured events: 26200
- Machine: 9TH
- OS: Microsoft Windows NT 10.0.26200.0
- CLR: 4.0.30319.42000
- Process architecture: x64
- Build configuration: Release

## Scheduler Mode Summary

| Scheduler mode | Runs | Samples | Events | Total median ms | Total p95 ms | Total max ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Total >=1 ms | Input >=1 ms | Wait >=1 ms |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| task-factory-default | 1 | 1680 | 13100 | 0.012 | 0.026 | 14.311 | 0.009 | 0.016 | 0.771 | 0.002 | 0.013 | 14.105 | 2 | 0 | 2 |
| production-like-low-latency | 1 | 1680 | 13100 | 0.032 | 0.06 | 16.403 | 0.006 | 0.017 | 1.361 | 0.023 | 0.05 | 16.334 | 22 | 1 | 22 |

## Per-Run Attribution

| Scheduler mode | Run | Samples | Events | Total median ms | Total p95 ms | Total max ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Total >=1 ms | Input >=1 ms | Wait >=1 ms |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| task-factory-default | 0 | 1680 | 13100 | 0.012 | 0.026 | 14.311 | 0.009 | 0.016 | 0.771 | 0.002 | 0.013 | 14.105 | 2 | 0 | 2 |
| production-like-low-latency | 0 | 1680 | 13100 | 0.032 | 0.06 | 16.403 | 0.006 | 0.017 | 1.361 | 0.023 | 0.05 | 16.334 | 22 | 1 | 22 |

## Component Comparison

| Component | Scheduler mode | Median ms | P95 ms | Max ms | Mean ms | >=1 ms count |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| observation_wait | production-like-low-latency | 0.023 | 0.05 | 16.334 | 0.181 | 22 |
| observation_wait | task-factory-default | 0.002 | 0.013 | 14.105 | 0.014 | 2 |
| raw_input | production-like-low-latency | 0.006 | 0.017 | 1.361 | 0.011 | 1 |
| raw_input | task-factory-default | 0.009 | 0.016 | 0.771 | 0.012 | 0 |
| total_replay | production-like-low-latency | 0.032 | 0.06 | 16.403 | 0.193 | 22 |
| total_replay | task-factory-default | 0.012 | 0.026 | 14.311 | 0.027 | 2 |

## Slowest Raw Input and Watcher Wait Samples

| Scheduler mode | Component | Run | Iteration | Group | Scenario | Component ms | Total ms | Input ms | Wait ms | Press input ms | Move input ms | Release input ms |
| --- | --- | ---: | ---: | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| production-like-low-latency | observation_wait | 0 | 2 | single-stroke | baseline-D | 16.334 | 16.403 | 0.066 | 16.334 | 0.025 | 0.001 | 0.04 |
| production-like-low-latency | observation_wait | 0 | 4 | multi-stroke | baseline-DR | 16.023 | 16.169 | 0.137 | 16.023 | 0.087 | 0.003 | 0.046 |
| production-like-low-latency | observation_wait | 0 | 4 | single-stroke | baseline-L | 15.869 | 15.919 | 0.048 | 15.869 | 0.01 | 0.001 | 0.038 |
| production-like-low-latency | observation_wait | 0 | 2 | generated-positive | positive-L-start--800-360-distance-72-density-2-jitter-0 | 15.794 | 15.859 | 0.062 | 15.794 | 0.013 | 0.001 | 0.049 |
| production-like-low-latency | observation_wait | 0 | 3 | generated-positive | positive-U-start-2600-540-distance-32-density-7-jitter-3 | 15.772 | 15.821 | 0.046 | 15.772 | 0.009 | 0.001 | 0.036 |
| production-like-low-latency | raw_input | 0 | 3 | generated-positive | positive-U-start-2600-540-distance-32-density-4-jitter-0 | 1.361 | 14.848 | 1.361 | 13.484 | 1.319 | 0.001 | 0.042 |
| production-like-low-latency | raw_input | 0 | 4 | generated-positive | positive-R-start-2600-540-distance-72-density-2-jitter-3 | 0.672 | 0.681 | 0.672 | 0.009 | 0.667 | 0.001 | 0.004 |
| production-like-low-latency | raw_input | 0 | 3 | generated-positive | positive-DR-start-400-400-distance-32-density-7-jitter-3 | 0.618 | 0.628 | 0.618 | 0.009 | 0.596 | 0.018 | 0.004 |
| production-like-low-latency | raw_input | 0 | 2 | generated-positive | positive-L-start-400-400-distance-32-density-2-jitter-0 | 0.608 | 0.618 | 0.608 | 0.01 | 0.606 | 0.001 | 0.002 |
| production-like-low-latency | raw_input | 0 | 0 | generated-positive | positive-DR-start-400-400-distance-48-density-2-jitter-3 | 0.557 | 0.601 | 0.557 | 0.043 | 0.012 | 0.001 | 0.545 |
| task-factory-default | observation_wait | 0 | 0 | generated-positive | positive-R-start-2600-540-distance-32-density-2-jitter-3 | 14.105 | 14.311 | 0.204 | 14.105 | 0.158 | 0.001 | 0.045 |
| task-factory-default | observation_wait | 0 | 0 | generated-positive | positive-R-start-400-400-distance-48-density-2-jitter-3 | 3.652 | 4.021 | 0.365 | 3.652 | 0.323 | 0.001 | 0.041 |
| task-factory-default | observation_wait | 0 | 1 | generated-positive | positive-L-start--800-360-distance-32-density-4-jitter-0 | 0.576 | 0.587 | 0.011 | 0.576 | 0.007 | 0.001 | 0.003 |
| task-factory-default | observation_wait | 0 | 2 | generated-positive | positive-UD-start-2600-540-distance-72-density-7-jitter-3 | 0.057 | 0.074 | 0.016 | 0.057 | 0.006 | 0.002 | 0.008 |
| task-factory-default | observation_wait | 0 | 2 | generated-positive | positive-D-start-400-400-distance-72-density-7-jitter-0 | 0.045 | 0.066 | 0.02 | 0.045 | 0.011 | 0.001 | 0.009 |
| task-factory-default | raw_input | 0 | 0 | generated-positive | positive-U-start-2600-540-distance-72-density-7-jitter-0 | 0.771 | 0.799 | 0.771 | 0.026 | 0.754 | 0.003 | 0.014 |
| task-factory-default | raw_input | 0 | 2 | generated-positive | positive-DR-start-2600-540-distance-32-density-4-jitter-3 | 0.408 | 0.437 | 0.408 | 0.027 | 0.005 | 0.391 | 0.011 |
| task-factory-default | raw_input | 0 | 4 | generated-positive | positive-L-start--800-360-distance-72-density-4-jitter-3 | 0.389 | 0.398 | 0.389 | 0.008 | 0.387 | 0.001 | 0.002 |
| task-factory-default | raw_input | 0 | 3 | generated-positive | positive-UD-start-2600-540-distance-48-density-2-jitter-0 | 0.37 | 0.394 | 0.37 | 0.024 | 0.367 | 0.001 | 0.002 |
| task-factory-default | raw_input | 0 | 0 | generated-positive | positive-R-start-400-400-distance-48-density-2-jitter-3 | 0.365 | 4.021 | 0.365 | 3.652 | 0.323 | 0.001 | 0.041 |
