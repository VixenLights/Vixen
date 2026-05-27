# Export Wizard FPP Upload

## Overview

Today the Export wizard in Vixen (Vixen.Modules.App.ExportWizard) collects the information about which sequences 
and audio files to export to fseq filesto be exported to FPP. The user can export the fseq and audio files to a local 
folder, or to a network path. This is chosen in the wizard UI on Step 4. FPP by default in the latest versions no longer enables
samba by default and it takes a lot of configuration to get it enabled and working. Thus the network path is much harder 
for the user to working. This forces the user to save the files to a local disk and then manually upload them through
the FPP user interface. Updating the universes becomes more of a manual step since the co-universes file cannot be 
directly uploaded easily. The goal of this improvement is to utilize the new FPPClient functionality in 
Vixen.Modules.App to upload the music, sequence, and universe files directly to FPP via the RESTful API.

## Requirements

- Add a new option group in the Falcon Pi Player 2.x group section on the Step 4 page to choose between 
  `File Path` or `Direct Upload` to an FPP host by host name or ip address. The option should be an either or radio button.
- The default will be `File Upload` unless the user has previously choose `Direct Upload` and the settings were saved on
  the summary page. If `Direct Upload` was saved, the the `Direct Upload` option will be set on page load.
- If the `File Path` is choosen, then the existing file chooser button, text entry box and behavior will show. The user will not 
  see any difference.
- If the `Direct Upload` option is choosen, then a simple entry text box for the host name or ip address will be shown
  in place of the existing button and file text entry.
- The host / ip entry will be validated for a valid ip address format, or host name format. Text that does not confirm
  will be flagged and the next button disabled so the user cannot proceed until it is corrected.
- Once the user leaves the host / ip entry text box, the host / ip should be pinged to ensure it is accessible. If it is
  not, then an error message will be presented to the user to indicate the FPP instance is not accessible.
- On the summary page in a new section below the existing fields, if the user has choosen direct upload, the system info api in the FPPCLient should be used
  to pull basic info about the instance to show the user to confirm they have the right instance. Fields should include
  HostName, HostDescription, Platform, and Variant.
- The summary page does not change if the user chooses the file path option.
- When the user clicks next on the summary page, whichever option the user choose will saved to the settings so it 
  can be recalled on the Step 4 page.
- When the user clicks Next on the Summary page and the export process runs, if the user choose `Direct Upload` the following
  will occur.
  - The exported sequence (fseq) file will be uploaded to the sequences directory. 
  - If the user chose to include audio, the associated audio file will be uploaded to the music directory.
  - If the user has chose to backup the univers file, then the co-universes.json will be renamed in the config folder
    using the same name format it would have been on the file system. Rename will always occur before a new one is uploaded.
  - If the user has chose to create the universe file, then the co-universes.json file will be uploaded to the config
    directory.
- If the user chose the `File Path` option, then every export function remains as it is today.
- The existing progress bar should continue to report progress in the same way no matter which option is choosen.
- Any errors should be reported to the user in a user friendly dialog following existing patterns.
- Any errors should be logged using the existing logging patterns.
- A full unit test suite should cover the new functionality and any any modified logic to prove the "File Path" option
  still works as it did before.

## FPPClient

FPPClient should have most of the methods needed to achieve this with the exception of the file rename. The API docs are
located in docs\references\fpp-rest-api.md to enhance the FPPClient project to support file renaming. Any work to FPPCLient
should be segregated out into it's own milestone step including any unit tests. The plan should call out any other shortcomings
of the FPPCLient and address them in a similar way. 

## Guidelines

- Use the project skills dotnet-best-practices, csharp-async, csharp-docs, and dotnet-design-pattern-review.
- Use plans.md for creating the plan to design and build the functionality. 
- The first implementation milestone must include creating a JIRA issue covering the work in the plan including requirements,
  high level design, acceptance criteria and testing steps.
- Call out any risks or concerns in the plan when creating the design.
