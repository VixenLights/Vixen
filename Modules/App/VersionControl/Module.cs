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

namespace VersionControl {
	public class Module : AppModuleInstanceBase {
		public Module() {
			GitDetails = new List<ChangeDetails>();

		}

		Vixen.Sys.IApplication _application;
		GitSharp.Repository repo;
		Data _data;

		private const string MENU_ID_ROOT = "VersionControlRoot";

		public string GitRepositoryFolder { get { return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Vixen 3"); } }

		public override void Loading() {
			_AddApplicationMenu();

			var repoCreated = CreateRepositoryIfNotExists();

			repo = new GitSharp.Repository(GitRepositoryFolder);
			GetGitDetails();
			if (_data.IsEnabled) {
				AddItemsToGit(repoCreated);

				CreateWatcher(GitRepositoryFolder, true);
			}

		}
		bool watcherEnabled = false;
		private void CreateWatcher(string folder, bool recursive) {
			var watcher = new FileSystemWatcher(folder);
			watcher.Changed += watcher_Changed;
			watcher.Created += watcher_Changed;
			watcher.Deleted += watcher_Changed;
			watcher.Renamed += watcher_Changed;
			watcher.IncludeSubdirectories = false;
			watcher.EnableRaisingEvents = true;
			watchers.Add(watcher);
			Directory.GetDirectories(folder).ToList().ForEach(dir => CreateWatcher(dir, recursive));
			watcherEnabled = true;
		}
		private void DisableWatchers() {
			watchers.ForEach(w => w.EnableRaisingEvents = false);
			watchers.Clear();
			watcherEnabled = false;
		}

		void watcher_Changed(object sender, FileSystemEventArgs e) {
			if (!e.FullPath.Contains(".git") && !e.FullPath.Contains("\\Logs"))
				AddItemsToGit(false);
		}

		List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

		public override IModuleDataModel StaticModuleData {
			get {
				return _data;
			}
			set {
				_data = (Data)value;
			}
		}
		public override IModuleDataModel ModuleData {
			get {
				return _data;
			}
			set {
				_data = (Data)value;
			}
		}

		private void AddItemsToGit(bool initialCheckin) {
			var files = Directory.GetDirectories(GitRepositoryFolder).ToList();
			files.RemoveAll(r => r.Contains(".git") || r.Contains("\\Logs"));
			files.AddRange(Directory.GetFiles(GitRepositoryFolder));

			repo.Index.Add(files.ToArray());
			string commitMessage = initialCheckin ? "Initial Load of Existing V3 Data Files" : "Updated/Added File to V3 Folders";

			var commit = repo.Commit(commitMessage, new Author() { Name = Environment.UserName, EmailAddress = string.Empty });

		}
		private void GetGitDetails() {

			var tree = repo.Head;
			foreach (Commit commit in repo.Head.CurrentCommit.Ancestors) {
				foreach (Change change in commit.Changes) {
					ChangeDetails details = new ChangeDetails();
					details.FileName = change.Path;
					details.Hash = commit.Hash;
					details.ChangeDate = commit.AuthorDate;
					GitDetails.Add(details);
				}
			}

		}
		public List<ChangeDetails> GitDetails { get; set; }


		private bool CreateRepositoryIfNotExists() {
			bool createRepository = !System.IO.Directory.Exists(System.IO.Path.Combine(GitRepositoryFolder, ".git"));

			if (createRepository) {
				GitSharp.Git.Init(GitRepositoryFolder);
				using (var sw = new StreamWriter(Path.Combine(GitRepositoryFolder, ".gitignore"))) {
					sw.WriteLine("/Logs");
					sw.WriteLine("/.git");

					sw.Flush();
				}
			}
			return createRepository;
		}
		public override void Unloading() {
			DisableWatchers();
		}

		public override Vixen.Sys.IApplication Application {
			set { _application = value; }
		}
		private AppCommand _showCommand;

		private void _AddApplicationMenu() {
			if (_AppSupportsCommands()) {
				AppCommand toolsMenu = _GetToolsMenu();
				AppCommand rootCommand = new AppCommand(MENU_ID_ROOT, "Version Control");

				rootCommand.Add(_showCommand ?? (_showCommand = _CreateShowCommand()));

				toolsMenu.Add(rootCommand);
			}
		}

		private AppCommand _CreateShowCommand() {
			AppCommand showCommand = new AppCommand("VersionControl", "Versioning");
			showCommand.Click += (sender, e) => {
				using (Versioning cs = new Versioning(_data, repo, GitDetails)) {

					if (cs.ShowDialog() == System.Windows.Forms.DialogResult.OK) {

						_data.IsEnabled = cs.VersionControlData.IsEnabled;
						DisableWatchers();
						if (_data.IsEnabled) {
							CreateWatcher(GitRepositoryFolder, true);
						}

					}
				}
			};

			return showCommand;
		}
		private void _RemoveApplicationMenu() {
			if (_AppSupportsCommands()) {
				AppCommand toolsMenu = _GetToolsMenu();
				toolsMenu.Remove(MENU_ID_ROOT);
			}
		}
		private bool _AppSupportsCommands() {
			return _application != null && _application.AppCommands != null;
		}
		private AppCommand _GetToolsMenu() {
			AppCommand toolsMenu = _application.AppCommands.Find("Tools");
			if (toolsMenu == null) {
				toolsMenu = new AppCommand("Tools", "Tools");
				_application.AppCommands.Add(toolsMenu);
			}
			return toolsMenu;
		}
	}
}
