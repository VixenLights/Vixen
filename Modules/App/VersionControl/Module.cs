using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GitSharp;
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
                using (Versioning cs = new Versioning((Data)StaticModuleData, repo))
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
        private void CreateWatcher(string folder, bool recursive)
        {
            var watcher = new FileSystemWatcher(folder);
            watcher.Changed += watcher_FileSystemChanges;

            watcher.Created += watcher_FileSystemChanges;
            watcher.Deleted += watcher_FileSystemChanges;
            watcher.Renamed += watcher_FileSystemChanges;
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;
            watchers.Add(watcher);
            Directory.GetDirectories(folder).ToList().ForEach(dir => CreateWatcher(dir, recursive));

        }

        private void watcher_FileSystemChanges(object sender, FileSystemEventArgs e)
        {

            if (e.FullPath.Contains("\\.git") || e.FullPath.Contains("\\Logs")) return;
            Task.Factory.StartNew(() =>
            {
                try
                {

             
                lock (fileLockObject)
                {
                    //Wait for the file to fully save...
                    Thread.Sleep(2000);


                    switch (e.ChangeType)
                    {

                        case WatcherChangeTypes.Changed:
                            repo.Index.Add(e.FullPath);
                            if ((repo.Status.Modified.Count + repo.Status.Added.Count +
                                 repo.Status.Removed.Count) > 0)
                                repo.Commit(string.Format("Changed {0}{1}", e.Name,
                                    restoringFile ? " [Restored]" : ""));
                            break;
                        case WatcherChangeTypes.Created:
                            repo.Index.Add(e.FullPath);
                            if ((repo.Status.Modified.Count + repo.Status.Added.Count +
                                 repo.Status.Removed.Count) > 0)
                                repo.Commit(string.Format("Added {0}{1}", e.Name,
                                    restoringFile ? " [Restored]" : ""));
                            break;
                        case WatcherChangeTypes.Deleted:
                            repo.Index.Delete(e.FullPath);
                            if ((repo.Status.Modified.Count + repo.Status.Added.Count +
                                 repo.Status.Removed.Count) > 0)
                                repo.Commit(string.Format("Deleted {0}{1}", e.Name,
                                    restoringFile ? " [Restored]" : ""));
                            break;
                        case WatcherChangeTypes.Renamed:
                            repo.Index.Delete(((RenamedEventArgs)e).OldFullPath);
                            repo.Index.Add(e.FullPath);
                            if ((repo.Status.Modified.Count + repo.Status.Added.Count +
                                 repo.Status.Removed.Count) > 0)
                                repo.Commit(string.Format("Renamed file {0} to {1}{2}",
                                    ((RenamedEventArgs)e).OldName, e.Name,
                                    restoringFile ? " [Restored]" : ""));
                            break;
                    }
                }
                }
                catch (Exception eee)
                {

                    Logging.Error(eee.Message,eee);
                }

                restoringFile = false;
                //Reset the cache when GIT changes
                Versioning.GitDetails = null;


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
