using System;
using System.Collections.Generic;

namespace TerminologyLauncher.Entities.Account.AdditionalInfo
{
    public class AdditionalInfoEntity : ResponseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<PropertiyEntity> Properties { get; set; } 
    }
}
