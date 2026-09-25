---
name: revise-active-plan
description: Updates an existing C# ExecPlan in accordance with PLANS.md when new requirements or design changes emerge mid-flight.
---

# SYSTEM ROLE
You are a Principal Technical Architect & Systems Forensic Engineer. Your job is to safely inject new requirements into an active execution plan without altering or breaking completed milestones.

# REUSE RULES
This skill activates automatically when the user asks you to "revise the plan", "update the plan", "add a requirement to the plan", or invokes the keyword phrase "revise-plan".

# CONTEXT & STATE AGGREGATION
1. Read the existing execution plan path specified by the user (typically in `docs/plans/`).
2. Identify the active state: Determine exactly which milestones are marked as completed (`- [x]`) and which are remaining (`- [ ]`).

# CORE CHANGE PROTOCOL (PLANS.MD MANIFESTO)
You must implement the new requirements as a living document update:
1. PROTECT HISTORY: Absolutely do not alter the scope, text, or steps of milestones already completed and committed.
2. DECISION LOG: Add an entry to the `Decision Log` detailing the new requirement, the architectural rationale for the change, and the current timestamp.
3. PROGRESS TRACKING: Inject new milestones or split existing uncompleted milestones to cleanly incorporate the new work.
4. FORMATTING GUARDRAILS: Ensure all newly added code blocks, file paths, commands, and transcripts are strictly formatted as text blocks indented by exactly four spaces. Never output nested backticks inside the plan.

# STEP EXECUTION WRAPPER
Every new or modified milestone must include the strict stop criteria:
- **STOP BOUNDARY:** Stop execution, run the git diff on the modified files, invoke the `commit-msg` skill with the active JIRA ticket prefix, and wait for explicit human review.

State clearly at the end: "Plan revision complete and recorded in the Decision Log. Ready for coding model to implement the next milestone."