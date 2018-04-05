using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls;
using NGit;
using NGit.Api;
using NGit.Storage.File;
using GC = NGit.Storage.File.GC;


namespace VersionControl
{
    public partial class Module
    {
        Repository _repo;
		private static HashSet<string> Ignore = new HashSet<string>(new [] {"/Export", "/Core Logs", "/logs", "/.git", "/.lock", "/.gitignore", "Temp", "Thumbs.db" });

        #region Properties

        public string GitRepositoryFolder { get { return Vixen.Sys.Paths.DataRootPath; } }
       
        #endregion

        private void EnableDisableSourceControl(bool enabled)
        {
	        Logging.Info("Initializing version control. Enabled:{0}.", enabled);

			_showCommand.Enabled = enabled;

            DisableWatchers();

            if (enabled)
            {
                var repoCreated = CreateRepositoryIfNotExists();

	            if (!repoCreated)
	            {
		            CreateUpdateGitIgnore();
	            }

	            _repo = OpenRepository();

	            if (!_repo.ObjectDatabase.Exists())
	            {
					_repo.Create(true);
	            }

	           AddItemsToGit(repoCreated);
                
                CreateWatcher(GitRepositoryFolder);
			}

	        Logging.Info("Initializing version control complete.");
		}

	    private Repository OpenRepository()
	    {
		    FileRepositoryBuilder builder = new FileRepositoryBuilder();

			return builder.ReadEnvironment().SetGitDir(Path.Combine(GitRepositoryFolder, ".git")).Build();
	    }

	    
        private void AddItemsToGit(bool initialCheckin)
        {
            using (var frmStatus = new Status())
            {

				frmStatus.Show();

				if (initialCheckin)
	            {
					frmStatus.SetStatusText("Gathering items to add to version control");
					
		            Git git = new Git(_repo);
		            var status = git.Status().Call();

		            frmStatus.SetMaximum(status.GetUntracked().Count);

		            var add = git.Add();

		            foreach (var item in status.GetUntracked().ToList())
		            {
						add.AddFilepattern(item);
			            frmStatus.SetValue(frmStatus.Value + 1);
					}
		            
		            add.Call();
					
					frmStatus.SetStatusText("Committing profile items to version control.");
					git.Commit().SetAll(true).SetMessage(@"Initial commit of existing Vixen profile data files.").Call();
		            frmStatus.SetValue(2);
					frmStatus.SetStatusText(@"Completed initial load of existing Vixen profile data files.");
	            }
	            else
	            {
					//This handles checking in any changes that might have happened outside of a Vixen session.
		            frmStatus.SetStatusText("Gathering items that have changed outside of Vixen and updating version control.");
					Git git = new Git(_repo);

		            try
		            {
						var status = git.Status().Call();
						var changed = status.GetAdded().Count > 0 || status.GetChanged().Count > 0 || status.GetModified().Count > 0
							   || status.GetRemoved().Count > 0 || status.GetUntracked().Count > 0 || status.GetMissing().Count > 0;

						if (changed)
						{
							frmStatus.SetMaximum(status.GetAdded().Count + status.GetRemoved().Count + status.GetModified().Count +
												 status.GetChanged().Count + status.GetMissing().Count + status.GetUntracked().Count);

							if (status.GetAdded().Count > 0 || status.GetUntracked().Count > 0 || status.GetModified().Count > 0)
							{
								var add = git.Add();
								status.GetAdded().ToList().ForEach(a =>
								{
									add.AddFilepattern(a);
									frmStatus.SetValue(frmStatus.Value + 1);
								});

								status.GetModified().ToList().ForEach(a =>
								{
									add.AddFilepattern(a);
									frmStatus.SetValue(frmStatus.Value + 1);
								});

								status.GetChanged().ToList().ForEach(a =>
								{
									add.AddFilepattern(a);
									frmStatus.SetValue(frmStatus.Value + 1);
								});

								status.GetUntracked().ToList().ForEach(a =>
								{
									add.AddFilepattern(a);
									frmStatus.SetValue(frmStatus.Value + 1);
								});

								add.Call();
							}


							if (status.GetMissing().Count > 0 || status.GetRemoved().Count > 0)
							{
								var removed = git.Rm();

								status.GetRemoved().ToList().ForEach(a =>
								{
									removed.AddFilepattern(a);
									frmStatus.SetValue(frmStatus.Value + 1);
								});


								status.GetMissing().ToList().ForEach(a =>
								{
									removed.AddFilepattern(a);
									frmStatus.SetValue(frmStatus.Value + 1);
								});

								removed.Call();
							}

							git.Commit().SetMessage(@"Update of profile changes outside of Vixen.").Call();
							frmStatus.SetStatusText(@"Committed profile changes outside of Vixen.");
						}

			            RunCleanup(frmStatus);
					}
		            catch (Exception e)
		            {
			            Logging.Error(e, "Unable to access repository.");
		            }
					
				}
	            
                frmStatus.Close();
            }

        }

