using System;

namespace TerminologyLauncher.Utils.Exceptions
{
    public class SolutionProvidedException : System.Exception
    {
        public String SlutionTip { get; protected set; }

        public SolutionProvidedException()
        {
            this.SlutionTip = "No tips available";
        }

        public SolutionProvidedException(string message)
            : base(message)
        {
            this.SlutionTip = "No tips available";
        }

        public SolutionProvidedException(string message, string slutionTip)
            : base(message)
        {
            this.SlutionTip = slutionTip;
        }

        public SolutionProvidedException(string message, System.Exception innerException, string slutionTip)
            : base(message, innerException)
        {
            this.SlutionTip = slutionTip;
        }

        public override string ToString()
        {
            return String.Format("{0}{1} Solution Tips: {2}", base.ToString(), Environment.NewLine, this.SlutionTip);
        }

        public override string Message
        {
            get { return String.Format("{0} Solution tip: {1}", base.Message, this.SlutionTip); }
        }
    }
}
