## 12. Diagnostics and Performance

### 12.1 Diagnostic Harness Boundary

Diagnostic performance tests live in `CreviceAppTests/GesturePerformanceHarnessTests.cs`.

The diagnostic performance harness runs only when `CREVICE_RUN_PERFORMANCE_HARNESS=1` is set. Without that environment variable, diagnostic tests report an inconclusive result before measurement and before artifact writing.

### 12.2 Output Directory

Diagnostic performance output goes to `CREVICE_PERF_OUTPUT_DIR` when it is set. The harness owns output creation below that configured directory.

The harness does not hard-code POC output directories.

### 12.3 Measurement Model

Performance measurements account for run-to-run variability. Diagnostic results are evidence for analysis, not default CI pass/fail thresholds.

Repeatability runs record multiple summaries so variance is visible. Scheduler comparison runs record separate summaries for each scheduler mode.

### 12.4 Supported Diagnostic Families

The harness records:

- Synthetic gesture replay baseline.
- Synthetic gesture replay attribution.
- Synthetic gesture replay repeatability.
- Synthetic gesture replay scheduler comparison.
- PointProcessor zero-interval guard comparison.
- Production interval comparison.

### 12.5 Tuning Contract

Performance tuning changes preserve gesture correctness. Correctness tests run before or with performance comparison work.

Timing improvements are accepted only with supporting measurement artifacts and no regression in deterministic gesture replay tests.

### 12.6 POC Evidence

The `poc/2026-05-06-gesture-reliability-performance/` directory is an evidence archive for the first reliability and performance investigation.

The reusable POC harness protocol lives in `poc/README.md`, `poc/protocol.md`, `poc/prompts.md`, and `poc/template/`.
