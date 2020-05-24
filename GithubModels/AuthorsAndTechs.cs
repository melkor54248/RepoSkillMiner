using System;
using System.Collections.Generic;
using System.Text;

namespace GithubModels
{
    public class AuthorsAndTechs
    {

        public string Login { get; set; }
        public string Avatar_url { get; set; }
        public List<TechnologiesCount> Technologies { get; set; }
        public List<TechWithDates> TechWithDates { get;set;}
    }
}
