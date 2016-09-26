using System;
using System.Collections.Generic;
using System.Linq;

namespace TerminologyLauncher.Utils.ProgressService
{
    public class InternalNodeProgress : Progress
    {
        public Dictionary<Progress, double> SubProgressesAndPercentage { get; set; }

        public override string TaskName
        {
            get
            {
                var currentTasks =
                    this.SubProgressesAndPercentage.Where(x => ((x.Key.Percent > 0) && (x.Key.Percent < 100)));
                var keyValuePairs = currentTasks as KeyValuePair<Progress, double>[] ?? currentTasks.ToArray();
                if (keyValuePairs.Count() == 0)
                {
                    return base.TaskName;
                }
                var currentTask = keyValuePairs.First();
                return currentTask.Key.TaskName;
            }

        }

        public override double Percent
        {
            get
            {
                if (this.SubProgressesAndPercentage.Count == 0)
                {
                    return base.Percent;
                }
                var sum = this.SubProgressesAndPercentage.Sum(subProgress => (subProgress.Value / 100) * subProgress.Key.Percent);
                sum = this.CheckPercentage(sum);
                return base.Percent + sum;
            }
            set
            {
                base.Percent = value;
                this.SubProgressesAndPercentage.Clear();
                this.OnProgressChanged();
            }
        }

        public InternalNodeProgress(string taskName)
            : base(taskName)
        {
            this.SubProgressesAndPercentage = new Dictionary<Progress, double>();
            base.Percent = 0D;
        }
        public InternalNodeProgress CreateNewInternalSubProgress(string taskName, double taskPercentage)
        {
            taskPercentage = this.CheckPercentage(taskPercentage);
            var progress = new InternalNodeProgress(taskName);
            this.SubProgressesAndPercentage.Add(progress, taskPercentage);
            progress.ProgressChanged += sender => { this.OnProgressChanged(); };
            return progress;
        }

        public LeafNodeProgress CreateNewLeafSubProgress(string taskName, double taskPercentage)
        {
            taskPercentage = this.CheckPercentage(taskPercentage);
            var progress = new LeafNodeProgress(taskName);
            this.SubProgressesAndPercentage.Add(progress, taskPercentage);
            progress.ProgressChanged += sender => { this.OnProgressChanged(); };
            return progress;
        }

        protected override double CheckPercentage(double percent)
        {
            percent = base.CheckPercentage(percent);
            if (this.SubProgressesAndPercentage.Sum(x => (x.Value)) > 100)
            {
                throw new InvalidOperationException($"Add {percent}% will over 100%.");
            }
            return percent;
        }
    }
}
