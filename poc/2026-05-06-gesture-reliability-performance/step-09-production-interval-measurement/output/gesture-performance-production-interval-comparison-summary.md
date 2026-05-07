# Gesture Performance Scheduler Comparison Summary

Step 09 interval comparison measurement; compares StrokeWatchInterval 0 and production default without hard pass/fail timing thresholds. Interval > 0 replay uses production-paced point delivery to preserve semantic validation.

Step: 09-production-interval-measurement. This run compares deterministic attribution scenarios across replay scheduler and StrokeWatchInterval modes.

- Started UTC: 2026-05-06T16:11:35.4974045+00:00
- Started JST: 2026-05-07T01:11:35.4974045+09:00
- Warmup iterations per run: 0
- Measured iterations per run: 1
- Repeat runs per scheduler: 1
- Scenario count: 336
- Events per measured iteration: 2620
- Total measured events: 13100
- Machine: 9TH
- OS: Microsoft Windows NT 10.0.26200.0
- CLR: 4.0.30319.42000
- Process architecture: x64
- Build configuration: Release

## Scheduler Mode Summary

| Role | Stroke interval ms | Scheduler mode | Runs | Samples | Events | Total median ms | Total p95 ms | Total max ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Total >=1 ms | Input >=1 ms | Wait >=1 ms |
| --- | ---: | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| primary | 0 | interval-0-task-factory-default | 1 | 336 | 2620 | 0.015 | 0.07 | 29.694 | 0.011 | 0.036 | 29.654 | 0.003 | 0.029 | 15.255 | 7 | 3 | 4 |
| primary | 0 | interval-0-production-like-low-latency | 1 | 336 | 2620 | 0.031 | 0.163 | 16.381 | 0.007 | 0.055 | 0.318 | 0.022 | 0.069 | 16.241 | 12 | 0 | 12 |
| primary | 10 | interval-10-task-factory-default | 1 | 336 | 2620 | 63.14 | 218.087 | 373.031 | 0.243 | 0.407 | 2.356 | 0.015 | 0.038 | 0.493 | 334 | 2 | 0 |
| primary | 10 | interval-10-production-like-low-latency | 1 | 336 | 2620 | 63.044 | 218.484 | 375.447 | 0.225 | 0.396 | 1.393 | 0.015 | 0.034 | 0.064 | 334 | 1 | 0 |
| exploratory | 10 | interval-10-production-like-low-latency-pool4 | 1 | 336 | 2620 | 63.324 | 218.054 | 371.941 | 0.243 | 0.431 | 1.718 | 0.015 | 0.042 | 0.21 | 334 | 1 | 0 |

## Per-Run Attribution

| Role | Stroke interval ms | Scheduler mode | Run | Samples | Events | Total median ms | Total p95 ms | Total max ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Total >=1 ms | Input >=1 ms | Wait >=1 ms |
| --- | ---: | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| primary | 0 | interval-0-task-factory-default | 0 | 336 | 2620 | 0.015 | 0.07 | 29.694 | 0.011 | 0.036 | 29.654 | 0.003 | 0.029 | 15.255 | 7 | 3 | 4 |
| primary | 0 | interval-0-production-like-low-latency | 0 | 336 | 2620 | 0.031 | 0.163 | 16.381 | 0.007 | 0.055 | 0.318 | 0.022 | 0.069 | 16.241 | 12 | 0 | 12 |
| primary | 10 | interval-10-task-factory-default | 0 | 336 | 2620 | 63.14 | 218.087 | 373.031 | 0.243 | 0.407 | 2.356 | 0.015 | 0.038 | 0.493 | 334 | 2 | 0 |
| primary | 10 | interval-10-production-like-low-latency | 0 | 336 | 2620 | 63.044 | 218.484 | 375.447 | 0.225 | 0.396 | 1.393 | 0.015 | 0.034 | 0.064 | 334 | 1 | 0 |
| exploratory | 10 | interval-10-production-like-low-latency-pool4 | 0 | 336 | 2620 | 63.324 | 218.054 | 371.941 | 0.243 | 0.431 | 1.718 | 0.015 | 0.042 | 0.21 | 334 | 1 | 0 |

## Component Comparison

