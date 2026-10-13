# Restore export output configuration before handling edits (VIX-4022)


This ExecPlan is a living document maintained in accordance with .agents/PLANS.md. Keep Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective current. This document records the design and tracks implementation progress; Milestones 1–3 are complete, with implementation, automated validation, user-reported hands-on UI results, and Jira completion evidence recorded below. Issue: https://vixenlights.atlassian.net/browse/VIX-4022, “Export wizard shows output path does not exist before fully loading config”.

## Purpose / Big Picture


A saved Direct Upload export configuration must open its output stage with Direct Upload and its saved host selected, without asking the user to create an irrelevant output directory. Returning to this stage or changing the export format while using Direct Upload must likewise avoid directory prompts. File output must still validate its directory when the user edits an applicable file destination.

## Progress


- [x] (2026-10-13) Read VIX-4022, the project analysis skill, .agents/PLANS.md, the original FPP Direct Upload design, and the output-stage source. Issue has no attachments, comments, or linked issues. Initial working tree was clean.
- [x] (2026-10-13) Identify the synchronous restoration/event ordering defect and save this implementation plan.
- [x] (2026-10-13) Milestone 1: Updated VIX-4022 with user-facing scope, acceptance criteria, and test plan; read back and confirmed. Status and assignee unchanged.
- [x] (2026-10-13) Milestone 2: Implemented restoration suppression and mode-aware validation in the output stage; Release build and 41 ExportWizard tests passed. User reports reproducing the issue before the fix and confirming the Direct Upload, File Path, saved-setting, and host-validation behavior after the fix.
- [x] (2026-10-13) Milestone 3: Confirmed the existing Jira description matches delivered behavior, added and read back validation comment 40586; issue remains In Progress and no workflow change was made.

## Surprises & Discoveries


StageStart selects the saved format before restoring the upload radio buttons. outputFormatComboBox_SelectedIndexChanged calls UpdateFalconPaths and ValidateFalconOutputFolder for Falcon formats without checking the upload mode. ValidateFalconOutputFolder can display exactly the reported “The selected output path does not exist. Do you want to create it?” dialog. This supports the reported hypothesis directly from source; the user subsequently reproduced the dialog on the pre-change build and confirmed its absence after the fix for the saved Direct Upload profile.

UpdateFalconPaths also rewrites OutputFolder and AudioOutputFolder from FalconOutputFolder. Restoring controls is therefore capable of changing stored profile values, beyond displaying an early dialog. Checkbox and radio/text change handlers also write into the active profile as controls are restored. Merely moving the radio-button assignment earlier does not suppress these side effects or the format handler's unconditional directory validation.

The original design in docs/plans/export/fpp-direct-upload-export-wizard.md promises saved mode/host restoration and distinguishes Direct Upload host validation from File Path validation. That document's standalone dotnet test commands are outdated for the current C++/CLI dependencies; use the full MSBuild workflow below, as required by AGENTS.md.

The Gortex file impact query returned an incomplete lower bound, five direct dependents and no test files. Its HIGH risk label does not establish a specific runtime risk; the query is not complete caller/test coverage. Reassess the actual changed symbols during implementation.

The Milestone 2 Gortex detect pass identified the output-stage type and changed handlers. Its impact remains a lower bound, and the test coverage query found no covering tests (0/20 changed symbols). Gortex guard analysis found no configured rules. The Rider file-problem inspection reported existing warnings in the file; no unrelated cleanup was made. The Release solution rebuild succeeded with 74 warnings and 0 errors; the focused test run passed 41 tests with 0 skipped.

The user subsequently reported reproducing the early directory prompt before the fix and confirming it no longer appears for the saved Direct Upload profile afterward. The user also observed saved settings retained, expected prompting for a File Path profile with a bad path, no warning for a valid File Path, Next enabled for a reachable Direct Upload host, and the expected unreachable-host warning for an invalid/unreachable host.

## Decision Log


Decision: Treat restoring an existing profile as reading settings into controls, not as a user edit. Add a private restoration flag with try/finally in StageStart and suppress handlers while it is set. Rationale: WinForms assignments synchronously raise events, and ordering changes alone cannot prevent the observed prompt or profile mutations. Date/Author: 2026-10-13 / Codex.

