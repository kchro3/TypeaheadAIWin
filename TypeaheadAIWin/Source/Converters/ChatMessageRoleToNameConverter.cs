using System.Globalization;
using System.Windows.Data;
using TypeaheadAIWin.Source.Model;

namespace TypeaheadAIWin.Source.Converters
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
                        return "User";
                    case ChatMessageRole.Tool:
                        return "Tool";
                    case ChatMessageRole.Assistant:
                        return "Assistant";
                    default:
                        return "Unknown Role";
                }
            }
            return "Unknown Role";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
