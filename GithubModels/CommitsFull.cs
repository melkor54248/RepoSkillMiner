namespace GithubModels
{
   
        public class CommitsFull
        {
            public string Sha { get; set; }
            public string Node_id { get; set; }
            public Commit Commit { get; set; }
            public string Url { get; set; }
            public string Html_url { get; set; }
            public string Comments_url { get; set; }
            public AuthorDetails Author { get; set; }
            public CommiterDetails Committer { get; set; }
            public Parent[] Parents { get; set; }
            public Stats Stats { get; set; }
            public File[] Files { get; set; }
        }

   
}
