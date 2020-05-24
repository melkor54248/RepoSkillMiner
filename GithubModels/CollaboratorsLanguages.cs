using System;
using System.Collections.Generic;
using System.Text;

namespace GithubModels
{
    public class CollaboratorsLanguages
    {
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public List<string> Languages { get; set; }
    }
}
