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

namespace RepoSkillMiner.Services
{
    public class GitHubApiScanService : IScanService
    {
        [Inject]
        private IConfiguration Configuration { get; set; }

      //  HttpClient Http;

        //public GitHubApiScanService(HttpClient http)
        //{
        //    Http = http;

        //}

        RepoSkillMiner.Services.AppData AppData { get; set; }
        /// <summary>
        /// Get a list with <see cref="CommitDetails"/> lists
        /// </summary>
        /// <param name="repocommits"></param>
        /// <param name="commitsWithFiles"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<List<List<CommitDetails>>> GetCommitsDetails(List<CommitsFull> commitsWithFiles, string url, HttpClient Http)
        {
            var ListOfCommitDetailsList = new List<List<CommitDetails>>();
            try
            {



                var commitDetailList = await Http.GetFromJsonAsync<List<CommitDetails>>(url);// + "?per_page=100");
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
                return ListOfCommitDetailsList;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return null;
            }
        }

       

        /// <summary>
        /// Get The organisation given the name.
        /// </summary>
        /// <param name="SearchString">The name of the organization</param>
        /// <returns>An <see cref="Organization"/></returns>
        public Task<Organization> GetOrganization(string SearchString, HttpClient Http)
        {
            return Http.GetFromJsonAsync<Organization>("https://api.github.com/orgs/" + SearchString);
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
        /// <param name="organization">The <see cref="Organization"/></param>
        /// <param name="pageNumber">The page nubmer (github fetches only 100 per page). </param>
        /// <returns>List of <see cref="Repository"/></returns>
        public Task<Repository[]> GetRepositories(Organization organization, int pageNumber, HttpClient Http)
        {
            return Http.GetFromJsonAsync<Repository[]>(organization.Repos_url + $"?page={pageNumber}&per_page=100");
        }


    }
}
