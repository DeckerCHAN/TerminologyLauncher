using System;

namespace TerminologyLauncher.Utils.ProgressService
{
    public delegate void ProgressChangedEventHandler(object sender);

    public abstract class Progress
    {
        public Progress(string taskName)
        {
            this.TaskName = taskName;
        }

        public event ProgressChangedEventHandler ProgressChanged;
        public virtual double Percent { get; set; }

        public virtual string ReadablePercent => $"{this.Percent:0.00}%";

        public virtual string TaskName { get; set; }

        protected virtual double CheckPercentage(double percent)
        {
            if (percent > 100D)
            {
                percent = 100D;
            }
            if (percent < 0D)
            {
                percent = 0D;
            }
            return percent;
        }

        protected void OnProgressChanged()
        {
            this.ProgressChanged?.Invoke(this);
        }
    }
}