Decision: Gate Falcon directory updates and validation on the applicable file-output mode as well as the restoration flag. Rationale: A user changing the Falcon format while Direct Upload is selected can trigger the same file-only handler outside restoration. Date/Author: 2026-10-13 / Codex.

Decision: Preserve the current synchronous StageStart signature and the existing background host-ping workflow. Rationale: This bug is established by synchronous event ordering; changing the networking lifecycle, host support, ping policy, or upload service is outside the fix. Date/Author: 2026-10-13 / Codex.

Decision: Use a focused manual WinForms regression matrix and existing ExportWizard tests; do not add a boolean-only unit test or a new UI automation framework for this fix. Rationale: The failure depends on actual control events and modal dialogs. An isolated flag test would not prove that initialization avoids those events' side effects. Date/Author: 2026-10-13 / Codex.

Decision: Prepare Jira updates as execution milestones without posting them during design. Rationale: The current request authorizes solution design and the local plan. Each implementation milestone has a manual review boundary required by the analysis skill; commits require explicit user instruction under AGENTS.md. Date/Author: 2026-10-13 / Codex.

Decision: Keep the Milestone 2 implementation confined to the planned output-stage changes and do not add tests or refactor helpers solely to satisfy broad graph warnings. Rationale: The plan explicitly calls for a private restoration flag, targeted handler guards, no boolean-only unit test, and manual control-event verification; Gortex found no existing coverage, so the required UI evidence was collected through the planned manual scenarios rather than replaced by an unrelated test. Date/Author: 2026-10-13 / Codex.

Decision: Treat the user's report of the pre-fix reproduction and post-fix validation matrix as the manual acceptance evidence for Milestone 2. Rationale: The user exercised the old behavior and the changed application, and reported retained settings and the distinct Direct Upload/File Path outcomes specified by this plan. Date/Author: 2026-10-13 / Codex.

Decision: Keep the existing Jira description unchanged at Milestone 3 and add a completion comment only. Rationale: The fetched description already matched the delivered Direct Upload and File Path behavior and acceptance criteria; changing workflow state was not authorized. Jira readback confirmed comment 40586 and status In Progress with the assignee unchanged. Date/Author: 2026-10-13 / Codex.

## Outcomes & Retrospective


Milestone 1 is complete. VIX-4022 records the observable Direct Upload restoration behavior, File Path behavior to preserve, acceptance criteria, and user test plan; at that milestone the issue was Accepted and assigned as before. Milestone 2 is complete: the output stage now suppresses restoration side effects and makes Falcon path validation mode-aware. The full Release rebuild, focused ExportWizard tests, and user-reported manual scenarios passed. Milestone 3 is complete: the current Jira description already matched delivered behavior, so it was left unchanged. Completion and validation comment 40586 was added and read back. At readback the issue was In Progress and remained assigned to Jeff Uchitjil; no workflow change was made. The Release rebuild had 74 warnings and 0 errors; the ExportWizard tests passed 41, failed 0, skipped 0. User-reported checks confirmed no early prompt for Direct Upload, preserved settings, correct File Path warning behavior, and expected host reachability feedback.

## Architecture Design: VIX-4022


Detected IDE environment: JetBrains Rider 2026.2, established by session metadata. Use Rider's file navigation, inspection view, and Test Runner. Tool availability must be checked at execution; no Rider automation hook was assumed to have run during analysis. If supported, apply focused snippets through Rider's “Apply snippet from chat” workflow, subject to the repository's mandatory Gortex mutation tools.

Core strategy: Maintain this existing WinForms stage with a small restoration guard and explicit destination-mode checks, following .agents/skills/dotnet-best-practices/SKILL.md. The stage is not a Catel WPF view model; Catel routing is unnecessary. Use .agents/skills/csharp-docs/SKILL.md for the changed public StageStart override and CanMoveNext behavior documentation. New private implementation details do not require a new interface or dependency-injection layer.

