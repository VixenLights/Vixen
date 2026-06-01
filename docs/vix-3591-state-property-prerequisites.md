# State Property Prerequisites for the State Effect

## Overview

VIX-3591 adds a State property that lets users define named prop states in Display Setup. VIX-3924 will add a State effect
that discovers those properties, stores a stable reference to one selected property, and activates its configured State items.

Before State effect development begins, the State property must provide stable identity and stricter naming behavior. These
updates should be completed on the VIX-3591 branch as a prerequisite change. This document intentionally excludes State effect
implementation.

## Terminology

- A **State property** is a configured property attached to an `IElementNode`. It has a name, a description, and an ordered
  list of State items.
- A **State item** is one row within a State property. It has a name, a color, and one or more assigned element nodes.
- A **stable identifier** is a persisted `Guid` that lets future effects keep referring to the same State property after its
  user-visible name changes.
- A **logical clone** is a cloned data object that still represents the same attached State property.
- A **copied property** is a new State property attached to another element by copying values from an existing State property.

## Requirements

### Stable State Property Identity

- Add a persisted stable identifier to each State property.
- The identifier belongs to the overall State property, not to an individual State item.
- Newly created State properties must receive a new non-empty identifier.
- State properties created while importing xLights data must receive a new non-empty identifier.
- Copying a State property from one element to another must create a new identifier for the destination property.
- Cloning data that still represents the same logical attached property must retain the existing identifier.
- Renaming a State property must not change its identifier.
- Editing the description, State items, colors, or assignments must not change the identifier.
- Loading or normalizing State data with a missing or empty identifier must assign a new identifier.
- No legacy-data migration path is required because the State property has not been released publicly.

### State Property Name Validation

- The overall State property name must contain at least one non-whitespace character.
- Leading and trailing whitespace must be removed before the State property is saved.
- A whitespace-only State property name must prevent saving.
- If the trimmed State property name contains fewer than three characters, Display Setup must show a non-blocking warning
  recommending a longer descriptive name.
- The short-name warning is informational. It must not require acknowledgment or prevent saving.

### State Item Name Validation

- Every State item name must contain at least one non-whitespace character.
- Leading and trailing whitespace must be removed before State items are saved.
- A whitespace-only State item name must prevent saving.
- State item names are case-sensitive. For example, `Open` and `open` are distinct names.
- Duplicate State item names are allowed. This is required because multiple rows with the same name will activate together in
  the future State effect.
- When two or more State item names differ only by letter casing, State Property Setup must show a non-blocking warning because the
  difference may be an accidental typo.
- The case-only warning is informational. It must not require acknowledgment or prevent saving.
- New State item rows must always receive a valid non-whitespace default name.

### xLights Import Name Handling

- xLights import must continue to create State properties and State items from parsed `stateInfo` data.
- Imported State properties must receive new stable property identifiers.
- Imported State items must continue to receive their own stable item identifiers.
- If an imported State property name is empty or contains only whitespace, assign a best-effort generated name. Fall back to
  a numbered default name when the XML context does not provide a usable name.
- If an imported State item name is empty or contains only whitespace, assign a generated default name based on the XML
  attribute tag being parsed.
- For example, an unnamed item parsed from the `s1` attribute group should receive a deterministic fallback derived from
  `s1`, such as `s1`.
- Generated names must contain at least one non-whitespace character and must satisfy the same persisted State item contract
  as names entered in the State Property Setup.
- Import normalization is best effort and must produce valid persisted names without displaying interactive validation
  warnings.

### Setup Dialog Behavior

- Validate existing names when the State Property Setup dialog opens.
- Validate edited names when the user completes editing a field. For the State item grid, editing is complete when the cell
  edit is committed, including when the user moves to another row.
- If the user clicks OK while a name field is still active, complete the edit, trim the value, and validate it before the OK
  command can execute.
- Blocking validation errors clearly identify the invalid field or row.
- Use Catel validation behavior to display all warnings and errors in a summary section at the top of the form.
- Provide field-level visual feedback for blocking validation errors, such as the standard red validation outline.
- Disable the OK button while any blocking validation errors are present.
- Keep Cancel and the window close button available while validation errors are present. Canceling discards the invalid draft
  edits and leaves the persisted State property unchanged.
- Non-blocking warnings appear in the validation summary when editing completes. They do not require acknowledgment and do not
  disable the OK button.
- Name trimming occurs when editing completes so the user sees the modified result immediately.
- All validation errors must be satisfied before data is copied back to the persisted State property.
- Canceling the dialog must continue to leave the persisted State property unchanged.

## Copy and Clone Design Concern

The existing property API uses `CloneValues(IProperty sourceProperty)` in more than one workflow. Some workflows copy values
into a different attached property, while other workflows may synchronize or clone data that still represents the same
logical property. The new identifier rules require these cases to produce different results:

- Copying values into a distinct State property attached to another element creates a new property identifier.
- Cloning the data model for editing, serialization, or another representation of the same logical State property retains the
  existing identifier.

