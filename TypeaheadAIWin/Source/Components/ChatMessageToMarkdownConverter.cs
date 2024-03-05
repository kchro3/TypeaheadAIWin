using MdXaml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TypeaheadAIWin.Source.Components
{
    class ChatMessageToMarkdownConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = (string)value;
            var md = new Markdown();
            // Replace all "\n" with "\n\n" in _text
            string updatedText = text.Replace("\n", "\n\n");
            return md.Transform(updatedText);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
