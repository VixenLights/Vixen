# Technical Specification: VIX-3021 - Preserve Spacing for Empty and Blank Text Rows
**Target File Path:** `docs/effects/vix-3021-text-empty-row-spacing.md`  
**Status:** Ready for Review

---

## 1. Refined Requirements

- **Functional Overview:** The Text effect must treat every empty or blank-only user-entered row as a row containing exactly one ordinary space during rendering. This preserves one full line of vertical spacing and ensures that behavior is identical before saving and after sequence serialization converts a whitespace-only value to an empty string.
- **Scope:** The change is limited to plain, normal Text rendering where `TextSource == TextSource.None` and `TextMode == TextMode.Normal`.
- **Persistence:** Do not change sequence XML serialization or the serialized `TextData.Text` collection.
- **State Isolation:** Prepare a separate render-time list. Do not modify `TextLines`, `TextData.Text`, dirty state, or property notifications.
- **Blank Row Definition:** A row is blank when it is null, empty, or consists entirely of characters recognized by `String.IsNullOrWhiteSpace(...)`.
- **Canonical Render Value:** Every blank row becomes exactly `" "` in the render-time list, regardless of whether its original value was `null`, `""`, `" "`, multiple spaces, a tab, or another whitespace-only sequence.
- **Visible Content:** Rows containing any non-whitespace character remain unchanged, including their leading and trailing whitespace.
- **Consecutive Rows:** Every consecutive blank row contributes exactly one line of spacing.
- **Boundary Rows:** Leading and trailing blank rows contribute to overall text height and scrolling bounds.
- **Empty Collection:** A collection containing no rows remains empty; do not synthesize a row.
- **Mark Sources:** Preserve current handling for `MarkCollection` and `MarkCollectionLabels`.
- **Other Text Modes:** Preserve the existing `SplitTextIntoCharacters(...)` behavior for rotated and other non-normal modes.
- **Visual Result:** A normalized blank row draws no visible glyph but advances the vertical position by the measured height of a single-space string.

### Data Model & State Changes

No serialized fields, public properties, or migration callbacks are required.

`TextData.Text` remains a `List<string>`. The persisted collection may contain an empty string after reload. Blank-row normalization occurs only while preparing `_text` for rendering.

Introduce private helpers in `Text.cs`, conceptually:

```csharp
private static string PrepareTextRowForRendering(string row)
{
	return String.IsNullOrWhiteSpace(row) ? " " : row;
}

private List<string> PrepareTextLinesForRendering()
{
	if (TextSource == TextSource.None && TextMode == TextMode.Normal)
	{
		return TextLines.Select(PrepareTextRowForRendering).ToList();
	}

	if (TextMode == TextMode.Normal || TextSource == TextSource.MarkCollection)
	{
		return TextLines.Where(x => !String.IsNullOrEmpty(x)).ToList();
	}

	return SplitTextIntoCharacters(TextLines);
}
```

`SetupRender()` should assign:

```csharp
_text = PrepareTextLinesForRendering();
```

The exact private method names may follow nearby conventions, but the normalization and scope contracts are mandatory. Private helpers avoid adding or modifying a public/protected API, so no XML documentation change is required.

## 2. Technical Architecture & Impact

### Implementation Strategy

Replace the inline `_text` preparation in `Text.SetupRender()` with a focused private helper.

For plain normal text:

1. Enumerate `TextLines` in its original order.
2. Map every null, empty, or whitespace-only row to exactly `" "`.
3. Preserve every row containing visible content exactly, including leading and trailing whitespace.
4. Materialize a new `List<string>`.
5. Use the existing measurement and drawing loops without special cases in those loops.

The existing rendering code calls `Graphics.MeasureString(...)` for each prepared row. Measuring `" "` supplies normal font height, and the drawing loops advance `p.Y` by that height even though the row produces no visible glyph. Because every blank representation is canonicalized before measurement, initial in-memory behavior and post-reload behavior are identical.

Do not:

