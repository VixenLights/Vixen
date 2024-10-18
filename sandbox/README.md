# Windows Sandbox Testing

At times it is necessary to test under a clean environment to ensure things work as they should for users who may be installing on a clean system. Other times you may want to test without disturbing your actual machine settings. Windows Sandbox provides for a clean virtual environment to spin up and test with very little effort. Once testing is done, closing it will clean up and leave no traces. 

## Configuration

Included here is a basic configuration to enable this testing. The file sandbox.wsb and the startup.bat in the scripts folder has the basics to spool up a windows sandbox. You will need to ensure you have Windows Sandbox enabled as a windows feature.

If you are not familiar with windows sandbox or how to enable it, you can read about it at the links below. 

This file launches the sandbox and attempts to map in the folders where the installer builds in the source tree are created. They are located in \Release\Setup in folders for Test, Development, and Release depending on the type of build you create. See making local installer builds. [Vixen Installer Builds](..\Installer\README.md). It also opens the Add/Remove programs panel so you can see what is installed as the base and what happens when Vixen is installed. This also allows testing of the installer and uninstall as well. 

There are a few adjustments you will need to make since the configuration for the sandbox does not support relative paths. In sandbox.wsb, there are two paths used and both need the same adjustment. This defaults to C:\Dev\Vixen\Release\Setup. This assumes you have cloned the repository into a Dev folder at the root of the C drive. You just need to adjust the C:\Dev part on both paths to match where your Vixen source tree resides. Once that is done, you can execute the sandbox.wsb file and you should get a sandbox running with the basics you need. 

### Resources
[Windows Sandbox]("https://learn.microsoft.com/en-us/windows/security/application-security/application-isolation/windows-sandbox/windows-sandbox-overview")

[Windows Sandbox Configuration]("https://learn.microsoft.com/en-us/windows/security/application-security/application-isolation/windows-sandbox/windows-sandbox-configure-using-wsb-file")
