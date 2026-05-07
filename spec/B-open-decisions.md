## Appendix B. Open Decisions

### B.1 Purpose

Appendix B records unresolved decisions, future candidates, and items that require more evidence. Items in this appendix do not change implemented behavior until they move into a numbered specification document or Appendix A.

### B.2 Open Items

#### B.2.1 Test Identifier Backfill

Existing tests do not yet include adjacent comments with Appendix A identifiers. A later cleanup adds identifiers to test methods without changing runtime behavior.

#### B.2.2 Startup Failure Reporting

The exact user-facing reporting path for startup failures is not yet specified. The decision covers message boxes, log files, process exit behavior, and preservation of previous user script state.

#### B.2.3 Interactive Hook Validation

Default CI avoids real global hooks. The project still needs a documented opt-in path for interactive hook validation on a developer machine or dedicated Windows test environment.

#### B.2.4 Release Documentation Text

The unsigned GitHub release zip needs clear release-note wording for direct-download users. The wording needs to explain that the zip is unsigned while Store packages are signed by Microsoft after submission.

#### B.2.5 Performance Thresholds

Performance diagnostics currently produce measurement evidence without hard thresholds. Thresholds require more baseline data across multiple machines and runner conditions.

#### B.2.6 Store Submission Checklist

The Store upload workflow generates unsigned upload artifacts. A complete human release checklist for Partner Center submission remains separate from the workflow.
