using System;
using System.Collections.Generic;
using TerminologyLauncher.GUI.Windows.ConfigWindow.ConfigObjects;
using TerminologyLauncher.I18n;
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
                var javaExeConfig =
                    new TextInputConfigObject(
                        TranslationManager.GetManager.Localize("JavaPathTranslation", "Java Path:"), "javaExePath",
                        this.Engine.JreManager.JavaRuntime.JavaPath);
                var jvmExtraArguments =
                    new TextInputConfigObject(
                        TranslationManager.GetManager.Localize("ExtraJvmArgumentTranslation", "Extra JvmArguments:"),
                        "extraJvmArguments",
                        this.Engine.InstanceManager.Config.GetConfigString("extraJvmArguments"));
                var memoryconfigs =
                    new RangeRestrictedSelectConfigObject(
                        TranslationManager.GetManager.Localize("MaxiumMemoryAllocate", "Maxium Memory Allocate:"),
                        "maxMemorySizeMega", MachineUtils.GetTotalMemoryInMiB(), 512L,
                        Convert.ToInt64(this.Engine.InstanceManager.Config.GetConfigString("maxMemorySizeMega")));


                var reslut =
                    this.Engine.UiControl.StartConfigWindow(
                        new List<TextInputConfigObject> {javaExeConfig, jvmExtraArguments}, null,
                        new List<RangeRestrictedSelectConfigObject> {memoryconfigs});
                if (reslut == null || !reslut.Value)
                {
                    return;
                }
                try
                {
                    this.Engine.JreManager.JavaRuntime =
                        JavaUtils.GetJavaRuntimeFromJavaExe(javaExeConfig.Value);
                    TerminologyLogger.GetLogger().InfoFormat($"Refreshed jre to {javaExeConfig.Value}");
                }
                catch (Exception)
                {
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("Jre not valid",
                        "The java path that you inputed is not valid! Ignore to set.");
                    TerminologyLogger.GetLogger().Error("Trying to set invalid java exe path. Ignore.");
                }
                this.Engine.InstanceManager.Config.SetConfigString("maxMemorySizeMega", memoryconfigs.Value.ToString());
                TerminologyLogger.GetLogger().InfoFormat($"Refreshed memory size to {memoryconfigs.Value}");

                this.Engine.InstanceManager.Config.SetConfigString("extraJvmArguments", jvmExtraArguments.Value);
                TerminologyLogger.GetLogger().InfoFormat($"Refreshed extra jvm args to {jvmExtraArguments.Value}");
            }
            catch (Exception ex)
            {
                TerminologyLogger.GetLogger()
                    .Error($"Cannot update because {ex}");
                this.Engine.UiControl.MainWindow.PopupNotifyDialog("Cannot launch",
                    $"Caused by an internal error, we cannot update right now. Detail: {ex.Message}");
            }
        }
    }
}