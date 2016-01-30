using System;

namespace TerminologyLauncher.Entities.Account.AdditionalInfo.Properties
{
    public class TexturesInfoEntity
    {
        public Int64 Timestamp { get; set; }
        public String ProfileId { get; set; }
        public String ProfileName { get; set; }
        public Boolean IsPublic { get; set; }
        public TexturesEntity Textures { get; set; }

    }
}