Data model and property contracts: Continue using ActiveProfile.Format, Interval, OutputFolder, AudioOutputFolder, FalconOutputFolder, IncludeAudio, RenameAudio, EnableCompression, CreateUniverseFile, BackupUniverseFile, FppDirectUpload, and FppHostAddress. Add only private bool _isRestoringProfile to BulkExportOutputFormatStage. No persisted fields, serialization changes, public signatures, or project references are required. StageStart must not overwrite profile settings while populating controls. A null saved host may display as an empty string without being written back during restoration.

Boundary logic: While _isRestoringProfile is true, edit/leave handlers return before writes, path derivation, dialogs, pings, or stage-change notifications. CanMoveNext returns false before evaluating interval or paths. After restoration, the existing interval validation and destination validation resume. A Falcon Direct Upload profile uses host syntax and reachability for Next; a Falcon File Path profile uses existing directory checks. Non-Falcon formats retain their existing sequence/audio file requirements even if the saved FppDirectUpload flag is true. Therefore a direct-mode bypass must test both the applicable Falcon format and the selected upload mode; it must not globally disable file validation for other formats.

Subsystem component matrix, expressed as file responsibilities: src/Vixen.Modules/App/ExportWizard/BulkExportOutputFormatStage.cs owns all proposed behavior changes. Its StageStart restores the controls, its event handlers write user edits, and its validators show directory prompts. BulkExportOutputFormatStage.Designer.cs supplies controls and event wiring but needs no planned layout change. ExportProfile.cs supplies existing saved fields and needs no planned model change. BulkExportSummaryStage.cs and FppDirectUploadService.cs consume the final profile and need no planned changes. Existing tests under src/Vixen.Tests/ExportWizard/ provide surrounding regression coverage; the actual stage event sequence is verified in Rider with the application.

## Context and Orientation


Vixen is a Windows .NET 10 light-show application. The ExportWizard module lives in src/Vixen.Modules/App/ExportWizard/. Its output stage, BulkExportOutputFormatStage, derives from Common.Controls.Wizard.WizardStage. StageStart is called when that page is entered. _data.ActiveProfile is the settings object shared with other pages. _WizardStageChanged requests that the wizard reevaluate navigation. CanMoveNext determines whether Next is available.

FPP means Falcon Player, the device that receives exported sequences. File Path writes to a local or network directory. Direct Upload sends output to the saved FPP host. The output stage already has both modes and a background ping that enables Next when a valid host is reachable.

Read docs/plans/export/fpp-direct-upload-export-wizard.md for the original saved-mode contract, but use this document and current source for the repair. At implementation start, recall the VIX-4022 note with Gortex, inspect git status, read the current source, and preserve user changes. Follow AGENTS.md: graph-based diagnosis, at most one unresolved source follow-up, impact before edits, Gortex edit/refactor for mutation, then detect/tests/guards/contract. Inspect any additional source before changing it. Avoid formatting unrelated lines; use tabs and LF.

## Plan of Work


### Milestone 1: Record agreed user behavior in Jira


Context: Update the existing VIX-4022 issue when execution is authorized. Preserve the reporter's reproduction as context and add concise user-facing requirements; keep the technical design in this file. Read .agents/skills/jira/SKILL.md before the write. Do not change status or assignee as part of this step.

Plan of Work: The description should say that selecting an existing Direct Upload export configuration restores the output mode and host without asking to create a file-output directory. Acceptance must cover first entry, returning with Back/Next, selecting another profile, changing Falcon formats in Direct Upload, retaining saved values, and preserving File Path directory prompts. Include a short user-facing test plan for those interactions, host validation, and a local file export.

Concrete Steps: Fetch VIX-4022 again through Atlassian MCP before editing. Compare its current description with the proposed requirements, merge any newer reporter details, and read back the updated description. No repository source edits are needed for this milestone.

Validation and Acceptance: The Jira issue presents observable behavior and acceptance criteria without internal class names or formulas. Record its URL and update Progress.

STOP HERE for manual review and commit execution before proceeding.

