using System;

namespace TerminologyLauncher.GUI.ToolkitWindows.ConfigWindow.ConfigObjects
{
    public class TextInputConfigObject : ConfigObject
    {
        public TextInputConfigObject(string name, string key, string value)
            : base(name, key)
        {
            this.Value = value;
        }

        private string RealValue;


        public string Value
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
