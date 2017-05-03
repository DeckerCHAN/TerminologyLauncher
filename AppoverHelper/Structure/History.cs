using System.Collections.Generic;

namespace AppoverHelper.Structure
{
    public class History
    {
        public Project Project { get; set; }
        public ICollection<Build> Builds { get; set; }
    }
}