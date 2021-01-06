using GithubModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RepoSkillMiner.Services
{
    public interface IScanService
    {


        public Task GetCommitsDetails(List<CommitsFull> commitsWithFiles, string url, HttpClient Http);
        public Task<Repository[]> GetRepositories(Organization organization, HttpClient Http);
        public Task<Repository[]> GetRepositories(string owner, int pageNumber, HttpClient Http);

        public Task<Repository[]> GetRepositories(Organization organization, int pageNumber, HttpClient Http);
        public Task<Repository[]> GetRepositories(string owner, HttpClient Http);
        public Task<Organization> GetOrganization(string SearchString, HttpClient Http);
        public Task<List<AuthorsAndTechs>> ReportFindingsAsync(List<CommitsFull> commitsWithFiles, bool UseLuis, int patchesToScan, List<AuthorsAndTechs> authorsList, HttpClient http);
        public IEnumerable<string> GetCommitUrls(Repository[] repositories, int reposToScan);
        public IEnumerable<string> GetCommitUrls(Repository[] repositories, string selectedRepo);
       
        public List<AuthorDetails> GetAuthorDetails(List<CommitsFull> commits);

        public string LuisKey { get; set; }
        public string LuisEndPoint { get; set; }

        Task<User> GetUser(string searchString, HttpClient http);
    }
}
