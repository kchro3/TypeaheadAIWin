using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeaheadAIWin.Source.Model.Accessibility
{
    public class AXUIElement
    {
        public int Id { get; set; }
        public string ControlType { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string ClassName { get; set; }
        public string AriaRole { get; set; }

        public List<AXUIElement> Children { get; set; } = new List<AXUIElement>();

        public string ShortId()
        {
            return $"AX{ToCamelCase(ControlType)}{Id}";
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(ShortId() + ": ");
            if (Name != "")
            {
                sb.Append($"{Name}");
            }

            if (Label != "")
            {
                sb.Append($" Label: {Label}");
            }

            // NOTE: When there is a WebView (i.e. AriaRole is not null), the ClassName is often obfuscated
            if (AriaRole == null && ClassName != "")
            {
                sb.Append($" ClassName: {ClassName}");
            }

            return sb.ToString();
        }

        private static string ToCamelCase(string input)
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            // Split the string into words
            string[] words = input.Split(' ');

            // Capitalize the first letter of each word and concatenate
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = textInfo.ToTitleCase(words[i]);
            }

            // Join the words without spaces
            return string.Concat(words);
        }
    }
}