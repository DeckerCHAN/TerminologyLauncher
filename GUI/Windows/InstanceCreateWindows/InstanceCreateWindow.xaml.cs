using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Entities.InstanceManagement.FileSystem;
using TerminologyLauncher.GUI.Annotations;
using TerminologyLauncher.I18n;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.GUI.Windows.InstanceCreateWindows
{
    /// <summary>
    /// Interaction logic for InstanceCreateWindow.xaml
    /// </summary>
    public partial class InstanceCreateWindow : Window, INotifyPropertyChanged
    {
        private InstanceEntity InstanceValue;

        public InstanceCreateWindow(InstanceEntity instance)
        {
            this.Instance = instance;
            this.InitializeComponent();
        }

        public InstanceEntity Instance
        {
            get { return this.InstanceValue; }
            set
            {
                this.InstanceValue = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public object InstanceCreateWindowTitleTranslation
            => TranslationManager.GetManager.Localize("InstanceCreateWindowTitle", "Create New Instance");

        public object InstanceVerificationButtonTranslation
            => TranslationManager.GetManager.Localize("InstanceVerificationButton", "Create My Instance");

        public object GenerateInstanceFileButtonTranslation
            => TranslationManager.GetManager.Localize("GenerateInstanceFileButton", "Get Instance File");


        private void HeadBarPanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) this.DragMove();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.CrossThreadClose();
        }

        public void CrossThreadClose()
        {
            this.Dispatcher.Invoke(this.Close);
        }

        private void AddNewPackageFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Instance.FileSystem.EntirePackageFiles.Add(new EntirePackageFileEntity()
            {
                Name = "Your package name" + Guid.NewGuid().ToString(),
                DownloadLink = "http://example.com/package",
                LocalPath = "/your_path",
                Md5 = "abcdef123456"
            });

            this.OnPropertyChanged(nameof(this.Instance));

            TerminologyLogger.GetLogger().Info("Add one!");
        }

        private void OnInstanceUpdated(object sender, DataTransferEventArgs e)
        {
            //            var bindingExpression = this.JsonViewTextBox.GetBindingExpression(TextBox.TextProperty);
            //            bindingExpression?.UpdateTarget();
            this.OnPropertyChanged(nameof(this.Instance));
            TerminologyLogger.GetLogger().Info("Updated!");
        }

        private void GenerateInstanceFileButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
//            this.OnPropertyChanged();
//            TerminologyLogger.GetLogger().Info("Changed!");
        }
    }
}