- edit `SequenceXElementReader`, `XElementFileReader`, or another serialization component;
- add `OnSerializing` or `OnDeserialized` transformations to `TextData`;
- replace values in the persisted collection;
- mark the effect dirty during render preparation;
- raise property-change notifications during render preparation;
- change mark timing or mark-to-word mapping;
- change non-normal Text modes;
- change generated visual-representation behavior unless required by a failing regression test.

### Mathematical / Logical Formulas

For plain normal text, define:

```text
RenderRow(row) =
    " "  when String.IsNullOrWhiteSpace(row)
    row  otherwise
```

The prepared collection is:

```text
PreparedRows = Map(RenderRow, TextLines)
```

The row-count invariant is:

```text
PreparedRows.Count == TextLines.Count
```

The canonicalization invariant for all blank inputs is:

```text
RenderRow(null)
    == RenderRow("")
    == RenderRow(" ")
    == RenderRow("   ")
    == RenderRow("\t")
    == " "
```

For a constant font line height `L` and `N` prepared rows:

```text
TotalTextHeight = L x N
```

For `K` consecutive blank rows between visible rows:

```text
AddedVerticalGap = L x K
```

Example:

```text
Input:    ["First", "", "   ", "Second"]
Prepared: ["First", " ", " ", "Second"]
Gap:      2 x line height
```

### Component Impact Matrix

| Component | Change | Runtime Effect |
|---|---|---|
| `src/Vixen.Modules/Effect/Text/Text.cs` | Extract render-list preparation and canonicalize blank plain-text rows to `" "` | All blank representations consume one rendered line |
| `src/Vixen.Modules/Effect/Text/TextData.cs` | No change | Serialized contract remains compatible |
| Sequence XML serializers | No change | No spillover to other effects or string properties |
| Effect Editor | No change | Empty and whitespace-only Text rows behave identically |
| Mark collection handling | No change | Existing word/mark behavior remains intact |
| Non-normal Text modes | No change | Existing character/newline splitting remains intact |
| `src/Vixen.Tests/Effects/TextEmptyRowSpacingTests.cs` | Add focused regression coverage | Protects canonicalization and scope boundaries |

### Compatibility

Existing sequences containing empty Text rows gain the intended one-line spacing when rendered as plain normal text.

Existing sequences containing one or more whitespace characters on otherwise blank rows continue to produce one line of spacing. The exact blank character count no longer affects rendering, so the initial session and restored session behave identically even if XML persistence converts the stored value to an empty string.

Rows containing visible text render as before. Other effects and serialized string properties are unaffected because shared serialization is not modified.

## 3. Acceptance Criteria

### Happy Path

- **Given** plain normal Text with rows `["First", "", "Second"]`  
  **When** render preparation runs  
  **Then** the working rows are `["First", " ", "Second"]`  
  **And** the original `TextLines[1]` remains empty.

- **Given** plain normal Text with a row containing one space  
  **When** render preparation runs before the sequence is saved  
  **Then** that row is represented by exactly one space.

- **Given** sequence persistence later restores that row as an empty string  
  **When** render preparation runs after reopening  
  **Then** that row is again represented by exactly one space  
  **And** its visual spacing is identical to the initial session.

- **Given** a vertically scrolling Text effect with a blank row between two visible rows  
  **When** the effect is rendered  
  **Then** the visible rows are separated by one font line height.

### Boundary and Edge Cases

- **Given** blank rows represented by `null`, `""`, `" "`, multiple spaces, or a tab  
  **When** render preparation runs  
  **Then** each prepared value is exactly `" "`.

- **Given** two consecutive blank rows using different blank representations  
  **When** the effect renders  
  **Then** they produce two equal line heights of spacing.

- **Given** leading or trailing blank rows  
  **When** scrolling bounds are calculated  
  **Then** each row contributes one line to the total text height.

- **Given** a row containing visible text with leading or trailing whitespace  
  **When** render preparation runs  
  **Then** the complete value is preserved exactly.

- **Given** an empty `TextLines` collection  
  **When** render preparation runs  
  **Then** the prepared collection remains empty  
  **And** rendering does not throw.

- **Given** a collection containing only blank rows  
  **When** the effect renders  
  **Then** no visible glyphs are produced  
  **And** each row still contributes one line of layout height  
  **And** no exception occurs.

