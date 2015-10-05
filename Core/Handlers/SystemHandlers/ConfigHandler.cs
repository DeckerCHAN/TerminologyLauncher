using System;
using System.Collections.Generic;
using System.Linq;
using TerminologyLauncher.GUI.ToolkitWindows;
using TerminologyLauncher.GUI.ToolkitWindows.ConfigWindow.ConfigObjects;
using TerminologyLauncher.Logging;
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
                        this.Engine.JreManager.JavaRuntime.JavaPath),
                    new TextInputConfigObject("Extra jvm arguments", "extraJvmArguments",
                        this.Engine.InstanceManager.Config.GetConfigString("extraJvmArguments"))
                };

                var rangeConfigs = new List<RangeRestrictedSelectConfigObject>
                {
                    new RangeRestrictedSelectConfigObject("Max instance memory allocate size(MB)","maxMemorySizeMega",MachineUtils.GetTotalMemoryInMiB(),512L,Convert.ToInt64(this.Engine.InstanceManager.Config.GetConfigString("maxMemorySizeMega")))
                };

                var reslut = this.Engine.UiControl.StartMultiConfigWindo(textInputConfigs, null, rangeConfigs);
                if (reslut.Type == WindowResultType.Canceled)
                {
                    return;
                }
                var mixedConfigs = reslut.Result as List<ConfigObject>;
                if (mixedConfigs == null) return;
                try
                {
                    this.Engine.JreManager.JavaRuntime =
                  JavaUtils.GetJavaRuntimeFromJavaExe(
                      (mixedConfigs.First(x => x.Key.Equals("javaBinPath")) as TextInputConfigObject).Value);

                }
                catch (Exception)
                {

                    this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MainWindow, "Jre not valid", "The java path that you inputed is not valid! Ignore to set.");
                }
                this.Engine.InstanceManager.Config.SetConfigString("maxMemorySizeMega", (mixedConfigs.First(x => x.Key.Equals("maxMemorySizeMega")) as RangeRestrictedSelectConfigObject).Value.ToString());
                this.Engine.InstanceManager.Config.SetConfigString("extraJvmArguments", (mixedConfigs.First(x => x.Key.Equals("extraJvmArguments")) as TextInputConfigObject).Value);
                return;
            }
            catch (Exception ex)
            {

                Logger.GetLogger()
                        .Error(String.Format("Can not update because {0}", ex));
                this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MainWindow, "Can not launch", String.Format(
                    "Caused by an internal error, we can not update right now. Detail: {0}", ex.Message));
            }
        }
    }
}
