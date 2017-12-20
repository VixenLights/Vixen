using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GitSharp;
using GitSharp.Commands;

namespace VersionControl
{
    public partial class Module
    {
        GitSharp.Repository repo;
		private static HashSet<string> Ignore = new HashSet<string>(new [] {"/Export", "/Core Logs", "/logs", "/.git", "/.lock", "/.gitignore" });

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

                repo = new Repository(GitRepositoryFolder);
				
                AddItemsToGit(repoCreated);
                
                CreateWatcher(GitRepositoryFolder, true);
			}

	        Logging.Info("Initializing version control complete.");
		}

        private void AddItemsToGit(bool initialCheckin)
        {
            using (var frmStatus = new Status())
            {

                frmStatus.Show();

                frmStatus.SetStatusText("Gathering Items to Add to Source Control");
                
                frmStatus.SetStatusText("Adding Items");
               
	            if (initialCheckin)
	            {
		            var directories = Directory.GetDirectories(GitRepositoryFolder, "*.*", SearchOption.AllDirectories)
			            .ToList();

		            directories.RemoveAll(r =>
			            r.Contains(".git") || r.Contains("\\Logs") || r.Contains("\\logs") || r.Contains("\\Core Logs") || r.Contains("\\Export"));
		            directories.Add(GitRepositoryFolder);

		            var files = new List<string>();

		            directories.ForEach(dir => files.AddRange(Directory.GetFiles(dir).Where(f => !f.EndsWith(".lock") && !f.EndsWith(".gitignore"))));

		            frmStatus.SetMaximum(files.Count);

		            files.ForEach(file =>
		            {
			            repo.Index.Add(file);
						frmStatus.SetValue(frmStatus.Value + 1);
		            });

		            repo.Commit(@"Initial Load of Existing V3 Data Files");
		            frmStatus.SetStatusText(@"Initial Load of Existing V3 Data Files");

				}
	            else
	            {
					frmStatus.SetMaximum(repo.Status.Added.Count + repo.Status.Removed.Count +
					                     repo.Status.Modified.Count);
		            repo.Status.Added.ToList().ForEach(a =>
		            {
			            repo.Index.Add(Path.Combine(GitRepositoryFolder, a));
			            frmStatus.SetValue(frmStatus.Value + 1);
					});
					
		            repo.Status.Removed.ToList().ForEach(a =>
		            {
			            repo.Index.Add(Path.Combine(GitRepositoryFolder, a));
			            frmStatus.SetValue(frmStatus.Value + 1);
					});
		            repo.Status.Modified.ToList().ForEach(a =>
		            {
			            repo.Index.Add(Path.Combine(GitRepositoryFolder, a));
			            frmStatus.SetValue(frmStatus.Value + 1);
					});

		            repo.Commit(@"Commited external profile changes.");
		            frmStatus.SetStatusText(@"Commited external profile changes.");
		            
				}
	            
                frmStatus.Close();
            }

        }
       
        private bool CreateRepositoryIfNotExists()
        {
            bool createRepository = !System.IO.Directory.Exists(System.IO.Path.Combine(GitRepositoryFolder, ".git"));

            if (createRepository)
            {
                Directory.CreateDirectory(Path.Combine(GitRepositoryFolder, ".git"));
                Directory.CreateDirectory(Path.Combine(GitRepositoryFolder, ".git\\refs\\remotes"));
                Directory.CreateDirectory(Path.Combine(GitRepositoryFolder, ".git\\hooks"));
                Directory.CreateDirectory(Path.Combine(GitRepositoryFolder, ".git\\refs\\heads"));
                Directory.CreateDirectory(Path.Combine(GitRepositoryFolder, ".git\\refs\\tags"));
                Directory.CreateDirectory(Path.Combine(GitRepositoryFolder, ".git\\objects\\info"));
                Directory.CreateDirectory(Path.Combine(GitRepositoryFolder, ".git\\objects\\pack"));

                using (var s = new FileStream(System.IO.Path.Combine(GitRepositoryFolder, ".git\\objects\\info\\exclude"), FileMode.Create))
                {
                    s.Write(Properties.Resources.exclude.ToArray(), 0, Properties.Resources.exclude.Length);
                }
                using (var s = new FileStream(System.IO.Path.Combine(GitRepositoryFolder, ".git\\HEAD"), FileMode.Create))
                {
                    s.Write(Properties.Resources.HEAD.ToArray(), 0, Properties.Resources.HEAD.Length);
                }
                using (var s = new FileStream(System.IO.Path.Combine(GitRepositoryFolder, ".git\\config"), FileMode.Create))
                {
                    s.Write(Properties.Resources.config.ToArray(), 0, Properties.Resources.config.Length);
                }
                using (var s = new FileStream(System.IO.Path.Combine(GitRepositoryFolder, ".git\\description"), FileMode.Create))
                {
                    s.Write(Properties.Resources.description.ToArray(), 0, Properties.Resources.description.Length);
                }
                using (var s = new FileStream(System.IO.Path.Combine(GitRepositoryFolder, ".git\\hooks\\README.sample"), FileMode.Create))
                {
                    s.Write(Properties.Resources.README.ToArray(), 0, Properties.Resources.README.Length);
                }

                CreateUpdateGitIgnore();

                var di = new DirectoryInfo(Path.Combine(GitRepositoryFolder, ".git"));
                di.Attributes = FileAttributes.Hidden;

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
