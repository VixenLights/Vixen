# Delete / Rename Export Profile — ExecPlan

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain this document in accordance with `.agents/PLANS.md`.

---

## Purpose / Big Picture

Vixen's bulk export wizard lets users save export configurations as named "profiles" (output format, output folder, audio settings, universe file settings, etc.). Today, once a profile is created, there is no way to rename or delete it from the wizard UI — the user must edit an XML file on disk. This change replaces the simple "Create New or Select Existing" first stage of the wizard with a full profile-management screen that lets users add new profiles, rename existing ones, and delete ones they no longer need — all from within the wizard.

A user can see it working by opening the Export Wizard. The first step now shows a "Configurations" dropdown with Add (+), Delete (trash), and Rename (pencil) buttons. Clicking Add prompts for a name and creates a new blank profile. Selecting a profile and clicking Rename lets the user give it a new name; the profile is saved immediately. Selecting a profile and clicking Delete asks for confirmation, removes it, and saves immediately. If no profiles exist, a "Default" profile is shown automatically so the user can proceed through the wizard; it is not written to disk unless the user explicitly saves it in the final summary stage.

---

## Progress

- [x] Milestone 1: Update JIRA issue VIX-3632 with requirements, high-level design, acceptance criteria, and testing steps. → [VIX-3632](https://vixenlights.atlassian.net/browse/VIX-3632)
- [x] Milestone 2: Wire `BulkExportConfigStage` into `BulkExportWizard` as Stage 1, replacing `BulkExportCreateOrSelectStage`. Verify the wizard opens and reaches Stage 1 correctly.
- [x] Milestone 3: Implement full `BulkExportConfigStage` behavior — default profile, button states, Add/Delete/Rename dialogs, trimming, same-name no-op, save-after-rename, save-after-delete, re-create Default after last profile is deleted.
- [ ] Milestone 4: Apply project skills — `csharp-docs`, `dotnet-best-practices`, `csharp-async`, `dotnet-design-pattern-review` — and address any findings.
- [ ] Milestone 5 (post-acceptance): Remove `BulkExportCreateOrSelectStage` and its Designer file after the feature is accepted. Do not remove prematurely.

---

## Surprises & Discoveries

*(Fill in as work progresses.)*

- `string?` nullable annotation is not valid in `src/Vixen.Modules/App/` because that subtree sets `<Nullable>disable</Nullable>`. The `excludeName` parameter in `GetProfileName` was changed from `string?` to `string` with a `null` default. This matches the pattern established in the prior FPP Direct Upload plan.

- TBD: Verify whether `BindingList<ExportProfile>` propagates `Name` property changes to the ComboBox automatically. `ExportProfile.Name` implements `INotifyPropertyChanged`, and `BindingList<T>` forwards item-level property-change notifications when items implement that interface — but this must be confirmed during Milestone 3. If it does not propagate automatically, force a ComboBox refresh after rename (call `comboProfiles.Refresh()` or re-bind).

---

## Decision Log

- Decision: Clone the selected profile in `comboProfiles_SelectedIndexChanged` before assigning to `_data.ActiveProfile`.
  Rationale: The old `BulkExportCreateOrSelectStage` always cloned on selection so that wizard-stage edits to `ActiveProfile` (output folder, format, etc.) would not mutate the saved profile if the user cancelled or backed out without saving. `BulkExportConfigStage` must preserve this invariant. A direct reference would cause silent in-memory mutations to the saved profile whenever the user steps through the wizard stages.
  Date/Author: (set when work begins)

- Decision: The transient "Default" profile is added to `_data.Profiles` in memory but not saved to disk.
  Rationale: The combobox is bound to a `BindingList<ExportProfile>` backed by `_data.Profiles`, so the easiest way to display the default profile is to add it to that list. This does not auto-persist — the only code path that writes to disk is `VixenSystem.SaveModuleConfigAsync()`, which is called exclusively from `BulkExportSummaryStage.SaveActiveConfig()` and from the rename/delete handlers in this stage. Therefore the default profile exists in memory for the duration of the session but is not on disk unless the user explicitly saves in the summary stage.
  Date/Author: (set when work begins)

- Decision: `buttonAddProfile_Click` does NOT call `VixenSystem.SaveModuleConfigAsync()`.
  Rationale: Adding a profile creates a new blank configuration that the user still needs to configure through the remaining wizard stages. Premature saving would write an incomplete profile to disk. Persistence happens when the user checks "Save as configuration" on the Summary stage.
  Date/Author: (set when work begins)

- Milestone 1 complete: JIRA issue updated at [VIX-3632](https://vixenlights.atlassian.net/browse/VIX-3632) on 2026-05-28.

- Decision: `BulkExportCreateOrSelectStage` and its Designer file are NOT deleted in this plan.
  Rationale: The feature doc explicitly says "Leave the old BulkExportCreateOrSelectStage until the very end. Once the new work is accepted, it can be removed." Milestone 5 covers removal; it must not be done during initial implementation.
  Date/Author: (set when work begins)

---

## Outcomes & Retrospective

*(Fill in at completion.)*

---

## Context and Orientation

This repository is a .NET 10 WinForms/WPF application called Vixen for animating light shows. The feature touches one App module.

**ExportWizard module** lives at `src/Vixen.Modules/App/ExportWizard/`. It is a WinForms wizard built on the `Common.Controls.Wizard.Wizard` base class. A wizard is a sequence of `WizardStage` subclasses. The stage list is defined in `BulkExportWizard.cs`. Each stage receives a shared `BulkExportWizardData` instance (`_data`) that carries both the persistent profile list and the transient active profile used while stepping through the wizard.

Key files and their roles:

- `BulkExportWizard.cs` — defines the list of stages in order. Stage 1 is currently `BulkExportCreateOrSelectStage`. This is where the substitution happens.
- `BulkExportCreateOrSelectStage.cs` / `.Designer.cs` — the current Stage 1. Simple two-radio "create new or select existing" UI. **Do not delete these files yet.**
- `BulkExportConfigStage.cs` / `.Designer.cs` — the replacement Stage 1. Already contains a ComboBox (`comboProfiles`), Add button (`btnAddProfile`), Delete button (`btnDeleteProfile`), and Rename button (`btnRename`), wired with partial event handlers. Most of the logic is there but several requirements are unmet. See the current gaps enumerated in the Plan of Work.
- `BulkExportWizardData.cs` — `ModuleDataModelBase` subclass that holds `List<ExportProfile> Profiles` (serialized, persistent) and `ExportProfile ActiveProfile` (not serialized, transient working copy). `CreateDefaultProfile()` returns a new profile with default settings but does NOT add it to `Profiles`.
- `ExportProfile.cs` — `[DataContract]` class. `Name` is a string property that implements `INotifyPropertyChanged`. Has a `Clone()` method that returns a deep copy. All settings fields are decorated with `[DataMember]`.
- `BulkExportSummaryStage.cs` — the final stage. Contains `SaveActiveConfig()` which calls `VixenSystem.SaveModuleConfigAsync()`. This is the ONLY place in the module that persists profiles to disk during normal wizard completion.
- `VixenSystem.SaveModuleConfigAsync()` in `src/Vixen.Core/Sys/VixenSystem.cs` — saves the `ModuleStore`, which serializes all `BulkExportWizardData` (including `Profiles`) to disk. This is called after rename and delete so those changes persist immediately without the user needing to finish the wizard.

**WizardStage contract** (from `Common.Controls.Wizard`):

- `StageStart()` — called when the stage becomes the active step. Initialize controls here.
- `CanMoveNext` — bool property the wizard framework queries to enable/disable the Next button. Call `_WizardStageChanged()` to notify the framework that this value may have changed.
- `_WizardStageChanged()` — triggers the framework to re-evaluate `CanMoveNext` and update the Next button.

**Dialog conventions** in Vixen WinForms:

- `TextDialog(string description, string title, string initialValue)` — single-line text input dialog. Returns `DialogResult.OK` or `DialogResult.Cancel`. The user's input is in `dialog.Response`.
- `MessageBoxForm(string message, string title, MessageBoxButtons buttons, SystemIcon icon)` — standard Vixen-themed message box. Check `messageBox.DialogResult == DialogResult.OK` for confirmation.

There are no automated tests in this repository. Validation is manual.

---

## Plan of Work

### Milestone 1: Update JIRA Issue VIX-3632

The JIRA ticket VIX-3632 already exists (the current branch is named for it). This milestone updates it with the full design documented here so that stakeholders have a single source of truth.

Use `mcp__atlassian__editJiraIssue` with issue key `VIX-3632`. The update should include:

Summary: Allow users to delete and rename export profiles from the Export Wizard UI

Description content (convert to ADF format):

    Overview: The bulk export wizard stores named export profiles (format, output folder, universe settings, etc.) but
    provides no way to delete or rename them from the UI. Users currently must edit an XML file on disk. This feature
    replaces Stage 1 of the wizard (currently a simple "Create New or Select Existing" screen) with a full profile
    management screen that supports Add, Rename, and Delete.

    High-Level Design:
    * Stage 1 is replaced by BulkExportConfigStage (already partially implemented), which shows a Configurations
      dropdown with Add (+), Delete (trash), and Rename (pencil) buttons.
    * Add: opens "New Profile" dialog, validates non-blank unique name (trimmed), creates a default profile, adds to
      list. Does not save to disk — incomplete profile is configured through the remaining wizard stages.
    * Rename: opens "Rename Profile" dialog pre-filled with current name, validates non-blank and unique-among-others,
      updates name (trimmed), saves immediately with VixenSystem.SaveModuleConfigAsync(). If new name equals old name,
      silently does nothing (no error).
    * Delete: shows "Confirm Delete" dialog with the message "Are you sure you want to delete <Profile Name>?",
      removes profile, saves immediately. If last profile is removed, re-creates a transient "Default" profile.
    * If no profiles exist at Stage 1, a "Default" profile is created in-memory and shown; it is not written to disk
      unless the user explicitly saves at the Summary stage.
    * Button states: Add always enabled; Delete enabled when at least one profile exists; Rename enabled when a
      profile is selected.
    * The old BulkExportCreateOrSelectStage is retained and only removed after the feature is accepted.

    Acceptance Criteria:
    1. Clicking Add opens a "New Profile" dialog; cancelling does nothing; OK with blank name shows error;
       OK with duplicate name shows error; OK with valid name creates and selects the new profile (name is trimmed).
    2. Clicking Rename opens a "Rename Profile" dialog pre-filled with current name; cancelling does nothing;
       OK with blank name shows error; OK with another profile's name shows error; OK with the same name does nothing;
       OK with a new valid name renames the profile (trimmed) and the updated name appears in the combo.
    3. After a successful Rename, the renamed profile remains selected in the combo.
    4. After a successful Rename, the profile is immediately saved to disk.
    5. Clicking Delete shows "Confirm Delete" with the message "Are you sure you want to delete <Profile Name>?";
       cancelling does nothing; OK removes the profile and selects the first remaining profile.
    6. After a successful Delete, the profiles are immediately saved to disk.
    7. If the last profile is deleted, a "Default" profile appears in the list automatically.
    8. If no profiles exist on entry to Stage 1, a "Default" profile is created and shown but not saved to disk.
    9. All names are trimmed of leading and trailing whitespace.

    Testing Steps:
    1. Launch Vixen and open the Export Wizard.
    2. No-profiles scenario: verify a "Default" profile appears automatically; proceed through all stages to
       Summary WITHOUT saving; close the wizard; reopen — verify the list is still empty (Default was not saved).
    3. Add: click Add, enter "  My Profile  " (spaces), click OK — verify it appears trimmed as "My Profile".
    4. Add duplicate: click Add, enter "My Profile" — verify error dialog; no duplicate created.
    5. Add blank: click Add, leave empty, click OK — verify error dialog.
    6. Rename: select "My Profile", click Rename, change to "  Renamed  ", click OK — verify "Renamed" and selected.
    7. Rename same name: select "Renamed", click Rename, leave unchanged, click OK — verify no error, no change.
    8. Delete: select "Renamed", click Delete; cancel — verify unchanged. Click Delete again, confirm — verify removed.
    9. Delete last profile: delete all profiles — verify a "Default" auto-appears.
    10. Save and persist: complete the wizard saving a profile; restart app; reopen wizard — verify profile persists.

Verification: Record the returned JIRA issue URL in the Decision Log of this plan.

---

### Milestone 2: Wire BulkExportConfigStage into BulkExportWizard

This milestone replaces `BulkExportCreateOrSelectStage` with `BulkExportConfigStage` as the first stage of the wizard. At the end of this milestone the wizard opens and shows the config stage (dropdown and three buttons) as its first step. The old stage files are NOT deleted.

**File to change:** `src/Vixen.Modules/App/ExportWizard/BulkExportWizard.cs`

In the `BulkExportWizard` constructor, replace the first entry in `_stages`:

    // Before:
    new BulkExportCreateOrSelectStage(_data),

    // After:
    new BulkExportConfigStage(_data),

Keep all other stages unchanged.

Build to confirm no compilation errors:

    dotnet build src/Vixen.Modules/App/ExportWizard/ExportWizard.csproj

Then launch Vixen, open the Export Wizard, and confirm Step 1 shows the "Configurations" label, a dropdown combo, and three small icon buttons rather than the radio buttons of the old stage.

---

### Milestone 3: Implement Full BulkExportConfigStage Behavior

This milestone fixes all the gaps in `BulkExportConfigStage.cs` so every requirement is met. The current code has these defects:

- Default profile is not named "Default".
- `comboProfiles_SelectedIndexChanged` assigns a direct reference instead of a clone — wizard-stage edits would silently mutate the saved profile.
- `buttonAddProfile_Click` passes "New Profile" as the `initialName` argument to `GetProfileName` rather than as the dialog title.
- `buttonDeleteProfile_Click` uses message "Are you sure you want to delete this profile?" (no profile name) and title "Delete a Profile" instead of "Confirm Delete".
- `btnRename_Click` passes "New Profile" as the dialog title instead of "Rename Profile"; does not call `SaveModuleConfigAsync`; does not handle the same-name no-op.
- `GetProfileName` does not trim the entered name; does not support an excluded name for the duplicate check (needed for rename).
- No `UpdateButtonStates` helper; buttons are always enabled or disabled based on no logic.
- After delete, if the list becomes empty, no Default is re-created.

**File to change:** `src/Vixen.Modules/App/ExportWizard/BulkExportConfigStage.cs`

#### 3a. Add UpdateButtonStates helper

Add a private method that enforces button-enable rules at every state change:

    private void UpdateButtonStates()
    {
        btnAddProfile.Enabled = true;
        btnDeleteProfile.Enabled = _profiles != null && _profiles.Count > 0;
        btnRename.Enabled = comboProfiles.SelectedItem is ExportProfile;
    }

Call `UpdateButtonStates()` at the end of `PopulateProfiles()`, `comboProfiles_SelectedIndexChanged()`, `buttonAddProfile_Click()`, `buttonDeleteProfile_Click()`, and `btnRename_Click()`.

#### 3b. Fix PopulateProfiles

Replace the current method. The default profile is now named "Default", and the initial ActiveProfile is set via a clone:

    private void PopulateProfiles()
    {
        if (_data.Profiles.Count == 0)
        {
            ExportProfile profile = _data.CreateDefaultProfile();
            profile.Name = "Default";
            _data.Profiles.Add(profile);
        }

        _profiles = new BindingList<ExportProfile>(_data.Profiles);

        comboProfiles.DataSource = new BindingSource { DataSource = _profiles };
        comboProfiles.SelectedIndex = 0;

        _data.ActiveProfile = (_profiles[0]).Clone() as ExportProfile;

        UpdateButtonStates();
        _WizardStageChanged();
    }

#### 3c. Fix comboProfiles_SelectedIndexChanged — clone on selection

Replace the current handler to clone the selected profile. This preserves the saved profile if the user navigates through wizard stages but cancels without saving:

    private void comboProfiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (comboProfiles.SelectedItem is not ExportProfile item)
            return;

        _data.ActiveProfile = item.Clone() as ExportProfile;
        UpdateButtonStates();
        _WizardStageChanged();
    }

#### 3d. Replace GetProfileName — trim, configurable title, optional excluded name

Replace the current method. The `excludeName` parameter lets rename pass the profile's current name so that it is not flagged as a duplicate of itself. The returned name is always trimmed:

    private Tuple<bool, string> GetProfileName(string dialogTitle, string initialName, string? excludeName = null)
    {
        TextDialog dialog = new TextDialog("Enter a name for the profile", dialogTitle, initialName);

        while (dialog.ShowDialog() == DialogResult.OK)
        {
            string trimmed = dialog.Response.Trim();
            if (trimmed == string.Empty)
            {
                var messageBox = new MessageBoxForm(
                    "Profile name can not be blank.", "Error", MessageBoxButtons.OK, SystemIcons.Error);
                messageBox.ShowDialog();
            }
            else if (comboProfiles.Items.Cast<ExportProfile>()
                .Any(p => p.Name == trimmed && p.Name != excludeName))
            {
                var messageBox = new MessageBoxForm(
                    "A profile with the name " + trimmed + @" already exists.",
                    "Warning", MessageBoxButtons.OK, SystemIcons.Warning);
                messageBox.ShowDialog();
            }
            else
            {
                return new Tuple<bool, string>(true, trimmed);
            }
        }

        return new Tuple<bool, string>(false, string.Empty);
    }

#### 3e. Fix buttonAddProfile_Click — correct dialog title, clone for ActiveProfile

    private void buttonAddProfile_Click(object sender, EventArgs e)
    {
        var response = GetProfileName("New Profile", string.Empty);
        if (!response.Item1)
            return;

        ExportProfile item = _data.CreateDefaultProfile();
        item.Name = response.Item2;
        _profiles.Add(item);
        _data.ActiveProfile = item.Clone() as ExportProfile;
        comboProfiles.SelectedIndex = comboProfiles.Items.Count - 1;

        UpdateButtonStates();
        _WizardStageChanged();
    }

Add does NOT call `VixenSystem.SaveModuleConfigAsync()`. The new profile is incomplete until the user configures it through the remaining wizard stages and explicitly saves at the Summary stage.

#### 3f. Fix buttonDeleteProfile_Click — correct dialog, include name, save, re-create Default

    private async void buttonDeleteProfile_Click(object sender, EventArgs e)
    {
        if (comboProfiles.SelectedItem is not ExportProfile item)
            return;

        var messageBox = new MessageBoxForm(
            $"Are you sure you want to delete {item.Name}?",
            "Confirm Delete", MessageBoxButtons.OKCancel, SystemIcons.Warning);
        messageBox.ShowDialog();

        if (messageBox.DialogResult != DialogResult.OK)
            return;

        _profiles.Remove(item);

        if (_profiles.Count == 0)
        {
            ExportProfile defaultProfile = _data.CreateDefaultProfile();
            defaultProfile.Name = "Default";
            _profiles.Add(defaultProfile);
        }

        comboProfiles.SelectedIndex = 0;
        _data.ActiveProfile = (_profiles[0]).Clone() as ExportProfile;

        await VixenSystem.SaveModuleConfigAsync();

        UpdateButtonStates();
        _WizardStageChanged();
    }

This is `async void`, which is acceptable for WinForms event handlers. The `csharp-async` skill review in Milestone 4 must verify that any exception from `SaveModuleConfigAsync` is caught and surfaced in a `MessageBoxForm` rather than swallowed or allowed to crash the application.

#### 3g. Fix btnRename_Click — correct title, exclude current name, same-name no-op, save

    private async void btnRename_Click(object sender, EventArgs e)
    {
        if (comboProfiles.SelectedItem is not ExportProfile profile)
            return;

        var response = GetProfileName("Rename Profile", profile.Name, profile.Name);
        if (!response.Item1)
            return;

        if (response.Item2 == profile.Name)
            return;

        profile.Name = response.Item2;

        await VixenSystem.SaveModuleConfigAsync();

        UpdateButtonStates();
    }

The `excludeName` argument `profile.Name` ensures the duplicate check skips the current profile's own name. If the user clicks OK with the same name unchanged, the `response.Item2 == profile.Name` guard exits without saving — this is the required same-name no-op.

After renaming, the ComboBox should display the updated name automatically because `ExportProfile.Name` implements `INotifyPropertyChanged` and `BindingList<T>` forwards property-change notifications. If testing reveals the name does not update in the drop-down, add `comboProfiles.Refresh()` after the assignment and record the finding in Surprises & Discoveries.

**Build verification:**

    dotnet build src/Vixen.Modules/App/ExportWizard/ExportWizard.csproj

Expected: 0 errors, 0 warnings (treat any new warning as a defect).

---

### Milestone 4: Apply Project Skills

Before considering the feature complete, run the following project skills against the changed files. Read the skill file from `.agents/skills/<skill>/SKILL.md` before applying each one.

- **`csharp-docs`**: Ensure `BulkExportConfigStage` — a `public partial class` — has XML `<summary>` on any public members added or modified. The class itself and its public constructor should have `<summary>` docs if not already present.
- **`dotnet-best-practices`**: Check naming, null handling (pattern matching with `is not`), disposal, and consistency.
- **`csharp-async`**: Review `buttonDeleteProfile_Click` and `btnRename_Click`. Both are `async void` event handlers. Confirm that any exception thrown by `VixenSystem.SaveModuleConfigAsync()` is caught and shown to the user via `MessageBoxForm` rather than being silently swallowed or crashing the application. Wrap the `await` calls in try/catch if needed.
- **`dotnet-design-pattern-review`**: Confirm no architectural concerns with the implementation.

Address all findings before marking this milestone complete.

---

### Milestone 5 (Post-Acceptance): Remove BulkExportCreateOrSelectStage

This milestone must NOT be executed until the feature has been accepted. When approved, remove:

- `src/Vixen.Modules/App/ExportWizard/BulkExportCreateOrSelectStage.cs`
- `src/Vixen.Modules/App/ExportWizard/BulkExportCreateOrSelectStage.Designer.cs`

After deletion, build the full solution to confirm no remaining references:

    msbuild Vixen.sln -m -t:Rebuild -p:Configuration=Release

---

## Concrete Steps (Command Reference)

All commands are run from the repository root `C:\Dev\Vixen` unless stated otherwise.

Build only the ExportWizard project (fast compilation check):

    dotnet build src/Vixen.Modules/App/ExportWizard/ExportWizard.csproj

Build the full solution (Debug):

    msbuild Vixen.sln -m -t:Rebuild -p:Configuration=Debug

Launch the application after a Debug build:

    Debug\Output\Vixen.exe

There are no automated tests in this repository. All verification is manual via the acceptance steps below.

---

## Validation and Acceptance

End-to-end acceptance is performed by running the Vixen application and using the Export Wizard manually.

1. Launch `Debug\Output\Vixen.exe`.
2. Open the Export Wizard. Step 1 must show the "Configurations" label, a dropdown combo, and three small icon buttons (Add, Delete, Rename). The radio buttons from the old stage must not appear.
3. **No-profiles scenario**: If no profiles exist, a "Default" profile must appear in the combo automatically. Delete and Rename buttons must be enabled. Proceed through all stages to Summary without checking "Save as configuration", then close. Reopen the wizard — the profile list must be empty (Default was transient); a new Default auto-appears.
4. **Add**: Click Add. Verify dialog title is "New Profile". Enter `"  Test Profile  "` (with spaces). Click OK. Verify the profile appears as `"Test Profile"` (trimmed) in the combo and is selected.
5. **Add duplicate**: Click Add. Enter `"Test Profile"`. Verify a warning dialog appears. No duplicate is created.
6. **Add blank**: Click Add. Leave the name empty. Click OK. Verify an error dialog appears. No profile is created.
7. **Rename**: Select "Test Profile". Click Rename. Verify dialog title is "Rename Profile" and the field is pre-filled with "Test Profile". Change to `"  Renamed Profile  "`. Click OK. Verify the combo shows "Renamed Profile" (trimmed) and it remains selected.
8. **Rename same name**: Select "Renamed Profile". Click Rename. Leave the name unchanged. Click OK. Verify no error dialog and no observable change.
9. **Rename duplicate**: Add a second profile "OtherProfile". Select "Renamed Profile". Click Rename. Enter "OtherProfile". Verify a warning dialog appears.
10. **Delete with cancel**: Select "Renamed Profile". Click Delete. Verify title is "Confirm Delete" and message is "Are you sure you want to delete Renamed Profile?". Click Cancel. Verify the profile remains.
11. **Delete with confirm**: Click Delete again on "Renamed Profile". Click OK. Verify it is removed and the first remaining profile is selected.
12. **Delete last profile**: Delete all profiles until none remain. Verify a "Default" profile auto-appears.
13. **Wizard flow**: Select any profile. Click Next. Verify the wizard advances to the Sources stage. Complete the wizard to the Summary stage, check "Save as configuration", provide a name, and finish. Reopen the wizard and verify the saved profile persists.

---

## Idempotence and Recovery

The change to `BulkExportWizard.cs` (swapping the stage class name) is a one-line edit and trivially reversible. The changes to `BulkExportConfigStage.cs` rewrite existing event handlers; if a build fails, revert the individual method and re-check. No database migrations or file-system changes are needed. `VixenSystem.SaveModuleConfigAsync()` writes to the module store on disk; if a save fails it logs an error and returns `false` — the worst case is that a rename or delete is not persisted across sessions, which is recoverable.

---

## Risks and Concerns

**BindingList + INotifyPropertyChanged propagation for Rename.** Setting `profile.Name = response.Item2` in `btnRename_Click` may not automatically refresh the ComboBox display. If the combo shows the old name after rename, call `comboProfiles.Refresh()` or rebind the data source. Confirm during Milestone 3 and document the result in Surprises & Discoveries.

**async void exception handling.** The two new `async void` event handlers must catch exceptions from `VixenSystem.SaveModuleConfigAsync()` and surface them via `MessageBoxForm`. The `csharp-async` skill review in Milestone 4 enforces this. If overlooked, a disk-write failure would be silently swallowed.

**Cloning on selection.** If the clone is omitted in `comboProfiles_SelectedIndexChanged`, wizard-stage edits silently mutate the saved profile in-memory. The Decision Log and the code review in Milestone 4 guard against this regression.

**Default profile inadvertently persisted.** The "Default" profile is added to `_data.Profiles` in memory. Any call to `VixenSystem.SaveModuleConfigAsync()` from elsewhere in the app would persist it. In practice this method is only called from the export module, but it is worth noting.

---

## Interfaces and Dependencies

At the end of all milestones the following must be true:

In `src/Vixen.Modules/App/ExportWizard/BulkExportWizard.cs`:

    _stages = new List<WizardStage>
    {
        new BulkExportConfigStage(_data),   // replaces BulkExportCreateOrSelectStage
        new BulkExportSourcesStage(_data),
        new BulkExportControllersStage(_data),
        new BulkExportOutputFormatStage(_data),
        new BulkExportSummaryStage(_data),
        new BulkExportFinishedStage()
    };

In `src/Vixen.Modules/App/ExportWizard/BulkExportConfigStage.cs`, the following private members must exist:

    private void UpdateButtonStates()
    private void PopulateProfiles()
    private Tuple<bool, string> GetProfileName(string dialogTitle, string initialName, string? excludeName = null)
    private void comboProfiles_SelectedIndexChanged(object sender, EventArgs e)
    private void buttonAddProfile_Click(object sender, EventArgs e)
    private async void buttonDeleteProfile_Click(object sender, EventArgs e)   // saves after delete
    private async void btnRename_Click(object sender, EventArgs e)             // saves after rename

    // Wizard contract (unchanged signatures, existing implementation correct)
    public override void StageStart()
    public override bool CanMoveNext { get; }   // _data.Profiles.Count > 0 && _data.ActiveProfile != null

---

## Artifacts and Notes

Dialog patterns used in this feature:

    // Text input dialog (title is second argument)
    TextDialog dialog = new TextDialog("Enter a name for the profile", "New Profile", string.Empty);
    if (dialog.ShowDialog() == DialogResult.OK)
        string name = dialog.Response.Trim();

    // Confirmation dialog
    var messageBox = new MessageBoxForm(
        $"Are you sure you want to delete {profile.Name}?",
        "Confirm Delete", MessageBoxButtons.OKCancel, SystemIcons.Warning);
    messageBox.ShowDialog();
    if (messageBox.DialogResult == DialogResult.OK) { ... }

    // Save to disk
    await VixenSystem.SaveModuleConfigAsync();
