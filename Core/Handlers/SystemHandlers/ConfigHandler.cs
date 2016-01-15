using System;
using System.Collections.Generic;
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
                var javaExeConfig = new TextInputConfigObject(I18n.TranslationProvider.TranslationProviderInstance.TranslationObject.GuiTranslation.ConfigWindowTranslation.JavaPathTranslation, "javaExePath",
                    this.Engine.JreManager.JavaRuntime.JavaPath);
                var jvmExtraArguments = new TextInputConfigObject(I18n.TranslationProvider.TranslationProviderInstance.TranslationObject.GuiTranslation.ConfigWindowTranslation.ExtraJvmArgumentTranslation, "extraJvmArguments",
                    this.Engine.InstanceManager.Config.GetConfigString("extraJvmArguments"));
                var memoryconfigs = new RangeRestrictedSelectConfigObject(I18n.TranslationProvider.TranslationProviderInstance.TranslationObject.GuiTranslation.ConfigWindowTranslation.MaxiumMemoryAllocateTranslation,
                    "maxMemorySizeMega", MachineUtils.GetTotalMemoryInMiB(), 512L,
                    Convert.ToInt64(this.Engine.InstanceManager.Config.GetConfigString("maxMemorySizeMega")));


                var reslut = this.Engine.UiControl.StartConfigWindow(new List<TextInputConfigObject> { javaExeConfig, jvmExtraArguments }, null, new List<RangeRestrictedSelectConfigObject> { memoryconfigs });
                if (reslut == null || !reslut.Value)
                {
                    return;
                }
                try
                {
                    this.Engine.JreManager.JavaRuntime =
                  JavaUtils.GetJavaRuntimeFromJavaExe(javaExeConfig.Value);
                    Logger.GetLogger().InfoFormat("Refreshed jre to {0}", javaExeConfig.Value);
                }
                catch (Exception)
                {
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("Jre not valid", "The java path that you inputed is not valid! Ignore to set.");
                    Logger.GetLogger().Error("Trying to set invalid java exe path. Ignore.");
                }
                this.Engine.InstanceManager.Config.SetConfigString("maxMemorySizeMega", memoryconfigs.Value.ToString());
                Logger.GetLogger().InfoFormat("Refreshed memory size to {0}", memoryconfigs.Value);

                this.Engine.InstanceManager.Config.SetConfigString("extraJvmArguments", jvmExtraArguments.Value);
                Logger.GetLogger().InfoFormat("Refreshed extra jvm args to {0}", jvmExtraArguments.Value);
            }
            catch (Exception ex)
            {

                Logger.GetLogger()
                        .Error(String.Format("Can not update because {0}", ex));
                this.Engine.UiControl.MainWindow.PopupNotifyDialog("Can not launch", String.Format(
                    "Caused by an internal error, we can not update right now. Detail: {0}", ex.Message));
            }
        }
    }
}
