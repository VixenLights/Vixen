# Git Commit Message Guide

Based on [Chris Beams' seven rules](https://cbea.ms/git-commit/) for writing great commit messages.

---

## The Seven Rules

### 1. Separate subject from body with a blank line

```
Short summary (50 chars max)

Detailed explanation wrapped at 72 characters per line.
Explains why this change was needed and provides context.
```

### 2. Limit subject line to 50 characters

Forces clarity and conciseness. GitHub truncates at 72 characters.

### 3. Capitalize the subject line

- ❌ `add user validation`
- ✅ `Add user validation`

### 4. Do not end subject line with a period

Subject lines are titles, not sentences.

### 5. Use imperative mood in subject line

**Test**: "If applied, this commit will _____"

- ❌ `Fixed bug`, `Fixes bug`, `Fixing bug`
- ✅ `Fix bug`

**Common imperative verbs**:
- Add, Remove, Fix, Refactor, Update, Implement, Optimize, Document, Test, Revert, Merge

### 6. Wrap body at 72 characters

Git doesn't auto-wrap text. Hard-wrap for readability in `git log`.

### 7. Explain *what* and *why*, not *how*

**Include**:
- What changed (high-level summary)
- Why this change was necessary
- Context or background
- Trade-offs or side effects

**Exclude**:
- Line-by-line code explanations (visible in diff)
- Implementation details

---

## Atomic Commits

**Principle**: One commit = one logical change

### Benefits
- Easier code review
- Clearer git history
- Simpler rollback/cherry-pick
- Better git bisect debugging

### How to Create Atomic Commits

Use `git add -p` to stage specific chunks:
```bash
git add -p file.js
# Select hunks: y (yes), n (no), s (split), e (edit)
```

### Examples

❌ **Non-atomic** (multiple unrelated changes):
```
Fix login bug and update README and refactor tests

- Fix null pointer in auth handler
- Update installation docs
- Refactor test suite
- Add TypeScript types
```

✅ **Atomic** (separate logical commits):
```
Commit 1: Fix null pointer exception in auth handler
Commit 2: Update installation documentation
Commit 3: Refactor authentication test suite
Commit 4: Add TypeScript type definitions for auth
```

---

## Subject Line Guidelines

### Strong Imperative Verbs

**New functionality**:
- `Add` - New feature, file, or capability
- `Implement` - Complete feature implementation

**Modifications**:
- `Update` - Modify existing feature (not a fix)
- `Refactor` - Restructure without behavior change
- `Optimize` - Performance improvement

**Fixes**:
- `Fix` - Bug fix
- `Revert` - Undo previous commit

**Non-code**:
- `Document` - Documentation only
- `Test` - Add or modify tests

### Examples

```
Add user authentication middleware
Fix token expiration race condition
Refactor database connection pooling
Update API error handling
Optimize image loading performance
Document deployment process
Test authentication flow
```

---

## Body Structure

### Template

```
[One-line summary of what changed]

[Why this change was needed - motivation/context]

[How it solves the problem - approach taken]

[Side effects or implications - if any]

[Issue references - if applicable]
```

### Example

```
Implement request timeout middleware

API calls were hanging indefinitely when upstream services
became unresponsive, causing resource exhaustion and poor
user experience.

This middleware wraps all requests with a configurable timeout
(default 30s) and returns 504 Gateway Timeout when exceeded.
Includes exponential backoff for retries on timeout.

Side effect: Requests that legitimately take >30s will now
fail. Clients should handle 504 responses gracefully.

Fixes #1234
See: docs/api-timeouts.md
```

---

## Common Patterns

### Breaking Changes

```
Change API response format to REST standard

BREAKING CHANGE: Response structure modified from
  { data: {...} }
to
  { success: true, data: {...}, meta: {...} }

Clients must update response parsing.
See: docs/api-v2-migration.md

Closes #456
```

### Co-authored Commits

```
Implement OAuth2 authentication flow

Co-authored-by: Jane Doe <jane@example.com>
```

### Issue References

- `Fixes #123` or `Closes #123` - Auto-closes issue
- `Refs #123` - References without closing
- `Resolves: #123` - Resolves issue
- `See: docs/file.md` - Documentation link
- `Related to #123` - Loose association

---

## Anti-Patterns

### ❌ Vague Messages
```
Update code
Fix stuff
Changes
WIP
```

### ❌ Over-detailed (visible in diff)
```
Change variable userAuthenticationToken to userToken on
line 45 of auth.js and update function call on line 67
```

### ❌ Explaining HOW instead of WHY
```
Add if statement to check token expiry before processing
request and return 401 if expired
```

### ✅ Good Message
```
Validate token expiry before request processing

Prevents race condition where expired tokens were accepted
during the brief window between expiry and cache invalidation.
```

---

## Quick Checklist

Before committing:

- [ ] ONE logical change only
- [ ] Subject completes "If applied, this commit will..."
- [ ] Subject ≤50 characters
- [ ] Subject capitalized, no period
- [ ] Imperative mood
- [ ] Body explains WHY, not HOW
- [ ] Tests pass
- [ ] Message useful 6 months from now