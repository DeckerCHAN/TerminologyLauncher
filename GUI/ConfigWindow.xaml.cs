using System.Windows;

namespace TerminologyLauncher.GUI
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
        }

        public void CrossThreadClose()
        {
            this.Dispatcher.Invoke(this.Close);
        }
    }
}
