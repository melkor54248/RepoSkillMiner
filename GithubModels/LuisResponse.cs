namespace GithubModels
{
    /// <summary>
/// The response of the v2 LUIS API
/// </summary>
    public class LuisResponse
    {
        public string Query { get; set; }
        public Topscoringintent TopScoringIntent { get; set; }
        public Intent[] Intents { get; set; }
        public object[] Entities { get; set; }
    }

}