| Component | Scheduler mode | Median ms | P95 ms | Max ms | Mean ms | >=1 ms count |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| observation_wait | interval-0-production-like-low-latency | 0.022 | 0.069 | 16.241 | 0.498 | 12 |
| observation_wait | interval-0-task-factory-default | 0.003 | 0.029 | 15.255 | 0.118 | 4 |
| observation_wait | interval-10-production-like-low-latency | 0.015 | 0.034 | 0.064 | 0.018 | 0 |
| observation_wait | interval-10-production-like-low-latency-pool4 | 0.015 | 0.042 | 0.21 | 0.019 | 0 |
| observation_wait | interval-10-task-factory-default | 0.015 | 0.038 | 0.493 | 0.02 | 0 |
| raw_input | interval-0-production-like-low-latency | 0.007 | 0.055 | 0.318 | 0.014 | 0 |
| raw_input | interval-0-task-factory-default | 0.011 | 0.036 | 29.654 | 0.115 | 3 |
| raw_input | interval-10-production-like-low-latency | 0.225 | 0.396 | 1.393 | 0.239 | 1 |
| raw_input | interval-10-production-like-low-latency-pool4 | 0.243 | 0.431 | 1.718 | 0.262 | 1 |
| raw_input | interval-10-task-factory-default | 0.243 | 0.407 | 2.356 | 0.268 | 2 |
| total_replay | interval-0-production-like-low-latency | 0.031 | 0.163 | 16.381 | 0.513 | 12 |
| total_replay | interval-0-task-factory-default | 0.015 | 0.07 | 29.694 | 0.242 | 7 |
| total_replay | interval-10-production-like-low-latency | 63.044 | 218.484 | 375.447 | 90.233 | 334 |
| total_replay | interval-10-production-like-low-latency-pool4 | 63.324 | 218.054 | 371.941 | 90.299 | 334 |
| total_replay | interval-10-task-factory-default | 63.14 | 218.087 | 373.031 | 90.216 | 334 |

## Slowest Raw Input and Watcher Wait Samples

