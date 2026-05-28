# Delete Or Rename Export Profile

## Overview

Vixen provides a bulk sequence export tool that can export sequences to and FPP instance. It allows you to save the 
settings to a profile to recall later. This is concienient for the user so they do not have to reconfigure it everytime
they use it.

## Problem

Once a user creates a saved profile, they can not delete it or rename it without editing an xml file. This is not user
friendly and needs a solution to allow the user to do those actions from the UI.

JIRA issue VIX-3622 created by a user to report the problem. 

## Requirements

Replace the existing BulkExportCreateOrSelectStage with the partially implemented BulkExportConfigStage.
Use the partially implemented BulkExportConfigStage to implement the requirements.
Leave the old BulkExportCreateOrSelectStage until the very end. Once the new work is accepeted, it can be removed.
Enable the existing add button on the Stage 1 configuration page at all times.
Enable the existing delete button on the Stage 1 configuration page when there is at least one profile in the list.
Enable the existing rename button on the Stage 1 configuration page when there is at least one profile selected in the list.
If the user does not have any profiles, a default one should be created and named Default to show in the list.
The default profile will not be saved unless the user does so in the summary stage. 

### Add

When the user clicks the add button, they will be presented with a dialog to enter a new profile name.
The dialog should be a standard dialog in the Vixen style with Ok and Cancel as options.
The dialog title should be New Profile.
The dialog should ensure the user enters some non blank text for the name that is not the same as another profile name.
When the user clicks cancel on the dialog, it exits and does nothing to the existing profiles.
When the user clicks Ok, a new default profile is created with the name the user entered trimmed of leading and trailing spaces. 

### Rename

When the rename button is clicked, the user should be presented with a dialog box to enter the new name.
The dialog should be a standard dialog in the Vixen style with Ok and Cancel as options.
The dialog title should be Rename Profile.
The dialog should ensure the user enters some non blank text for the name that is not the same as another profile name.
When the user clicks cancel on the dialog, it exits and does nothing to the existing profiles.
When the user clicks Ok, and the new name is not the same as the old name, the selected profile name is updated with the 
new name that is trimmed of leading and trailing spaces.
After the new name is set on the profile the profiles are saved using the VixenSystem.SaveModuleConfigAsync() method.
Once the profile is renamed, the Configurations combo box should have the renamed profile updated and it should remain 
selected.

### Delete

When the delete button is clicked, the user should be presented with an Ok Cancel dialog to confirm they want to delete
the profile.
The confirm dialog should be a standard dialog in the Vixen style with Ok and Cancel as options.
The confirm dialog title should be Confirm Delete.
The confirm dialog message should be Are you sure you want to delete `Profile Name`?
If the user clicks Ok on the confirm dialog the selected profile should be removed from the configuration and the new configuration
should be saved.
When the user clicks cancel on the dialog, it exits and does nothing to the existing profiles.
Once the profile is deleted, the Configurations combo box should have the deleted profile removed and set back to the
first entry in the list. 

## Guidelines

- Use the project skills dotnet-best-practices, csharp-async, csharp-docs, and dotnet-design-pattern-review.
- Use plans.md for creating the plan to design and build the functionality.
- The first implementation milestone must include updating the existing JIRA issue VIX-3632 covering the work in the plan including 
  these requirements, high level design, acceptance criteria and testing steps.
- Call out any risks or concerns in the plan when creating the design.
