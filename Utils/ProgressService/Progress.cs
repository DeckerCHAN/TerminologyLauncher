using System;

namespace TerminologyLauncher.Utils.ProgressService
{
    public delegate void ProgressChangedEventHandler(object sender);
    public abstract class Progress
    {
        public event ProgressChangedEventHandler ProgressChanged;
        public virtual Double Percent { get; set; }

        protected virtual void CheckPercentage(Double percent)
        {
            if (percent > 100D || percent < 0D)
            {
                throw new InvalidOperationException("Can not set percent value over 100.");
            }
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