| Scheduler mode | Component | Run | Iteration | Group | Scenario | Component ms | Total ms | Input ms | Wait ms | Press input ms | Move input ms | Release input ms |
| --- | --- | ---: | ---: | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| interval-0-production-like-low-latency | observation_wait | 0 | 0 | single-stroke | baseline-D | 16.241 | 16.381 | 0.132 | 16.241 | 0.061 | 0.001 | 0.071 |
| interval-0-production-like-low-latency | observation_wait | 0 | 0 | generated-positive | positive-UD-start-400-400-distance-48-density-7-jitter-0 | 15.839 | 15.931 | 0.087 | 15.839 | 0.008 | 0.002 | 0.078 |
| interval-0-production-like-low-latency | observation_wait | 0 | 0 | generated-positive | positive-UD-start-2600-540-distance-32-density-2-jitter-3 | 15.792 | 15.86 | 0.065 | 15.792 | 0.007 | 0.001 | 0.058 |
| interval-0-production-like-low-latency | observation_wait | 0 | 0 | generated-positive | positive-U-start-400-400-distance-32-density-7-jitter-3 | 15.765 | 15.818 | 0.051 | 15.765 | 0.008 | 0.001 | 0.042 |
| interval-0-production-like-low-latency | observation_wait | 0 | 0 | generated-positive | positive-UD-start-400-400-distance-72-density-7-jitter-0 | 15.746 | 15.806 | 0.057 | 15.746 | 0.01 | 0.001 | 0.045 |
| interval-0-production-like-low-latency | raw_input | 0 | 0 | generated-positive | positive-UD-start-400-400-distance-48-density-2-jitter-0 | 0.318 | 0.327 | 0.318 | 0.007 | 0.308 | 0.003 | 0.007 |
| interval-0-production-like-low-latency | raw_input | 0 | 0 | generated-positive | positive-UD-start-400-400-distance-72-density-4-jitter-3 | 0.139 | 0.213 | 0.139 | 0.068 | 0.131 | 0.003 | 0.005 |
| interval-0-production-like-low-latency | raw_input | 0 | 0 | single-stroke | baseline-D | 0.132 | 16.381 | 0.132 | 16.241 | 0.061 | 0.001 | 0.071 |
| interval-0-production-like-low-latency | raw_input | 0 | 0 | generated-positive | positive-UD-start-2600-540-distance-32-density-4-jitter-0 | 0.105 | 0.173 | 0.105 | 0.061 | 0.087 | 0.003 | 0.016 |
| interval-0-production-like-low-latency | raw_input | 0 | 0 | generated-positive | positive-U-start-400-400-distance-32-density-7-jitter-0 | 0.099 | 0.158 | 0.099 | 0.056 | 0.095 | 0.002 | 0.003 |
| interval-0-task-factory-default | observation_wait | 0 | 0 | generated-positive | positive-U-start-400-400-distance-48-density-7-jitter-3 | 15.255 | 15.296 | 0.039 | 15.255 | 0.007 | 0.001 | 0.032 |
| interval-0-task-factory-default | observation_wait | 0 | 0 | generated-positive | positive-U-start--800-360-distance-32-density-4-jitter-0 | 15.007 | 15.063 | 0.054 | 15.007 | 0.009 | 0.001 | 0.045 |
| interval-0-task-factory-default | observation_wait | 0 | 0 | generated-positive | positive-U-start-400-400-distance-48-density-2-jitter-3 | 3.715 | 3.757 | 0.04 | 3.715 | 0.013 | 0.001 | 0.027 |
| interval-0-task-factory-default | observation_wait | 0 | 0 | single-stroke | baseline-U | 2.777 | 7.344 | 1.901 | 2.777 | 0.661 | 0.112 | 1.127 |
| interval-0-task-factory-default | observation_wait | 0 | 0 | generated-negative | baseline-below-threshold | 0.814 | 1.42 | 0.604 | 0.814 | 0.004 | 0 | 0.599 |
| interval-0-task-factory-default | raw_input | 0 | 0 | wheel | wheel-WheelUp | 29.654 | 29.694 | 29.654 | 0 | 22.704 | 0 | 3.085 |
| interval-0-task-factory-default | raw_input | 0 | 0 | single-stroke | baseline-U | 1.901 | 7.344 | 1.901 | 2.777 | 0.661 | 0.112 | 1.127 |
| interval-0-task-factory-default | raw_input | 0 | 0 | generated-positive | positive-L-start-2600-540-distance-32-density-4-jitter-0 | 1.805 | 1.809 | 1.805 | 0.002 | 0.006 | 0.001 | 1.798 |
| interval-0-task-factory-default | raw_input | 0 | 0 | generated-negative | baseline-below-threshold | 0.604 | 1.42 | 0.604 | 0.814 | 0.004 | 0 | 0.599 |
| interval-0-task-factory-default | raw_input | 0 | 0 | wheel | wheel-WheelDown | 0.164 | 0.164 | 0.164 | 0 | 0.069 | 0 | 0.012 |
| interval-10-production-like-low-latency | observation_wait | 0 | 0 | generated-positive | positive-DR-start-2600-540-distance-32-density-2-jitter-3 | 0.064 | 62.414 | 0.248 | 0.064 | 0.069 | 0.041 | 0.138 |
| interval-10-production-like-low-latency | observation_wait | 0 | 0 | generated-positive | positive-L-start-2600-540-distance-48-density-4-jitter-3 | 0.055 | 63.329 | 0.377 | 0.055 | 0.211 | 0.038 | 0.129 |
| interval-10-production-like-low-latency | observation_wait | 0 | 0 | generated-positive | positive-DR-start-400-400-distance-32-density-4-jitter-0 | 0.053 | 127.035 | 0.307 | 0.053 | 0.085 | 0.109 | 0.113 |
| interval-10-production-like-low-latency | observation_wait | 0 | 0 | generated-positive | positive-DR-start-400-400-distance-48-density-7-jitter-3 | 0.051 | 220.49 | 0.406 | 0.051 | 0.076 | 0.167 | 0.163 |
| interval-10-production-like-low-latency | observation_wait | 0 | 0 | generated-positive | positive-R-start--800-360-distance-48-density-2-jitter-3 | 0.049 | 29.986 | 0.186 | 0.049 | 0.081 | 0.004 | 0.102 |
| interval-10-production-like-low-latency | raw_input | 0 | 0 | generated-positive | positive-L-start-2600-540-distance-32-density-4-jitter-3 | 1.393 | 60.999 | 1.393 | 0.015 | 1.305 | 0.029 | 0.059 |
| interval-10-production-like-low-latency | raw_input | 0 | 0 | generated-negative | negative-noisy-URDL | 0.537 | 375.447 | 0.537 | 0.031 | 0.081 | 0.371 | 0.085 |
| interval-10-production-like-low-latency | raw_input | 0 | 0 | generated-positive | positive-UD-start--800-360-distance-48-density-7-jitter-0 | 0.453 | 217.863 | 0.453 | 0.03 | 0.163 | 0.229 | 0.061 |
| interval-10-production-like-low-latency | raw_input | 0 | 0 | generated-positive | positive-UD-start-400-400-distance-48-density-7-jitter-0 | 0.437 | 219.574 | 0.437 | 0.014 | 0.149 | 0.226 | 0.062 |
| interval-10-production-like-low-latency | raw_input | 0 | 0 | generated-positive | positive-DR-start--800-360-distance-32-density-7-jitter-0 | 0.435 | 218.632 | 0.435 | 0.015 | 0.222 | 0.156 | 0.057 |
| interval-10-production-like-low-latency-pool4 | observation_wait | 0 | 0 | generated-positive | positive-DR-start--800-360-distance-48-density-7-jitter-3 | 0.21 | 219.24 | 0.637 | 0.21 | 0.101 | 0.209 | 0.327 |
| interval-10-production-like-low-latency-pool4 | observation_wait | 0 | 0 | generated-positive | positive-DR-start-2600-540-distance-32-density-4-jitter-0 | 0.068 | 125.637 | 0.28 | 0.068 | 0.058 | 0.096 | 0.126 |
| interval-10-production-like-low-latency-pool4 | observation_wait | 0 | 0 | generated-positive | positive-DR-start--800-360-distance-32-density-4-jitter-0 | 0.067 | 123.223 | 0.396 | 0.067 | 0.115 | 0.156 | 0.125 |
| interval-10-production-like-low-latency-pool4 | observation_wait | 0 | 0 | generated-positive | positive-DR-start-2600-540-distance-48-density-7-jitter-3 | 0.062 | 218.169 | 0.545 | 0.062 | 0.158 | 0.187 | 0.199 |
| interval-10-production-like-low-latency-pool4 | observation_wait | 0 | 0 | generated-positive | positive-D-start-400-400-distance-32-density-4-jitter-0 | 0.06 | 62.797 | 0.302 | 0.06 | 0.057 | 0.06 | 0.185 |
| interval-10-production-like-low-latency-pool4 | raw_input | 0 | 0 | generated-positive | positive-UD-start-400-400-distance-48-density-4-jitter-0 | 1.718 | 123.53 | 1.718 | 0.05 | 1.472 | 0.111 | 0.135 |
| interval-10-production-like-low-latency-pool4 | raw_input | 0 | 0 | generated-positive | positive-DR-start--800-360-distance-48-density-7-jitter-3 | 0.637 | 219.24 | 0.637 | 0.21 | 0.101 | 0.209 | 0.327 |
| interval-10-production-like-low-latency-pool4 | raw_input | 0 | 0 | generated-negative | negative-noisy-URDL | 0.597 | 371.941 | 0.597 | 0.015 | 0.093 | 0.441 | 0.062 |
| interval-10-production-like-low-latency-pool4 | raw_input | 0 | 0 | generated-positive | positive-DR-start--800-360-distance-72-density-2-jitter-0 | 0.574 | 62.795 | 0.574 | 0.014 | 0.47 | 0.042 | 0.062 |
| interval-10-production-like-low-latency-pool4 | raw_input | 0 | 0 | generated-positive | positive-DR-start-2600-540-distance-48-density-7-jitter-3 | 0.545 | 218.169 | 0.545 | 0.062 | 0.158 | 0.187 | 0.199 |
| interval-10-task-factory-default | observation_wait | 0 | 0 | generated-negative | baseline-below-threshold | 0.493 | 32.039 | 0.157 | 0.493 | 0.07 | 0.032 | 0.055 |
| interval-10-task-factory-default | observation_wait | 0 | 0 | generated-positive | positive-DR-start-2600-540-distance-32-density-4-jitter-3 | 0.111 | 122.227 | 0.26 | 0.111 | 0.095 | 0.106 | 0.059 |
| interval-10-task-factory-default | observation_wait | 0 | 0 | generated-positive | positive-UD-start-2600-540-distance-48-density-7-jitter-0 | 0.075 | 215.355 | 0.404 | 0.075 | 0.071 | 0.215 | 0.118 |
| interval-10-task-factory-default | observation_wait | 0 | 0 | generated-positive | positive-DR-start-2600-540-distance-48-density-2-jitter-3 | 0.057 | 63.206 | 0.222 | 0.057 | 0.076 | 0.036 | 0.11 |
| interval-10-task-factory-default | observation_wait | 0 | 0 | generated-positive | positive-L-start--800-360-distance-48-density-7-jitter-0 | 0.057 | 112.893 | 0.371 | 0.057 | 0.073 | 0.101 | 0.197 |
| interval-10-task-factory-default | raw_input | 0 | 0 | generated-positive | positive-U-start--800-360-distance-72-density-2-jitter-0 | 2.356 | 31.967 | 2.356 | 0.035 | 2.227 | 0.011 | 0.118 |
| interval-10-task-factory-default | raw_input | 0 | 0 | generated-positive | positive-DR-start-400-400-distance-72-density-7-jitter-3 | 1.383 | 216.847 | 1.383 | 0.048 | 1.058 | 0.176 | 0.149 |
| interval-10-task-factory-default | raw_input | 0 | 0 | generated-positive | positive-DR-start-400-400-distance-48-density-7-jitter-3 | 0.789 | 215.726 | 0.789 | 0.056 | 0.131 | 0.156 | 0.503 |
| interval-10-task-factory-default | raw_input | 0 | 0 | generated-negative | negative-noisy-URDL | 0.689 | 373.031 | 0.689 | 0.026 | 0.082 | 0.528 | 0.08 |
| interval-10-task-factory-default | raw_input | 0 | 0 | generated-positive | positive-DR-start-400-400-distance-72-density-7-jitter-0 | 0.567 | 216.025 | 0.567 | 0.043 | 0.283 | 0.187 | 0.098 |
