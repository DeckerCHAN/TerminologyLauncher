using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.Entities.System
{
    public interface IComponent
    {
        void Standby();
        void Shutdown();
    }
}
