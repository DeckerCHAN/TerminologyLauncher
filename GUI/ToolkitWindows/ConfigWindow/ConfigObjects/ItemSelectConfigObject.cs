using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.GUI.ToolkitWindows.ConfigWindow.ConfigObjects
{
    public class ItemSelectConfigObject : ConfigObject
    {
        private KeyValuePair<String, Object> RealSelection;
        private Dictionary<String, Object> AvailableAvailableSelectionsValue;

        public Dictionary<String, Object> AvailableSelections
        {
            get { return this.AvailableAvailableSelectionsValue; }
            set
            {
                this.AvailableAvailableSelectionsValue = value; 
                this.OnPropertyChanged();
            }
        }

        public ItemSelectConfigObject(String name, String key, Dictionary<String, Object> availableSelections, String defaultSelectKey)
            : base(name, key)
        {
            this.AvailableSelections = availableSelections;
            this.Selection = this.AvailableSelections.First(x => x.Key.Equals(defaultSelectKey));
        }

        public KeyValuePair<String,Object> Selection
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
