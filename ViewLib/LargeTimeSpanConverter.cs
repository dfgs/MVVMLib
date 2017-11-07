using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ViewLib
{
	public class LargeTimeSpanConverter : IValueConverter
	{
		private static Regex regex = new Regex(@"(\d\d?):(\d\d)");


		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			TimeSpan time;

			if (!(value is TimeSpan)) return null;

			time = (TimeSpan)value;

			return $"{(int)time.TotalHours}:{time.Minutes.ToString("00")}";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string text;
			Match match;
			int h, m;

			if (!(value is string)) return TimeSpan.Zero;
			text = (string)value;

			match = regex.Match(text);
			if (!match.Success) return TimeSpan.Zero;

			h = Int32.Parse(match.Groups[1].Value);
			m = Int32.Parse(match.Groups[2].Value);

			return new TimeSpan(h, m, 0);
		}




	}
}
