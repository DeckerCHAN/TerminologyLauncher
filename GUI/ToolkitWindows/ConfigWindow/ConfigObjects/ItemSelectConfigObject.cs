using System;
using System.Collections.Generic;
using System.Linq;

namespace TerminologyLauncher.GUI.ToolkitWindows.ConfigWindow.ConfigObjects
{
    public class ItemSelectConfigObject : ConfigObject
    {
        private KeyValuePair<string, object> RealSelection;
        private Dictionary<string, object> AvailableAvailableSelectionsValue;

        public Dictionary<string, object> AvailableSelections
        {
            get { return this.AvailableAvailableSelectionsValue; }
            set
            {
                this.AvailableAvailableSelectionsValue = value; 
                this.OnPropertyChanged();
            }
        }

        public ItemSelectConfigObject(string name, string key, Dictionary<string, object> availableSelections, string defaultSelectKey)
            : base(name, key)
        {
            this.AvailableSelections = availableSelections;
            this.Selection = this.AvailableSelections.First(x => x.Key.Equals(defaultSelectKey));
        }

        public KeyValuePair<string,object> Selection
        {
            get { return this.RealSelection; }
            set
            {
                this.RealSelection = value;
                this.OnPropertyChanged();
            }
        }
    }
}
