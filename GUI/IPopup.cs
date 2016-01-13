using System;
using System.Collections.Generic;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.GUI.ToolkitWindows.ProgressWindow;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.GUI
{
    public interface IPopup
    {
        void PopupNotifyDialog(String title, String content);
        Boolean? PopupConfirmDialog(String title, String content, Boolean? decision);
        Boolean? PopupSingleSelectDialog(String title, String fieldName, IEnumerable<String> options, FieldReference<String> selection);
        Boolean? PopupSingleLineInputDialog(String title, String fieldName, FieldReference<String> content);
        ProgressWindow BeginPopupProgressWindow(Progress progress);
    }
}
