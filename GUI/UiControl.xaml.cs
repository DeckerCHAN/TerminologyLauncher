﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.GUI.Animations;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.GUI.ToolkitWindows.NotifyWindow;
using TerminologyLauncher.GUI.ToolkitWindows.ProgressWindow;
using TerminologyLauncher.GUI.ToolkitWindows.SingleLineInput;
using TerminologyLauncher.GUI.ToolkitWindows.SingleSelect;
using TerminologyLauncher.GUI.Windows.ConfigWindows;
using TerminologyLauncher.GUI.Windows.ConfigWindows.ConfigObjects;
using TerminologyLauncher.GUI.Windows.InstanceCreator;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.GUI
{
    /// <summary>
    /// Interaction logic for UiControl.xaml
    /// </summary>
    public partial class UiControl : IPopup
    {
        public InstanceCreateWindow InstanceCrateWindow { get; set; }
        //This new keywork hidding base MainWindow
        public new Windows.MainWindow MainWindow { get; set; }
        public Windows.LoginWindow LoginWindow { get; set; }
        public Windows.ConsoleWindow ConsoleWindow { get; set; }
        public Config Config { get; set; }

        public UiControl(string configPath)
        {
            this.Config = new Config(new FileInfo(configPath));
            this.MainWindow = new Windows.MainWindow(this.Config);
            this.LoginWindow = new Windows.LoginWindow(this.Config);
            this.ConsoleWindow = new Windows.ConsoleWindow(this.Config);
            this.InstanceCrateWindow = new InstanceCreateWindow();
        }

        public Storyboard FadeInStoryboard => Fade.CreateFadeInStoryboard(TimeSpan.FromMilliseconds(500));

        public Storyboard FadeOutStoryboard => Fade.CreateFadeOutStoryboard(TimeSpan.FromMilliseconds(500));

        public void ShowLoginWindow()
        {
            try
            {
                this.LoginWindow.Dispatcher.Invoke(
                    () => { Fade.FadeInShowWindow(this.LoginWindow, TimeSpan.FromMilliseconds(300)); });
            }
            catch (Exception ex)
            {
                Logging.TerminologyLogger.GetLogger().FatalFormat($"Cannot show login window right now! Cause:{ex}");
                this.Shutdown();
            }
        }

        public void HideLoginWindow()
        {
            try
            {
                this.LoginWindow.Dispatcher.Invoke(
                    () => { Fade.FadeOutHideWindow(this.LoginWindow, TimeSpan.FromMilliseconds(300)); });
            }
            catch (Exception ex)
            {
                Logging.TerminologyLogger.GetLogger().FatalFormat($"Cannot hide login window right now! Cause:{ex}");
                this.Shutdown();
            }
        }

        public void ShowMainWindow()
        {
            try
            {
                this.MainWindow.Dispatcher.Invoke(
                    () => { Fade.FadeInShowWindow(this.MainWindow, TimeSpan.FromMilliseconds(300)); });
            }
            catch (Exception ex)
            {
                Logging.TerminologyLogger.GetLogger().FatalFormat($"Cannot show main window right now! Cause:{ex}");
                this.Shutdown();
            }
        }

        public void HideMainWindow()
        {
            try
            {
                this.MainWindow.Dispatcher.Invoke(
                    () => { Fade.FadeOutHideWindow(this.MainWindow, TimeSpan.FromMilliseconds(300)); });
            }
            catch (Exception ex)
            {
                Logging.TerminologyLogger.GetLogger()
                    .FatalFormat($"Cannot hide main window right now! Cause:{ex.Message}");
                this.Shutdown();
            }
        }

        public void ShowConsoleWindow()
        {
            try
            {
                this.MainWindow.Dispatcher.Invoke(
                    () => { Fade.FadeInShowWindow(this.ConsoleWindow, TimeSpan.FromMilliseconds(300)); });
            }
            catch (Exception ex)
            {
                Logging.TerminologyLogger.GetLogger().FatalFormat($"Cannot show console window right now! Cause:{ex}");
                this.Shutdown();
            }
        }

        public void HideConsoleWindow()
        {
            try
            {
                this.MainWindow.Dispatcher.Invoke(
                    () => { Fade.FadeOutHideWindow(this.ConsoleWindow, TimeSpan.FromMilliseconds(300)); });
            }
            catch (Exception ex)
            {
                Logging.TerminologyLogger.GetLogger().FatalFormat($"Cannot hide console window right now! Cause:{ex}");
                this.Shutdown();
            }
        }

        //TODO: Build config window in construction!
        public bool? StartConfigWindow(IEnumerable<TextInputConfigObject> textInputConfigs,
            IEnumerable<ItemSelectConfigObject> itemSelectConfigs,
            IEnumerable<RangeRestrictedSelectConfigObject> rangeRestrictedSelectConfigs)
        {
            bool? result = false;
            this.Dispatcher.Invoke(
                () =>
                {
                    result =
                        new ConfigWindow(textInputConfigs, itemSelectConfigs, rangeRestrictedSelectConfigs)
                            .ShowDialog();
                });
            return result;
        }

        public void ShowInstanceCreateWindow()
        {
            try
            {
                this.MainWindow.Dispatcher.Invoke(
                    () => { Fade.FadeInShowWindow(this.InstanceCrateWindow, TimeSpan.FromMilliseconds(300)); });
            }
            catch (Exception ex)
            {
                Logging.TerminologyLogger.GetLogger().FatalFormat($"Cannot show instnace create window right now! Cause:{ex}");
                this.Shutdown();
            }
        }

        public void HideInstanceCreateWindow()
        {
            try
            {
                this.MainWindow.Dispatcher.Invoke(
                    () => { Fade.FadeOutHideWindow(this.InstanceCrateWindow, TimeSpan.FromMilliseconds(300)); });
            }
            catch (Exception ex)
            {
                Logging.TerminologyLogger.GetLogger()
                    .FatalFormat($"Cannot hide instnace create right now! Cause:{ex.Message}");
                this.Shutdown();
            }
        }

        private void UiControl_OnExit(object sender, ExitEventArgs e)
        {
            this.MainWindow.Close();
            this.LoginWindow.Close();
        }

        public void PopupNotifyDialog(string title, string content)
        {
            this.Dispatcher.Invoke(() => { new NotifyWindow(null, title, content).ShowDialog(); });
        }

        public bool? PopupConfirmDialog(string title, string content)
        {
            throw new NotImplementedException();
        }

        public bool? PopupSingleSelectDialog(string title, string fieldName, IEnumerable<string> options,
            FieldReference<string> selection)
        {
            bool? result = false;
            this.Dispatcher.Invoke(() =>
            {
                var selectWindow = new SingleSelectWindow(null, title, fieldName, options, selection);
                result = selectWindow.ShowDialog();
            });
            return result;
        }

        public bool? PopupSingleLineInputDialog(string title, string fieldName, FieldReference<string> content)
        {
            bool? result = null;
            this.Dispatcher.Invoke(() =>
            {
                var inputWindow = new SingleLineInputWindow(null, title, fieldName, content);
                result = inputWindow.ShowDialog();
            });
            return result;
        }

        public ProgressWindow BeginPopupProgressWindow(Progress progress)
        {
            ProgressWindow progressWindow = null;
            this.Dispatcher.InvokeAsync(() =>
            {
                progressWindow = new ProgressWindow(null, progress);
                progressWindow.Show();
            });
            return progressWindow;
        }
    }
}