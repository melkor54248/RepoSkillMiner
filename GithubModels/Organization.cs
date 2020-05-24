using System;
using System.Collections.Generic;
using System.Text;

namespace GithubModels
{
    public class Organization
    {
        public string Login { get; set; }
        public int Id { get; set; }
        public string Node_id { get; set; }
        public string Url { get; set; }
        public string Repos_url { get; set; }
        public string Events_url { get; set; }
        public string Hooks_url { get; set; }
        public string Issues_url { get; set; }
        public string Members_url { get; set; }
        public string Public_members_url { get; set; }
        public string Avatar_url { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public object Company { get; set; }
        public object Blog { get; set; }
        public string Location { get; set; }
        public object Email { get; set; }
        public bool Is_verified { get; set; }
        public bool Has_organization_projects { get; set; }
        public bool Has_repository_projects { get; set; }
        public int Public_repos { get; set; }
        public int Public_gists { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
        public string Html_url { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public string type { get; set; }
    }
}