The implementation plan must inspect every State property clone and copy call path before editing this behavior. If the
existing API does not provide enough context to distinguish a copied property from a logical clone, the plan must introduce
the smallest explicit mechanism that makes the distinction reliable. Do not infer identity from display names.

## Automated Test Expectations

- Add tests proving a newly constructed State property receives a non-empty stable identifier.
- Add tests proving normalization assigns an identifier when loaded State data has an empty identifier.
- Add tests proving a logical `StateData.Clone()` preserves the State property identifier.
- Add tests proving edits to State property fields and items do not change the identifier.
- Add tests proving copying a State property to a different element creates a new destination identifier.
- Add tests proving imported State properties receive distinct non-empty identifiers.
- Add tests proving imported unnamed State properties receive valid best-effort names or numbered fallback names.
- Add tests proving imported unnamed State items receive deterministic names derived from their XML attribute tags.
- Add tests proving overall State property names are trimmed when editing completes.
- Add tests proving whitespace-only overall State property names show validation feedback and disable OK.
- Add tests proving State item names are trimmed when editing completes.
- Add tests proving whitespace-only State item names show validation feedback and disable OK.
- Add tests proving OK is enabled again after all blocking validation errors are corrected.
- Add tests proving Cancel remains enabled while blocking validation errors are present.
- Add tests proving name trimming is visible in the field immediately after focus loss or grid-cell commit.
- Add tests proving exact duplicate State item names are allowed.
- Add tests proving case-sensitive item names remain distinct.
- Add tests proving case-only State item differences produce a non-blocking warning in the validation summary without
  disabling OK.
- Add tests proving State property names shorter than three characters produce a non-blocking warning in the validation
  summary without disabling OK.
- Add tests proving initial dialog data is validated when the dialog opens.
- Add tests proving canceling the setup dialog does not persist trimmed values, validation changes, or generated edits.

## Manual Acceptance Scenarios

### Stable Identity

1. Add a State property to an element in Display Setup.
2. Configure a name, description, and at least one State item.
3. Save and reopen the property.
4. Confirm the visible values remain unchanged.
5. Rename the State property, save, and reopen it.
6. Confirm the property still represents the same persisted State property identity.
7. Copy the property to another element.
8. Confirm the copied property receives its own distinct identity while preserving the copied user-visible values.

### Name Validation

1. Open a State property in Display Setup.
2. Enter an overall name containing only whitespace.
3. Move focus away from the field.
4. Confirm a validation error appears in the summary at the top of the form and the field is marked as invalid.
5. Confirm saving is blocked because the OK button is disabled.
6. Confirm Cancel remains available and discards the invalid draft edits.
7. Enter an overall name with fewer than three characters and move focus away from the field.
8. Confirm an informational warning appears in the validation summary and the OK button remains enabled.
9. Add a State item whose name contains only whitespace and commit the grid-cell edit.
10. Confirm a validation error appears in the summary and the invalid field is outlined in red.
11. Confirm saving is blocked because the OK button is disabled.
12. Correct the State item name and confirm the OK button becomes enabled again.
13. Add State items named `Open` and `open`, then commit the grid-cell edit.
14. Confirm an informational warning appears in the validation summary and the OK button remains enabled.
15. Enter leading and trailing whitespace in valid names, move focus away from each edited field, and confirm the trimmed
    values are immediately visible.
16. Save and reopen the property. Confirm the trimmed values remain persisted.

### xLights Import

1. Import an xLights model containing `stateInfo` data.
2. Confirm each generated State property has its own stable identity.
3. Import a model containing a State property with a missing or whitespace-only name.
4. Confirm the generated State property receives a valid best-effort name or numbered fallback name.
5. Import a model containing a State item with a missing or whitespace-only name.
6. Confirm the generated State item receives a non-empty fallback name derived from its XML attribute tag.

## Out of Scope

- Do not implement the State effect as part of this prerequisite change.
- Do not add State effect discovery, rendering, Mark Collection handling, or Effect Editor controls.
- Do not add `State` to `MarkCollectionType` yet. That belongs to VIX-3924.
- Do not implement future State effect playback modes such as `Countdown`, `Time Countdown`, or `Number`.
- Do not add user-selectable overlap strategies such as `First Wins` or `Last Wins`.

## Guidelines

- Use the project skills dotnet-best-practices, csharp-async, csharp-docs, dotnet-design-pattern-review, and catel-mvvm as part
  of the design process.
- Use `.agents\PLANS.md` when creating the implementation plan.
- Treat `docs\vix-3591-state-feature.md` as the original State property requirements and this document as the prerequisite
  follow-up requirements for VIX-3924.
- Update Jira issue VIX-3591 with the prerequisite scope, high-level design, acceptance criteria, and testing steps before
  implementation.
- Call out risks or concerns in the implementation plan, especially the copy-versus-clone identity distinction.
- Ask questions if further clarification is required.
