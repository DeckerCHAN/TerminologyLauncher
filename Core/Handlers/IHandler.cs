using System;

namespace TerminologyLauncher.Core.Handlers
{
    public interface IHandler
    {
        void HandleEvent(Object sender, EventArgs e);
    }
}
