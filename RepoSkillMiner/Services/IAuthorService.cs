using GithubModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoSkillMiner.Services
{
    interface IAuthorService
    {
        public List<TechnologiesCount> GetTechsByName(string authorName);
        Task<User> GetUserDetailsAsync(string name);
    }
}
