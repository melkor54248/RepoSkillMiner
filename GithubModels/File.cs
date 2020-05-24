using System;

namespace GithubModels
{

    public class File
    {
        public string Sha { get; set; }
        public string Filename { get; set; }
        public string Status { get; set; }
        public int Additions { get; set; }
        public int Deletions { get; set; }
        public int Changes { get; set; }
        public string Blob_url { get; set; }
        public string Raw_url { get; set; }
        public string Contents_url { get; set; }
        public string Patch { get; set; }
        public DateTime Date { get; set; }
    }


}
