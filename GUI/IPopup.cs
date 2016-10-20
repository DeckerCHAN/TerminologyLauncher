using System;
using System.Collections.Generic;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.GUI.ToolkitWindows.ProgressWindow;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.GUI
{
    public interface IPopup
    {
        void PopupNotifyDialog(string title, string content);
        bool? PopupConfirmDialog(string title, string content);
        bool? PopupSingleSelectDialog(string title, string fieldName, IEnumerable<string> options, FieldReference<string> selection);
        bool? PopupSingleLineInputDialog(string title, string fieldName, FieldReference<string> content);
        ProgressWindow BeginPopupProgressWindow(Progress progress);
    }
}
