using System;

namespace TerminologyLauncher.GUI.Windows.ConfigWindow.ConfigObjects
{
    public class RangeRestrictedSelectConfigObject : ConfigObject
    {
        private long MaxiumValue;
        private long MiniumValue;
        private long RealValue;

        public long Maxium
        {
            get { return this.MaxiumValue; }
            set
            {
                this.MaxiumValue = value;
                this.OnPropertyChanged();
            }
        }

        public long Minium
        {
            get { return this.MiniumValue; }
            set
            {
                this.MiniumValue = value;
                this.OnPropertyChanged();
            }
        }

        public long Value
        {
            get { return this.RealValue; }
            set
            {
                if (value > this.Maxium || value < this.Minium)
                    throw new InvalidOperationException("Could not set value higher than maxium or less than minium");
                this.RealValue = value;
                this.OnPropertyChanged();
            }
        }

        public RangeRestrictedSelectConfigObject(string name, string key, long maxium, long minium, long defaultValue)
            : base(name, key)
        {
            this.Maxium = maxium;
            this.Minium = minium;
            this.Value = defaultValue;
        }
    }
}