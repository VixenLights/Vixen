using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NGit;
using NGit.Api;
using NGit.Storage.File;


namespace VersionControl
{
    public partial class Module
    {
        Repository repo;
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

	            repo = OpenRepository();
				
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
					Git git = new Git(repo);
		            var status = git.Status().Call();

		            frmStatus.SetMaximum(status.GetUntracked().Count);

					var add = git.Add();
		           
		            status.GetUntracked().ToList().ForEach(a =>
		            {
			            add.AddFilepattern(a);
			            frmStatus.SetValue(frmStatus.Value + 1);
		            });

		            add.Call();
		            frmStatus.SetStatusText("Committing profile items to version control.");
					git.Commit().SetMessage(@"Initial commit of existing Vixen profile data files.").Call();
					frmStatus.SetStatusText(@"Completed initial load of existing Vixen profile data files.");

	            }
	            else
	            {
					//This handles checking in any changes that might have happened outside of a Vixen session.
		            frmStatus.SetStatusText("Gathering items that have changed outside of Vixen and updating version control.");
					Git git = new Git(repo);

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

					
				}
	            
                frmStatus.Close();
            }

        }

	   
        private bool CreateRepositoryIfNotExists()
        {
            bool createRepository = !System.IO.Directory.Exists(System.IO.Path.Combine(GitRepositoryFolder, ".git"));

            if (createRepository)
            {
				CreateUpdateGitIgnore();

	            Git.Init().SetDirectory(GitRepositoryFolder).Call();


	            //var add = git.Add();
	            //add.AddFilepattern("*");
	            //add.Call();

	            //var commit = git.Commit();
	            //commit.SetMessage("Initial Commit.");
	            //commit.SetAll(true);
	            //commit.Call();

	            //Directory.CreateDirectory(Path.Combine(GitRepositoryFolder, ".git"));
	            //Directory.CreateDirectory(Path.Combine(GitRepositoryFolder, ".git\\refs\\remotes"));
	            //Directory.CreateDirectory(Path.Combine(GitRepositoryFolder, ".git\\hooks"));
	            //Directory.CreateDirectory(Path.Combine(GitRepositoryFolder, ".git\\refs\\heads"));
	            //Directory.CreateDirectory(Path.Combine(GitRepositoryFolder, ".git\\refs\\tags"));
	            //Directory.CreateDirectory(Path.Combine(GitRepositoryFolder, ".git\\objects\\info"));
	            //Directory.CreateDirectory(Path.Combine(GitRepositoryFolder, ".git\\objects\\pack"));

	            //using (var s = new FileStream(System.IO.Path.Combine(GitRepositoryFolder, ".git\\objects\\info\\exclude"), FileMode.Create))
	            //{
	            //    s.Write(Properties.Resources.exclude.ToArray(), 0, Properties.Resources.exclude.Length);
	            //}
	            //using (var s = new FileStream(System.IO.Path.Combine(GitRepositoryFolder, ".git\\HEAD"), FileMode.Create))
	            //{
	            //    s.Write(Properties.Resources.HEAD.ToArray(), 0, Properties.Resources.HEAD.Length);
	            //}
	            //using (var s = new FileStream(System.IO.Path.Combine(GitRepositoryFolder, ".git\\config"), FileMode.Create))
	            //{
	            //    s.Write(Properties.Resources.config.ToArray(), 0, Properties.Resources.config.Length);
	            //}
	            //using (var s = new FileStream(System.IO.Path.Combine(GitRepositoryFolder, ".git\\description"), FileMode.Create))
	            //{
	            //    s.Write(Properties.Resources.description.ToArray(), 0, Properties.Resources.description.Length);
	            //}
	            //using (var s = new FileStream(System.IO.Path.Combine(GitRepositoryFolder, ".git\\hooks\\README.sample"), FileMode.Create))
	            //{
	            //    s.Write(Properties.Resources.README.ToArray(), 0, Properties.Resources.README.Length);
	            //}

	            //CreateUpdateGitIgnore();

	            //var di = new DirectoryInfo(Path.Combine(GitRepositoryFolder, ".git"));
	            //di.Attributes = FileAttributes.Hidden;

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

	}
}
