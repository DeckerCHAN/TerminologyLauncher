using System.Collections.Generic;

namespace AppoverHelper.Structure
{
    public class SecurityDescriptor
    {
        public ICollection<AccessRightDefinition> AccessRightDefinitions { get; set; }
        public ICollection<RoleAce> RoleAces { get; set; }
    }
}