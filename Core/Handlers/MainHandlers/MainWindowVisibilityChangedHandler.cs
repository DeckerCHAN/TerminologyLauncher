using System;
using System.Collections.ObjectModel;
using System.Windows;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.GUI;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.Core.Handlers.MainHandlers
{
    public class MainWindowVisibilityChangedHandler : HandlerBase
    {
        public override void HandleEvent(Object sender, EventArgs e)
        {
            throw new NotSupportedException();
        }

        public void HandleEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            Logger.GetLogger().Debug("Handle main show or hide at here.");
            var window = (MainWindow)this.Engine.UiControl.MainWindow;
            Logger.GetLogger().Debug(String.Format("Main window changed status to {0}", window.Visibility));
            //TODO:clear or load data at here.
            switch (window.Visibility)
            {
                case Visibility.Hidden:
                    {

                        break;
                    }
                case Visibility.Visible:
                    {
                        if (this.Engine.UiControl.MajorWindow.SelectInstance == null)
                        {
                            this.Engine.UiControl.MajorWindow.InstanceList =
                                         new ObservableCollection<InstanceEntity>(this.Engine.InstanceManager.InstancesWithLocalImageSource);
                        }

                        break;
                    }
                default:
                    {
                        Logger.GetLogger().Error(String.Format("HandlerBase could not handle {0} status.", window.Visibility));
                        break;
                    }
            }
            return;
        }

        public MainWindowVisibilityChangedHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.MainWindow.IsVisibleChanged += this.HandleEvent;
        }
    }
}
