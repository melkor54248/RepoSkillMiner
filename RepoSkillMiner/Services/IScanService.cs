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


        public Task<List<List<CommitDetails>>> GetCommitsDetails(  List<CommitsFull> commitsWithFiles, string url);
        public Task<Repository[]> GetRepositories(Organization organization);

        public Task<Repository[]> GetRepositories(Organization organization,int pageNumber);
        public Task<Organization> GetOrganization(string SearchString);
         
      }
}
