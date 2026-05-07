# Gesture Performance Repeatability Summary

No hard pass/fail timing threshold is applied. This Step 06 run repeats the attribution measurement to separate raw input timing, watcher observation wait, and run-to-run variance.

- Started UTC: 2026-05-06T15:22:59.5618051+00:00
- Started JST: 2026-05-07T00:22:59.5618051+09:00
- Warmup iterations per run: 2
- Measured iterations per run: 5
- Repeat runs: 3
- Scenario count: 336
- Events per measured iteration: 2620
- Total measured events: 39300
- Machine: 9TH
- OS: Microsoft Windows NT 10.0.26200.0
- CLR: 4.0.30319.42000
- Process architecture: x64
- Build configuration: Release

## Per-Run Attribution

| Run | Samples | Events | Total median ms | Total p95 ms | Total max ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Total >=1 ms | Input >=1 ms | Wait >=1 ms |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 0 | 1680 | 13100 | 0.023 | 0.069 | 14.876 | 0.013 | 0.037 | 1.887 | 0.008 | 0.035 | 14.786 | 28 | 5 | 22 |
| 1 | 1680 | 13100 | 0.024 | 0.075 | 14.692 | 0.012 | 0.042 | 1.732 | 0.008 | 0.038 | 14.663 | 28 | 6 | 22 |
| 2 | 1680 | 13100 | 0.022 | 0.07 | 13.662 | 0.012 | 0.042 | 2.174 | 0.007 | 0.035 | 13.628 | 26 | 5 | 21 |

## Run-to-Run Range

| Component | Runs | Run median min ms | Run median max ms | Run median range ms | Run p95 min ms | Run p95 max ms | Run p95 range ms | Run max min ms | Run max max ms | Run max range ms | >=1 ms total |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| total_replay | 3 | 0.022 | 0.024 | 0.001 | 0.069 | 0.075 | 0.005 | 13.662 | 14.876 | 1.214 | 82 |
| raw_input | 3 | 0.012 | 0.013 | 0.001 | 0.037 | 0.042 | 0.005 | 1.732 | 2.174 | 0.442 | 16 |
| observation_wait | 3 | 0.007 | 0.008 | 0.001 | 0.035 | 0.038 | 0.003 | 13.628 | 14.786 | 1.157 | 65 |
| validation | 3 | 0 | 0 | 0 | 0 | 0 | 0 | 0.024 | 0.045 | 0.021 | 0 |

## Slowest Raw Input and Watcher Wait Samples

| Component | Run | Iteration | Group | Scenario | Component ms | Total ms | Input ms | Wait ms | Press input ms | Move input ms | Release input ms |
| --- | ---: | ---: | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| observation_wait | 0 | 3 | generated-positive | positive-R-start--800-360-distance-32-density-2-jitter-3 | 14.786 | 14.876 | 0.089 | 14.786 | 0.004 | 0 | 0.085 |
| observation_wait | 1 | 3 | generated-positive | positive-R-start-2600-540-distance-72-density-4-jitter-3 | 14.663 | 14.692 | 0.027 | 14.663 | 0.008 | 0.001 | 0.019 |
| observation_wait | 1 | 3 | generated-positive | positive-D-start-2600-540-distance-48-density-2-jitter-3 | 14.395 | 14.52 | 0.122 | 14.395 | 0.008 | 0.001 | 0.114 |
| observation_wait | 1 | 0 | generated-positive | positive-DR-start-400-400-distance-32-density-4-jitter-0 | 13.962 | 14.004 | 0.039 | 13.962 | 0.004 | 0.001 | 0.034 |
| observation_wait | 0 | 4 | generated-positive | positive-R-start-2600-540-distance-32-density-4-jitter-0 | 13.843 | 13.863 | 0.019 | 13.843 | 0.004 | 0.001 | 0.014 |
| observation_wait | 0 | 4 | generated-positive | positive-U-start-2600-540-distance-32-density-7-jitter-0 | 13.79 | 13.818 | 0.027 | 13.79 | 0.004 | 0.001 | 0.022 |
| observation_wait | 0 | 2 | generated-positive | positive-D-start-400-400-distance-32-density-4-jitter-0 | 13.691 | 13.723 | 0.031 | 13.691 | 0.006 | 0.001 | 0.025 |
| observation_wait | 2 | 1 | generated-positive | positive-UD-start-2600-540-distance-32-density-4-jitter-0 | 13.628 | 13.662 | 0.033 | 13.628 | 0.004 | 0.001 | 0.027 |
| observation_wait | 2 | 0 | generated-positive | positive-L-start-400-400-distance-72-density-7-jitter-0 | 13.535 | 13.566 | 0.031 | 13.535 | 0.004 | 0.001 | 0.026 |
| observation_wait | 1 | 3 | generated-positive | positive-DR-start-400-400-distance-32-density-7-jitter-3 | 13.484 | 13.531 | 0.046 | 13.484 | 0.005 | 0.002 | 0.04 |
| raw_input | 2 | 4 | generated-positive | positive-R-start--800-360-distance-32-density-7-jitter-3 | 2.174 | 2.204 | 2.174 | 0.028 | 2.163 | 0.002 | 0.009 |
| raw_input | 0 | 1 | generated-positive | positive-UD-start-400-400-distance-32-density-4-jitter-0 | 1.887 | 1.909 | 1.887 | 0.018 | 1.877 | 0.002 | 0.008 |
| raw_input | 1 | 3 | generated-positive | positive-L-start--800-360-distance-72-density-4-jitter-0 | 1.732 | 1.784 | 1.732 | 0.051 | 1.717 | 0.002 | 0.012 |
| raw_input | 0 | 3 | generated-positive | positive-R-start--800-360-distance-48-density-2-jitter-0 | 1.705 | 1.72 | 1.705 | 0.011 | 1.691 | 0.003 | 0.012 |
| raw_input | 1 | 2 | generated-positive | positive-U-start--800-360-distance-72-density-4-jitter-3 | 1.7 | 1.727 | 1.7 | 0.026 | 1.69 | 0.002 | 0.008 |
| raw_input | 2 | 0 | generated-positive | positive-UD-start-2600-540-distance-32-density-4-jitter-3 | 1.674 | 1.704 | 1.674 | 0.027 | 1.657 | 0.003 | 0.014 |
| raw_input | 0 | 1 | wheel | wheel-WheelUp | 1.576 | 1.576 | 1.576 | 0 | 1.559 | 0 | 0.003 |
| raw_input | 2 | 2 | generated-positive | positive-UD-start-400-400-distance-48-density-2-jitter-3 | 1.483 | 1.499 | 1.483 | 0.015 | 1.473 | 0.002 | 0.009 |
| raw_input | 1 | 1 | generated-positive | positive-R-start-400-400-distance-48-density-4-jitter-0 | 1.463 | 1.469 | 1.463 | 0.004 | 1.455 | 0.002 | 0.006 |
| raw_input | 2 | 0 | generated-positive | positive-U-start-400-400-distance-72-density-7-jitter-3 | 1.335 | 1.346 | 1.335 | 0.008 | 1.321 | 0.003 | 0.011 |
