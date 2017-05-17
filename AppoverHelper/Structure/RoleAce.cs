using System.Collections.Generic;

namespace AppoverHelper.Structure
{
    public class RoleAce
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public ICollection<AccessRight> AccessRights { get; set; }

    }
}