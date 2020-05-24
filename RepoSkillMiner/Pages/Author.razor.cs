using GithubModels;
using Microsoft.AspNetCore.Components;
using RepoSkillMiner.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RepoSkillMiner.Pages
{
    public partial class Author
    {
        [Parameter]
        public string name { get; set; }

        public List<TechnologiesCount> Techs { get; set; }

        public User User { get; set; }
        [Inject]
        private IAuthorService  Service { get; set; }


       
        protected override void OnInitialized()
        {
            this.Techs = Service.GetTechsByName(name); //get the technologies t
        }

        protected override async Task OnInitializedAsync()
        {
            this.User = await Service.GetUserDetailsAsync(name);
        }
    }
}