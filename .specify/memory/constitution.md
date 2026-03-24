<!--
Sync Impact Report
- Version change: template-draft -> 1.0.0
- Modified principles:
	- Principle 1 (template placeholder) -> I. Spec-Driven Delivery
	- Principle 2 (template placeholder) -> II. Data Minimization and Privacy by Default
	- Principle 3 (template placeholder) -> III. Test-First Quality Gates
	- Principle 4 (template placeholder) -> IV. Observability and Explainability
	- Principle 5 (template placeholder) -> V. Simplicity, Compatibility, and Reversibility
- Added sections:
	- Engineering Standards
	- Workflow and Quality Controls
- Removed sections:
	- None
- Templates requiring updates:
	- .specify/templates/plan-template.md ✅ updated
	- .specify/templates/spec-template.md ✅ updated
	- .specify/templates/tasks-template.md ✅ updated
	- .github/agents/speckit.tasks.agent.md ✅ updated
	- .specify/templates/commands/*.md ⚠ pending (directory not present)
	- .github/prompts/speckit.constitution.prompt.md ✅ reviewed (no update required)
- Follow-up TODOs:
	- None
-->

# POC-Addressfinder Constitution

## Core Principles

### I. Spec-Driven Delivery
Every change MUST start from an explicit feature specification with independent user
stories, measurable acceptance criteria, and stated edge cases before implementation
planning begins. Plans and tasks MUST trace directly to the approved specification.
Rationale: This preserves delivery focus, reduces rework, and enables predictable
incremental releases.

### II. Data Minimization and Privacy by Default
The system MUST collect, process, and store only the minimum location/address data
required to fulfill each user story. Sensitive data fields MUST be classified in the
specification, and retention handling MUST be defined before implementation. Logs,
telemetry, and test fixtures MUST NOT expose real personal data.
Rationale: Address and location data is sensitive and requires explicit handling.

### III. Test-First Quality Gates
For every user story, tests that validate expected behavior MUST be defined and MUST
fail before production code is considered complete. The delivery pipeline MUST include
unit tests for core logic and integration tests for external dependencies or data
contracts that the story touches.
Rationale: Failing-first verification prevents regressions and unclear requirements.

### IV. Observability and Explainability
New or changed workflows MUST emit structured diagnostics that allow operators to
understand input source, match decisions, error reasons, and latency without exposing
sensitive user data. Any user-visible failure path MUST define actionable error
messages and corresponding troubleshooting signals.
Rationale: Address-matching outcomes must be auditable and support rapid debugging.

### V. Simplicity, Compatibility, and Reversibility
Designs MUST choose the least complex approach that satisfies current requirements.
Backward-incompatible behavior or interface changes MUST include migration guidance,
rollback steps, and explicit approval in planning artifacts before implementation.
Rationale: Small, reversible changes reduce operational risk in a POC codebase.

## Engineering Standards

- Specifications MUST include explicit non-functional requirements for privacy,
	performance targets, and failure handling when relevant to a story.
- Plans MUST document dependency choices and justify any new external service or
	package added to the project.
- Tasks MUST include evidence-producing work items for tests, observability updates,
	and user-facing documentation updates for each deliverable story.

## Workflow and Quality Controls

1. Specification: Create or update spec with priorities, edge cases, and measurable
	 outcomes.
2. Planning: Produce a plan that passes the Constitution Check gates.
3. Tasks: Generate story-grouped tasks with required testing and observability work.
4. Implementation: Deliver in priority order, validating each story independently.
5. Review: Verify constitutional compliance in reviews before merge.

## Governance

This constitution overrides conflicting local workflow habits and templates.
Amendments require:
1. A written rationale and impact summary in `.specify/memory/constitution.md`.
2. Updates to affected templates in `.specify/templates/` in the same change.
3. Semantic version updates using this policy:
	 - MAJOR: Removes or redefines principles/governance in a backward-incompatible way.
	 - MINOR: Adds a principle/section or materially expands mandatory guidance.
	 - PATCH: Clarifies wording without changing obligations.
4. Compliance review in planning and pull request review, with unresolved violations
	 tracked as blockers.

**Version**: 1.0.0 | **Ratified**: 2026-03-23 | **Last Amended**: 2026-03-23