### Scope and Regression Boundaries

- **Given** `TextSource` is `MarkCollection` or `MarkCollectionLabels`  
  **When** render preparation runs  
  **Then** existing empty-entry filtering and mark behavior remain unchanged.

- **Given** a non-normal `TextMode`  
  **When** render preparation runs  
  **Then** the existing character/newline splitting path remains unchanged.

- **Given** render preparation has completed  
  **Then** `TextData.Text` is unchanged  
  **And** `IsDirty` is not changed by canonicalization  
  **And** no property-changed notification is raised.

- **Given** another effect contains a whitespace-only serialized string  
  **When** a sequence is saved and loaded  
  **Then** its behavior remains unchanged because the shared serializer is not modified.

## 4. Test Plan

### Automated Testing Strategy

Create `src/Vixen.Tests/Effects/TextEmptyRowSpacingTests.cs`.

Use the existing xUnit and reflection conventions demonstrated by `TextCycleColorModeTests`.

Tests should cover:

- `TextRendering_EmptyPlainTextRowBecomesSingleSpace`
- `TextRendering_AllBlankRepresentationsBecomeSingleSpace`
- `TextRendering_BlankRowCanonicalizationDoesNotMutateTextLines`
- `TextRendering_ConsecutiveBlankRowsRemainDistinct`
- `TextRendering_LeadingAndTrailingBlankRowsArePreserved`
- `TextRendering_VisibleTextWhitespaceIsUnchanged`
- `TextRendering_EmptyCollectionRemainsEmpty`
- `TextRendering_MarkCollectionRetainsExistingEmptyEntryBehavior`
- `TextRendering_NonNormalModeRetainsExistingSplitBehavior`

The all-blank-representations test must include at least:

```csharp
new string?[] { null, String.Empty, " ", "   ", "\t" }
```

and assert that every prepared value equals `" "`.

The primary assertions should target the private render-preparation helper through reflection, matching existing Text effect test conventions. Tests must also assert that the original `TextLines` entries remain unchanged.

Add a render-level test if stable bitmap assertions can be produced:

1. Render `"First"` and `"Second"` with no blank row.
2. Render the same rows with one empty row between them.
3. Render them again with a whitespace-only row between them.
4. Assert that the empty-row and whitespace-row results have equal vertical glyph spacing.
5. Assert that both have approximately one measured font line more spacing than the no-blank-row result.
6. Allow a small tolerance for GDI measurement rounding.

Run:

```powershell
dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~TextEmptyRowSpacingTests
```

Then run the existing Text-focused tests:

```powershell
dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter FullyQualifiedName~Text
```

Finally run:

```powershell
dotnet test src/Vixen.Tests/Vixen.Tests.csproj
```

### Manual / Verification Testing

1. Start Vixen from a Debug or Release build.
2. Create a sequence with an element suitable for the Text effect.
3. Add a Text effect configured for normal vertically scrolling text.
4. Add three Text rows: `First`, an empty row, and `Second`.
5. Scrub or play the effect and confirm a one-line gap appears.
6. Change the middle row to one space, then multiple spaces, and confirm the gap remains exactly one line high in each case.
7. Save and close the sequence while the middle row contains whitespace.
8. Reopen it and confirm the gap remains exactly one line high.
9. Add two consecutive blank rows using different blank representations and confirm a two-line gap.
10. Add leading and trailing blank rows and confirm the scrolling duration and bounds include them.
11. Verify a Mark Collection Text effect still follows its existing timing and word-selection behavior.
12. Verify rotated Text behavior is unchanged.

### Performance & Regression Boundaries

- Complexity remains `O(N)` for `N` Text rows.
- The existing implementation already materializes a new list, so allocation behavior remains equivalent.
- No shared mutable state is introduced.
- Rendering remains instance-local and requires no synchronization.
- No sequence-file size or serialization-time change is expected.
- No public API or data-contract compatibility impact is permitted.

# NEXT STEPS

Is this specification approved? Once approved, we can proceed to trigger the code execution plan (execplan).
