using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.GUI.ToolkitWindows.ConfigWindow.ConfigObjects
{
    public class RangeRestrictedSelectConfigObject : ConfigObject
    {
        private double MaxiumValue;
        private double MiniumValue;
        private double RealValue;

        private Double Maxium
        {
            get { return this.MaxiumValue; }
            set
            {
                this.MaxiumValue = value;
                this.OnPropertyChanged();
            }
        }

        private Double Minium
        {
            get { return this.MiniumValue; }
            set
            {
                this.MiniumValue = value;
                this.OnPropertyChanged();
            }
        }

        private Double Value
        {
            get { return this.RealValue; }
            set
            {
                if (value > this.Maxium || value < this.Minium) throw new InvalidOperationException("Could not set value higher than maxium or less than minium");
                this.RealValue = value;
                this.OnPropertyChanged();
            }
        }

        public RangeRestrictedSelectConfigObject(string name, string key, Double maxium, Double minium, Double defaultValue)
            : base(name, key)
        {
            this.Maxium = maxium;
            this.Minium = minium;
            this.Value = defaultValue;
        }
    }
}
