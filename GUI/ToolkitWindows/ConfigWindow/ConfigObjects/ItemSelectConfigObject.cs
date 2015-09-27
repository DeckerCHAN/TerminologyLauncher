using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.GUI.ToolkitWindows.ConfigWindow.ConfigObjects
{
    public class ItemSelectConfigObject : ConfigObject
    {
        private int RealSelectIndex;
        private LinkedList<string> AvailableAvailableSelectionsValue;

        public LinkedList<String> AvailableSelections
        {
            get { return this.AvailableAvailableSelectionsValue; }
            set
            {
                this.AvailableAvailableSelectionsValue = value; 
                this.OnPropertyChanged();
            }
        }

        public ItemSelectConfigObject(String name, String key, LinkedList<String> availableSelections, Int32 defaultSelection)
            : base(name, key)
        {
            this.AvailableSelections = availableSelections;
            this.SelectIndex = defaultSelection;
        }

        public Int32 SelectIndex
        {
            get { return this.RealSelectIndex; }
            set
            {
                this.RealSelectIndex = value;
                this.OnPropertyChanged();
            }
        }
    }
}
