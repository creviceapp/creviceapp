## 1. Foundations and Writing Rules

### 1.1 Scope

This specification defines the implementation contract for Crevice4. The contract covers the desktop application, the reusable gesture core library, tests, build automation, packaging, and release artifacts.

The audience is maintainers, implementers, reviewers, test authors, and release operators. Product documentation and README files describe user-facing usage. They do not override this specification.

### 1.2 Writing Rules

Specification text uses direct declarative language.

Examples:

- `The CI runs gesture coverage tests.`
- `Performance diagnostics run only when CREVICE_RUN_PERFORMANCE_HARNESS=1 is set.`
- `GitHub releases publish an unsigned zip artifact for direct download.`

The following styles are excluded from the numbered specification documents:

- RFC-style keywords such as `MUST`, `SHOULD`, `MAY`, `REQUIRED`, and `RECOMMENDED`.
- Ambiguous requirement phrasing such as `should`, `may`, `can`, `where practical`, `when possible`, and `in principle`.
- Proposals, preferences, and future ideas that have not been accepted as implementation work.

Unresolved decisions and future candidates are recorded in Appendix B. A future candidate becomes part of the implementation contract only after it moves into a numbered specification document or Appendix A.

### 1.3 Document Responsibilities

Each requirement appears in one primary document. Other documents refer to that primary document instead of restating the same rule.

Primary ownership:

- Product-visible behavior belongs in Section 3.
- Runtime structure belongs in Section 4.
- Gesture matching and stroke processing rules belong in Section 5.
- User script format, script execution, and configuration belong in Section 6.
- Windows API boundaries belong in Section 7.
- UI lifetime belongs in Section 8.
- Packaging, signing, and artifact shape belong in Section 9.
- Version resolution and version consistency belong in Section 10.
- GitHub Actions build and test execution belongs in Section 11.
- Diagnostic measurement behavior belongs in Section 12.
- Test coverage identifiers belong in Appendix A.
- Unresolved decisions belong in Appendix B.

### 1.4 Vocabulary

`Crevice4` is the product name. `CreviceApp` is the Windows desktop application project. `CreviceLib` is the reusable core library project. `CreviceAppPackage` is the Windows Application Packaging Project used for Store upload package generation.

Core terms:

- Gesture: A configured input pattern that triggers an executor.
- Stroke: A directional mouse movement token such as `MoveUp`, `MoveDown`, `MoveLeft`, or `MoveRight`.
- Stroke sequence: An ordered sequence of stroke tokens.
- Wheel gesture: A gesture triggered by a wheel token while a modifier gesture input is active.
- Gesture machine: The runtime state machine that evaluates input events against configured gestures.
- Gesture candidate: A partially matched gesture path that remains eligible for execution.
- User script: A C# script file that defines gesture bindings and executors.
- Default user script: `CreviceApp/Scripts/DefaultUserScript.csx`.
- Synthetic replay: A deterministic test path that feeds generated input events into the gesture machine without installing global Windows hooks.
- Diagnostic performance harness: An opt-in test harness that records timing data and writes measurement artifacts.
- Default CI: The GitHub Actions CI workflow without opt-in diagnostic or interactive test environment variables.

### 1.5 Source Boundaries

The numbered specification documents define product and engineering behavior. Appendix A defines stable test coverage identifiers. Appendix B defines unresolved decisions.

The `poc/` directory records experiments and supporting analysis. POC reports provide evidence, not standing product requirements.

The GitHub Actions workflows define the operational build and release commands. When workflows and this specification diverge, the divergence is resolved by updating the implementation, the workflow, or this specification in the same change set.

### 1.6 File and Identifier Style

Specification files use ASCII text. Code identifiers, paths, environment variables, Windows constants, and test identifiers use monospace formatting.

Section references use the form `Section X.Y` and appendix references use the form `Appendix A.2`.

Test identifiers use the format defined in Appendix A. Existing identifiers remain stable after publication.
