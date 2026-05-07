# Gesture Performance Scheduler Comparison Summary

No hard pass/fail timing threshold is applied. This Step 07 run compares the default test replay scheduler with a production-like low-latency replay scheduler using the same deterministic attribution scenarios.

- Started UTC: 2026-05-06T15:36:55.5478128+00:00
- Started JST: 2026-05-07T00:36:55.5478128+09:00
- Warmup iterations per run: 2
- Measured iterations per run: 5
- Repeat runs per scheduler: 3
- Scenario count: 336
- Events per measured iteration: 2620
- Total measured events: 78600
- Machine: 9TH
- OS: Microsoft Windows NT 10.0.26200.0
- CLR: 4.0.30319.42000
- Process architecture: x64
- Build configuration: Release

## Scheduler Mode Summary

| Scheduler mode | Runs | Samples | Events | Total median ms | Total p95 ms | Total max ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Total >=1 ms | Input >=1 ms | Wait >=1 ms |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| task-factory-default | 3 | 5040 | 39300 | 0.024 | 0.059 | 14.789 | 0.014 | 0.033 | 1.68 | 0.008 | 0.029 | 14.712 | 75 | 7 | 68 |
| production-like-low-latency | 3 | 5040 | 39300 | 30.964 | 32.063 | 33.135 | 0.141 | 0.274 | 1.549 | 30.813 | 31.918 | 32.852 | 5006 | 6 | 5005 |

## Per-Run Attribution

| Scheduler mode | Run | Samples | Events | Total median ms | Total p95 ms | Total max ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Total >=1 ms | Input >=1 ms | Wait >=1 ms |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| task-factory-default | 0 | 1680 | 13100 | 0.024 | 0.062 | 14.789 | 0.014 | 0.034 | 1.68 | 0.008 | 0.037 | 14.712 | 25 | 3 | 22 |
| task-factory-default | 1 | 1680 | 13100 | 0.026 | 0.06 | 14.115 | 0.015 | 0.034 | 1.57 | 0.008 | 0.027 | 14.059 | 26 | 2 | 24 |
| task-factory-default | 2 | 1680 | 13100 | 0.022 | 0.054 | 14.222 | 0.012 | 0.029 | 1.501 | 0.007 | 0.025 | 14.183 | 24 | 2 | 22 |
| production-like-low-latency | 0 | 1680 | 13100 | 30.969 | 32.066 | 32.637 | 0.139 | 0.26 | 1.256 | 30.824 | 31.918 | 32.385 | 1669 | 1 | 1669 |
| production-like-low-latency | 1 | 1680 | 13100 | 30.963 | 32.062 | 32.866 | 0.145 | 0.289 | 1.549 | 30.802 | 31.924 | 32.573 | 1669 | 3 | 1669 |
| production-like-low-latency | 2 | 1680 | 13100 | 30.958 | 32.061 | 33.135 | 0.141 | 0.271 | 1.291 | 30.805 | 31.911 | 32.852 | 1668 | 2 | 1667 |

## Component Comparison

| Component | Scheduler mode | Median ms | P95 ms | Max ms | Mean ms | >=1 ms count |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| observation_wait | production-like-low-latency | 30.813 | 31.918 | 32.852 | 27.531 | 5005 |
| observation_wait | task-factory-default | 0.008 | 0.029 | 14.712 | 0.169 | 68 |
| raw_input | production-like-low-latency | 0.141 | 0.274 | 1.549 | 0.156 | 6 |
| raw_input | task-factory-default | 0.014 | 0.033 | 1.68 | 0.019 | 7 |
| total_replay | production-like-low-latency | 30.964 | 32.063 | 33.135 | 27.692 | 5006 |
| total_replay | task-factory-default | 0.024 | 0.059 | 14.789 | 0.189 | 75 |

## Slowest Raw Input and Watcher Wait Samples

