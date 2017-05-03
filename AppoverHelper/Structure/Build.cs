using System.Collections.Generic;

namespace AppoverHelper.Structure
{
    public class Build
    {
        public int BuildId { get; set; }
        public ICollection<Job> Jobs { get; set; }
        public int BuildNumber { get; set; }
        public string Version { get; set; }
        public string Message { get; set; }
        public string Branch { get; set; }
        public bool IsTag { get; set; }
        public string CommitId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorUsername { get; set; }
        public string CommitterName { get; set; }

        public string CommitterUsername { get; set; }
        public string Committed { get; set; }
        public ICollection<string> Messages { get; set; }


        public string Status { get; set; }
        public string Started { get; set; }
        public string Finished { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
    }
}