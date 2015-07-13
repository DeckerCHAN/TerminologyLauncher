using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.Utils.ProgressService
{
    public class LeafNodeProgress : Progress
    {
        public LeafNodeProgress()
        {
            this.Percent = 0;
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
