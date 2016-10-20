using System;

namespace TerminologyLauncher.Utils.ProgressService
{
    public class LeafNodeProgress : Progress
    {
        public LeafNodeProgress(string taskName)
            : base(taskName)
        {
            base.Percent = 0;
        }
        public override double Percent
        {
            get { return base.Percent; }
            set
            {
                value = this.CheckPercentage(value);
                base.Percent = value;
                this.OnProgressChanged();
            }
        }
    }
}
