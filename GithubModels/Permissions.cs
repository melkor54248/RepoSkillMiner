using System;
using System.Collections.Generic;
using System.Text;

namespace GithubModels
{
    public class Permissions
    {
        public bool Admin { get; set; }
        public bool Push { get; set; }
        public bool Pull { get; set; }
    }
}
