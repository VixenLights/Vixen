using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitSharp;

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
            DisableWatchers();
            if (enabled)
            {
                var repoCreated = CreateRepositoryIfNotExists();

                repo = new GitSharp.Repository(GitRepositoryFolder);


                AddItemsToGit(repoCreated);
                GetGitDetails();
                CreateWatcher(GitRepositoryFolder, true);
            }
        }

        private void AddItemsToGit(bool initialCheckin)
        {


            var Directories = Directory.GetDirectories(GitRepositoryFolder, "*.*", SearchOption.AllDirectories).ToList();

            Directories.RemoveAll(r => r.Contains(".git") || r.Contains("\\Logs"));
            Directories.Add(GitRepositoryFolder);

            List<string> files = new List<string>();

            Directories.ForEach(dir => files.AddRange(Directory.GetFiles(dir)));




            string commitMessage = initialCheckin ? "Initial Load of Existing V3 Data Files" : "Updated/Added File to V3 Folders";
            if (initialCheckin)
                files.ForEach(file =>
                {
                    repo.Index.Add(file);
                    repo.Commit(commitMessage);
                });


            repo.Status.Added.ToList().ForEach(a =>
            {
                repo.Index.Add(Path.Combine(GitRepositoryFolder, a));
                repo.Commit(string.Format("Added {0}", new FileInfo(Path.Combine(GitRepositoryFolder, a)).Name));
            });
            repo.Status.Removed.ToList().ForEach(a =>
            {
                repo.Index.Add(Path.Combine(GitRepositoryFolder, a));
                repo.Commit(string.Format("Removed {0}", new FileInfo(Path.Combine(GitRepositoryFolder, a)).Name));
            });
            repo.Status.Modified.ToList().ForEach(a =>
            {
                repo.Index.Add(Path.Combine(GitRepositoryFolder, a));
                repo.Commit(string.Format("Changed {0}", new FileInfo(Path.Combine(GitRepositoryFolder, a)).Name));
            });
            


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
                GitSharp.Git.Init(GitRepositoryFolder);
                using (var sw = new StreamWriter(Path.Combine(GitRepositoryFolder, ".gitignore")))
                {
                    sw.WriteLine("/Logs");
                    sw.WriteLine("/.git");

                    sw.Flush();
                }
                DirectoryInfo di = new DirectoryInfo(Path.Combine(GitRepositoryFolder, ".git"));
                di.Attributes = FileAttributes.Hidden;
            }
            return createRepository;
        }

    }
}
