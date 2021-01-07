using GithubModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Json;
using RepoSkillMiner.Utilities;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RepoSkillMiner.Services
{
    public class GitHubApiScanService : IScanService
    {
        [Inject]
        private IConfiguration Configuration { get; set; }
         
        
        [Inject]
        ILogger Logger { get; set; }
        [Inject]
        ILoggerFactory LoggerFactory { get; set; }


        //  HttpClient Http;

        //public GitHubApiScanService(HttpClient http)
        //{
        //    Http = http;

        //}

        public List<AuthorDetails> GetAuthorDetails(List<CommitsFull> commits)
        {
            return commits.Where(y => y.Author != null).Select(x => x.Author).DistinctBy(a => a.Login).ToList();
        }

        RepoSkillMiner.Services.AppData AppData { get; set; }
        /// <summary>
        /// Get a list with <see cref="CommitDetails"/> lists
        /// </summary>
        /// <param name="repocommits"></param>
        /// <param name="commitsWithFiles"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task  GetCommitsDetails(List<CommitsFull> commitsWithFiles, string url, HttpClient Http)
        {
            var ListOfCommitDetailsList = new List<List<CommitDetails>>();
            try
            {



                var commitDetailList = await Http.GetFromJsonAsync<List<CommitDetails>>(url + "?per_page=100");// + "?per_page=100");
                int i = 2;
                var tempList = commitDetailList;
                while (tempList.Count == 100)
                {
                    tempList = await Http.GetFromJsonAsync<List<CommitDetails>>(url + $"?page={i}&per_page = 100");
                    commitDetailList.AddRange(tempList);
                    i++;
                }
                ListOfCommitDetailsList.Add(commitDetailList);
                var commitlist = ListOfCommitDetailsList.SelectMany(x => x).ToList();
                var commitsDetailsUrl = commitlist.Select(x => x.Url);
                foreach (var commiturl in commitsDetailsUrl)
                {

                    var commitresponse = await Http.GetFromJsonAsync<CommitsFull>(commiturl);
                    commitsWithFiles.Add(commitresponse);

                }
               

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
              
            }
        }



        /// <summary>
        /// Get The organisation given the name.
        /// </summary>
        /// <param name="SearchString">The name of the organization</param>
        /// <returns>An <see cref="Organization"/></returns>
        public async Task<Organization> GetOrganization(string SearchString, HttpClient Http)
        {
          using var httpResponse = await Http.GetAsync("https://api.github.com/orgs/" + SearchString);

            if (httpResponse.IsSuccessStatusCode)
            {
                // set error message for display, log to console and return
                return await  httpResponse.Content.ReadFromJsonAsync<Organization>();

            }
            return null;
           
        }

        /// <summary>
        /// Get all Repositories for the given organization
        /// </summary>
        /// <param name="organization">The <see cref="Organization"/></param>
        /// <returns>List of <see cref="Repository"/></returns>
        public Task<Repository[]> GetRepositories(Organization organization, HttpClient Http)
        {
            return Http.GetFromJsonAsync<Repository[]>(organization.Repos_url + "?per_page=100");
        }
        /// <summary>
        /// Get all Repositories for the given organization
        /// </summary>
        /// <param name="owner">The name of the organisation or user </param>
        /// <returns>List of <see cref="Repository"/></returns>
        public Task<Repository[]> GetRepositories(string owner, HttpClient Http)
        {
            return Http.GetFromJsonAsync<Repository[]>("https://api.github.com/users/"+owner+"/repos" + "?per_page=100");
        }
        /// <summary>
        /// Get all Repositories for the given organization
        /// </summary>
        /// <param name="owner">The name of the organisation or user </param>
        /// <returns>List of <see cref="Repository"/></returns>
        public Task<Repository[]> GetRepositories(string owner, int pageNumber, HttpClient Http)
        {
            return Http.GetFromJsonAsync<Repository[]>("https://api.github.com/users/" + owner + "/repos" + $"?page={pageNumber}&per_page=100");
        }

        /// <summary>
        /// Get all Repositories for the given organization
        /// </summary>
        /// <param name="organization">The <see cref="Organization"/></param>
        /// <param name="pageNumber">The page nubmer (github fetches only 100 per page). </param>
        /// <returns>List of <see cref="Repository"/></returns>
        public Task<Repository[]> GetRepositories(Organization organization, int pageNumber, HttpClient Http)
        {
            return Http.GetFromJsonAsync<Repository[]>(organization.Repos_url + $"?page={pageNumber}&per_page=100");
        }

        /// <summary>
        /// Extract author data and tachnologies given the commits data as input.
        /// </summary>
        /// <param name="commitsWithFiles"></param>
        /// <returns></returns>
        public async Task<List<AuthorsAndTechs>> ReportFindingsAsync(List<CommitsFull> commitsWithFiles, bool UseLuis, int patchesToScan, List<AuthorsAndTechs> authorsList,HttpClient http)
        {
            try
            {
                List<AuthorDetails> AuthorsFull = new List<AuthorDetails>();
                Dictionary<string, int> techWeighted;
                List<string> tech;
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
                        var csPatches = patches.Where(x => (x.Filename.EndsWith(".cs") || x.Filename.EndsWith(".js") || x.Filename.EndsWith(".html")) && !x.Filename.Contains("config.js"));
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


                                
                                techLuis = await MakeLuisRequestAsync(LuisKey, patch.Patch ?? "none",http);
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
                   
                }
                return authorsList;
            }
            catch (Exception ex)
            {

               Console.WriteLine("Error:"+ex.Message+ex.StackTrace);
                return new List<AuthorsAndTechs>();
            }
            

        }
        /// <summary>
        /// Make a call to Luis with a string input.
        /// </summary>
        /// <param name="key">Luis Subscription key</param>
        /// <param name="utterance">The input utterance. </param>
        /// <returns></returns>
        private async Task<string> MakeLuisRequestAsync(string key, string utterance,HttpClient http)
        {
            var client = new System.Net.Http.HttpClient();

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            var endpointUri = String.Format(LuisEndPoint + "&query={0}", utterance.Substring(0, Math.Min(utterance.Length, 450)));
            LuisResponseV3 luisResponse=null;
            try
            {
                  luisResponse = await http.GetFromJsonAsync<LuisResponseV3>(endpointUri);
            }
            catch (Exception ex)
            {

                Console.WriteLine("GOTCHA:"  +ex.Message + ex.StackTrace);
            }

            // Return the top scoring intent from Luis.

            return luisResponse?.prediction.topIntent; //TopScoringIntent?.Intent;
        }

        public   string LuisKey { get; set; }
        public   string LuisEndPoint { get; set; }

        public IEnumerable<string> GetCommitUrls(Repository[] repositories, int reposToScan)
        {
            return repositories.OrderBy(x => x.Name).Take(reposToScan).Select(x => x.Commits_url.Replace("{/sha}", ""));
        }

        public IEnumerable<string> GetCommitUrls(Repository[] repositories, string selectedRepo)
        {
            return repositories.Where(x => x.Name == selectedRepo).Select(x => x.Commits_url.Replace("{/sha}", ""));
        }

        public Task<User> GetUser(string searchString, HttpClient http)
        {
            return http.GetFromJsonAsync<User>("https://api.github.com/users/" + searchString);
        }
    }
}