	    private void RunCleanup(Status status)
	    {
		    try
		    {
				status.SetStatusText("Compacting repository.");
				FileRepositoryBuilder builder = new FileRepositoryBuilder();
				FileRepository fr = builder.ReadEnvironment().SetGitDir(Path.Combine(GitRepositoryFolder, ".git")).Build();
			    GC gc = new GC(fr);
				var stats = gc.GetStatistics();
			    LogGitStats(stats);
				
				ValueProgressMonitor w = new ValueProgressMonitor(status);
				gc.SetProgressMonitor(w);
				gc.Gc();
			    stats = gc.GetStatistics();
				LogGitStats(stats);
		    }
		    catch (Exception e)
		    {
			    Logging.Error(e, "Error running GC on version repository");
		    }

		}

	    private static void LogGitStats(GC.RepoStatistics stats)
	    {
		    StringBuilder sb = new StringBuilder();
		    sb.AppendLine($"Loose Objects: {stats.numberOfLooseObjects}");
		    sb.AppendLine($"Loose Refs: {stats.numberOfLooseRefs}");
		    sb.AppendLine($"Number Pack Files: {stats.numberOfPackFiles}");
		    sb.AppendLine($"Packed Objects: {stats.numberOfPackedObjects}");
		    sb.AppendLine($"Packed Refs: {stats.numberOfPackedRefs}");
		    sb.AppendLine($"Size Loose Objects: {stats.sizeOfLooseObjects}");
		    sb.AppendLine($"Size of Packed Objects: {stats.sizeOfPackedObjects}");
		    Logging.Info($"Git GC stats: \n {sb}");
	    }

	    private bool CreateRepositoryIfNotExists()
        {
            bool createRepository = !Directory.Exists(Path.Combine(GitRepositoryFolder, ".git"));

            if (createRepository)
            {
				CreateUpdateGitIgnore();

				FileRepositoryBuilder builder = new FileRepositoryBuilder();
				Repository r = builder.SetGitDir(Path.Combine(GitRepositoryFolder, ".git")).Build();
				r.Create();
	           //Git.Init().SetDirectory(GitRepositoryFolder).Call();
			
			}
            else
            {
	            try
	            {
		            var git = Git.Open(Path.Combine(GitRepositoryFolder, ".git"));
		            git.Status().Call();
	            }
	            catch (Exception e)
	            {
					Logging.Error(e, "Error opening repository. Attempting to recreate it.");
		            try
		            {
			            Directory.Delete(Path.Combine(GitRepositoryFolder, ".git"), true);
			            CreateUpdateGitIgnore();
			            Git.Init().SetDirectory(GitRepositoryFolder).Call();
		            }
		            catch (Exception e2)
		            {
			            Logging.Error(e2, "Error attempting to delete and recreate the repository.");
						var messageBox = new MessageBoxForm("The version repository is corrupt. Please delete the .git folder in the profile directory and try again.", "Corrupt version repository!", MessageBoxButtons.OK, SystemIcons.Error);
			            messageBox.ShowDialog();
					}
					
				}
	            
            }
            return createRepository;
        }

	    private void CreateUpdateGitIgnore()
	    {
		    var path = Path.Combine(GitRepositoryFolder, ".gitignore");
		    var gitIgnore = new HashSet<string>(Ignore);

			if (File.Exists(path))
			{
				string[] lines = File.ReadAllLines(path);
			    foreach (var line in lines)
			    {
				    gitIgnore.Add(line);
			    }
		    }

		    using (var sw = new StreamWriter(path, false))
		    {
			    foreach (var item in gitIgnore)
			    {
				    sw.WriteLine(item);
			    }
			    sw.Flush();
		    }

		}

	    public class ValueProgressMonitor: ProgressMonitor
	    {
		    private readonly Status _status;
		    private int _totalWork = 0;
		    private int _progress = 0;
		    public ValueProgressMonitor(Status formStatus)
		    {
			    _status = formStatus;
		    }

		    #region Overrides of ProgressMonitor

		    /// <inheritdoc />
		    public override void Start(int totalTasks)
		    {
				_status.SetMaximum(totalTasks);
			    _status.SetValue(0);
			}

		    /// <inheritdoc />
		    public override void BeginTask(string title, int totalWork)
		    {
			    _status.SetStatusText(title);
			    _totalWork = totalWork;
				_status.SetMaximum(totalWork);
			    _progress = 0;
			    _status.SetValue(0);
		    }

		    /// <inheritdoc />
		    public override void Update(int completed)
		    {
			    _progress += completed;
				_status.SetValue(_progress<=_totalWork?_progress:_totalWork);
			}

		    /// <inheritdoc />
		    public override void EndTask()
		    {
				_status.SetValue(_status.Maximum);
			}

		    /// <inheritdoc />
		    public override bool IsCancelled()
		    {
			    return false;
		    }

		    #endregion
	    }

	}
}
