using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.GUI.Toolkits
{
    public class FieldReference<T>
    {
        public T Value { get; set; }

        public FieldReference(T value)
        {
            this.Value = value;
        }


    }
}
