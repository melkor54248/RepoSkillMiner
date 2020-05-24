using GithubModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RepoSkillMiner.Services
{
    public class GitHubApiAuthorService : IAuthorService
    {
        RepoSkillMiner.Services.AppData AppData;
        HttpClient Http;

        public GitHubApiAuthorService(AppData appData, HttpClient http)
        {
            AppData = appData;
           // Http = http;
        }
        /// <summary>
        /// Get a list of technologies and the number of commits for each one, given the author name.
        /// </summary>
        /// <param name="authorName">The name of the author.</param>
        /// <returns></returns>
        public List<TechnologiesCount> GetTechsByName(string authorName)
        {
            return AppData.AuthorsAndTechs.Where(x => x.Login == authorName).FirstOrDefault().Technologies;
        }

        /// <summary>
        /// Get a user given the name.
        /// </summary>
        /// <param name="name">The name of the User</param>
        /// <returns>a <see cref="User"/></returns>
        public async Task<User> GetUserDetailsAsync(string name)
        {
            var user = await AppData.Http.GetFromJsonAsync<User>("https://api.github.com/users/" + name);
            return user;
        }
    }
}