STOP BOUNDARY: Halt code execution. From C:\Dev\Vixen run git status --short and git diff -- docs/plans/export/vix-4022-export-output-config-loading.md. If the living plan changed, invoke the project commit-msg skill with VIX-4022 as the subject prefix and provide its clean, paste-ready Commit message block. Do not create a commit without explicit instruction. Wait for explicit user confirmation before advancing.

### Milestone 2: Restore controls without executing edit side effects


Context: Modify only src/Vixen.Modules/App/ExportWizard/BulkExportOutputFormatStage.cs. Use Rider to navigate to BulkExportOutputFormatStage.StageStart and the named handlers below. Keep layout and upload services intact.

Plan of Work: Add private bool _isRestoringProfile. In StageStart set it before outputFormatComboBox.Items.Clear and wrap the entire synchronous control restoration in try/finally, clearing it in finally. Restore the saved mode and host along with all the other controls, and call UpdateUploadModeVisibility once on the final mode. Restore format-group visibility and compression availability explicitly because the format handler is now suppressed; use the existing format capability method for compression. Reset _fppHostReachable for this entry. Keep the saved-host background ping and the final _WizardStageChanged after successful restoration, outside the guarded block. A failed restore clears the flag but must not publish a successfully restored page.

Add early returns for _isRestoringProfile at outputFormatComboBox_SelectedIndexChanged, chkIncludeAudio_CheckedChanged, chkRenameAudio_CheckedChanged, chkCreateUniverseFile_CheckedChanged, chkBackupUniverseFile_CheckedChanged, chkCompress_CheckedChanged, rdoUploadMode_CheckedChanged, txtFppHostAddress_TextChanged, resolutionComboBox_TextChanged, txtOutputFolder_Leave, txtAudioOutputFolder_Leave, txtFalconOutputFolder_Leave, and txtFppHostAddress_Leave. This prevents profile writes and reentrant navigation updates during restoration, including visibility changes that move focus. In the format handler also handle SelectedItem == null before calling ToString, because clearing the combo can produce a transient no-selection state.

In the format handler retain user-driven format/profile and UI updates, but run UpdateFalconPaths and ValidateFalconOutputFolder only for file output. Refresh upload-mode visibility after a user format change. Protect UpdateFalconPaths and ValidateFalconOutputFolder against restoration and applicable Falcon Direct Upload mode as a second boundary. ValidateFalconOutputFolder returns false when validation is inapplicable; it must not claim that a directory was successfully validated. Guard the sequence/audio Leave validators and ValidatePath against applicable Falcon Direct Upload too. Use the existing IsFalconFormat predicate and FppDirectUpload setting after restore; do not use the radio flag alone for non-Falcon formats.

At the top of CanMoveNext's getter return false while restoring, before IsIntervalValid can write Interval. Document StageStart's restoration guarantee with XML summary/remarks and document CanMoveNext's restoration and destination-dependent conditions with summary/value text. Do not change signatures. Do not fix unrelated CanTestPath logic, networking race conditions, or other existing warnings.

Concrete Steps: In C:\Dev\Vixen, inspect git status --short. Read the current affected source through Gortex and assess impact. Apply small focused edits with Gortex edit. Run change detect, then tests, guards, and contract on returned changed symbols. Run the IDE get_file_problems check on the affected file; resolve only problems within changed lines. If that tool is unavailable, record the limitation and use Rider's inspection view plus build diagnostics.

Build with full MSBuild in a Visual Studio Developer PowerShell where msbuild resolves to the installed C++-capable toolset. Build the application for manual acceptance:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64

Then build the tests before invoking the test runner:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~ExportWizard"

Expected results are a successful build and zero failed ExportWizard tests. Record actual passed/skipped counts rather than inventing a count. Use native exec_command with yield_time_ms=10000 for long commands; poll completion with write_stdin using at least 30000 milliseconds. Rider Test Runner may exercise the already-built tests too.

Validation and Acceptance: Perform the complete manual matrix below using Release/Output/Vixen.exe and record each result. The restoration defect must reproduce on the old build and disappear after the change. Inspect the final diff to ensure the fix does not grow beyond the stage and its XML docs.

