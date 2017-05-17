using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TerminologyLauncher.Utils
{
    public static class StringUtils
    {
        public static string ToLiteral(string input)
        {
            var result = input;

            var literList = @"\^$.|?*+[](){}".ToCharArray();

            result = literList.Aggregate(result, (current, character) => current.Replace(character.ToString(),new string(new[] { '\\', character })));


            return result;

        }

        public static List<string> ReverseStringFormat(string template, string str)
        {
            template = ToLiteral(template);
            string pattern = "^" + Regex.Replace(template, @"\\{[0-9]+\\}", @"(.*?)") + "$";

            var r = new Regex(pattern);
            var m = r.Match(str);

            var ret = new List<string>();

            for (var i = 1; i < m.Groups.Count; i++)
            {
                ret.Add(m.Groups[i].Value);
            }

            return ret;
        }
    }
}