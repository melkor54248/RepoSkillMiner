namespace GithubModels
{
    
        public class Commit
        {
            public Author Author { get; set; }
            public Committer Committer { get; set; }
            public string Message { get; set; }
            public Tree Tree { get; set; }
            public string Url { get; set; }
            public int Comment_count { get; set; }
            public Verification Verification { get; set; }
        }

   
}
