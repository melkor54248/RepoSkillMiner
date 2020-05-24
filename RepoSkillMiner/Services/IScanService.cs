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


        public Task<List<List<CommitDetails>>> GetCommitsDetails(  List<CommitsFull> commitsWithFiles, string url, HttpClient Http);
        public Task<Repository[]> GetRepositories(Organization organization, HttpClient Http);

        public Task<Repository[]> GetRepositories(Organization organization,int pageNumber, HttpClient Http);
        public Task<Organization> GetOrganization(string SearchString,HttpClient Http);
         
      }
}
