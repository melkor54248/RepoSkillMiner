namespace GithubModels
{
    public class LuisResponse
    {
        public string Query { get; set; }
        public Topscoringintent TopScoringIntent { get; set; }
        public Intent[] Intents { get; set; }
        public object[] Entities { get; set; }
    }

}
