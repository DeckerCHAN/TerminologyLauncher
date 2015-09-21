using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.GUI.ToolkitWindows;

namespace TerminologyLauncher.Core.Handlers.SystemHandlers
{
    public class ConfigHandler : HandlerBase
    {
        public ConfigHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.MajorWindow.ConfigButton.Click += this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            try
            {
                var configs = new Dictionary<String, String>();
                configs.Add("Java bin path", this.Engine.InstanceManager.Config.GetConfig("javaBinPath"));
                configs.Add("Max instance memory allocate size(MB)", this.Engine.InstanceManager.Config.GetConfig("maxMemorySizeMega"));
                configs.Add("Extra jvm arguments", this.Engine.InstanceManager.Config.GetConfig("extraJvmArguments"));

                var reslut = this.Engine.UiControl.StartMultiConfigWindo("Configs", configs);
                if (reslut.Type == WindowResultType.Canceled)
                {
                    return;
                }
                configs = reslut.Result as Dictionary<String, String>;
                if (configs == null) return;
                this.Engine.InstanceManager.Config.SetConfig("javaBinPath", configs["Java bin path"]);
                this.Engine.InstanceManager.Config.SetConfig("maxMemorySizeMega", configs["Max instance memory allocate size(MB)"]);
                this.Engine.InstanceManager.Config.SetConfig("extraJvmArguments", configs["Extra jvm arguments"]);
                return;
            }
            catch (Exception ex)
            {

                Logging.Logger.GetLogger()
                        .Error(String.Format("Can not update because {0}", ex));
                this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Can not launch", String.Format(
                    "Caused by an internal error, we can not update right now. Detail: {0}", ex.Message));
            }
        }
    }
}
