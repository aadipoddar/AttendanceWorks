using System.Windows;
using System.Windows.Controls;

namespace AttendanceWorks.Admin;

/// <summary>
/// Interaction logic for CoursePage.xaml
/// </summary>
public partial class CoursePage : Page
{
	public CoursePage() =>
		InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		var course = await CommonData.LoadTableData<CourseModel>(TableNames.Course);
		courseDataGrid.ItemsSource = course;
	}

	private void integerTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private void courseDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
	{
		if (courseDataGrid.SelectedItem is CourseModel course)
		{
			nameTextBox.Text = course.Name;
			codeTextBox.Text = course.Code;
			creditsTextBox.Text = course.Credits.ToString();
			statusCheckBox.IsChecked = course.Status;
		}

		else
			ClearForm();
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm())
			return;

		await CourseData.InsertCourse(new CourseModel
		{
			Id = (courseDataGrid.SelectedItem as CourseModel)?.Id ?? 0,
			Code = codeTextBox.Text,
			Name = nameTextBox.Text,
			Credits = int.Parse(creditsTextBox.Text),
			Status = statusCheckBox.IsChecked ?? false
		});

		await LoadData();
		ClearForm();
	}

	private void ClearForm()
	{
		nameTextBox.Text = string.Empty;
		codeTextBox.Text = string.Empty;
		creditsTextBox.Text = string.Empty;
		statusCheckBox.IsChecked = true;
	}

	private bool ValidateForm()
	{
		if (string.IsNullOrWhiteSpace(nameTextBox.Text))
		{
			MessageBox.Show("Please enter a name.");
			nameTextBox.Focus();
			return false;
		}

		if (string.IsNullOrWhiteSpace(codeTextBox.Text))
		{
			MessageBox.Show("Please enter a Code.");
			codeTextBox.Focus();
			return false;
		}

		if (string.IsNullOrWhiteSpace(creditsTextBox.Text))
		{
			MessageBox.Show("Please enter a Credit.");
			creditsTextBox.Focus();
			return false;
		}

		return true;
	}
}
