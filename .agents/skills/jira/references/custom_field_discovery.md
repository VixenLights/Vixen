# Custom Field Discovery Methodology

JIRA instances heavily customize their field schemas with custom fields that vary by organization, project, and issue type. These fields have opaque identifiers (e.g., `customfield_10352`) that must be discovered and mapped to human-readable names for automation.

## The Challenge

**Problem**: You need to create tickets with specific fields, but you only know the human-readable name (e.g., "Owning Team", "Acceptance Criteria", "Risk Assessment") and not the custom field ID required by the API.

**Symptom**: Creating tickets fails with "field not found" or required fields are silently ignored because you're using the wrong identifier.

## Discovery Workflow

### Step 1: Search by Keyword

Use `mcp__atlassian__jira_search_fields` to find fields by partial name match:

```json
{
  "keyword": "owning",
  "limit": 10
}
```

**Returns:**
```json
[
  {
    "id": "customfield_11077",
    "name": "Owning Team",
    "custom": true,
    "schema": {
      "type": "option",
      "custom": "com.atlassian.jira.plugin.system.customfieldtypes:select"
    }
  }
]
```

**Key Information Extracted:**
- **Field ID**: `customfield_11077` (use this in API calls)
- **Field Name**: "Owning Team" (human-readable)
- **Field Type**: `option` with `select` custom type (dropdown/single-select)
- **Custom**: `true` (not a standard JIRA field)

### Step 2: Understand Field Type

The `schema.type` and `schema.custom` fields tell you how to format values:

| Schema Type | Custom Type | Value Format | Example |
|-------------|-------------|--------------|---------|
| `string` | `textarea` | Plain string | `"Risk assessment text"` |
| `number` | `float` | Number | `3` or `3.5` |
| `option` | `select` | Object with value | `{"value": "Team A"}` |
| `array` | `multiselect` | Array of objects | `[{"value": "Label1"}, {"value": "Label2"}]` |
| `user` | - | User identifier | `"user@example.com"` |
| `date` | - | ISO 8601 date | `"2025-01-15"` |
| `datetime` | - | ISO 8601 datetime | `"2025-01-15T10:30:00.000+0000"` |

### Step 3: Test with Single Issue

Create a test issue using the discovered field ID:

```json
{
  "project_key": "PROJ",
  "summary": "Test custom field",
  "issue_type": "Task",
  "additional_fields": {
    "customfield_11077": {
      "value": "Platform Team"
    }
  }
}
```

**Verify**: Check the created issue in JIRA UI to confirm the field populated correctly.

### Step 4: Document the Mapping

Once verified, document the field mapping in a project-specific configuration file:

```json
{
  "project_key": "PROJ",
  "custom_fields": {
    "owning_team": {
      "field_id": "customfield_11077",
      "field_name": "Owning Team",
      "field_type": "select",
      "required": true,
      "description": "Team responsible for this work"
    },
    "acceptance_criteria": {
      "field_id": "customfield_10352",
      "field_name": "Acceptance Criteria_gxp",
      "field_type": "textarea",
      "required": true,
      "description": "GXP acceptance criteria"
    },
    "story_points": {
      "field_id": "customfield_10060",
      "field_name": "Story Points",
      "field_type": "number",
      "required": false
    }
  }
}
```

## Advanced Discovery Techniques

### Discovering Required Fields

Some fields are required by project configuration or workflow rules. To discover these:

1. **Attempt to create without the field** - The error message often reveals required fields
2. **Check project settings** - Use `jira_get_all_projects` and examine field configurations
3. **Inspect existing issues** - Use `jira_get_issue` with `fields=*all` to see all populated fields

### Discovering Field Value Constraints

For `select`, `multiselect`, or `option` fields, you need to know valid values:

**Method 1: Inspect existing issues**
```bash
# Get issue with all fields
jira_get_issue(issue_key="PROJ-123", fields="*all")

# Look for the custom field in response
# Example: "customfield_11077": {"value": "Platform Team"}
```

**Method 2: Trial and error with descriptive errors**
- JIRA often returns error messages listing valid options when you provide an invalid value

**Method 3: Admin access**
- If you have admin access, check field configuration in JIRA admin panel for allowed values

### Handling Multi-Project Schemas

Different projects may use different field IDs for similar concepts:

```json
{
  "projects": {
    "PROJ-A": {
      "owning_team_field": "customfield_11077"
    },
    "PROJ-B": {
      "owning_team_field": "customfield_12034"
    }
  }
}
```

**Best Practice**: Store mappings per project, not globally.

## Common Patterns

### Pattern 1: GXP/Compliance Fields

Regulated industries often have custom fields for compliance:
- Acceptance Criteria (GXP)
- Risk Assessment (GXP)
- Validation Status
- Quality Gate

**Discovery**: Search for keywords like "gxp", "compliance", "validation", "risk", "acceptance"

### Pattern 2: Agile/Scrum Fields

Agile teams add custom fields:
- Story Points (often `customfield_10060` or `customfield_10016`)
- Sprint (often `customfield_10020`)
- Epic Link (often `customfield_10014`)

**Discovery**: Search for "story", "sprint", "epic"

### Pattern 3: Team/Ownership Fields

Organizations add team tracking:
- Owning Team / Responsible Team
- Technical Lead
- Product Owner

**Discovery**: Search for "team", "owner", "lead", "responsible"

## Automation Strategy

### 1. Build a Field Cache

```json
{
  "last_updated": "2025-01-15T10:00:00Z",
  "fields": [
    {
      "id": "customfield_11077",
      "name": "Owning Team",
      "type": "select",
      "projects": ["PROJ-A", "PROJ-B"]
    }
  ]
}
```

**Refresh** the cache periodically (e.g., weekly) or when field discovery fails.

### 2. Create Field Accessor Functions

```python
def get_field_id(field_name, project_key):
    """Get custom field ID by name and project."""
    cache = load_field_cache()
    for field in cache['fields']:
        if field['name'] == field_name and project_key in field['projects']:
            return field['id']
    # Fallback: search fields API
    return search_and_cache_field(field_name, project_key)
```

### 3. Validate Before Creation

```python
def validate_custom_fields(project_key, fields):
    """Ensure all custom fields exist and have correct format."""
    for field_name, value in fields.items():
        field_id = get_field_id(field_name, project_key)
        field_type = get_field_type(field_id)
        validate_value_format(value, field_type)
```

## Error Recovery

When field discovery or usage fails:

**Error**: `"Field 'customfield_XXXXX' cannot be set"`
- **Cause**: Field doesn't exist, wrong project, or wrong issue type
- **Solution**: Re-run field search, check project/issue type constraints

**Error**: `"Field value is not valid"`
- **Cause**: Incorrect value format for field type
- **Solution**: Check schema type, verify value format matches expected type

**Error**: `"Field is required"`
- **Cause**: Missing required custom field
- **Solution**: Search for required fields, add to field mappings

## Best Practices Summary

1. **Search First**: Always use `jira_search_fields` before assuming field IDs
2. **Document Mappings**: Store field ID mappings in project configuration files
3. **Test Thoroughly**: Create test issues to verify field IDs and value formats
4. **Cache Strategically**: Build field caches to reduce API calls
5. **Project-Specific**: Don't assume field IDs are the same across projects
6. **Type-Aware**: Respect field types when formatting values
7. **Error-Friendly**: Expect field discovery to fail, have fallback strategies
8. **Version Control**: Check field mappings into version control for team sharing
