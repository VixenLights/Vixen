---
name: commit-msg
description: |
  Generate professional commit messages from unstaged or staged changes following Chris Beams' seven rules.
  Use when the user requests a commit message or when completing an
  ExecPlan milestone that changes repository files. Triggers include:
  - English: "create commit message", "generate commit message", "write commit"
  - Context: User has staged changes (git add) and needs a commit message
---

# Commit Message Generator

## Overview

Generate professional commit messages from staged or unstaged changes following Chris Beams' seven rules for commit messages.

## ⚠️ Critical Execution Rules

**NEVER cd to skill folder.** Always execute scripts from user's current working directory to preserve git repository context.

## Mandatory ExecPlan Milestone Trigger

Use this skill whenever completing an ExecPlan milestone that changes
repository files, even when the user did not explicitly request a commit
message. Inspect only the files changed by that milestone, including tracked
diffs and untracked files. Follow Step 1 and Step 2 to determine and inspect
the scope. Do not create a commit unless the user explicitly requests one.

## Required Milestone Completion Output

The final response for a completed milestone that changed repository files MUST
end with a `Commit message` section containing a complete, paste-ready message:
a subject, blank line, and wrapped explanatory body. A subject line alone is
not sufficient.

## Related To

A related to section should be added when the JIRA issue being worked on is related to another JIRA issue. The Related To
refers to the other issue(s). Do not add a related to section that relates to the same issue being committed.

## Workflow

### Step 1: Determine Commit Scope

Choose the scope before analyzing changes.

- **ExecPlan milestone completion:** inspect only files changed for that
  milestone. Do not include unrelated working-tree changes.

- **User-requested commit message:** inspect the user’s intended commit scope.
  Prefer staged changes when any are staged. If nothing is staged, inspect all
  current tracked and untracked changes. If staged and unstaged changes appear
  unrelated, ask the user which set to describe.

Do not stage, modify, or discard files while generating a commit message.

### Step 2: Inspect the Selected Scope

Use `git status --short` first.

- For staged scope: `git diff --cached`
- For all current changes: `git diff HEAD`, plus direct inspection of
  untracked files shown by `git status --short`
- For milestone scope: inspect only the milestone’s changed file list,
  including untracked files

### Step 2a: Analyze the Diff

Read the actual code changes to understand:
- What changed (high-level)
- Why this change matters
- Appropriate imperative verb (Add/Fix/Refactor/Update/etc.)

### Step 2b:

The Subject line must start with the JIRA issue. Ex VIX-XXXX This is typically the branch name.

### Step 3: Generate Commit Message

Follow Chris Beams' seven rules:

1. **Separate subject from body with blank line**
2. **Limit subject line to 50 characters**
3. **Capitalize the subject line**
4. **Do not end the subject line with a period**
5. **Use imperative mood** ("Fix bug" not "Fixed bug")
6. **Wrap body at 72 characters**
7. **Explain what and why, not how**

**Format**:
```
Subject line (50 chars max, imperative, capitalized, no period)

Body paragraph explaining WHY this change was needed and WHAT
it accomplishes (not HOW). Wrap at 72 characters per line.

Can include multiple paragraphs, bullet points, or issue refs.

Related to VIX-XXXX
```

### Step 4: Present to User in format to be pasted into commit message

```
VIX-XXXX Prevent token expiration race condition

Expired tokens were accepted during brief window between
expiry and cache invalidation, allowing unauthorized access.

Add expiry validation before processing requests and
implement immediate cache invalidation on token expiry.
```

## Reference

See `docs\references\commit-guide.md` for detailed explanation of the seven rules with examples.

## Examples

**Simple change**:
```
VIX-XXXX Add user email validation

Email addresses were accepted without format validation,
causing database errors when invalid emails were stored.

Add regex validation before saving to ensure valid format.
```

**Bug fix**:
```
VIX-XXXX Fix race condition in token refresh

Background token refresh could override user-initiated
refresh, causing authentication failures.

Add mutex lock around refresh operations to prevent
concurrent modifications.

Related to VIX-XXXX
```

**Feature addition**:
```
VIX-XXXX Add dark mode toggle to settings

Users requested ability to switch between light and dark
themes without changing system preferences.

Implement theme toggle in settings page with localStorage
persistence across sessions.

Depends on VIX-XXXX
```

