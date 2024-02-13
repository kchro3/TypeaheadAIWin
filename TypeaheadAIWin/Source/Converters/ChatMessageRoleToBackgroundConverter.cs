using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using TypeaheadAIWin.Source.Model;

namespace TypeaheadAIWin.Source.Converters
{
    class ChatMessageRoleToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ChatMessageRole role)
            {
                switch (role)
                {
                    case ChatMessageRole.User:
                        return Brushes.LightBlue; // Or new SolidColorBrush(Color.FromRgb(r, g, b))
                    default:
                        return Brushes.Transparent; // Default background
                }
            }
            return Brushes.Transparent; // Default background for unexpected values
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported.");
        }
    }
}
