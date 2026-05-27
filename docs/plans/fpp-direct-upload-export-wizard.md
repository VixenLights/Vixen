# FPP Direct Upload — Export Wizard ExecPlan

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain this document in accordance with `.agents/PLANS.md` (repository root).

---

## Purpose / Big Picture

Today the Vixen Export Wizard lets users export sequences and audio to FPP (Falcon Player) only via a local or network-share folder. Recent FPP versions disable Samba by default, making the network-share path difficult to configure. This change adds a "Direct Upload" mode: the user enters the FPP host address, and Vixen uploads fseq files, audio files, and the universe configuration file directly to FPP via its REST API. After this change, a user can complete a full FPP export — including universe file delivery — without ever mounting a network drive or manually uploading files through the FPP web UI.

The user can see it working by opening the Export Wizard, choosing a Falcon Player 2.6+ format, selecting "Direct Upload" on Step 4, entering their FPP host, then completing the wizard. Vixen will upload files directly to the device and the FPP web UI will reflect the new sequences and universe under its file browser.

---

## Progress

- [x] Milestone 1: Create JIRA issue documenting requirements, high-level design, acceptance criteria, and testing steps. → [VIX-3922](https://vixenlights.atlassian.net/browse/VIX-3922)
- [x] Milestone 2: Add `RenameFileAsync` to FPPClient — interface, implementation, unit tests.
- [x] Milestone 3: Update `ExportProfile` data model — new `[DataMember]` fields, update `Clone()`.
- [x] Milestone 4: Step 4 UI — radio buttons, host text box, validation, async ping, `CanMoveNext`.
- [x] Milestone 5: Summary Stage UI — FPP info panel, populate on `StageStart()`, write profile on radio/text change.
- [ ] Milestone 6: Export logic — direct-upload code path in `Export()` and `CreateUniverseFile()`, project reference, `FppDirectUploadService`.
- [ ] Milestone 7: Unit tests for ExportWizard logic (`FppHostValidator`, `ExportProfile` clone, `FppDirectUploadService`).

---

## Surprises & Discoveries

- `DoExport()` in `Vixen.Core/Export/Export.cs` writes to a file path set via the `OutFileName` property. All FSEQ writer implementations (`FSEQWriter`, `FSEQCompressedWriter`, etc.) call `File.Create(sessionData.OutFileName)` internally. There is no stream-based overload. For direct upload the implementer must set `OutFileName` to a temp path, then open a `FileStream` on that temp file and pass it to `UploadSequenceAsync`, then delete the temp file. This is load-bearing: any attempt to skip the temp file will require refactoring `Export.DoExport` and all writers, which is out of scope.

- `Write2xUniverseFile(fileName)` in `Export.cs` (called from `CreateUniverseFile()` in the Summary stage) also writes to a file path. The same temp-file pattern applies for direct upload.

- The ExportWizard `.csproj` does not yet reference the FPPClient project. This reference must be added in Milestone 6.

- The `MockHttpMessageHandler.CreateClient()` helper in the test project directly constructs `FppClient` (an `internal sealed class`), which is accessible because `FPPClient.csproj` declares `InternalsVisibleTo("Vixen.Tests")`. New FPPClient tests follow the exact same pattern.

- There is no `Moq` usage in the existing FPPClient tests — they use a real `FppClient` wired to a mock `HttpMessageHandler`. For ExportWizard tests in Milestone 7, `Moq` is already a test project dependency and must be used to mock `IFppClient`.

- The existing `PingHost()` in `BulkExportOutputFormatStage` is synchronous and blocks the UI thread for up to 500 ms. The new host ping on `Leave` must be `async void` (the only acceptable WinForms async event handler pattern per project async rules) and must call `Task.Run` to move the ping off the UI thread.

- `CanMoveNext` is a synchronous property. Ping results cannot be awaited inside it. The solution is a `_fppHostReachable` bool field on the stage (default `false`). `txtFppHostAddress_TextChanged` resets it to `false` and calls `_WizardStageChanged()`. `txtFppHostAddress_Leave` runs the ping via `Task.Run`, sets `_fppHostReachable` to the result, and calls `_WizardStageChanged()` so the wizard framework re-evaluates `CanMoveNext`. This means the user cannot advance until the host passes a ping check. An error message is shown on Leave if unreachable, explaining why Next remains disabled.

- `FppSystemInfo` already has `HostName`, `HostDescription`, `Platform`, and `Variant` properties — all four fields required by the spec are present in the existing model.

---

## Decision Log

- Decision: Temp-file approach for fseq and universe uploads rather than refactoring `Export.DoExport` writers.
  Rationale: All FSEQ/universe writers are path-bound (`File.Create(OutFileName)`). Refactoring to stream would require changes across six writer classes in `Vixen.Core` and is out of scope for this feature. The temp file is created in `Path.GetTempPath()`, uploaded as a `FileStream`, and deleted in a `finally` block to ensure cleanup.
  Date/Author: (to be set by implementer on start)

- Decision: Host ping on `Leave` gates `CanMoveNext` via a `_fppHostReachable` flag; an unreachable host blocks the Next button.
  Rationale: The user must not be able to proceed to the Summary Stage if the FPP host cannot be reached, because the export will fail at upload time and the Summary Stage FPP info panel will also fail. `CanMoveNext` is synchronous and cannot await a ping directly. The solution is a `private bool _fppHostReachable` field on `BulkExportOutputFormatStage` (default `false`). `txtFppHostAddress_TextChanged` resets it to `false` and calls `_WizardStageChanged()`, immediately disabling Next when the user edits the address. `txtFppHostAddress_Leave` runs `PingHost` via `Task.Run`, stores the result in `_fppHostReachable`, calls `_WizardStageChanged()`, and shows a `MessageBoxForm` (SystemIcons.Error) explaining the host is unreachable if the ping fails. `CanMoveNext` for Direct Upload returns `IsHostAddressValid(host) && _fppHostReachable`. When switching radio buttons back to File Path, `_fppHostReachable` is irrelevant (the Direct Upload branch is not entered).
  Date/Author: (to be set by implementer on start)

- Decision: `FppClientFactory` is instantiated directly in `BulkExportSummaryStage` (not injected via DI).
  Rationale: Vixen's module system does not use constructor injection for WinForms stages. The factory is constructed inline with `new FppClientFactory()` and the client is disposed after each use. This matches every other module-internal usage pattern in the solution.
  Date/Author: (to be set by implementer on start)

- Decision: `RenameFileAsync` swallows HTTP 404 silently (backup rename of non-existent file).
  Rationale: If the co-universes.json file does not exist on the FPP device yet, a 404 from the rename endpoint is expected and harmless. Throwing `FppClientException` for 404 on rename would fail new installs. The method logs a debug message and returns without error on 404.
  Date/Author: (to be set by implementer on start)

- Decision: Profile fields `FppDirectUpload` and `FppHostAddress` are written to `_data.ActiveProfile` immediately from Step 4 event handlers (not deferred to `StageEnd`).
  Rationale: The Summary Stage's `StageStart()` needs to read these values to call `GetSystemInfoAsync()`. Writing on `rdoUploadMode_CheckedChanged` and `txtFppHostAddress_TextChanged` ensures they are always current when the user advances to the Summary Stage.
  Date/Author: (to be set by implementer on start)

- Milestone 1 complete: JIRA issue created as [VIX-3922](https://vixenlights.atlassian.net/browse/VIX-3922) on 2026-05-27.

---

## Outcomes & Retrospective

_(To be written at plan completion.)_

---

## Context and Orientation

This repository is a .NET 10 WinForms/WPF application called Vixen for animating light shows. The feature touches two App modules and the test project.

**FPPClient module** (`src/Vixen.Modules/App/FPPClient/`) is a self-contained HTTP client library for communicating with Falcon Player (FPP) devices. FPP is a Raspberry Pi / Beaglebone Black application that drives physical LED hardware. The module exposes `IFppClient` (defined in `Client/IFppClient.cs`) as its public API surface. An instance is obtained via `IFppClientFactory.Create(FppClientOptions)`. `FppClientOptions` requires a `BaseUrl` such as `"http://192.168.1.50/"`. The `FppClient` internal class sets `BaseAddress` to `BaseUrl + "api/"` and all URL segments are relative to that base. File uploads use `POST api/file/{dirName}/{filename}` with `application/octet-stream` body. The new rename endpoint is `POST api/file/{dirName}/rename/{source}/{dest}` (no body). `FppSystemInfo` is a record in `Models/FppSystemInfo.cs` with properties `HostName`, `HostDescription`, `Platform`, `Variant`, and others — all needed fields already exist.

**ExportWizard module** (`src/Vixen.Modules/App/ExportWizard/`) is a WinForms wizard. The wizard flow is managed by a `WizardStage` base class from `Common.Controls.Wizard`. Step 4 is `BulkExportOutputFormatStage` (a `WizardStage` subclass in `BulkExportOutputFormatStage.cs`). The final export step is `BulkExportSummaryStage` (`BulkExportSummaryStage.cs`). The shared data object passed between stages is `BulkExportWizardData`, accessed in each stage as `_data`. The active export profile is `_data.ActiveProfile` — an instance of `ExportProfile` (a `[DataContract]` class in `ExportProfile.cs`). Settings are persisted by marking properties `[DataMember]`. `IsFalcon2xFormat` is a computed bool property on `ExportProfile` that is `true` when the user has selected a "Falcon Player Sequence 2.6+" format; this gates universe file creation and will also gate the FPP info panel on the summary page.

**Export engine** (`src/Vixen.Core/Export/Export.cs`) exposes `DoExport(ISequence, string format, bool compress, IProgress<ExportProgressStatus> progress, bool matchAudioName)`. The caller sets `export.OutFileName` to a file path before calling `DoExport`. All writers create the file at that path. There is no stream overload; temp files are the only option for direct upload without modifying the core.

**Test project** (`src/Vixen.Tests/`) uses xunit.v3 with `[Fact]` attributes and the AAA pattern. FPPClient tests use `MockHttpMessageHandler.CreateClient(Func<HttpRequestMessage, HttpResponseMessage>)` from `Vixen.Tests/FPPClient/Helpers/MockHttpMessageHandler.cs`. ExportWizard tests will use `Moq` (already in the project) to mock `IFppClient`. Run all tests with `dotnet test src/Vixen.Tests/Vixen.Tests.csproj` from the repository root.

**Designer files**: `BulkExportOutputFormatStage.Designer.cs` and `BulkExportSummaryStage.Designer.cs` contain WinForms auto-generated control layout code. When adding new controls, add them in the Designer file's `InitializeComponent()`: declare the field in the `#endregion` block, instantiate it at the top of `InitializeComponent`, configure it in the middle, and add it to the parent control with `.Controls.Add()`. Keep all event logic in the non-designer `.cs` file.

---

## Plan of Work

### Milestone 1: Create JIRA Issue

The first milestone creates the JIRA tracking issue that anchors all subsequent work. Use the Atlassian MCP tool. If the project key is unknown, call `getVisibleJiraProjects` first to find it.

JIRA issue content:

    Summary: Add FPP Direct Upload capability to Export Wizard

    Description (ADF):
    Overview: The Export Wizard currently supports only file-system paths for FPP output (local or SMB share).
    FPP v7+ disables Samba by default. This feature adds "Direct Upload" via FPP's REST API so users can export
    sequences, audio, and the universe configuration file directly to FPP without mounting a network share.

    High-Level Design:
    * Step 4 gains two radio buttons inside the existing "Falcon Pi Player 2.x" group box: "File Path" (existing)
      and "Direct Upload" (new).
    * In "Direct Upload" mode, a host/IP text box replaces the folder picker. Host/IP is validated on input;
      an async ping fires on focus-leave as a non-blocking warning.
    * The summary page (Falcon 2.6+ + Direct Upload only) shows FPP system info (HostName, HostDescription,
      Platform, Variant) fetched via IFppClient.GetSystemInfoAsync().
    * On export with Direct Upload: fseq is written to a temp file then uploaded to sequences/; audio is opened
      from disk and uploaded to music/; co-universes.json is written to a temp file, the existing remote copy is
      renamed (backup), and the new file is uploaded to config/.
    * Settings (FppDirectUpload, FppHostAddress) are persisted as [DataMember] on ExportProfile.
    * RenameFileAsync is added to IFppClient / FppClient mapping to POST api/file/:DirName/rename/:source/:dest.

    Acceptance Criteria:
    1. When "Direct Upload" is selected and a valid host is entered, clicking Next on Step 4 is enabled.
    2. When "File Path" is selected, the existing folder-picker behavior is unchanged.
    3. On the summary page with Direct Upload + Falcon 2.6+ format, HostName/HostDescription/Platform/Variant
       are displayed from GetSystemInfoAsync.
    4. Export with Direct Upload places the fseq file in the sequences directory of FPP (verified in FPP web UI).
    5. Export with Direct Upload + Include Audio places the audio file in the music directory of FPP.
    6. Export with Direct Upload + Backup Universe + Create Universe: existing co-universes.json is renamed with
       the date-time stamp; new file appears in config/.
    7. If the FPP host is unreachable on Leave, an error dialog is shown and the Next button remains disabled until a reachable host is entered.
    8. Selecting "Direct Upload" and completing the wizard saves FppDirectUpload=true and FppHostAddress to the
       profile; reopening the wizard pre-selects "Direct Upload" with the saved address.
    9. All existing File Path behavior (folder create, path validation, progress, audio copy, universe file write)
       is unchanged.

    Testing Steps:
    1. Open the Export Wizard, choose "Falcon Player Sequence 2.6+" format, select a sequence, go to Step 4.
    2. Verify "File Path" radio is selected by default and folder picker is visible.
    3. Select "Direct Upload" — verify folder picker disappears and host text box appears.
    4. Enter an invalid value (e.g. "http://bad") — verify Next is disabled.
    5. Enter a valid hostname that is unreachable — verify an error is shown on Leave and the Next button is disabled.
    6. Enter a reachable FPP host — proceed to summary. Verify FPP system info panel is populated.
    7. Complete export. Open FPP web UI -> File Manager. Verify fseq in sequences/, audio in music/ (if enabled),
       co-universes.json in config/ (if enabled).
    8. Save config, reopen wizard, verify "Direct Upload" and host address are pre-loaded.
    9. Run all unit tests: dotnet test src/Vixen.Tests/Vixen.Tests.csproj — all pass.

Verification: Record the returned JIRA issue URL in this plan's Decision Log.

---

### Milestone 2: Add `RenameFileAsync` to FPPClient

This milestone extends the FPPClient library with the file-rename capability needed by the backup-universe step. At the end of this milestone, `IFppClient` has a new `RenameFileAsync` method, `FppClient` implements it, and `FppClientRenameTests` cover all cases.

**Files to change:**

- `src/Vixen.Modules/App/FPPClient/Client/IFppClient.cs` — add method declaration
- `src/Vixen.Modules/App/FPPClient/Client/FppClient.cs` — add implementation
- `src/Vixen.Tests/FPPClient/FppClientRenameTests.cs` — create new test file

**Changes to `IFppClient.cs`**

Add after the last existing method. Follow the XML doc style of existing members:

    /// <summary>
    /// Renames a file within a directory on the FPP instance.
    /// </summary>
    /// <param name="dirName">The FPP media directory containing the file (e.g. <c>"config"</c>).</param>
    /// <param name="source">The current filename (without path).</param>
    /// <param name="dest">The desired filename after rename (without path).</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="Task"/> that completes when the rename succeeds.
    /// If the file does not exist (HTTP 404), the method returns without error.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="dirName"/>, <paramref name="source"/>, or <paramref name="dest"/> is
    /// <see langword="null"/> or whitespace.
    /// </exception>
    /// <exception cref="FppClientException">
    /// The HTTP response indicated a non-success status code other than 404.
    /// </exception>
    Task RenameFileAsync(string dirName, string source, string dest,
        CancellationToken cancellationToken = default);

**Changes to `FppClient.cs`**

Add after `UploadVideoAsync`. The FPP rename endpoint is `POST api/file/{dirName}/rename/{source}/{dest}` with no body. A 404 means the source file does not exist; treat it as a no-op and log at Debug. Any other non-success status throws `FppClientException`:

    /// <inheritdoc/>
    public async Task RenameFileAsync(
        string dirName, string source, string dest, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dirName);
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(dest);

        var url = $"file/{Uri.EscapeDataString(dirName)}/rename"
                + $"/{Uri.EscapeDataString(source)}/{Uri.EscapeDataString(dest)}";

        Log.Debug("Renaming {Source} to {Dest} in {DirName}", source, dest, dirName);

        using var response = await _httpClient.PostAsync(url, content: null, cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Log.Debug("Rename of {Source} in {DirName}: file not found (404), treating as no-op",
                source, dirName);
            return;
        }

        if (!response.IsSuccessStatusCode)
        {
            Log.Error("Rename of {Source} in {DirName} failed with HTTP {StatusCode}",
                source, dirName, (int)response.StatusCode);
            throw new FppClientException(
                $"Rename of '{source}' in '{dirName}' failed with HTTP {(int)response.StatusCode}.",
                (int)response.StatusCode);
        }
    }

**New test file: `src/Vixen.Tests/FPPClient/FppClientRenameTests.cs`**

Follow the same namespace, usings, and style as `FppClientUploadTests.cs`. Implement these four tests:

1. `RenameFileAsync_Success_PostsToCorrectUrl` — arrange a 200 response with `{"status":"success","original":"co-universes.json","new":"co-universes.json_1012025-101530"}`, act on `client.RenameFileAsync("config", "co-universes.json", "co-universes.json_1012025-101530", token)`, assert `captured.Method == Post` and `captured.RequestUri.PathAndQuery` contains `file/config/rename/co-universes.json/co-universes.json_1012025-101530`.

2. `RenameFileAsync_FileNotFound_DoesNotThrow` — arrange a 404 response, call `RenameFileAsync`, assert no exception is thrown (task completes successfully).

3. `RenameFileAsync_HttpError_ThrowsFppClientException` — arrange a 500 response, assert `FppClientException` is thrown with `ex.HttpStatusCode == 500`.

4. `RenameFileAsync_NullDirName_ThrowsArgumentNullException` — assert `ArgumentNullException` is thrown synchronously.

**Verification:** `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter "FullyQualifiedName~FppClientRenameTests"` — 4 tests pass. Then run the full suite to confirm no regressions.

---

### Milestone 3: Update `ExportProfile` Data Model

This milestone adds two new persisted fields to `ExportProfile` so the wizard can save and restore the user's Direct Upload preference.

**Files to change:**

- `src/Vixen.Modules/App/ExportWizard/ExportProfile.cs`

`ExportProfile` is decorated with `[DataContract]` and uses `[DataMember]` attributes for persistence. Adding `[DataMember(EmitDefaultValue = false)]` bool and string properties is backward-compatible: existing saved profiles without these fields will deserialize with defaults (`false` and `null`), which maps to "File Path" mode.

Add these two properties after `FalconOutputFolder` (around line 74):

    [DataMember(EmitDefaultValue = false)]
    public bool FppDirectUpload { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string FppHostAddress { get; set; }

In the `Clone()` method (around line 128), add the two new fields to the object initializer:

    FppDirectUpload = FppDirectUpload,
    FppHostAddress = FppHostAddress,

No change to `OnDeserialized` is needed because `false` and `null` are the correct defaults for missing fields.

**Verification:** `dotnet build src/Vixen.Modules/App/ExportWizard/ExportWizard.csproj` completes without errors.

---

### Milestone 4: Step 4 UI — `BulkExportOutputFormatStage`

This milestone modifies Step 4 to add "File Path" / "Direct Upload" radio buttons inside the Falcon group box, conditionally show/hide the folder picker versus the host text box, validate the host/IP format, update `CanMoveNext`, and perform an async ping on focus-leave.

**Files to change:**

- `src/Vixen.Modules/App/ExportWizard/BulkExportOutputFormatStage.Designer.cs`
- `src/Vixen.Modules/App/ExportWizard/BulkExportOutputFormatStage.cs`

**Layout changes in `BulkExportOutputFormatStage.Designer.cs`**

The existing `grpFalcon` group box contains (among others) `txtFalconOutputFolder` and `btnFalconUniverseFolder` which sit at approximately Y=135. The new controls go above them (at Y=110), shifting the folder controls down by 30px (to Y=165). Increase `grpFalcon.Size.Height` by 30 to accommodate.

Add four new control fields in the `#endregion` block:

    private System.Windows.Forms.RadioButton rdoFilePath;
    private System.Windows.Forms.RadioButton rdoDirectUpload;
    private System.Windows.Forms.TextBox txtFppHostAddress;
    private System.Windows.Forms.Label lblFppHostAddress;

In `InitializeComponent()`, instantiate and configure:

    rdoFilePath: Text="File Path", Location=(24,110), AutoSize=true, Checked=true, TabIndex=20
      wire: CheckedChanged += rdoUploadMode_CheckedChanged

    rdoDirectUpload: Text="Direct Upload", Location=(120,110), AutoSize=true, TabIndex=21
      wire: CheckedChanged += rdoUploadMode_CheckedChanged

    lblFppHostAddress: Text="FPP Host / IP:", Location=(24,136), AutoSize=true, Visible=false

    txtFppHostAddress: Location=(120,133), Size=(372,23), Visible=false, TabIndex=22
      wire: Leave += txtFppHostAddress_Leave
      wire: TextChanged += txtFppHostAddress_TextChanged

Add all four to `grpFalcon.Controls`. Shift `txtFalconOutputFolder` and `btnFalconUniverseFolder` to Y=165.

**Logic changes in `BulkExportOutputFormatStage.cs`**

In `StageStart()`, after setting `txtFalconOutputFolder.Text`, add:

    rdoDirectUpload.Checked = _data.ActiveProfile.FppDirectUpload;
    rdoFilePath.Checked = !_data.ActiveProfile.FppDirectUpload;
    txtFppHostAddress.Text = _data.ActiveProfile.FppHostAddress ?? string.Empty;
    UpdateUploadModeVisibility();

Add private method `UpdateUploadModeVisibility()`:

    private void UpdateUploadModeVisibility()
    {
        bool isDirect = rdoDirectUpload.Checked;
        txtFalconOutputFolder.Visible = !isDirect;
        btnFalconUniverseFolder.Visible = !isDirect;
        lblFppHostAddress.Visible = isDirect;
        txtFppHostAddress.Visible = isDirect;
    }

Add event handler `rdoUploadMode_CheckedChanged`:

    private void rdoUploadMode_CheckedChanged(object sender, EventArgs e)
    {
        _data.ActiveProfile.FppDirectUpload = rdoDirectUpload.Checked;
        UpdateUploadModeVisibility();
        _WizardStageChanged();
    }

Add a private field to track whether the current host address has been confirmed reachable by a ping. It defaults to `false` so that Next is disabled until the user tabs out of the field and a ping succeeds:

    private bool _fppHostReachable;

Add static validation helper (also extracted to `FppHostValidator` in Milestone 7, but can be written inline here first):

    private static bool IsHostAddressValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        if (System.Net.IPAddress.TryParse(value, out _)) return true;
        if (value.Contains('/') || value.Contains('\\') || value.Contains(':')) return false;
        return Uri.CheckHostName(value) != UriHostNameType.Unknown;
    }

Add `txtFppHostAddress_TextChanged`. Whenever the user edits the text, reset `_fppHostReachable` to `false` so Next is immediately disabled until the new address is re-pinged:

    private void txtFppHostAddress_TextChanged(object sender, EventArgs e)
    {
        _data.ActiveProfile.FppHostAddress = txtFppHostAddress.Text.Trim();
        _fppHostReachable = false;
        _WizardStageChanged();
    }

Add `txtFppHostAddress_Leave` (async void — acceptable for WinForms event handlers). When the user leaves the text box, ping the host, store the result in `_fppHostReachable`, and refresh the wizard state. If the ping fails, show an error dialog explaining why Next remains disabled:

    private async void txtFppHostAddress_Leave(object sender, EventArgs e)
    {
        var host = txtFppHostAddress.Text.Trim();
        if (!IsHostAddressValid(host)) return; // format invalid; CanMoveNext already false

        bool reachable = await Task.Run(() => PingHost(host)).ConfigureAwait(true);
        _fppHostReachable = reachable;
        _WizardStageChanged();

        if (!reachable)
        {
            var messageBox = new MessageBoxForm(
                $"FPP host '{host}' could not be reached. Verify the host is powered on and connected,\n" +
                "then re-enter the address to try again.",
                "FPP Host Unreachable", MessageBoxButtons.OK, SystemIcons.Error);
            messageBox.ShowDialog(this);
        }
    }

Also reset `_fppHostReachable` in `StageStart()` after restoring the saved host address, then trigger an immediate ping if a saved address is present (so that returning to Step 4 with a previously saved reachable host does not force the user to tab out again):

In `StageStart()`, after the lines that set `rdoDirectUpload.Checked` and `txtFppHostAddress.Text`, add:

    _fppHostReachable = false;
    if (_data.ActiveProfile.FppDirectUpload
        && IsHostAddressValid(_data.ActiveProfile.FppHostAddress))
    {
        // Re-validate the saved host on stage entry so the user is not forced to re-tab
        _ = Task.Run(() => PingHost(_data.ActiveProfile.FppHostAddress))
               .ContinueWith(t =>
               {
                   _fppHostReachable = t.Result;
                   Invoke(_WizardStageChanged);
               }, TaskScheduler.Default);
    }

Update `CanMoveNext` to require both a valid format AND a confirmed-reachable host for Direct Upload:

    public override bool CanMoveNext
    {
        get
        {
            if (!IsIntervalValid()) return false;
            if (_data.ActiveProfile.IsFalconFormat)
            {
                if (rdoDirectUpload.Checked)
                    return IsHostAddressValid(txtFppHostAddress.Text.Trim()) && _fppHostReachable;

                if (CanTestPath() &&
                    Directory.Exists(_data.ActiveProfile.IsFalcon2xFormat
                        ? _data.ActiveProfile.FalconOutputFolder
                        : _data.ActiveProfile.OutputFolder))
                    return true;

                return false;
            }
            bool ok = Directory.Exists(_data.ActiveProfile.OutputFolder);
            if (_data.ActiveProfile.IncludeAudio && !Directory.Exists(_data.ActiveProfile.AudioOutputFolder))
                ok = false;
            return ok;
        }
    }

Guard `txtFalconOutputFolder_Leave` to skip when Direct Upload is active (the control is hidden but the event may still fire):

    private void txtFalconOutputFolder_Leave(object sender, EventArgs e)
    {
        if (rdoDirectUpload.Checked) return;
        UpdateFalconPaths(txtFalconOutputFolder.Text);
        ValidateFalconOutputFolder();
    }

**Verification:** Build and run. Open the Export Wizard with a Falcon 2.6+ format. Confirm: (1) "File Path" radio checked by default. (2) Switching to "Direct Upload" hides folder picker and shows host text box. (3) Typing `192.168.1.50` enables Next. (4) Typing `not valid!!` disables Next. (5) Switching back to "File Path" restores folder-picker behavior. (6) Entering a valid but unreachable IP shows the warning dialog on Leave.

---

### Milestone 5: Summary Stage UI — `BulkExportSummaryStage`

This milestone adds an FPP system info panel to the Summary Stage visible only for Falcon 2.6+ + Direct Upload, populated via `GetSystemInfoAsync()` on `StageStart()`.

**Files to change:**

- `src/Vixen.Modules/App/ExportWizard/BulkExportSummaryStage.Designer.cs`
- `src/Vixen.Modules/App/ExportWizard/BulkExportSummaryStage.cs`

**Layout changes in `BulkExportSummaryStage.Designer.cs`**

The `mainLayoutPanel` currently has 18 rows. Insert 5 new rows after the universe warning row (row 9), shifting the save-config rows (previously rows 10-11) down to rows 15-16. Increment `RowCount` from 18 to 23 and add corresponding `RowStyles`.

Add nine new label fields in the `#endregion` block:

    private System.Windows.Forms.Label lblFppInfo;
    private System.Windows.Forms.Label lblFppHostName;
    private System.Windows.Forms.Label lblFppHostNameValue;
    private System.Windows.Forms.Label lblFppDescription;
    private System.Windows.Forms.Label lblFppDescriptionValue;
    private System.Windows.Forms.Label lblFppPlatform;
    private System.Windows.Forms.Label lblFppPlatformValue;
    private System.Windows.Forms.Label lblFppVariant;
    private System.Windows.Forms.Label lblFppVariantValue;

Instantiate and configure each in `InitializeComponent()`. Set `AutoSize = true` and `Visible = false` on all. Set label text: `lblFppInfo.Text = "FPP Device Info"` (spans both columns), `lblFppHostName.Text = "Host Name"`, `lblFppDescription.Text = "Description"`, `lblFppPlatform.Text = "Platform"`, `lblFppVariant.Text = "Variant"`. The value labels have empty text initially. Add to `mainLayoutPanel` at rows 10-14 using `mainLayoutPanel.Controls.Add(control, col, row)` and call `mainLayoutPanel.SetColumnSpan(lblFppInfo, 2)`.

**Logic changes in `BulkExportSummaryStage.cs`**

In `ConfigureSummary()`, after the universe warning block, hide all FPP info controls initially:

    lblFppInfo.Visible = false;
    lblFppHostName.Visible = lblFppHostNameValue.Visible = false;
    lblFppDescription.Visible = lblFppDescriptionValue.Visible = false;
    lblFppPlatform.Visible = lblFppPlatformValue.Visible = false;
    lblFppVariant.Visible = lblFppVariantValue.Visible = false;

Change `StageStart()` signature to `async void` (overrides `virtual void` — `async void` is the correct WinForms override pattern). After `ConfigureSummary()`, conditionally fetch FPP info:

    public override async void StageStart()
    {
        // ... existing code unchanged ...
        ConfigureSummary();

        if (_data.ActiveProfile.IsFalcon2xFormat && _data.ActiveProfile.FppDirectUpload)
            await PopulateFppInfoAsync().ConfigureAwait(false);
    }

Add `PopulateFppInfoAsync()`:

    private async Task PopulateFppInfoAsync()
    {
        var host = _data.ActiveProfile.FppHostAddress;
        if (string.IsNullOrWhiteSpace(host)) return;

        try
        {
            var factory = new FppClientFactory();
            await using var client = factory.Create(new FppClientOptions { BaseUrl = $"http://{host}/" });
            var info = await client.GetSystemInfoAsync().ConfigureAwait(true); // true = stay on UI thread

            lblFppHostNameValue.Text = info.HostName;
            lblFppDescriptionValue.Text = info.HostDescription;
            lblFppPlatformValue.Text = info.Platform;
            lblFppVariantValue.Text = info.Variant;

            lblFppInfo.Visible = true;
            lblFppHostName.Visible = lblFppHostNameValue.Visible = true;
            lblFppDescription.Visible = lblFppDescriptionValue.Visible = true;
            lblFppPlatform.Visible = lblFppPlatformValue.Visible = true;
            lblFppVariant.Visible = lblFppVariantValue.Visible = true;
        }
        catch (Exception ex)
        {
            Logging.Warn(ex, "Could not retrieve FPP system info for host {Host}", host);
            lblFppInfo.Text = "FPP Device Info (unavailable -- check host address)";
            lblFppInfo.Visible = true;
        }
    }

`ConfigureAwait(true)` on `GetSystemInfoAsync` is intentional: it marshals the continuation back to the WinForms UI thread so label assignments do not require `Invoke`.

Add required using directives at the top of `BulkExportSummaryStage.cs`:

    using VixenModules.App.FPPClient.Client;

**Verification:** Build and run. Select Direct Upload, enter a real FPP host, proceed to Summary. Verify the "FPP Device Info" section appears and shows correct HostName, Description, Platform, Variant. Verify it does not appear when "File Path" is selected.

---

### Milestone 6: Export Logic — Direct Upload Code Path

This milestone adds the Direct Upload execution path in `BulkExportSummaryStage`, introduces the `FppDirectUploadService` helper class, and adds the project reference.

**Files to change:**

- `src/Vixen.Modules/App/ExportWizard/ExportWizard.csproj`
- `src/Vixen.Modules/App/ExportWizard/BulkExportSummaryStage.cs`
- `src/Vixen.Modules/App/ExportWizard/FppDirectUploadService.cs` (new file)

**Project reference in `ExportWizard.csproj`**

Add with `Private=false` (Copy Local = No) to match solution conventions:

    <ItemGroup>
      <ProjectReference Include="..\FPPClient\FPPClient.csproj">
        <Private>false</Private>
      </ProjectReference>
    </ItemGroup>

**New file: `src/Vixen.Modules/App/ExportWizard/FppDirectUploadService.cs`**

This internal class receives `IFppClient` via its primary constructor (matching best-practices DI pattern), isolating all FPP upload operations for testability:

    using NLog;
    using VixenModules.App.FPPClient.Client;

    namespace VixenModules.App.ExportWizard;

    /// <summary>
    /// Encapsulates the FPP direct-upload operations for sequences, audio, and universe files.
    /// </summary>
    internal sealed class FppDirectUploadService(IFppClient client)
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>Uploads an already-written fseq file to the FPP sequences directory.</summary>
        internal async Task UploadSequenceFileAsync(
            string tempPath, string fseqFileName, CancellationToken ct = default)
        {
            await using var stream = File.OpenRead(tempPath);
            await client.UploadSequenceAsync(fseqFileName, stream, ct).ConfigureAwait(false);
        }

        /// <summary>Uploads an audio file to the FPP music directory.</summary>
        internal async Task UploadAudioFileAsync(
            string sourcePath, string destFileName, CancellationToken ct = default)
        {
            await using var stream = File.OpenRead(sourcePath);
            await client.UploadMusicAsync(destFileName, stream, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Renames the existing co-universes.json to a backup filename.
        /// Silently no-ops if the file does not exist on the FPP device.
        /// </summary>
        internal async Task BackupUniverseFileAsync(string backupName, CancellationToken ct = default)
        {
            await client.RenameFileAsync("config", "co-universes.json", backupName, ct)
                .ConfigureAwait(false);
        }

        /// <summary>Uploads a universe JSON file to the FPP config directory.</summary>
        internal async Task UploadUniverseFileAsync(string tempPath, CancellationToken ct = default)
        {
            await using var stream = File.OpenRead(tempPath);
            await client.UploadFileAsync("config", "co-universes.json", stream, ct).ConfigureAwait(false);
        }
    }

**Changes to `BulkExportSummaryStage.cs`**

Update the using directives:

    using VixenModules.App.FPPClient.Client;
    using VixenModules.App.FPPClient.Models;

Update `Export()` to branch on direct upload:

    private async Task Export(ISequence sequence, IProgress<ExportProgressStatus> progress)
    {
        // ... existing audio filename resolution unchanged (lines ~295-309) ...

        bool isDirect = _data.ActiveProfile.FppDirectUpload && _data.ActiveProfile.IsFalcon2xFormat;

        if (!isDirect)
        {
            // existing file-path code path -- unchanged
            bool canOutput = true;
            if (_data.ActiveProfile.IncludeAudio && _data.Export.AudioFilename != string.Empty)
            {
                if (!Directory.Exists(_data.ActiveProfile.AudioOutputFolder))
                    canOutput = CreateDirectory(_data.ActiveProfile.AudioOutputFolder);
                if (canOutput)
                {
                    string audioOutputPath = Path.Combine(_data.ActiveProfile.AudioOutputFolder,
                        _data.ActiveProfile.RenameAudio
                            ? _data.Export.FormatAudioFileName(sequence.Name)
                            : Path.GetFileName(_data.Export.AudioFilename));
                    File.Copy(_data.Export.AudioFilename, audioOutputPath, true);
                }
            }
            if (!Directory.Exists(_data.ActiveProfile.OutputFolder))
                canOutput = CreateDirectory(_data.ActiveProfile.OutputFolder);
            if (canOutput)
            {
                _data.Export.OutFileName = Path.Combine(_data.ActiveProfile.OutputFolder,
                    sequence.Name + "." + _data.Export.ExportFileTypes[_data.ActiveProfile.Format]);
                await _data.Export.DoExport(sequence, _data.ActiveProfile.Format,
                    _data.ActiveProfile.EnableCompression, progress, _data.ActiveProfile.RenameAudio);
            }
        }
        else
        {
            await ExportDirect(sequence, progress);
        }
    }

Add `ExportDirect()`:

    private async Task ExportDirect(ISequence sequence, IProgress<ExportProgressStatus> progress)
    {
        var factory = new FppClientFactory();
        await using var client = factory.Create(
            new FppClientOptions { BaseUrl = $"http://{_data.ActiveProfile.FppHostAddress}/" });
        var svc = new FppDirectUploadService(client);

        // Export fseq to temp file, then upload
        var tempFseq = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".fseq");
        try
        {
            _data.Export.OutFileName = tempFseq;
            await _data.Export.DoExport(sequence, _data.ActiveProfile.Format,
                _data.ActiveProfile.EnableCompression, progress, _data.ActiveProfile.RenameAudio);

            var fseqFileName = sequence.Name + "."
                + _data.Export.ExportFileTypes[_data.ActiveProfile.Format];
            await svc.UploadSequenceFileAsync(tempFseq, fseqFileName).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logging.Error(ex, "Direct upload of sequence {Name} failed", sequence.Name);
            ShowDirectUploadError(ex.Message);
        }
        finally
        {
            if (File.Exists(tempFseq)) File.Delete(tempFseq);
        }

        // Upload audio if needed
        if (_data.ActiveProfile.IncludeAudio && !string.IsNullOrEmpty(_data.Export.AudioFilename))
        {
            try
            {
                var audioFileName = _data.ActiveProfile.RenameAudio
                    ? _data.Export.FormatAudioFileName(sequence.Name)
                    : Path.GetFileName(_data.Export.AudioFilename);
                await svc.UploadAudioFileAsync(_data.Export.AudioFilename, audioFileName)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Direct upload of audio for {Name} failed", sequence.Name);
                ShowDirectUploadError(ex.Message);
            }
        }
    }

Update `CreateUniverseFile()` to branch on direct upload:

    private async Task CreateUniverseFile()
    {
        if (!_data.ActiveProfile.IsFalcon2xFormat) return;

        if (_data.ActiveProfile.FppDirectUpload)
        {
            await CreateUniverseFileDirect();
        }
        else
        {
            // existing file-path logic -- unchanged
            var path = Path.Combine(_data.ActiveProfile.FalconOutputFolder, "config");
            if (!Directory.Exists(path)) CreateDirectory(path);
            string fileName = Path.Combine(path, "co-universes.json");
            if (_data.ActiveProfile.BackupUniverseFile && File.Exists(fileName))
            {
                var now = DateTime.Now;
                var newFile = $"{fileName}_{now.Month}{now.Day}{now.Year}-{now.Hour}{now.Minute}{now.Second}";
                File.Move(fileName, newFile);
            }
            await _data.Export.Write2xUniverseFile(fileName);
        }
    }

Add `CreateUniverseFileDirect()`:

    private async Task CreateUniverseFileDirect()
    {
        if (!_data.ActiveProfile.CreateUniverseFile && !_data.ActiveProfile.BackupUniverseFile) return;

        var factory = new FppClientFactory();
        await using var client = factory.Create(
            new FppClientOptions { BaseUrl = $"http://{_data.ActiveProfile.FppHostAddress}/" });
        var svc = new FppDirectUploadService(client);

        try
        {
            if (_data.ActiveProfile.BackupUniverseFile)
            {
                var now = DateTime.Now;
                var backupName = $"co-universes.json_{now.Month}{now.Day}{now.Year}"
                               + $"-{now.Hour}{now.Minute}{now.Second}";
                await svc.BackupUniverseFileAsync(backupName).ConfigureAwait(false);
            }

            if (_data.ActiveProfile.CreateUniverseFile)
            {
                var tempUniverse = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");
                try
                {
                    await _data.Export.Write2xUniverseFile(tempUniverse);
                    await svc.UploadUniverseFileAsync(tempUniverse).ConfigureAwait(false);
                }
                finally
                {
                    if (File.Exists(tempUniverse)) File.Delete(tempUniverse);
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Error(ex, "Direct upload of universe file to {Host} failed",
                _data.ActiveProfile.FppHostAddress);
            ShowDirectUploadError(ex.Message);
        }
    }

Add error-display helper that is thread-safe:

    private void ShowDirectUploadError(string message)
    {
        if (InvokeRequired)
        {
            Invoke(new Action<string>(ShowDirectUploadError), message);
            return;
        }
        var msgBox = new MessageBoxForm(
            $"Direct upload failed:\n{message}",
            "FPP Upload Error", MessageBoxButtons.OK, SystemIcons.Error);
        msgBox.ShowDialog(this);
    }

**Verification:** Build the full solution. Run the application and test both code paths: (1) Direct Upload to a real or simulated FPP instance (or a local HTTP server that returns 200 for any POST). (2) File Path export to a local folder. Confirm the file-path path is unchanged.

---

### Milestone 7: Unit Tests for ExportWizard Logic

This milestone adds unit tests for `FppHostValidator`, `ExportProfile` clone correctness, and `FppDirectUploadService`. WinForms controls cannot be unit-tested in isolation, so logic is tested through the extracted classes.

**Files to change / create:**

- `src/Vixen.Modules/App/ExportWizard/FppHostValidator.cs` (new — extract from Milestone 4 inline logic)
- `src/Vixen.Modules/App/ExportWizard/ExportWizard.csproj` (add `InternalsVisibleTo`)
- `src/Vixen.Tests/Vixen.Tests.csproj` (add project reference to ExportWizard)
- `src/Vixen.Tests/ExportWizard/FppHostValidatorTests.cs` (new)
- `src/Vixen.Tests/ExportWizard/ExportProfileCloneTests.cs` (new)
- `src/Vixen.Tests/ExportWizard/FppDirectUploadServiceTests.cs` (new)

**`FppHostValidator.cs`**

Create in `src/Vixen.Modules/App/ExportWizard/`:

    namespace VixenModules.App.ExportWizard;

    /// <summary>Validates FPP host name or IP address strings.</summary>
    internal static class FppHostValidator
    {
        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="value"/> is a valid IPv4/IPv6 address
        /// or a valid DNS hostname; <see langword="false"/> otherwise.
        /// </summary>
        internal static bool IsHostAddressValid(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (System.Net.IPAddress.TryParse(value, out _)) return true;
            if (value.Contains('/') || value.Contains('\\') || value.Contains(':')) return false;
            return Uri.CheckHostName(value) != UriHostNameType.Unknown;
        }
    }

Update `BulkExportOutputFormatStage.cs` to call `FppHostValidator.IsHostAddressValid(...)` instead of the local inline method (remove the private `IsHostAddressValid` and delegate to the static class).

**`ExportWizard.csproj` — `InternalsVisibleTo`**

    <ItemGroup>
      <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleToAttribute">
        <_Parameter1>Vixen.Tests</_Parameter1>
      </AssemblyAttribute>
    </ItemGroup>

**`Vixen.Tests.csproj` — project reference**

    <ProjectReference Include="..\Vixen.Modules\App\ExportWizard\ExportWizard.csproj">
      <Private>false</Private>
    </ProjectReference>

**`FppHostValidatorTests.cs`** — 8 test cases:

1. `IsHostAddressValid_ValidIpv4_ReturnsTrue` — `"192.168.1.50"` -> `true`
2. `IsHostAddressValid_ValidIpv6_ReturnsTrue` — `"::1"` -> `true`
3. `IsHostAddressValid_ValidHostname_ReturnsTrue` — `"fpp"` -> `true`
4. `IsHostAddressValid_ValidFqdn_ReturnsTrue` — `"fpp.local"` -> `true`
5. `IsHostAddressValid_Null_ReturnsFalse` — `null` -> `false`
6. `IsHostAddressValid_EmptyString_ReturnsFalse` — `""` -> `false`
7. `IsHostAddressValid_ContainsScheme_ReturnsFalse` — `"http://192.168.1.50"` -> `false`
8. `IsHostAddressValid_ContainsBackslash_ReturnsFalse` — `"\\fpp\share"` -> `false`

**`ExportProfileCloneTests.cs`** — 3 test cases:

1. `Clone_CopiesFppDirectUpload_True` — set `FppDirectUpload = true`, clone, assert clone's value is `true`.
2. `Clone_CopiesFppHostAddress` — set `FppHostAddress = "192.168.1.10"`, clone, assert values match.
3. `Clone_DefaultFppDirectUploadIsFalse` — new `ExportProfile`, assert `FppDirectUpload == false`.

Note: `ExportProfile`'s public constructor requires `name, format, sequenceFolder, audioFolder` arguments. Supply plausible values (e.g., `"test", "Falcon Player Sequence 2.6+", "C:\\seq", "C:\\music"`).

**`FppDirectUploadServiceTests.cs`** — 4 test cases using `Moq`:

    using Moq;
    using VixenModules.App.ExportWizard;
    using VixenModules.App.FPPClient.Client;

1. `UploadSequenceFileAsync_CallsUploadSequenceAsync_WithCorrectFilename`:
   Write a temp file with known bytes. Call `svc.UploadSequenceFileAsync(tempPath, "test.fseq")`.
   Verify `mock.Verify(c => c.UploadSequenceAsync("test.fseq", It.IsAny<Stream>(), default), Times.Once)`.

2. `UploadAudioFileAsync_CallsUploadMusicAsync_WithCorrectFilename`:
   Similar for audio. Verify `UploadMusicAsync("song.mp3", ...)` called once.

3. `BackupUniverseFileAsync_CallsRenameFileAsync_WithConfigDir`:
   Call `svc.BackupUniverseFileAsync("co-universes.json_1012025-101530")`.
   Verify `RenameFileAsync("config", "co-universes.json", "co-universes.json_1012025-101530", default)` once.

4. `UploadUniverseFileAsync_CallsUploadFileAsync_WithConfigDir`:
   Write a temp JSON file. Call `svc.UploadUniverseFileAsync(tempPath)`.
   Verify `UploadFileAsync("config", "co-universes.json", It.IsAny<Stream>(), default)` once.

For Moq setup, return `Task.CompletedTask` for all mocked methods:
`mockClient.Setup(c => c.UploadSequenceAsync(...)).Returns(Task.CompletedTask);`

**Verification:** `dotnet test src/Vixen.Tests/Vixen.Tests.csproj` — all tests pass. Minimum new passing test count: 4 (rename) + 8 (host validator) + 3 (profile clone) + 4 (upload service) = 19 new tests.

---

## Concrete Steps (Command Reference)

All commands are run from the repository root `C:\Dev\Vixen` unless stated otherwise.

Build a single project to check compilation:

    dotnet build src/Vixen.Modules/App/FPPClient/FPPClient.csproj
    dotnet build src/Vixen.Modules/App/ExportWizard/ExportWizard.csproj

Run only FPPClient rename tests:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter "FullyQualifiedName~FppClientRenameTests"

Run only ExportWizard unit tests:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter "FullyQualifiedName~ExportWizard"

Run full test suite:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj

Expected output on success:

    Passed! - Failed: 0, Passed: N, Skipped: 0, Total: N

---

## Validation and Acceptance

End-to-end acceptance is performed by running the Vixen application and using the Export Wizard manually:

1. Launch `Vixen.exe` from the output directory.
2. Open the Export Wizard.
3. Select at least one sequence file on Step 1/2. Choose "Falcon Player Sequence 2.6+" on Step 4.
4. Verify "File Path" is the default selection and the folder picker is visible.
5. Select "Direct Upload". Verify the folder picker is replaced by a host/IP label and text box.
6. Type `not-valid!!`. Verify the Next button is disabled.
7. Type a valid but unreachable IP (e.g. `10.99.99.99`). Tab away. Verify an error dialog appears and the Next button remains disabled. Correct the address to a reachable host, tab away again, and verify Next becomes enabled after the ping succeeds.
8. Type a valid, reachable FPP host. Proceed to Step 5 (Summary). Verify the "FPP Device Info" section shows the device's hostname, description, platform, and variant.
9. Click Finish / Export. After completion, open FPP web UI -> File Manager. Verify: fseq in `sequences/`, audio in `music/` (if Include Audio checked), `co-universes.json` in `config/` (if Create Universe File checked), and a dated backup if Backup Universe File was checked.
10. Check "Save export settings" with a profile name and finish. Re-open the Export Wizard, select the saved profile. Verify "Direct Upload" is pre-selected and the host address is restored.
11. Switch back to "File Path" and export. Verify output files are written to the local folder; FPP device is not contacted.

---

## Idempotence and Recovery

All file writes in the direct upload path go to `Path.GetTempPath()` with random names and are cleaned up in `finally` blocks. If the export is cancelled or crashes, orphaned temp files may remain in the OS temp directory but will be cleaned by the OS eventually. If the FPP host is unreachable during upload, `FppClientException` is caught, logged, and surfaced in a `MessageBoxForm`. The export loop continues with the next sequence. The user can re-run the wizard at any time.

---

## Risks

**Temp file I/O overhead for fseq export.** The `DoExport` engine writes to disk, then the upload reads it back. For large sequences this adds full-file I/O. A cleaner alternative would be a `Stream`-based overload in `Vixen.Core`, but refactoring six writer classes is out of scope. The temp-file approach is safe and correct.

**Ping timeout blocking UI.** `PingHost()` is synchronous. The new `txtFppHostAddress_Leave` handler and the `StageStart` re-ping both call `PingHost` via `Task.Run` so neither blocks the UI thread. The 500 ms timeout in `PingHost` bounds the worst case. While the ping is in flight the Next button remains disabled (correct behavior); once the result arrives `_WizardStageChanged()` is invoked on the UI thread to re-evaluate `CanMoveNext`.

**RenameFileAsync on non-existent file returns 404.** `RenameFileAsync` treats 404 as a no-op (log at Debug, do not throw). This is documented in the Decision Log and in the XML doc on the interface method.

**`StageStart` becoming `async void`.** Any exception thrown after the first `await` is not observed by the caller. `PopulateFppInfoAsync` wraps everything in `try/catch` and logs; no unobserved exceptions propagate.

**ExportWizard -> FPPClient project reference.** Since FPPClient uses `EnableDynamicLoading=true`, both modules must be present at runtime. Both assemblies already go to the same output directory; no extra deployment step is needed.

---

## Interfaces and Dependencies

At the end of all milestones the following types must exist:

In `src/Vixen.Modules/App/FPPClient/Client/IFppClient.cs`:

    Task RenameFileAsync(string dirName, string source, string dest,
        CancellationToken cancellationToken = default);

In `src/Vixen.Modules/App/ExportWizard/ExportProfile.cs`:

    [DataMember(EmitDefaultValue = false)] public bool FppDirectUpload { get; set; }
    [DataMember(EmitDefaultValue = false)] public string FppHostAddress { get; set; }

In `src/Vixen.Modules/App/ExportWizard/FppHostValidator.cs` (new):

    internal static class FppHostValidator
    {
        internal static bool IsHostAddressValid(string? value) { ... }
    }

In `src/Vixen.Modules/App/ExportWizard/FppDirectUploadService.cs` (new):

    internal sealed class FppDirectUploadService(IFppClient client)
    {
        internal Task UploadSequenceFileAsync(string tempPath, string fseqFileName, CancellationToken ct = default);
        internal Task UploadAudioFileAsync(string sourcePath, string destFileName, CancellationToken ct = default);
        internal Task BackupUniverseFileAsync(string backupName, CancellationToken ct = default);
        internal Task UploadUniverseFileAsync(string tempPath, CancellationToken ct = default);
    }

---

## Artifacts and Notes

FPP rename endpoint confirmed in `docs/references/fpp-rest-api.md`:

    POST api/file/:DirName/rename/:source/:dest
    Response: { "status": "success", "original": "test.py", "new": "test2.py" }

HTTP POST with no body in `HttpClient`:

    using var response = await _httpClient.PostAsync(url, content: null, cancellationToken).ConfigureAwait(false);

Passing `null` for `HttpContent` is valid and sends a POST with no body (correct for the rename endpoint).