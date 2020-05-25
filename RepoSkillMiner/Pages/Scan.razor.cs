
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
            authorsList.Clear();
            scannpressed = true;
            commitsWithFiles = await ScanRepos(repositories, reposToScan, selectedRepo);
            AuthorsFull = commitsWithFiles.Where(y => y.Author != null).Select(x => x.Author).DistinctBy(a => a.Login).ToList();
            await ReportFindingsAsync(commitsWithFiles);
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
                commitsurls = repositories.Where(x => x.Name == selectedRepo).Select(x => x.Commits_url.Replace("{/sha}", ""));


            }
            else
            {
                commitsurls = repositories.OrderBy(x => x.Name).Take(reposToScan).Select(x => x.Commits_url.Replace("{/sha}", ""));
            }
            List<List<CommitDetails>> CommitDetailsLists = new List<List<CommitDetails>>();
            List<CommitsFull> commitsWithFiles = new List<CommitsFull>();

            foreach (var url in commitsurls)
            {
                displayurl = url.Replace(@"https://api.github.com/repos/", "").Replace(@"/commits", "");
                Console.WriteLine($"Scanning {displayurl}");
                this.StateHasChanged();
                CommitDetailsLists = await Service.GetCommitsDetails(commitsWithFiles, url, Http);
            }

            return commitsWithFiles;
        }


        /// <summary>
        /// Make a call to Luis with a string input.
        /// </summary>
        /// <param name="key">Luis Subscription key</param>
        /// <param name="utterance">The input utterance. </param>
        /// <returns></returns>
        private async Task<string> MakeLuisRequestAsync(string key, string utterance)
        {
            var client = new System.Net.Http.HttpClient();

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            var endpointUri = String.Format(Configuration["LuisEndPoint"] + "&q={0}", utterance.Substring(0, Math.Min(utterance.Length, 450)));

            LuisResponse luisResponse = await Http.GetFromJsonAsync<LuisResponse>(endpointUri);

            // Return the top scoring intent from Luis.

            return luisResponse.TopScoringIntent.Intent;
        }

        /// <summary>
        /// Extract author data and tachnologies given the commits data as input.
        /// </summary>
        /// <param name="commitsWithFiles"></param>
        /// <returns></returns>
        private async Task ReportFindingsAsync(List<CommitsFull> commitsWithFiles)
        {
            var authors = commitsWithFiles.Select(x => x.Author?.Login).Distinct().ToList();

            foreach (var author in authors)
            {
                var mycommits = commitsWithFiles.Where(x => x.Author != null && x.Author.Login == author).ToList();
                var files = mycommits.Select(x => x.Files.Select(f => { f.Date = x.Commit.Committer.Date; return f; })).ToList();
                var filesmerged = files.SelectMany(f => f).ToList();
                var filenames = filesmerged.Select(n => n.Filename);

                var techWithDates = new List<TechWithDates>();

                var extentions = filenames.Where(f => f.Contains(".")).Select(x => x.Substring(x.LastIndexOf("."), x.Length - x.LastIndexOf("."))).GroupBy(x => x).
    Select(y => new { Extention = y.Key, Count = y.Count() });
                var extentionsreference = FileExtensions.GetXml;

                tech = new List<string>();
                techWeighted = new Dictionary<string, int>();
                foreach (var file in filesmerged)
                {
                    var filename = file.Filename;
                    if (filename.Contains('.'))
                    {
                        int pos = filename.LastIndexOf(".");
                        var extention = filename.Substring(pos, filename.Length - pos);

                        if (extentionsreference.Descendants("Extension").Any(x => x.Value == extention))
                        {
                            string techCanditate = extentionsreference.Descendants("Extension").FirstOrDefault(x => x.Value == extention).Parent.Parent.Element("Name").Value;
                            if (!tech.Contains(techCanditate))
                            {
                                tech.Add(techCanditate);
                                // techWithDates.Add(new TechWithDates() { Tech = techCanditate, StartDate = file.date, EndDate = filesmerged.Where(x => x.filename == filename).Select(x => x.date).Max() });
                                techWeighted.Add(techCanditate, extentions.Where(x => x.Extention == extention).Select(x => x.Count).FirstOrDefault());
                            }
                        }
                    }
                }

                if (UseLuis)
                {
                    var patches = filesmerged.Select(x => new { x.Patch, x.Filename });
                    var csPatches = patches.Where(x => (x.Filename.EndsWith(".cs") || x.Filename.EndsWith(".js") || x.Filename.EndsWith(".html"))&& !x.Filename.Contains("config.js") );
                    Console.WriteLine($"Patches count:{csPatches.Count()}");
                    int c = 1;
                    var adjpatchesToScan = patchesToScan * 10;
                    if (patchesToScan == 5)
                        adjpatchesToScan = csPatches.Count();
                    Console.WriteLine($"patches to scan={adjpatchesToScan}");
                    foreach (var patch in csPatches.Take(csPatches.Count() > adjpatchesToScan ? adjpatchesToScan : csPatches.Count()))
                    {
                        if (patch != null)
                        {
                            if (c == 3)
                            {
                                System.Threading.Thread.Sleep(1000);
                                c = 0;
                            }
                            c++;
                            var techLuis = "";


                            techLuis = await MakeLuisRequestAsync(Configuration["LuisKey"], patch.Patch ?? "none");
                            if (techLuis != "None" && techLuis != null && !tech.Contains(techLuis))
                            {
                                tech.Add(techLuis);
                                techWeighted.Add(techLuis, 1);
                                Console.WriteLine($"LUIS match: {techLuis} in file: {patch.Filename}, Utterence:{patch.Patch}");
                            }
                            if (techLuis != "None" && techWeighted.ContainsKey(techLuis.ToString()))
                            {
                                techWeighted[techLuis]++;
                            }
                        }
                    }
                }

                authorsList.Add(new AuthorsAndTechs() { Login = author, Avatar_url = AuthorsFull.Where(a => a.Login == author).Select(a => a.Avatar_url).FirstOrDefault(), Technologies = techWeighted.Select(p => new GithubModels.TechnologiesCount { Name = p.Key, Count = p.Value }).ToList(), TechWithDates = techWithDates });
                AppData.AuthorsAndTechs = new List<AuthorsAndTechs>();
                AppData.AuthorsAndTechs = authorsList;
            }
        }
    }

}