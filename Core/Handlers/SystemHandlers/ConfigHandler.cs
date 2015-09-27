using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.GUI.ToolkitWindows;
using TerminologyLauncher.GUI.ToolkitWindows.ConfigWindow.ConfigObjects;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.Core.Handlers.SystemHandlers
{
    public class ConfigHandler : HandlerBase
    {
        public ConfigHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.MainWindow.ConfigButton.Click += this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            try
            {
                var textInputConfigs = new List<TextInputConfigObject>
                {
                    new TextInputConfigObject("Java bin path", "javaBinPath",
                        this.Engine.InstanceManager.Config.GetConfig("javaBinPath")),
                    new TextInputConfigObject("Extra jvm arguments", "extraJvmArguments",
                        this.Engine.InstanceManager.Config.GetConfig("extraJvmArguments"))
                };

                var rangeConfigs = new List<RangeRestrictedSelectConfigObject>
                {
                    new RangeRestrictedSelectConfigObject("Max instance memory allocate size(MB)","maxMemorySizeMega",MachineUtils.GetTotalMemoryInMiB(),512L,Convert.ToInt64(this.Engine.InstanceManager.Config.GetConfig("maxMemorySizeMega")))
                };

                var reslut = this.Engine.UiControl.StartMultiConfigWindo(textInputConfigs, null, rangeConfigs);
                if (reslut.Type == WindowResultType.Canceled)
                {
                    return;
                }
                var mixedConfigs = reslut.Result as List<ConfigObject>;
                if (mixedConfigs == null) return;
                this.Engine.InstanceManager.Config.SetConfig("javaBinPath", (mixedConfigs.First(x => x.Key.Equals("javaBinPath")) as TextInputConfigObject).Value);
                this.Engine.InstanceManager.Config.SetConfig("maxMemorySizeMega", (mixedConfigs.First(x => x.Key.Equals("maxMemorySizeMega")) as RangeRestrictedSelectConfigObject).Value.ToString());
                this.Engine.InstanceManager.Config.SetConfig("extraJvmArguments", (mixedConfigs.First(x => x.Key.Equals("extraJvmArguments")) as TextInputConfigObject).Value);
                  return;
            }
            catch (Exception ex)
            {

                Logging.Logger.GetLogger()
                        .Error(String.Format("Can not update because {0}", ex));
                this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MainWindow, "Can not launch", String.Format(
                    "Caused by an internal error, we can not update right now. Detail: {0}", ex.Message));
            }
        }
    }
}
