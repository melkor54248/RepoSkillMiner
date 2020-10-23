
using GithubModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using RepoSkillMiner.Services;
using RepoSkillMiner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;



namespace RepoSkillMiner.Pages
{
    public partial class Scan
    {
        #region Private fields
        private List<AuthorDetails> AuthorsFull = new List<AuthorDetails>();
        private string displayurl;
        private bool firsttime = true;

        private int patchesToScan = 1;
        private int reposToScan = 1;
        private bool scannpressed = false;
        private string url;
        private bool UseLuis = false;
        private List<AuthorsAndTechs> authorsList = new List<AuthorsAndTechs>();
        private List<CommitsFull> commitsWithFiles;
        private Dictionary<string, string> languageslogos = new Dictionary<string, string>()
{
        { "C#","devicon-csharp-plain colored" },
        { "CSharp","devicon-csharp-plain colored"},
        { "CSS","devicon-css3-plain colored" },
        { "Java","devicon-java-plain-wordmark colored" },
        { "JavaScript","devicon-javascript-plain colored" },
        { "HTML","devicon-html5-plain-wordmark colored" },
        { "ASP","devicon-dot-net-plain-wordmark colored" },
        { "ASPnetMVC","devicon-dot-net-plain-wordmark colored" },
        { "Vue","devicon-vuejs-plain-wordmark colored" },
        {"TSQL","perico-sqlserver" },
        {"Ruby","devicon-ruby-plain-wordmark colored" },
        {"PHP","devicon-php-plain colored" },
        {"Python","devicon-python-plain colored" },
        {"Node","devicon-nodejs-plain colored" },
        {"Shell","fa fa-file-code-o" },
        {"PowerShell","fa fa-file-code-o" },
        {"Smarty","devicon-php-plain" },
        {"Batchfile","fa fa-file-code-o" },
        {"Markdown","fab fa-markdown" },
        {"XML","fa fa-file-code-o" },
        {"YAML","fa fa-file-code-o" },
        {"TypeScript","devicon-typescript-plain colored" },
        {"React","devicon-react-original" },
        {"Ignore_List","devicon-github-plain" },
        {"Angular","devicon-angularjs-plain colored" }
    };


        private Organization organization;

        private int reposCount = -1;
        private Repository[] repositories;


        private List<string> tech;
        private Dictionary<string, int> techWeighted;
        #endregion

        #region Public properties
        public List<string> reponames { get; set; }
        public string ReposSearchString { get; set; }
        public string SearchString { get; set; }
        public string selectedRepo { get; set; }
        #endregion

        #region Dependency Injection

        [Inject]
        RepoSkillMiner.Services.AppData AppData { get; set; }


        [Inject]
        System.Net.Http.HttpClient Http { get; set; }
        [Inject]
        private IConfiguration Configuration { get; set; }

        [Inject]
        private IScanService Service { get; set; }
        #endregion Dependency Injection


        /// <summary>
        ///  Handles Scan button Click
        /// </summary>
        /// <returns>List of Commits <see cref="CommitsFull"/></returns>
        public async Task InitScan()
        {
            AppData.Configuration = Configuration;
            authorsList.Clear();
            scannpressed = true;
            commitsWithFiles = await ScanRepos(repositories, reposToScan, selectedRepo);
            AuthorsFull = commitsWithFiles.Where(y => y.Author != null).Select(x => x.Author).DistinctBy(a => a.Login).ToList();
            Service.LuisKey = Configuration["LuisKey"];
            Service.LuisEndPoint = Configuration["LuisEndPoint"];
            AppData.AuthorsAndTechs = await Service.ReportFindingsAsync(commitsWithFiles, UseLuis, patchesToScan, authorsList,Http);
        }

        /// <summary>
        /// Handles Search button Click
        /// </summary>
        /// <returns></returns>
        public async Task Search()
        {
            if (firsttime)
            {
                try
                {
                    AddAuthorizationHeader();

                    firsttime = false;
                }
                catch (Exception ex) { Console.Write(ex.Message); }

            }
            reposCount = -1;


            authorsList.Clear();
            try
            {
                organization = await Service.GetOrganization(SearchString, Http);
                repositories = await Service.GetRepositories(organization, Http);
                var reposlist = repositories.ToList();
                var tempcount = repositories.Count();
                var i = 2;
                while (tempcount == 100)// if there are more than 100 repos iterate  until you have them all.
                {
                    Repository[] temprepos = await Service.GetRepositories(organization, i, Http);
                    i++;
                    tempcount = temprepos.Count();
                    reposlist.AddRange(temprepos);
                }
                repositories = reposlist.ToArray();
                reposCount = repositories.Count();
                reponames = reposlist.Select(x => x.Name).ToList();

                this.StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);

            }
        }

        private void AddAuthorizationHeader()
        {
            Http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "token " + Configuration["GithubToken"]);
            AppData.Http = Http;

        }





        /// <summary>
        /// Scans the repos inside the array
        /// </summary>
        /// <param name="repositories">Array of <see cref="Repository"/></param>
        /// <param name="reposToScan">Number of repositories to scan</param>
        /// <param name="selectedRepo"></param>
        /// <returns>"/>List of Commits <see cref="CommitsFull"/</returns>
        private async Task<List<CommitsFull>> ScanRepos(Repository[] repositories, int reposToScan, string selectedRepo)
        {
            IEnumerable<string> commitsurls;
            if (!string.IsNullOrEmpty(selectedRepo))
            {
                commitsurls = Service.GetCommitUrls(repositories, selectedRepo);

            }
            else
            {
                commitsurls = Service.GetCommitUrls(repositories, reposToScan);
            }
           
            List<CommitsFull> commitsWithFiles = new List<CommitsFull>();

            foreach (var url in commitsurls)
            {
                displayurl = url.Replace(@"https://api.github.com/repos/", "").Replace(@"/commits", "");
                Console.WriteLine($"Scanning {displayurl}");
                this.StateHasChanged();
                await Service.GetCommitsDetails(commitsWithFiles, url, Http);
            }

            return commitsWithFiles;
        }

       

    }

}