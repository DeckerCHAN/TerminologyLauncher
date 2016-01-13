using System;

namespace TerminologyLauncher.GUI.ToolkitWindows.ConfigWindow.ConfigObjects
{
    public class TextInputConfigObject : ConfigObject
    {
        public TextInputConfigObject(String name, String key, String value)
            : base(name, key)
        {
            this.Value = value;
        }

        private string RealValue;


        public String Value
        {
            get { return this.RealValue; }
            set
            {
                this.RealValue = value;
                this.OnPropertyChanged();
            }
        }
    }
}