| Scheduler mode | Component | Run | Iteration | Group | Scenario | Component ms | Total ms | Input ms | Wait ms | Press input ms | Move input ms | Release input ms |
| --- | --- | ---: | ---: | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| production-like-low-latency | observation_wait | 2 | 0 | generated-positive | positive-D-start-400-400-distance-48-density-7-jitter-0 | 32.852 | 33.135 | 0.275 | 32.852 | 0.191 | 0.004 | 0.08 |
| production-like-low-latency | observation_wait | 2 | 1 | generated-positive | positive-UD-start-2600-540-distance-48-density-7-jitter-0 | 32.646 | 32.858 | 0.207 | 32.646 | 0.143 | 0.003 | 0.062 |
| production-like-low-latency | observation_wait | 1 | 3 | generated-positive | positive-R-start-2600-540-distance-48-density-2-jitter-3 | 32.573 | 32.866 | 0.282 | 32.573 | 0.135 | 0.002 | 0.145 |
| production-like-low-latency | observation_wait | 2 | 1 | generated-positive | positive-R-start--800-360-distance-48-density-2-jitter-0 | 32.445 | 32.618 | 0.168 | 32.445 | 0.101 | 0.003 | 0.064 |
| production-like-low-latency | observation_wait | 2 | 3 | generated-positive | positive-DR-start--800-360-distance-48-density-2-jitter-3 | 32.427 | 32.533 | 0.102 | 32.427 | 0.068 | 0.001 | 0.033 |
| production-like-low-latency | raw_input | 1 | 3 | generated-positive | positive-R-start-2600-540-distance-72-density-7-jitter-0 | 1.549 | 32.057 | 1.549 | 30.482 | 1.468 | 0.003 | 0.077 |
| production-like-low-latency | raw_input | 2 | 4 | generated-positive | positive-DR-start-400-400-distance-48-density-7-jitter-3 | 1.291 | 31.005 | 1.291 | 29.708 | 1.204 | 0.003 | 0.084 |
| production-like-low-latency | raw_input | 1 | 4 | generated-positive | positive-UD-start-2600-540-distance-48-density-4-jitter-0 | 1.27 | 31.926 | 1.27 | 30.651 | 1.202 | 0.003 | 0.065 |
| production-like-low-latency | raw_input | 0 | 0 | generated-positive | positive-R-start--800-360-distance-72-density-4-jitter-0 | 1.256 | 31.305 | 1.256 | 30.041 | 1.199 | 0.003 | 0.054 |
| production-like-low-latency | raw_input | 1 | 0 | generated-positive | positive-D-start-400-400-distance-32-density-7-jitter-0 | 1.145 | 30.898 | 1.145 | 29.748 | 1.059 | 0.002 | 0.083 |
| task-factory-default | observation_wait | 0 | 4 | generated-positive | positive-L-start-2600-540-distance-72-density-7-jitter-3 | 14.712 | 14.789 | 0.073 | 14.712 | 0.015 | 0.004 | 0.055 |
| task-factory-default | observation_wait | 2 | 4 | generated-positive | positive-R-start-2600-540-distance-32-density-2-jitter-3 | 14.183 | 14.222 | 0.037 | 14.183 | 0.006 | 0 | 0.031 |
| task-factory-default | observation_wait | 2 | 4 | generated-positive | positive-D-start--800-360-distance-72-density-2-jitter-0 | 14.16 | 14.191 | 0.029 | 14.16 | 0.005 | 0.001 | 0.023 |
| task-factory-default | observation_wait | 0 | 3 | generated-positive | positive-DR-start-2600-540-distance-72-density-4-jitter-0 | 14.12 | 14.195 | 0.071 | 14.12 | 0.005 | 0.001 | 0.065 |
| task-factory-default | observation_wait | 1 | 4 | generated-positive | positive-L-start-400-400-distance-32-density-4-jitter-0 | 14.059 | 14.115 | 0.054 | 14.059 | 0.007 | 0.001 | 0.046 |
| task-factory-default | raw_input | 0 | 2 | generated-positive | positive-D-start-400-400-distance-72-density-4-jitter-3 | 1.68 | 1.694 | 1.68 | 0.009 | 1.672 | 0.002 | 0.006 |
| task-factory-default | raw_input | 1 | 0 | generated-positive | positive-UD-start--800-360-distance-48-density-7-jitter-0 | 1.57 | 1.582 | 1.57 | 0.011 | 1.559 | 0.005 | 0.007 |
| task-factory-default | raw_input | 2 | 1 | generated-positive | positive-D-start--800-360-distance-32-density-2-jitter-3 | 1.501 | 1.503 | 1.501 | 0.002 | 0.012 | 0 | 1.489 |
| task-factory-default | raw_input | 0 | 2 | generated-positive | positive-DR-start-2600-540-distance-32-density-2-jitter-0 | 1.442 | 1.459 | 1.442 | 0.014 | 1.432 | 0.003 | 0.008 |
| task-factory-default | raw_input | 0 | 0 | generated-positive | positive-D-start-2600-540-distance-72-density-2-jitter-3 | 1.365 | 1.37 | 1.365 | 0.004 | 1.357 | 0.002 | 0.007 |