STOP HERE for manual review and commit execution before proceeding.

STOP BOUNDARY: Halt code execution. Run git status --short and git diff -- src/Vixen.Modules/App/ExportWizard/BulkExportOutputFormatStage.cs docs/plans/export/vix-4022-export-output-config-loading.md. Invoke .agents/skills/commit-msg/SKILL.md using VIX-4022 as the subject prefix; output its clean, paste-ready Commit message block. Do not create a commit without explicit instruction. Wait for explicit user confirmation before advancing.

### Milestone 3: Record delivered behavior and validation


Context: Finish the living plan and the existing Jira issue after successful implementation and review. Read the current Jira description before editing.

Plan of Work: Align requirements with the delivered behavior if implementation required a justified adjustment. Add a Jira comment describing that saved Direct Upload configurations open without irrelevant directory prompts, plus concise build/test and manual validation results. Report skipped validation honestly. Do not mark the issue resolved or change workflow state unless the user requests it. Update Outcomes & Retrospective, Progress, and Decision Log with actual evidence.

Concrete Steps: Review the final diff and validation transcript from Milestone 2. Use the Atlassian MCP issue edit/comment tools for authorized execution, then read back the issue. Record remaining limitations in this file.

Validation and Acceptance: Jira and this plan agree on delivered user behavior, and every validation claim is backed by a command result or an observed UI scenario.

STOP HERE for manual review and commit execution before proceeding.

STOP BOUNDARY: Halt code execution. Run git status --short and git diff -- docs/plans/export/vix-4022-export-output-config-loading.md. If repository files changed, invoke the project commit-msg skill using VIX-4022 as the subject prefix and output its clean Commit message block. Do not create a commit without explicit instruction. Wait for explicit user confirmation before further work.

## Concrete Steps


All commands above run from C:\Dev\Vixen. Application output is Release/Output/. Use full MSBuild, not dotnet build of the solution, because transitive QMLibrary and LiquidFunWrapper dependencies are C++/CLI projects. No packages or project files need changing. During Milestone 2, `git status --short` was clean before editing. The Release solution rebuild, `Vixen_Tests` MSBuild target, and ExportWizard-filtered test command listed above all completed successfully; see the execution record in Validation and Acceptance. The Rider file-problem inspection was available and run on the affected file. Desktop control was unavailable to the agent during implementation; the user subsequently performed the WinForms validation and reported results recorded below, including the old-build reproduction. For Milestone 3, the current Jira description was fetched and found already aligned, so it was not edited. Comment 40586 was added and read back; the issue remained In Progress and assigned to Jeff Uchitjil, with no workflow transition.

## Validation and Acceptance


Before applying the fix, prepare an existing saved Falcon Direct Upload profile with a nonempty FalconOutputFolder pointing to a missing disposable directory. Use an actual saved profile and select it through the wizard; avoid touching a production output share. Record Format, Interval, output/audio/Falcon paths, mode, host, compression, audio/rename, and universe settings. On the old build, entry should show the reported directory-create dialog before Direct Upload appears. After the fix, Direct Upload and its saved host appear without that dialog; the missing directory stays missing and recorded profile values remain intact. Inspect values in Rider if necessary, including after returning to the stage.

Repeat with an empty Falcon path, an existing directory, and a stale or unavailable network path. Stage restoration must not initiate file-path probing or directory creation in Direct Upload. Use Back then Next to reenter the page, and select a different saved profile through the normal earlier wizard step. Verify both file-to-direct and direct-to-file profile transitions and differing checkbox settings; each page must reflect the selected profile without writing the previous controls' values into it.

With Direct Upload active, change between supported Falcon formats. No directory prompt should appear and stored file destinations should not be recomputed for Direct Upload. Confirm the final controls and Next state reflect the selected format. Change to a non-Falcon format and verify its ordinary sequence/audio directory requirements still apply.

