using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AttendO.Teachers
{
	/// <summary>
	/// Interaction logic for StudentInfoWindow.xaml
	/// </summary>
	public partial class StudentInfoWindow : Window
	{
		private List<StudentModel> _allStudents = new List<StudentModel>();
		private List<StudentModel> _filteredStudents = new List<StudentModel>();

		public StudentInfoWindow()
		{
			InitializeComponent();

			// Register converters
			Resources.Add("StatusToColorConverter", new StatusToColorConverter());
			Resources.Add("StatusToTextConverter", new StatusToTextConverter());
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				await LoadStudents();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading student data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async Task LoadStudents()
		{
			// Load all students
			_allStudents = await CommonData.LoadTableData<StudentModel>(TableNames.Student);

			// Apply any active filters
			ApplyFilters();
		}

		private void ApplyFilters()
		{
			// Start with all students
			_filteredStudents = new List<StudentModel>(_allStudents);

			// Apply name filter if provided
			if (!string.IsNullOrWhiteSpace(nameSearchBox.Text))
			{
				string nameFilter = nameSearchBox.Text.Trim().ToLower();
				_filteredStudents = _filteredStudents.Where(s =>
					s.Name.ToLower().Contains(nameFilter)).ToList();
			}

			// Apply roll number filter if provided
			if (!string.IsNullOrWhiteSpace(rollSearchBox.Text))
			{
				string rollFilter = rollSearchBox.Text.Trim();
				_filteredStudents = _filteredStudents.Where(s =>
					s.Roll.ToString().Contains(rollFilter)).ToList();
			}

			// Apply phone filter if provided
			if (!string.IsNullOrWhiteSpace(phoneSearchBox.Text))
			{
				string phoneFilter = phoneSearchBox.Text.Trim();
				_filteredStudents = _filteredStudents.Where(s =>
					s.Phone.Contains(phoneFilter)).ToList();
			}

			// Update DataGrid source
			studentsDataGrid.ItemsSource = _filteredStudents;
		}

		private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ApplyFilters();
		}

		private void refreshButton_Click(object sender, RoutedEventArgs e)
		{
			// Clear all search boxes
			nameSearchBox.Clear();
			rollSearchBox.Clear();
			phoneSearchBox.Clear();

			// Reset to all students
			_filteredStudents = [.. _allStudents];
			studentsDataGrid.ItemsSource = _filteredStudents;
		}
	}

	// Converter to map student status to color
	public class StatusToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is bool status)
			{
				return status ? new SolidColorBrush(Color.FromRgb(76, 175, 80)) : // Green for active
							   new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Red for inactive
			}
			return new SolidColorBrush(Colors.Gray);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	// Converter to map student status to text
	public class StatusToTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is bool status)
			{
				return status ? "Active" : "Inactive";
			}
			return "Unknown";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
