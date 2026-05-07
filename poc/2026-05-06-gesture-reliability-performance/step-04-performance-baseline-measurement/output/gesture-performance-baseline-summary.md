# Gesture Performance Baseline Summary

No hard pass/fail timing threshold is applied. These numbers are noisy local measurements for Step 05 analysis.

- Started UTC: 2026-05-06T14:57:54.6839389+00:00
- Started JST: 2026-05-06T23:57:54.6839389+09:00
- Warmup iterations: 2
- Measured iterations: 5
- Scenario count: 336
- Events per measured iteration: 2620
- Total measured events: 13100
- Machine: 9TH
- OS: Microsoft Windows NT 10.0.26200.0
- CLR: 4.0.30319.42000
- Process architecture: x64
- Build configuration: Release

| Scope | Group | Scenarios | Samples | Events | Min ms | Median ms | P95 ms | Max ms | Mean ms |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| overall | all | 336 | 1680 | 13100 | 0.008 | 0.024 | 0.059 | 14.777 | 0.202 |
| group | generated-negative | 4 | 20 | 250 | 0.015 | 0.028 | 0.126 | 13.048 | 0.688 |
| group | generated-positive | 324 | 1620 | 12600 | 0.008 | 0.023 | 0.059 | 14.777 | 0.2 |
| group | multi-stroke | 2 | 10 | 100 | 0.016 | 0.026 | 0.042 | 0.042 | 0.027 |
| group | single-stroke | 4 | 20 | 120 | 0.012 | 0.026 | 0.048 | 0.066 | 0.028 |
| group | wheel | 2 | 10 | 30 | 0.02 | 0.024 | 0.055 | 0.055 | 0.032 |
