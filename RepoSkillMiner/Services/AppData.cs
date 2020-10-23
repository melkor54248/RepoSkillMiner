using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GithubModels;
using Microsoft.Extensions.Configuration;

namespace RepoSkillMiner.Services
{
    public class AppData
    {
        
        public List<AuthorsAndTechs> AuthorsAndTechs { get; set; }
        public HttpClient  Http { get; set; }

        public IConfiguration Configuration { get; set; }
    }
}
