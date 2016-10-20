using System;

namespace TerminologyLauncher.Entities.Account.AdditionalInfo.Properties
{
    public class TexturesInfoEntity
    {
        public long Timestamp { get; set; }
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public bool IsPublic { get; set; }
        public TexturesEntity Textures { get; set; }

    }
}
