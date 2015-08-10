using System;

namespace TerminologyLauncher.Utils.ProgressService
{
    public class LeafNodeProgress : Progress
    {
        public LeafNodeProgress(String taskName)
            : base(taskName)
        {
            base.Percent = 0;
        }
        public override double Percent
        {
            get { return base.Percent; }
            set
            {
                this.CheckPercentage(value);
                base.Percent = value;
                this.OnProgressChanged();
            }
        }
    }
}
