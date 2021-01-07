using System;
using System.Collections.Generic;
using System.Text;

namespace GithubModels
{
    /// <summary>
    /// The response of the v3 LUIS API 
    /// </summary>
    public class LuisResponseV3
    {

        
            public string query { get; set; }
            public Prediction prediction { get; set; }
        

        public class Prediction
        {
            public string topIntent { get; set; }
            //public Intents intents { get; set; }
            //public Entities entities { get; set; }
        }

        //public class Intents
        //{
           
        //}

       

        //public class Entities
        //{
        //}

    }
}
