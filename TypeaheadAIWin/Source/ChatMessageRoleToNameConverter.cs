using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TypeaheadAIWin.Source
{
    public class ChatMessageRoleToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ChatMessageRole role)
            {
                switch (role)
                {
                    case ChatMessageRole.User:
                        return "User Message";
                    case ChatMessageRole.Tool:
                        return "Tool Message";
                    case ChatMessageRole.Assistant:
                        return "Assistant Message";
                    default:
                        return "Unknown Message";
                }
            }
            return "Unknown Message";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
