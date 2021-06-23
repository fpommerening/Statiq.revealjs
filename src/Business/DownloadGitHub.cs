using Statiq.Core;

namespace FP.Statiq.RevealJS.Business
{
    public class DownloadGitHub : ReadWeb
    {
        private static string GetUrl(string user, string repository, string branch)
        {
            return $"https://github.com/{user}/{repository}/archive/refs/heads/{branch}.zip";
        }

        public DownloadGitHub(string user, string repository, string branch) : base(GetUrl(user, repository, branch))
        {
        }
    }

}
