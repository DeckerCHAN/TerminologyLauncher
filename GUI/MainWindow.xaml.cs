using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.GUI.Properties;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.GUI.ToolkitWindows.ConfirmWindow;
using TerminologyLauncher.GUI.ToolkitWindows.NotifyWindow;
using TerminologyLauncher.GUI.ToolkitWindows.ProgressWindow;
using TerminologyLauncher.GUI.ToolkitWindows.SingleLineInput;
using TerminologyLauncher.GUI.ToolkitWindows.SingleSelect;
using TerminologyLauncher.I18n.TranslationObjects.GUITranslations;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : INotifyPropertyChanged, IPopup
    {
        private ObservableCollection<InstanceEntity> InstanceListValue;
        private InstanceEntity SelectInstanceValue;
        private PlayerEntity PlayerValue;
        private string CoreVersionValue;

        public MainWindowTranslation Translation
        {
            get
            {
                return
                    I18n.TranslationProvider.TranslationProviderInstance.TranslationObject.GuiTranslation
                        .MainWindowTranslation;
            }
        }

        public PlayerEntity Player
        {
            get { return this.PlayerValue; }
            set
            {
                this.PlayerValue = value;
                this.OnPropertyChanged();
            }
        }

        public String CoreVersion
        {
            get { return this.CoreVersionValue; }
            set
            {
                this.CoreVersionValue = value;
                this.OnPropertyChanged();
            }
        }

        public InstanceEntity SelectInstance
        {
            get { return this.SelectInstanceValue; }
            set
            {
                this.SelectInstanceValue = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<InstanceEntity> InstanceList
        {
            get { return this.InstanceListValue; }
            set
            {
                this.InstanceListValue = value;
                this.OnPropertyChanged();
            }
        }

        public MainWindow(Config config)
        {
            this.InitializeComponent();
            this.OnPropertyChanged();
        }

        public void CrossThreadClose()
        {
            this.Dispatcher.Invoke(this.Close);
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) this.DragMove();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void InstanceAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void InstanceRemoveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public void PopupNotifyDialog(string title, string content)
        {
            this.Dispatcher.Invoke(() =>
            {
                var notifyWindow = new NotifyWindow(this, title, content);
                notifyWindow.ShowDialog();
            });
        }

        public Boolean? PopupConfirmDialog(string title, string content)
        {
            Boolean? result = false;
            this.Dispatcher.Invoke(() =>
            {
                var confirm = new ConfirmWindow(this, title, content);
                result = confirm.ShowDialog();
            });
            return result;
        }

        public Boolean? PopupSingleSelectDialog(string title, string fieldName, IEnumerable<string> options, FieldReference<String> selection)
        {
            Boolean? result = false;
            this.Dispatcher.Invoke(() =>
            {
                var selectWindow = new SingleSelectWindow(this, title, fieldName, options, selection);
                result = selectWindow.ShowDialog();
            });
            return result;
        }

        public Boolean? PopupSingleLineInputDialog(string title, string fieldName, FieldReference<String> content)
        {
            Boolean? result = null;
            this.Dispatcher.Invoke(() =>
            {
                var inputWindow = new SingleLineInputWindow(this, title, fieldName, content);
                result = inputWindow.ShowDialog();
            });
            return result;
        }


        public ProgressWindow BeginPopupProgressWindow(Progress progress)
        {
            ProgressWindow progressWindow = null;
            this.Dispatcher.Invoke(() =>
            {
                progressWindow = new ProgressWindow(this, progress);
            });
            this.Dispatcher.InvokeAsync(() => { progressWindow.Show(); });
            return progressWindow;
        }
    }


}
