using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Entities.InstanceManagement.FileSystem;
using TerminologyLauncher.GUI.Annotations;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.GUI.ToolkitWindows.SingleLineInput;
using TerminologyLauncher.I18n;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.GUI.Windows.InstanceCreator
{
    /// <summary>
    /// Interaction logic for InstanceCreateWindow.xaml
    /// </summary>
    public partial class InstanceCreateWindow : Window, INotifyPropertyChanged
    {
        public event EventHandler PackageFileRemoving;


        private InstanceEntity InstanceValue;

        public InstanceCreateWindow()
        {
            this.InitializeComponent();
        }

        public void LoadInstnace(InstanceEntity instance)
        {
            this.Instance = instance;


            this.Instance.FileSystem.EntirePackageFiles = this.Instance.FileSystem.EntirePackageFiles == null
                ? new ObservableCollection<EntirePackageFileEntity>()
                : new ObservableCollection<EntirePackageFileEntity>(this.Instance.FileSystem.EntirePackageFiles);


            this.Instance.FileSystem.OfficialFiles = this.Instance.FileSystem.OfficialFiles == null
                ? new ObservableCollection<OfficialFileEntity>()
                : new ObservableCollection<OfficialFileEntity>(this.Instance.FileSystem.OfficialFiles);

            this.Instance.FileSystem.CustomFiles = this.Instance.FileSystem.CustomFiles == null
                ? new ObservableCollection<CustomFileEntity>()
                : new ObservableCollection<CustomFileEntity>(this.Instance.FileSystem.CustomFiles);

            this.Instance.StartupArguments.JvmArguments = this.Instance.StartupArguments.JvmArguments == null
                ? new ObservableCollection<string>()
                : new ObservableCollection<string>(this.Instance.StartupArguments.JvmArguments);

            this.Instance.StartupArguments.LibraryPaths = this.Instance.StartupArguments.LibraryPaths == null
                ? new ObservableCollection<string>()
                : new ObservableCollection<string>(this.Instance.StartupArguments.LibraryPaths);

            this.Instance.StartupArguments.TweakClasses = this.Instance.StartupArguments.TweakClasses == null
                ? new ObservableCollection<string>()
                : new ObservableCollection<string>(this.Instance.StartupArguments.TweakClasses);

            this.OnPropertyChanged(nameof(this.Instance));
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
            var content = new FieldReference<string>("");
            var inputWindow = new SingleLineInputWindow(null, "Imput file.", "Pack file path", content);

            var showDialog = inputWindow.ShowDialog();
            if (showDialog != null && showDialog.Value && !string.IsNullOrEmpty(content.Value))
            {
                var packFileInfo = new FileInfo(content.Value);
                this.Instance.FileSystem.EntirePackageFiles.Add(new EntirePackageFileEntity
                {
                    Name = Path.GetFileNameWithoutExtension(packFileInfo.Name),
                    DownloadLink = "http://example.com/" + packFileInfo.Name,
                    LocalPath = Path.GetFileNameWithoutExtension(packFileInfo.Name) + "/",
                    Md5 = Utils.EncodeUtils.CalculateFileMd5(packFileInfo.FullName)
                });

                this.OnPropertyChanged(nameof(this.Instance));
            }
        }

        private void OnPackageFileRemoving(object sender, RoutedEventArgs routedEventArgs)
        {
            //this.PackageFileRemoving?.Invoke((Button)sender, routedEventArgs);
            this.Instance.FileSystem.EntirePackageFiles.Remove((EntirePackageFileEntity) ((Button) sender).DataContext);
            this.OnPropertyChanged(nameof(this.Instance));
        }


        private void OnInstanceUpdated(object sender, DataTransferEventArgs e)
        {
            this.OnPropertyChanged(nameof(this.Instance));
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