using System;
using System.Collections.Generic;

namespace TerminologyLauncher.Entities.Account.AdditionalInfo
{
    public class AdditionalInfoEntity : ResponseEntity
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public List<PropertiyEntity> Properties { get; set; } 
    }
}
