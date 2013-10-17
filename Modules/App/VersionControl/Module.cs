using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            GitDetails = new Dictionary<string, List<ChangeDetails>>();
        }

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
                AppCommand toolsMenu = _GetToolsMenu();
                AppCommand rootCommand = new AppCommand(MENU_ID_ROOT, "Version Control");
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
                using (Versioning cs = new Versioning((Data)StaticModuleData, repo, GitDetails))
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
            enabledCommand.Checked += (sender, e) =>
            {
                // Not setting the data member in _SetSchedulerEnableState because we want to be
                // able to call _SetSchedulerEnableState without affecting the data (to stop
                // the scheduler upon shutdown).
                _data.IsEnabled = e.CheckedState;
                EnableDisableSourceControl(_data.IsEnabled);
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
            watcher.Changed += watcher_Changed;

            watcher.Created += watcher_Created;
            watcher.Deleted += watcher_Deleted;
            watcher.Renamed += watcher_Renamed;
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;
            watchers.Add(watcher);
            Directory.GetDirectories(folder).ToList().ForEach(dir => CreateWatcher(dir, recursive));

        }

        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (!e.FullPath.Contains("\\.git") && !e.FullPath.Contains("\\Logs"))
            {
                lock (fileLockObject)
                {
                    repo.Index.Delete(e.OldFullPath);
                    repo.Index.Add(e.FullPath);
                    if ((repo.Status.Modified.Count + repo.Status.Added.Count + repo.Status.Removed.Count) > 0)
                        repo.Commit(string.Format("Renamed file {0} to {1}{2}", e.OldName, e.Name, restoringFile ? " [Restored]" : ""));
                    restoringFile = false;
                }
            }
        }

        void watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.Contains(".git") && !e.FullPath.Contains("\\Logs"))
            {
                lock (fileLockObject)
                {
                    repo.Index.Add(e.FullPath);
                    if ((repo.Status.Modified.Count + repo.Status.Added.Count + repo.Status.Removed.Count) > 0)
                        repo.Commit(string.Format("Added {0}{1}", e.Name, restoringFile ? " [Restored]" : ""));
                    restoringFile = false;
                }
            }
        }

        void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.Contains("\\.git") && !e.FullPath.Contains("\\Logs"))
            {
                lock (fileLockObject)
                {
                    repo.Index.Delete(e.FullPath);
                    if ((repo.Status.Modified.Count + repo.Status.Added.Count + repo.Status.Removed.Count) > 0)
                        repo.Commit(string.Format("Deleted {0}{1}", e.Name, restoringFile ? " [Restored]" : ""));
                    restoringFile = false;
                }
            }
        }
        private void DisableWatchers()
        {
            watchers.ForEach(w => w.EnableRaisingEvents = false);
            watchers.Clear();
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {

            if (!e.FullPath.Contains(".git") && !e.FullPath.Contains("\\Logs"))
            {
                lock (fileLockObject)
                {
                    try
                    {
                        //Allow the save operation to complete before we save it to source control... Also
                        //Prevents dozens of SC saves if a user gets click happy.. 
                        System.Threading.Thread.Sleep(10000);
                        repo.Index.Add(e.FullPath);
                        repo.Commit(string.Format("Changed {0}{1}", e.Name, restoringFile ? " [Restored]" : ""));

                    }
                    catch (InvalidOperationException) { }
                }
            }
        }

        internal static bool restoringFile = false;
        internal static object fileLockObject = new object();
        List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

        #endregion



    }
}
