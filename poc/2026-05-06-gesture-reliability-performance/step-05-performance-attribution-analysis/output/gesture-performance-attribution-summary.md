# Gesture Performance Attribution Summary

No hard pass/fail timing threshold is applied. This run separates replay input calls, stroke-watcher observation waits, observed-stroke reads, and validation.

- Started UTC: 2026-05-06T15:10:34.1415505+00:00
- Started JST: 2026-05-07T00:10:34.1415505+09:00
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

| Scope | Group | Scenarios | Samples | Events | Total median ms | Total p95 ms | Total max ms | Total mean ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Validation median ms | Validation p95 ms | Validation max ms |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| overall | all | 336 | 1680 | 13100 | 0.023 | 0.062 | 14.028 | 0.191 | 0.013 | 0.037 | 1.996 | 0.008 | 0.028 | 13.981 | 0.002 | 0.004 | 0.048 |
| group | generated-negative | 4 | 20 | 250 | 0.027 | 0.051 | 0.947 | 0.076 | 0.015 | 0.035 | 0.929 | 0.008 | 0.02 | 0.034 | 0 | 0.002 | 0.003 |
| group | generated-positive | 324 | 1620 | 12600 | 0.023 | 0.062 | 14.028 | 0.189 | 0.013 | 0.037 | 1.996 | 0.008 | 0.027 | 13.981 | 0.002 | 0.004 | 0.048 |
| group | multi-stroke | 2 | 10 | 100 | 0.022 | 11.944 | 11.944 | 1.22 | 0.011 | 0.044 | 0.044 | 0.012 | 11.922 | 11.922 | 0 | 0.002 | 0.002 |
| group | single-stroke | 4 | 20 | 120 | 0.024 | 0.051 | 0.059 | 0.028 | 0.012 | 0.034 | 0.04 | 0.013 | 0.024 | 0.047 | 0 | 0.001 | 0.002 |
| group | wheel | 2 | 10 | 30 | 0.028 | 0.173 | 0.173 | 0.045 | 0.028 | 0.172 | 0.172 | 0 | 0 | 0 | 0 | 0.005 | 0.005 |
