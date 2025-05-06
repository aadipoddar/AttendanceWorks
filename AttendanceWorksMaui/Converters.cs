// Add these converters to a separate file named Converters.cs
using System.Globalization;

namespace AttendanceWorksMaui
{
	public class BoolToStatusConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool present)
			{
				return present ? "Present" : "Absent";
			}
			return "Unknown";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class BoolToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool present)
			{
				return present ? Color.FromArgb("#4CAF50") : Color.FromArgb("#F44336");
			}
			return Colors.Gray;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class BoolToStyleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool present)
			{
				if (present)
					return Application.Current.Resources["PresentBadgeStyle"];
				else
					return Application.Current.Resources["AbsentBadgeStyle"];
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