Open a saved File Path profile with an existing output directory and verify it can proceed. Restore a File Path profile with a missing directory: initial population must not prompt, and Next must remain disabled. Focus and leave its destination field to trigger the established create-directory prompt. Cancel keeps the directory absent and Next disabled; accept creates only the disposable target and allows progression when the remaining settings are valid. Change a Falcon format in File Path mode and verify existing path derivation and appropriate validation still run. Export a short sequence to the disposable local output and verify its file appears.

For Direct Upload, a valid reachable saved host enables Next after the existing background ping finishes. An empty or invalid host keeps Next disabled, and an unreachable valid host keeps it disabled. Editing and leaving the host still uses the existing host validation behavior. These host checks must never trigger a file-directory dialog. A physical export/upload is not needed to prove this restoration fix; if tested, use a designated test device.

This matrix is the primary regression proof because it exercises actual WinForms events and the modal dialog. Existing ExportWizard tests protect host validation, profile cloning, and upload-service behavior but do not alone prove restoration order. Record build output, test counts, Rider inspection results, and observed UI outcomes.

Execution record (2026-10-13): The full Release solution rebuild completed with 0 errors and 74 warnings. The full-MSBuild `Vixen_Tests` target completed successfully. `dotnet test ... --filter "FullyQualifiedName~ExportWizard"` passed 41, failed 0, skipped 0. Rider file-problem analysis ran on `BulkExportOutputFormatStage.cs`; it reported file-wide existing warnings, including the longstanding unused return value warning for `ValidateFalconOutputFolder`, and no new errors in the modified logic. Gortex found no test coverage for the changed symbols and no guard rules.

User-reported manual results (2026-10-13): The issue reproduced before the change. After the change, the saved Direct Upload profile no longer displayed the directory prompt and its other settings remained as expected. A File Path profile with a bad path showed the expected warning and retained its other settings; a valid File Path profile showed no warning and retained its settings. Direct Upload with a valid host enabled Next. An invalid/unreachable host produced the expected unreachable warning. The user reports having performed the Validation and Acceptance steps; the cases above are the specific observations provided in the report.

Jira completion record (2026-10-13): The current VIX-4022 description was read before the update and already matched the delivered behavior, so it was left unchanged. Added comment 40586 with the user-facing behavior and concise validation results; Jira readback confirmed the comment is attached and the status remains In Progress with assignee Jeff Uchitjil. No validation was reported as skipped; no Jira workflow action was taken.

## Idempotence and Recovery


The restoration flag is cleared in finally even if control assignment throws. Repeated StageStart calls restore the current profile without directory side effects. Re-run checks after a failure once its cause is corrected; do not bypass the full MSBuild test build. Preserve unrelated working-tree changes. Remove only disposable directories created for the manual test after checking their exact targets; never delete a saved production path. If a patch must be reverted, reverse only this task's hunks after examining the current diff.

## Artifacts and Notes


Source evidence in BulkExportOutputFormatStage.cs:

    StageStart: outputFormatComboBox.SelectedItem = _data.ActiveProfile.Format;
    ...later: rdoDirectUpload.Checked = _data.ActiveProfile.FppDirectUpload;
    SelectedIndexChanged, Falcon branch: UpdateFalconPaths(...); ValidateFalconOutputFolder();
    ValidateFalconOutputFolder: "The selected output path does not exist. Do you want to create it?"

The first selection invokes the handler synchronously. The repair must suppress edit side effects throughout restoration, then publish the final page state. A reordered radio assignment by itself is insufficient.

## Interfaces and Dependencies


Retain public override void StageStart() and public override bool CanMoveNext. Add private bool _isRestoringProfile in the existing stage. Existing event-handler signatures, ExportProfile contracts, and FppHostValidator.IsHostAddressValid remain intact. No new library, interface, configuration schema, or public API is required. If implementation expands to async code changes, load the project csharp-async skill first; if a signature unexpectedly changes, run Gortex change verify before editing and revise this plan with the reason.

Analysis complete and plan integrated with plans.md.

Execution update (2026-10-13): Milestone 2 implementation, automated validation, and user-reported manual acceptance results are recorded above. Milestones 2 and 3 are complete. Milestone 3 recorded the current Jira readback and completion comment 40586 without changing workflow state.
