using System;
using System.Collections.Generic;
using System.Linq;

namespace TerminologyLauncher.Utils.ProgressService
{
    public class InternalNodeProgress : Progress
    {
        public Dictionary<Progress, Double> SubProgresses { get; set; }

        public override double Percent
        {
            get
            {
                if (this.SubProgresses.Count == 0)
                {
                    return 0D;
                }
                var sum = this.SubProgresses.Sum(subProgress => (subProgress.Value / 100) * subProgress.Key.Percent);
                this.CheckPercentage(sum);
                return Math.Max(sum, base.Percent);
            }
            set { base.Percent = value; }
        }

        public InternalNodeProgress()
        {
            this.SubProgresses = new Dictionary<Progress, Double>();
        }
        public InternalNodeProgress CreateNewInternalSubProgress(Double taskPercentage)
        {
            this.CheckPercentage(taskPercentage);
            var progress = new InternalNodeProgress();
            this.SubProgresses.Add(progress, taskPercentage);
            progress.ProgressChanged += sender => { this.OnProgressChanged(); };
            return progress;
        }

        public LeafNodeProgress CreateNewLeafSubProgress(Double taskPercentage)
        {
            this.CheckPercentage(taskPercentage);
            var progress = new LeafNodeProgress();
            this.SubProgresses.Add(progress, taskPercentage);
            progress.ProgressChanged += sender => { this.OnProgressChanged(); };
            return progress;
        }

    }
}
