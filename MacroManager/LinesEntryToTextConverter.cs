using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace MacroManager
{
    public class LinesEntryToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MacroEntry[] m = (MacroEntry[]) value;

            StringBuilder sb = new StringBuilder();
            foreach (MacroEntry e in m)
                sb.Append(e.Data + "\n");
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}