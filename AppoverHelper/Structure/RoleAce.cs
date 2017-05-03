using System.Collections.Generic;

namespace AppoverHelper.Structure
{
    public class RoleAce
    {
        public int roleId { get; set; }
        public string name { get; set; }
        public bool isAdmin { get; set; }
        public ICollection<AccessRight> accessRights { get; set; }

    }
}