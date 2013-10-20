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

        #region Properties

        public string GitRepositoryFolder { get { return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Vixen 3"); } }
        public Dictionary<string, List<ChangeDetails>> GitDetails { get; set; }

        #endregion

        private void EnableDisableSourceControl(bool enabled)
        {
            _showCommand.Enabled = enabled;

            DisableWatchers();

            if (enabled)
            {
                var repoCreated = CreateRepositoryIfNotExists();

                repo = new GitSharp.Repository(GitRepositoryFolder);

                // repo.WorkingDirectory = GitRepositoryFolder;

                AddItemsToGit(repoCreated);
                GetGitDetails();
                CreateWatcher(GitRepositoryFolder, true);
            }
        }

        private void AddItemsToGit(bool initialCheckin)
        {


            var directories = Directory.GetDirectories(GitRepositoryFolder, "*.*", SearchOption.AllDirectories).ToList();

            directories.RemoveAll(r => r.Contains(".git") || r.Contains("\\Logs"));
            directories.Add(GitRepositoryFolder);

            var files = new List<string>();

            directories.ForEach(dir => files.AddRange(Directory.GetFiles(dir)));

            using (var frmStatus = new Status())
            {

                frmStatus.Show();

                frmStatus.SetStatusText("Gathering Items to Add to Source Control");
                if (initialCheckin)
                    frmStatus.SetMaximum(files.Count);
                else
                    frmStatus.SetMaximum(files.Count + repo.Status.Added.Count + repo.Status.Removed.Count +
                                       repo.Status.Modified.Count);
               
                frmStatus.SetStatusText("Adding Items");
                string commitMessage = initialCheckin
                    ? "Initial Load of Existing V3 Data Files"
                    : "Updated/Added File to V3 Folders";
                if (initialCheckin)
                    files.ForEach(file =>
                    {
                        repo.Index.Add(file);
                        repo.Commit(commitMessage);
                        frmStatus.SetStatusText(commitMessage);
                        frmStatus.SetValue(frmStatus.Value + 1);
                    });
                // repo.Index.AddAll();

                repo.Status.Added.ToList().ForEach(a =>
                {
                    repo.Index.Add(Path.Combine(GitRepositoryFolder, a));
                    commitMessage = string.Format("Added {0}", new FileInfo(Path.Combine(GitRepositoryFolder, a)).Name);
                    repo.Commit(commitMessage);
                    frmStatus.SetStatusText(commitMessage);
                    frmStatus.SetValue(frmStatus.Value + 1);
                });
                repo.Status.Removed.ToList().ForEach(a =>
                {
                    repo.Index.Add(Path.Combine(GitRepositoryFolder, a));
                    commitMessage = string.Format("Removed {0}", new FileInfo(Path.Combine(GitRepositoryFolder, a)).Name);
                    repo.Commit(commitMessage);
                    frmStatus.SetStatusText(commitMessage);
                    frmStatus.SetValue(frmStatus.Value + 1);
                });
                repo.Status.Modified.ToList().ForEach(a =>
                {
                    repo.Index.Add(Path.Combine(GitRepositoryFolder, a));
                    commitMessage = string.Format("Changed {0}", new FileInfo(Path.Combine(GitRepositoryFolder, a)).Name);
                    repo.Commit(commitMessage);
                    frmStatus.SetStatusText(commitMessage);
                    frmStatus.SetValue(frmStatus.Value + 1);
                });
                frmStatus.Close();

            }

        }
        private void GetGitDetails()
        {

            var tree = repo.Head;
            if (repo.Head.CurrentCommit != null)
                foreach (Commit commit in repo.Head.CurrentCommit.Ancestors)
                {
                    foreach (Change change in commit.Changes)
                    {
                        ChangeDetails details = new ChangeDetails();
                        details.FileName = change.Path;
                        details.Hash = commit.Hash;
                        details.ChangeDate = commit.AuthorDate;
                        details.UserName = commit.Author.Name;
                        details.Message = commit.Message;

                        if (!GitDetails.ContainsKey(change.Path)) GitDetails.Add(change.Path, new List<ChangeDetails>());
                        GitDetails[change.Path].Add(details);

                    }
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

                if (!System.IO.File.Exists(Path.Combine(GitRepositoryFolder, ".gitignore")))
                {
                    using (var sw = new StreamWriter(Path.Combine(GitRepositoryFolder, ".gitignore")))
                    {
                        sw.WriteLine("/Logs");
                        sw.WriteLine("/.git");

                        sw.Flush();
                    }
                }

                var di = new DirectoryInfo(Path.Combine(GitRepositoryFolder, ".git"));
                di.Attributes = FileAttributes.Hidden;

            }
            return createRepository;
        }

    }
}
