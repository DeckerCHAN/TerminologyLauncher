using System;
using System.Reflection;
using System.Threading;

namespace TerminologyLauncher.Utils
{
    public static class CopyUtils
    {
        public static object MakeCopy(object original)
        {
            return MakeCopy(original, original.GetType());
        }

        public static object MakeCopy(object original, Type targetType)
        {
            var copy = Activator.CreateInstance(targetType);
            var piList =
                original.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var pi in piList)
            {
                var value = pi.GetValue(original);
                if (value == null)
                {
                    continue;
                }
                var type = value.GetType();
                if (type.IsPrimitive || type == typeof(string))
                {
                    pi.SetValue(copy, value);
                }
                else if ((type.GetInterface("IEnumerable") != null))
                {
                    pi.SetValue(copy, Activator.CreateInstance(type, value));
                }
                else
                {
                    var newValue = MakeCopy(value);
                    pi.SetValue(copy, newValue);
                }
            }
            return copy;
        }
    }
}