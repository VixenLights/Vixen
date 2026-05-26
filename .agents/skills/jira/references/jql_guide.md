# Comprehensive Guide to JIRA Query Language (JQL)

This guide provides a complete reference for using JIRA Query Language (JQL) to search and filter issues in JIRA. JQL is a powerful query language that allows you to create precise searches to find exactly the issues you need.

## Table of Contents

1. [Basic Syntax and Structure](#basic-syntax-and-structure)
2. [Common Fields](#common-fields)
3. [Operators](#operators)
4. [Functions](#functions)
5. [Date and Time Queries](#date-and-time-queries)
6. [Historical Search Operators](#historical-search-operators)
7. [Text Search](#text-search)
8. [Working with Sprints](#working-with-sprints)
9. [Working with Epics](#working-with-epics)
10. [Custom Fields](#custom-fields)
11. [Time Tracking Queries](#time-tracking-queries)
12. [Comments and Descriptions](#comments-and-descriptions)
13. [Watchers and Voters](#watchers-and-voters)
14. [Advanced Queries](#advanced-queries)
15. [Common Use Cases](#common-use-cases)
16. [Best Practices](#best-practices)
17. [Common Pitfalls](#common-pitfalls)

## Basic Syntax and Structure

A JQL query consists of three main components:

```
field operator value
```

- **Field**: The aspect of the issue you want to search (e.g., status, assignee, project)
- **Operator**: How to compare the field to the value (e.g., =, !=, >, <)
- **Value**: What you're searching for (e.g., "Open", currentUser(), "My Project")

### Combining Conditions

Use keywords to combine multiple conditions:
- **AND**: Both conditions must be true
- **OR**: Either condition must be true
- **NOT**: Negates a condition

**Examples:**
```jql
project = "DEV" AND status = "In Progress"
assignee = currentUser() OR reporter = currentUser()
project = "DEV" AND NOT status = "Closed"
```

### Precedence and Parentheses

Use parentheses to control order of evaluation:
```jql
project = "DEV" AND (status = "Open" OR status = "In Progress")
```

## Common Fields

### Standard Fields

| Field | Description | Example |
|-------|-------------|---------|
| `project` | Project name or key | `project = "DEV"` |
| `status` | Current status of the issue | `status = "In Progress"` |
| `assignee` | User assigned to the issue | `assignee = currentUser()` |
| `reporter` | User who created the issue | `reporter = "john.doe"` |
| `priority` | Priority level | `priority = "High"` |
| `labels` | Custom labels | `labels = "urgent"` |
| `component` | Project component | `component = "Frontend"` |
| `created` | Creation date | `created >= "2025-01-01"` |
| `updated` | Last update date | `updated >= -7d` |
| `resolved` | Resolution date | `resolved >= startOfMonth()` |
| `due` | Due date | `due <= now()` |
| `resolution` | Resolution status | `resolution = "Fixed"` |
| `type` or `issuetype` | Type of issue | `type = "Bug"` |
| `summary` | Issue summary/title | `summary ~ "error"` |
| `description` | Issue description | `description ~ "crash"` |
| `environment` | Environment field | `environment ~ "production"` |

### Agile Fields

| Field | Description | Example |
|-------|-------------|---------|
| `sprint` | Sprint assignment | `sprint = "Sprint 1"` |
| `epicLink` | Link to parent epic | `epicLink = "PROJ-123"` |
| `parent` | Parent issue (for subtasks) | `parent = "PROJ-456"` |

### Time Tracking Fields

| Field | Description | Example |
|-------|-------------|---------|
| `originalEstimate` | Original time estimate | `originalEstimate >= 8h` |
| `remainingEstimate` | Remaining time estimate | `remainingEstimate > 0` |
| `timeSpent` | Time logged | `timeSpent >= 4h` |

## Operators

### Equality Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `=` | Equals | `status = "Open"` |
| `!=` | Not equals | `status != "Closed"` |
| `IS` | Is (for null/empty) | `assignee IS EMPTY` |
| `IS NOT` | Is not (for null/empty) | `assignee IS NOT EMPTY` |

### Comparison Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `>` | Greater than | `priority > "Medium"` |
| `>=` | Greater than or equal | `created >= "2025-01-01"` |
| `<` | Less than | `priority < "High"` |
| `<=` | Less than or equal | `due <= now()` |

### List Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `IN` | Matches any value in list | `status IN ("Open", "In Progress")` |
| `NOT IN` | Doesn't match any value | `status NOT IN ("Closed", "Resolved")` |

### Text Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `~` | Contains text | `summary ~ "error"` |
| `!~` | Does not contain | `summary !~ "test"` |

### Historical Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `WAS` | Had a value in the past | `status WAS "In Progress"` |
| `WAS IN` | Had any of these values | `status WAS IN ("Open", "Reopened")` |
| `WAS NOT` | Never had this value | `assignee WAS NOT "john.doe"` |
| `WAS NOT IN` | Never had these values | `status WAS NOT IN ("Closed", "Done")` |
| `CHANGED` | Field value changed | `status CHANGED` |

## Functions

### User Functions

| Function | Description | Example |
|----------|-------------|---------|
| `currentUser()` | Currently logged-in user | `assignee = currentUser()` |
| `membersOf("group")` | Members of a group | `assignee IN membersOf("developers")` |

### Date/Time Functions

| Function | Description | Example |
|----------|-------------|---------|
| `now()` | Current date and time | `due <= now()` |
| `startOfDay()` | Start of current day (00:00) | `updated >= startOfDay()` |
| `endOfDay()` | End of current day (23:59) | `created <= endOfDay()` |
| `startOfWeek()` | Start of current week | `created >= startOfWeek()` |
| `endOfWeek()` | End of current week | `created <= endOfWeek()` |
| `startOfMonth()` | Start of current month | `resolved >= startOfMonth()` |
| `endOfMonth()` | End of current month | `due <= endOfMonth()` |
| `startOfYear()` | Start of current year | `created >= startOfYear()` |
| `endOfYear()` | End of current year | `created <= endOfYear()` |

### Date/Time Function Modifiers

You can add offsets to date functions:
```jql
startOfDay("+1d")      # Tomorrow at 00:00
startOfWeek("-1w")     # Start of last week
startOfMonth("+2M")    # Start of month, 2 months from now
```

### Sprint Functions (Agile)

| Function | Description | Example |
|----------|-------------|---------|
| `openSprints()` | Active sprints | `sprint IN openSprints()` |
| `closedSprints()` | Completed sprints | `sprint IN closedSprints()` |
| `futureSprints()` | Planned sprints | `sprint IN futureSprints()` |

### Subquery Functions

| Function | Description | Example |
|----------|-------------|---------|
| `issueHistory()` | Issues with history | `issuekey IN issueHistory()` |
| `linkedIssues()` | Issues linked to key | `issuekey IN linkedIssues("PROJ-123")` |
| `updatedBy()` | Issues updated by user | `issuekey IN updatedBy("john.doe", "-7d")` |

## Date and Time Queries

### Absolute Dates

Use ISO format (YYYY-MM-DD) or full datetime:
```jql
created >= "2025-01-01"
updated >= "2025-01-21 14:30"
```

### Relative Dates

Use shorthand notation for relative time periods:

| Unit | Description | Example |
|------|-------------|---------|
| `d` | Days | `-1d` (yesterday) |
| `w` | Weeks | `-2w` (2 weeks ago) |
| `M` | Months | `-3M` (3 months ago) |
| `y` | Years | `-1y` (1 year ago) |
| `h` | Hours | `-8h` (8 hours ago) |
| `m` | Minutes | `-30m` (30 minutes ago) |

**Examples:**
```jql
# Issues created in the last 7 days
created >= -7d

# Issues updated in the last 2 weeks
updated >= -2w

# Issues created this week
created >= startOfWeek() AND created <= endOfWeek()

# Issues created today
created >= startOfDay()

# Issues due in the next 3 days
due >= now() AND due <= 3d

# Issues created between specific dates
created >= "2025-01-01" AND created <= "2025-01-31"
```

## Historical Search Operators

Historical operators let you search based on past values of fields.

### WAS Operator

Find issues that had a specific value at any point:
```jql
# Issues that were once "In Progress"
status WAS "In Progress"

# Issues that were assigned to a specific user
assignee WAS "john.doe"
```

### WAS IN Operator

Find issues that had any of several values:
```jql
# Issues that were in any of these statuses
status WAS IN ("On Hold", "Blocked")
```

### WAS NOT Operators

Find issues that never had a specific value:
```jql
# Issues never assigned to a user
assignee WAS NOT "john.doe"

# Issues never in specific statuses
status WAS NOT IN ("Cancelled", "Rejected")
```

### CHANGED Operator

Find issues where a field changed, with optional predicates:

**Basic change:**
```jql
status CHANGED
priority CHANGED
```

**Change with time constraints:**
```jql
# Status changed in the last week
status CHANGED AFTER -7d

# Priority changed before a date
priority CHANGED BEFORE "2025-01-01"

# Status changed during a period
status CHANGED DURING ("2025-01-01", "2025-01-31")

# Status changed on a specific date
status CHANGED ON "2025-01-21"
```

**Change by user:**
```jql
# Status changed by a specific user
status CHANGED BY "john.doe"

# Assignee changed by current user
assignee CHANGED BY currentUser()
```

**Change from/to specific values:**
```jql
# Status changed from Open to In Progress
status CHANGED FROM "Open" TO "In Progress"

# Priority changed from Low
priority CHANGED FROM "Low"

# Status changed to Closed
status CHANGED TO "Closed"
```

**Combined predicates:**
```jql
# Status changed to In Progress by current user in last week
status CHANGED TO "In Progress" BY currentUser() AFTER -7d
```

## Text Search

### Contains Operator (~)

Search for text within fields:
```jql
# Summary contains "error"
summary ~ "error"

# Description contains "database"
description ~ "database"

# Environment contains "production"
environment ~ "production"
```

### Does Not Contain Operator (!~)

Exclude issues containing text:
```jql
# Summary does not contain "test"
summary !~ "test"

# Description does not contain "legacy"
description !~ "legacy"
```

### Wildcards

Use `*` for wildcard matching:
```jql
# Summary starts with "API"
summary ~ "API*"

# Summary ends with "error"
summary ~ "*error"

# Summary contains word starting with "auth"
summary ~ "auth*"
```

### Exact Phrase Matching

Use quotes for exact phrases:
```jql
# Exact phrase in summary
summary ~ "\"login failed\""

# Multiple word search
description ~ "\"database connection timeout\""
```

### Case Sensitivity

Text searches are case-insensitive by default:
```jql
summary ~ "error"  # Matches "Error", "ERROR", "error"
```

## Working with Sprints

### Current/Active Sprints

Find issues in currently active sprints:
```jql
sprint IN openSprints()

# For a specific board
sprint IN openSprints() AND project = "DEV"
```

### Past Sprints

Find issues in completed sprints:
```jql
sprint IN closedSprints()

# Issues from the last completed sprint
sprint IN closedSprints() ORDER BY sprint DESC
```

### Future Sprints

Find issues in planned sprints:
```jql
sprint IN futureSprints()
```

### Specific Sprint

Find issues in a named sprint:
```jql
sprint = "Sprint 23"

# Partial sprint name match
sprint ~ "Sprint 2*"
```

### Issues Not in Any Sprint

Find backlog issues not assigned to a sprint:
```jql
sprint IS EMPTY

# Backlog for a specific project
project = "DEV" AND sprint IS EMPTY
```

### Issues in Multiple Sprint States

Combine sprint queries:
```jql
# Issues in active or future sprints
sprint IN (openSprints(), futureSprints())

# Issues not in closed sprints
sprint NOT IN closedSprints()
```

### Sprint-Specific Examples

```jql
# Unfinished issues from closed sprints
sprint IN closedSprints() AND status != "Done"

# High priority items in current sprint
sprint IN openSprints() AND priority = "High"

# Bugs in active sprint
sprint IN openSprints() AND type = "Bug"

# Stories not estimated in backlog
project = "DEV" AND sprint IS EMPTY AND type = "Story" AND originalEstimate IS EMPTY
```

## Working with Epics

### All Issues in an Epic

Find all issues linked to a specific epic:
```jql
epicLink = "PROJ-123"

# Or using parent
parent = "PROJ-123"
```

### Issues in Multiple Epics

```jql
epicLink IN ("PROJ-123", "PROJ-456")
```

### Orphaned Issues

Find issues not linked to any epic:
```jql
epicLink IS EMPTY AND type != "Epic"

# Orphaned stories in a project
project = "DEV" AND type = "Story" AND epicLink IS EMPTY
```

### Epic-Specific Queries

```jql
# All epics in a project
project = "DEV" AND type = "Epic"

# Epics without children
type = "Epic" AND issueFunction NOT IN hasLinks("is parent of")

# Open issues in a specific epic
epicLink = "PROJ-123" AND status != "Done"

# Count issues by epic (use GROUP BY in UI)
project = "DEV" AND epicLink IS NOT EMPTY
```

### Epic Progress

```jql
# Completed issues in epic
epicLink = "PROJ-123" AND status = "Done"

# Remaining work in epic
epicLink = "PROJ-123" AND status != "Done"

# Blocked issues in epic
epicLink = "PROJ-123" AND status = "Blocked"
```

## Custom Fields

### Finding Custom Field Names

Custom fields use the format `cf[XXXXX]` where XXXXX is the field ID.

To find custom field IDs:
1. Use JIRA's field configuration UI
2. Inspect the field in the browser developer tools
3. Use the JIRA REST API

### Querying Custom Fields

```jql
# Custom field equals value
cf[10010] = "Value"

# Custom field contains text
cf[10010] ~ "search term"

# Custom field is empty
cf[10010] IS EMPTY

# Custom field in list
cf[10010] IN ("Option1", "Option2")
```

### Common Custom Field Examples

```jql
# Story points (common Agile field)
cf[10016] >= 8

# Epic name
cf[10011] ~ "Phase 1"

# Sprint (if custom field)
cf[10020] = "Sprint 5"

# Team field
cf[10030] = "Backend Team"

# Environment (if custom)
cf[10040] IN ("Production", "Staging")
```

## Time Tracking Queries

### Original Estimate

```jql
# Issues estimated at 8 hours or more
originalEstimate >= 8h

# Issues estimated between 4 and 8 hours
originalEstimate >= 4h AND originalEstimate <= 8h

# Unestimated issues
originalEstimate IS EMPTY

# Stories with no estimate
type = "Story" AND originalEstimate IS EMPTY
```

### Remaining Estimate

```jql
# Issues with remaining work
remainingEstimate > 0

# Issues with more than 8 hours remaining
remainingEstimate > 8h

# No remaining estimate
remainingEstimate IS EMPTY
```

### Time Spent

```jql
# Issues with logged time
timeSpent > 0

# Issues with more than 4 hours logged
timeSpent >= 4h

# Issues with no time logged
timeSpent IS EMPTY OR timeSpent = 0
```

### Combined Time Tracking

```jql
# Over-estimated issues (time spent exceeds original estimate)
timeSpent > originalEstimate

# Issues with remaining work
remainingEstimate > 0 AND status != "Done"

# Completed issues with time tracking
status = "Done" AND timeSpent > 0

# Issues nearing estimate (90% or more)
timeSpent >= originalEstimate * 0.9 AND status != "Done"
```

### Time Format Examples

Time can be specified in various units:
- `m` - minutes
- `h` - hours
- `d` - days (8 hours)
- `w` - weeks (40 hours)

```jql
originalEstimate = 30m    # 30 minutes
originalEstimate = 2h     # 2 hours
originalEstimate = 1d     # 1 day (8 hours)
originalEstimate = 2w     # 2 weeks (80 hours)
```

## Comments and Descriptions

### Searching Comments

Find issues with comments containing specific text:
```jql
# Comment contains text
comment ~ "approved"

# Comment does not contain text
comment !~ "rejected"

# Comment by specific user (requires plugin)
comment ~ "john.doe: approved"
```

### Searching Descriptions

```jql
# Description contains text
description ~ "database migration"

# Description does not contain text
description !~ "deprecated"

# Empty description
description IS EMPTY

# Has description
description IS NOT EMPTY
```

### Combined Text Search

```jql
# Text in summary, description, or comments
text ~ "critical issue"

# Text search excluding certain terms
text ~ "login" AND text !~ "test"
```

## Watchers and Voters

### Watchers

Find issues watched by specific users:
```jql
# Watched by current user
watcher = currentUser()

# Watched by specific user
watcher = "john.doe"

# Has watchers
watchers IS NOT EMPTY

# No watchers
watchers IS EMPTY
```

### Voters

Find issues voted on by users:
```jql
# Voted by current user
voter = currentUser()

# Voted by specific user
voter = "jane.smith"

# Has votes
votes > 0

# More than 5 votes
votes > 5

# No votes
votes = 0
```

### Combined Watcher/Voter Queries

```jql
# Issues you're watching or assigned to
watcher = currentUser() OR assignee = currentUser()

# Popular issues (many watchers and votes)
watchers > 10 AND votes > 5

# Issues with engagement
(watchers > 0 OR votes > 0) AND status = "Open"
```

## Advanced Queries

### Ordering Results

Use `ORDER BY` to sort results:
```jql
project = "DEV" ORDER BY created DESC

# Multiple sort fields
project = "DEV" ORDER BY priority DESC, created ASC

# Available sort directions: ASC (ascending), DESC (descending)
```

### Common Sort Fields

```jql
ORDER BY created DESC           # Newest first
ORDER BY updated DESC           # Recently updated first
ORDER BY priority DESC          # Highest priority first
ORDER BY due ASC                # Earliest due date first
ORDER BY status ASC             # Alphabetical by status
ORDER BY assignee ASC           # Alphabetical by assignee
ORDER BY votes DESC             # Most voted first
ORDER BY watchers DESC          # Most watched first
```

### Subqueries with Functions

```jql
# Issues updated by user in last 5 days
issuekey IN updatedBy("john.doe", "-5d")

# Issues linked to a specific issue
issuekey IN linkedIssues("PROJ-123")

# Issues linked with specific link type
issuekey IN linkedIssues("PROJ-123", "blocks")
```

### Complex Boolean Logic

```jql
# Parentheses for precedence
project = "DEV" AND (status = "Open" OR status = "Reopened") AND priority IN ("High", "Critical")

# Multiple OR conditions
(assignee = currentUser() OR reporter = currentUser() OR watcher = currentUser())

# Negation with NOT
project = "DEV" AND NOT (status = "Closed" OR status = "Resolved")
```

### Field Value Lists

```jql
# Status in multiple values
status IN ("To Do", "In Progress", "In Review")

# Priority not in list
priority NOT IN ("Low", "Lowest")

# Type in list
type IN ("Bug", "Incident", "Problem")
```

## Common Use Cases

### Personal Queries

```jql
# All my open issues
assignee = currentUser() AND status NOT IN ("Done", "Closed")

# Issues I reported
reporter = currentUser()

# Issues I'm watching
watcher = currentUser()

# Issues I'm involved in (assigned, reported, or watching)
assignee = currentUser() OR reporter = currentUser() OR watcher = currentUser()

# My issues updated today
assignee = currentUser() AND updated >= startOfDay()
```

### Team Queries

```jql
# Team's open issues
project = "DEV" AND assignee IN membersOf("dev-team") AND status != "Done"

# Unassigned issues in project
project = "DEV" AND assignee IS EMPTY

# Issues waiting for review
project = "DEV" AND status = "In Review"

# Blocked issues
project = "DEV" AND status = "Blocked"
```

### Sprint Queries

```jql
# Current sprint scope
sprint IN openSprints() AND project = "DEV"

# Current sprint bugs
sprint IN openSprints() AND type = "Bug"

# Unfinished sprint work
sprint IN openSprints() AND status != "Done"

# Sprint carryover (incomplete from closed sprints)
sprint IN closedSprints() AND status != "Done"
```

### Overdue and Due Soon

```jql
# Overdue issues
due < now() AND status NOT IN ("Done", "Closed")

# Due today
due >= startOfDay() AND due <= endOfDay()

# Due this week
due >= startOfWeek() AND due <= endOfWeek()

# Due in next 3 days
due >= now() AND due <= 3d AND status != "Done"
```

### Recently Updated

```jql
# Updated today
updated >= startOfDay()

# Updated in last 7 days
updated >= -7d

# Updated in last 24 hours
updated >= -24h

# Updated this week
updated >= startOfWeek()
```

### Quality and Bugs

```jql
# Open bugs
type = "Bug" AND status NOT IN ("Done", "Closed")

# Critical bugs
type = "Bug" AND priority = "Critical"

# Bugs in production
type = "Bug" AND environment ~ "production"

# Unresolved bugs assigned to me
type = "Bug" AND assignee = currentUser() AND status != "Done"

# Recently reported bugs
type = "Bug" AND created >= -7d
```

### Epic and Story Management

```jql
# All epics
type = "Epic"

# Stories without epic
type = "Story" AND epicLink IS EMPTY

# Incomplete stories in epic
epicLink = "PROJ-123" AND status != "Done"

# Unestimated stories
type = "Story" AND originalEstimate IS EMPTY
```

### Component-Based

```jql
# Issues in specific component
component = "Frontend"

# Issues in multiple components
component IN ("Frontend", "API")

# Issues without component
component IS EMPTY
```

### Label-Based

```jql
# Issues with specific label
labels = "urgent"

# Issues with multiple labels (AND)
labels = "urgent" AND labels = "customer-facing"

# Issues with any of multiple labels (OR)
labels IN ("urgent", "critical", "blocker")

# Issues without labels
labels IS EMPTY
```

### Version/Release Queries

```jql
# Issues in specific fix version
fixVersion = "2.0.0"

# Issues affecting specific version
affectedVersion = "1.5.0"

# Issues without fix version
fixVersion IS EMPTY

# Issues released
fixVersion IS NOT EMPTY AND status = "Done"
```

## Best Practices

### 1. Start Simple and Build Up

Begin with basic queries and add complexity gradually:
```jql
# Start: Find all bugs
type = "Bug"

# Add: Only open bugs
type = "Bug" AND status = "Open"

# Add: Only high priority open bugs
type = "Bug" AND status = "Open" AND priority = "High"

# Add: Only recent high priority open bugs
type = "Bug" AND status = "Open" AND priority = "High" AND created >= -7d
```

### 2. Use Parentheses for Clarity

Make complex logic explicit:
```jql
# Unclear precedence
project = "DEV" AND status = "Open" OR status = "Reopened" AND priority = "High"

# Clear precedence
project = "DEV" AND (status = "Open" OR status = "Reopened") AND priority = "High"
```

### 3. Use IN for Multiple Values

Cleaner than multiple OR conditions:
```jql
# Verbose
status = "Open" OR status = "In Progress" OR status = "Reopened"

# Cleaner
status IN ("Open", "In Progress", "Reopened")
```

### 4. Leverage Auto-Complete

JIRA's query editor provides auto-complete suggestions. Use it to:
- Discover available fields
- Find correct field values
- Learn function syntax
- Avoid typos

### 5. Save Frequently Used Queries

Save common queries as filters for quick access and sharing.

### 6. Use Relative Dates

Prefer relative dates for dynamic queries:
```jql
# Static - requires manual updates
created >= "2025-01-01"

# Dynamic - always shows last 7 days
created >= -7d
```

### 7. Consider Performance

For large JIRA instances:
- Limit searches to specific projects when possible
- Use indexed fields (status, priority, assignee)
- Avoid overly broad text searches
- Use time ranges to limit result sets

### 8. Test Queries Incrementally

Build and test queries step by step:
1. Start with basic criteria
2. Verify results
3. Add additional filters
4. Re-verify results
5. Repeat until complete

### 9. Document Complex Queries

Add comments to saved filters explaining the query logic:
```
Filter Name: Sprint Carryover Issues
Description: Incomplete work from closed sprints needing attention
JQL: sprint IN closedSprints() AND status != "Done" ORDER BY sprint DESC, priority DESC
```

### 10. Use Functions for Dynamic Queries

Prefer functions over hardcoded values:
```jql
# Hardcoded
assignee = "john.doe"

# Dynamic
assignee = currentUser()
```

## Common Pitfalls

### 1. Case Sensitivity in Field Names

**Problem:** Field names are case-sensitive.
```jql
# Wrong
Project = "DEV"
Status = "Open"

# Correct
project = "DEV"
status = "Open"
```

### 2. Incorrect Quote Usage

**Problem:** Mixing single and double quotes or using wrong quotes.
```jql
# Wrong
project = 'DEV'
status = "Open"

# Correct - use double quotes consistently
project = "DEV"
status = "Open"
```

### 3. Missing Parentheses in Complex Queries

**Problem:** Boolean logic without parentheses can produce unexpected results.
```jql
# Unclear - may not work as intended
project = "DEV" AND status = "Open" OR priority = "High"

# Clear - explicitly states intent
project = "DEV" AND (status = "Open" OR priority = "High")
```

### 4. Using = with Multi-Value Fields

**Problem:** Some fields can have multiple values (labels, components).
```jql
# May not work as expected
labels = "urgent"

# Better - explicitly check for inclusion
labels IN ("urgent")

# Or use contains for partial match
labels ~ "urgent"
```

### 5. Forgetting IS EMPTY vs = null

**Problem:** Using wrong syntax for null/empty checks.
```jql
# Wrong
assignee = null
assignee = ""

# Correct
assignee IS EMPTY
assignee IS NOT EMPTY
```

### 6. Incorrect Date Formats

**Problem:** Using wrong date format.
```jql
# Wrong
created >= "01/21/2025"
created >= "21-Jan-2025"

# Correct
created >= "2025-01-21"
created >= "2025-01-21 14:30"
```

### 7. Not Accounting for Field Changes

**Problem:** Fields change over time (status, assignee, etc.). Use historical operators when needed.
```jql
# Only shows current status
status = "In Progress"

# Shows if status was ever "In Progress"
status WAS "In Progress"
```

### 8. Overly Broad Text Searches

**Problem:** Text searches can be slow and return too many results.
```jql
# Too broad - very slow
text ~ "the"

# More specific - faster and more relevant
summary ~ "login error" OR description ~ "login error"
```

### 9. Ignoring Field Data Types

**Problem:** Treating all fields the same way.
```jql
# Wrong - priority is not numeric
priority > 3

# Correct - use priority names
priority = "High"
priority IN ("High", "Highest")
```

### 10. Not Using ORDER BY

**Problem:** Results in random or unhelpful order.
```jql
# No specific order
assignee = currentUser() AND status = "Open"

# Ordered by priority and creation date
assignee = currentUser() AND status = "Open" ORDER BY priority DESC, created ASC
```

### 11. Hardcoding User Names

**Problem:** Queries with hardcoded usernames don't work for other users.
```jql
# Hardcoded
assignee = "john.doe"

# Dynamic - works for any user
assignee = currentUser()
```

### 12. Forgetting Sprint States

**Problem:** Not considering sprint state when querying sprints.
```jql
# May return closed sprints
sprint IS NOT EMPTY

# More specific - only active sprints
sprint IN openSprints()
```

### 13. Assuming Field Names

**Problem:** Custom field names vary by instance.
```jql
# May not exist in your instance
"Story Points" = 8

# Better - use field ID or check field name
cf[10016] = 8
```

### 14. Not Handling Special Characters

**Problem:** Field values or names with special characters need quoting.
```jql
# Wrong if project key has special chars
project = DEV-API

# Correct
project = "DEV-API"

# Wrong if status has spaces
status = In Progress

# Correct
status = "In Progress"
```

### 15. Inefficient Negation

**Problem:** Using NOT inefficiently.
```jql
# Less efficient
NOT status = "Closed"

# More efficient
status != "Closed"

# Even better for multiple exclusions
status NOT IN ("Closed", "Done", "Resolved")
```

## Quick Reference Card

### Basic Structure
```jql
field operator value
field1 = value1 AND field2 = value2
field1 = value1 OR field2 = value2
(field1 = value1 OR field2 = value2) AND field3 = value3
```

### Common Patterns
```jql
# My open work
assignee = currentUser() AND status NOT IN ("Done", "Closed")

# Recent updates
updated >= -7d ORDER BY updated DESC

# Overdue items
due < now() AND status != "Done"

# Current sprint
sprint IN openSprints()

# Specific epic
epicLink = "PROJ-123"

# No assignee
assignee IS EMPTY

# Has comments
comment ~ "*"

# Text search
summary ~ "keyword" OR description ~ "keyword"
```

### Date Shortcuts
```jql
-1d    # Yesterday
-1w    # Last week
-1M    # Last month
now()  # Current time
startOfDay()
startOfWeek()
startOfMonth()
```

### Useful Functions
```jql
currentUser()
membersOf("group-name")
openSprints()
closedSprints()
```

---

## Additional Resources

- **JIRA Documentation**: Official Atlassian JQL reference
- **JQL Cheat Sheet**: Quick reference guide available from Atlassian
- **JIRA Query Builder**: Use JIRA's built-in simple/advanced search interface
- **Browser Extensions**: Consider JQL helper extensions for syntax highlighting

## Conclusion

JQL is a powerful tool for managing and analyzing JIRA issues. Start with simple queries and gradually build complexity as needed. Remember to:

1. Use auto-complete to discover fields and values
2. Test queries incrementally
3. Save frequently used queries as filters
4. Use relative dates and dynamic functions for flexible queries
5. Order results for better usability
6. Consider performance for large instances

With practice, JQL becomes an essential tool for efficiently managing your work in JIRA.
