using System;

namespace TerminologyLauncher.Utils.ProgressService
{
    public delegate void ProgressChangedEventHandler(object sender);
    public abstract class Progress
    {
        public Progress(String taskName)
        {
            this.TaskName = taskName;
        }
        public event ProgressChangedEventHandler ProgressChanged;
        public virtual Double Percent { get; set; }
        public virtual String TaskName { get; set; }
        protected virtual Double CheckPercentage(Double percent)
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
            if (this.ProgressChanged != null)
            {
                this.ProgressChanged(this);
            }
        }
    }
}
