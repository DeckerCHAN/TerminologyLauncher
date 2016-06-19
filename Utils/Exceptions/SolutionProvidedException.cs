using System;

namespace TerminologyLauncher.Utils.Exceptions
{
    public class SolutionProvidedException : System.Exception
    {
        public string SlutionTip { get; protected set; }

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
            return $"{base.ToString()}{Environment.NewLine} Solution Tips: {this.SlutionTip}";
        }

        public override string Message
        {
            get { return $"{base.Message} Solution tip: {this.SlutionTip}"; }
        }
    }
}
