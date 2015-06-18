using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.Entities.Account
{
    public class Player
    {
        public LoginMode LoginMode { get; set; }
        public String PlayerName { get; set; }
        public String PlayerAvatarImagePath { get; set; }
    }
}
