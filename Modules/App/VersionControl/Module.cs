using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NGit.Api;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Sys;

namespace VersionControl
{
    public partial class Module : AppModuleInstanceBase
    {
        public Module()
        {
            _data = new Data();

        }
        private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		
        #region Variables
        IApplication _application;
        Data _data;
        #endregion

        #region overrides

        public override IModuleDataModel StaticModuleData
        {
            get
            {

                return _data;
            }
            set
            {
                _data = (Data)value;
            }
        }

        public override void Loading()
        {

            _AddApplicationMenu();


            EnableDisableSourceControl(_data.IsEnabled);


        }

        public override void Unloading()
        {
            DisableWatchers();
        }

        public override Vixen.Sys.IApplication Application
        {
            set { _application = value; }
        }


        #endregion

        #region Application Menu
        private const string MENU_ID_ROOT = "VersionControlRoot";

        private AppCommand _showCommand;
        private LatchedAppCommand _enabledCommand;

        private void _AddApplicationMenu()
        {
            if (_AppSupportsCommands())
            {
                var toolsMenu = _GetToolsMenu();
                var rootCommand = new AppCommand(MENU_ID_ROOT, "Version Control");
                rootCommand.Add(_enabledCommand ?? (_enabledCommand = _CreateEnabledCommand()));
                rootCommand.Add(new AppCommand("s1", "-"));
                rootCommand.Add(_showCommand ?? (_showCommand = _CreateShowCommand()));

                toolsMenu.Add(rootCommand);
            }
        }

        private AppCommand _CreateShowCommand()
        {
            AppCommand showCommand = new AppCommand("VersionControl", "Browse");
            showCommand.Click += (sender, e) =>
            {
                using (Versioning cs = new Versioning((Data)StaticModuleData, _repo))
                {

                    if (cs.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {


                    }
                }
            };

            return showCommand;
        }
        private LatchedAppCommand _CreateEnabledCommand()
        {
            LatchedAppCommand enabledCommand = new LatchedAppCommand("VersionControlEnabled", "Enabled");
            enabledCommand.IsChecked = _data.IsEnabled;
            enabledCommand.Checked += async (sender, e) =>
            {
                _data.IsEnabled = e.CheckedState;
                EnableDisableSourceControl(_data.IsEnabled);
	            await VixenSystem.SaveModuleConfigAsync();
			};

            return enabledCommand;
        }


        private void _RemoveApplicationMenu()
        {
            if (_AppSupportsCommands())
            {
                AppCommand toolsMenu = _GetToolsMenu();
                toolsMenu.Remove(MENU_ID_ROOT);
            }
        }
        private bool _AppSupportsCommands()
        {
            return _application != null && _application.AppCommands != null;
        }
        private AppCommand _GetToolsMenu()
        {
            AppCommand toolsMenu = _application.AppCommands.Find("Tools");
            if (toolsMenu == null)
            {
                toolsMenu = new AppCommand("Tools", "Tools");
                _application.AppCommands.Add(toolsMenu);
            }
            return toolsMenu;
        }


        #endregion

        #region File System Watchers
        private void CreateWatcher(string folder)
        {
			Directory.GetDirectories(folder).Where(d => !d.EndsWith("logs") && !d.EndsWith("\\.git") && !d.EndsWith("Core Logs") && !d.EndsWith("Export")).ToList().ForEach(CreateDirectoryWatcher);
        }

	    private void CreateDirectoryWatcher(string folder)
	    {
		    var watcher = new FileSystemWatcher(folder);
		    watcher.Changed += watcher_FileSystemChanges;
		    watcher.Created += watcher_FileSystemChanges;
		    watcher.Deleted += watcher_FileSystemChanges;
		    watcher.Renamed += watcher_FileSystemChanges;
		    watcher.IncludeSubdirectories = true;
		    watcher.EnableRaisingEvents = true;
		    watchers.Add(watcher);
	    }

	    private void watcher_FileSystemChanges(object sender, FileSystemEventArgs e)
        {
			Task.Factory.StartNew(() =>
            {
                try
                {
					lock (fileLockObject)
					{
						//Wait for the file to fully save...
						Thread.Sleep(1000);
						while (VixenSystem.IsSaving())
						{
							Thread.Sleep(1);
						}
						Git git = new Git(_repo);

						var status = git.Status().Call();

						var changed = status.GetAdded().Count > 0 || status.GetChanged().Count > 0 || status.GetModified().Count > 0
						              || status.GetRemoved().Count > 0 || status.GetUntracked().Count > 0 || status.GetMissing().Count > 0;

						if (status.GetAdded().Count > 0 || status.GetUntracked().Count > 0 || status.GetModified().Count > 0
							|| status.GetChanged().Count > 0)
						{
							var add = git.Add();
							status.GetAdded().ToList().ForEach(a =>
							{
								add.AddFilepattern(a);
							});

							status.GetModified().ToList().ForEach(a =>
							{
								add.AddFilepattern(a);
							});

							status.GetChanged().ToList().ForEach(a =>
							{
								add.AddFilepattern(a);
							});

							status.GetUntracked().ToList().ForEach(a =>
							{
								add.AddFilepattern(a);
							});

							add.Call();
						}


						if (status.GetMissing().Count > 0 || status.GetRemoved().Count > 0)
						{
							var removed = git.Rm();

							status.GetRemoved().ToList().ForEach(a =>
							{
								removed.AddFilepattern(a);
							});

							status.GetMissing().ToList().ForEach(a =>
							{
								removed.AddFilepattern(a);
							});

							removed.Call();
						}

						if(changed)
						{
							git.Commit().SetMessage($"Profile changes {(restoringFile ? "restored." : "commited.")}").Call();
						}
					}
				}
                catch (Exception ex)
                {

                    Logging.Error(ex, ex.Message);
                }

                restoringFile = false;
            });
        }


        private void DisableWatchers()
        {
            watchers.ForEach(w => w.EnableRaisingEvents = false);
            watchers.Clear();
        }



        internal static bool restoringFile = false;
        internal static object fileLockObject = new object();
        List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

        #endregion



    }
}
