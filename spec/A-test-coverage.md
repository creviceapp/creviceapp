## Appendix A. Test Coverage Checklist

### A.1 Purpose

Appendix A defines stable test coverage identifiers for Crevice4. Each identifier maps a specification requirement to unit, integration, diagnostic, or manual verification.

Identifiers remain stable after publication. New coverage items receive new identifiers.

### A.2 Identifier Format

The identifier format is `CREV-<S><F><M>-<n>`.

Scope code `<S>`:

- `C`: Core library.
- `A`: Application layer.
- `P`: Packaging and release.
- `D`: Diagnostics and performance.
- `M`: Manual validation.

Family code `<F>`:

- `G`: Gesture recognition.
- `S`: Stroke processing.
- `U`: User scripting.
- `W`: Windows integration.
- `L`: Lifecycle and UI.
- `B`: Build and packaging.
- `V`: Versioning.
- `R`: Resource cleanup and failure behavior.
- `P`: Performance.

Method code `<M>`:

- `U`: Unit test.
- `I`: Integration test.
- `D`: Diagnostic test.
- `M`: Manual validation.

The counter `<n>` starts at 1 for each `<S><F><M>` group and increases in publication order.

Status values:

- `covered`: automated tests already cover the item.
- `partial`: automated tests cover part of the item.
- `planned`: the item is specified and not yet covered by automated tests.
- `manual`: the item is verified manually.

### A.3 Core Library Coverage

#### A.3.G Gesture Recognition

- `CREV-CGU-1` - Gesture machine executes a matching gesture exactly once.
  - Status: `covered`.
  - Refs: Section 3.2, Section 5.4.
- `CREV-CGU-2` - False predicates prevent executor invocation.
  - Status: `covered`.
  - Refs: Section 3.4.
- `CREV-CGU-3` - Wheel and stroke gesture paths do not cross-trigger.
  - Status: `covered`.
  - Refs: Section 3.2.

#### A.3.S Stroke Processing

- `CREV-CSU-1` - Below-threshold movement does not establish a stroke.
  - Status: `covered`.
  - Refs: Section 5.2.
- `CREV-CSU-2` - Unknown stroke sequences do not execute handlers.
  - Status: `covered`.
  - Refs: Section 5.3.
- `CREV-CSU-3` - Zero or negative `PointProcessor` watch intervals do not start background processing.
  - Status: `covered`.
  - Refs: Section 5.5.
- `CREV-CSU-4` - `PointProcessor.Dispose` is idempotent.
  - Status: `covered`.
  - Refs: Section 5.5.

### A.4 Application Coverage

#### A.4.G Gesture Replay

- `CREV-AGU-1` - Default user script gesture definitions are fully enumerated.
  - Status: `covered`.
  - Refs: Section 3.3, Section 5.6.
- `CREV-AGU-2` - Default browser wheel gestures replay through synthetic input.
  - Status: `covered`.
  - Refs: Section 3.3.
- `CREV-AGU-3` - Default browser stroke gestures replay through synthetic input.
  - Status: `covered`.
  - Refs: Section 3.3.
- `CREV-AGU-4` - Generated positive stroke patterns replay through synthetic input.
  - Status: `covered`.
  - Refs: Section 5.2.
- `CREV-AGU-5` - Generated negative stroke patterns do not execute handlers.
  - Status: `covered`.
  - Refs: Section 5.2, Section 5.3.

#### A.4.U User Scripting

- `CREV-AUU-1` - Default user script loads through the application script host.
  - Status: `covered`.
  - Refs: Section 6.1.
- `CREV-AUU-2` - Script DSL composition produces gesture definitions consumed by the gesture machine.
  - Status: `covered`.
  - Refs: Section 6.2.

#### A.4.W Windows Integration

- `CREV-AWU-1` - Hook wrappers preserve activation and disposal state.
  - Status: `covered`.
  - Refs: Section 7.1.
- `CREV-AWU-2` - `SendInput` wrapper emits ordered key down and key up sequences.
  - Status: `covered`.
  - Refs: Section 7.2.
- `CREV-AWI-1` - Opt-in manual input integration smoke tests validate low-level keyboard and mouse hooks with signature-filtered non-text keyboard input and mouse input.
  - Status: `covered`.
  - Refs: Section 7.1, Section 11.5.
- `CREV-AWU-3` - Window context helpers expose foreground and pointed window metadata.
  - Status: `planned`.
  - Refs: Section 7.3.

#### A.4.L Lifecycle and UI

- `CREV-ALU-1` - Reloadable main form replaces script-backed gesture configuration without process restart.
  - Status: `partial`.
  - Refs: Section 6.4, Section 8.2.
- `CREV-ALU-2` - UI error reporting stays outside hook callbacks.
  - Status: `planned`.
  - Refs: Section 8.5.

### A.5 Packaging and CI Coverage

#### A.5.B Build and Packaging

- `CREV-PBI-1` - CI restores and builds `CreviceLib`, `CreviceLibTests`, and `CreviceAppTests`.
  - Status: `covered`.
  - Refs: Section 11.4.
- `CREV-PBI-2` - Store packaging verifies Windows SDK and packaging targets.
  - Status: `covered`.
  - Refs: Section 9.2, Section 11.3.
- `CREV-PBI-3` - Store upload package generation disables local package signing.
  - Status: `covered`.
  - Refs: Section 9.2.
- `CREV-PBI-4` - Release zip generation creates an unsigned zip and `SHA256SUMS.txt`.
  - Status: `covered`.
  - Refs: Section 9.3.

#### A.5.V Versioning

- `CREV-PVI-1` - Workflows resolve and apply a four-part Crevice version.
  - Status: `covered`.
  - Refs: Section 10.1, Section 10.2.
- `CREV-PVI-2` - Tag-triggered release zip workflow creates or updates a GitHub Release.
  - Status: `covered`.
  - Refs: Section 10.3.

### A.6 Diagnostics Coverage

#### A.6.P Performance Diagnostics

- `CREV-DPD-1` - Diagnostic performance tests require `CREVICE_RUN_PERFORMANCE_HARNESS=1`.
  - Status: `covered`.
  - Refs: Section 12.1.
- `CREV-DPD-2` - Diagnostic output uses `CREVICE_PERF_OUTPUT_DIR` when configured.
  - Status: `covered`.
  - Refs: Section 12.2.
- `CREV-DPD-3` - Repeatability and scheduler comparisons record multiple run summaries.
  - Status: `covered`.
  - Refs: Section 12.3, Section 12.4.

### A.7 Manual Validation

- `CREV-MLM-1` - Manual validation confirms normal desktop startup and shutdown.
  - Status: `manual`.
  - Refs: Section 3.1, Section 8.2.
- `CREV-MGM-1` - Manual validation confirms default browser gestures in supported browser contexts.
  - Status: `manual`.
  - Refs: Section 3.3.
- `CREV-MBM-1` - Manual validation confirms Store upload and release zip artifacts on a release candidate.
  - Status: `manual`.
  - Refs: Section 9.2, Section 9.3.
