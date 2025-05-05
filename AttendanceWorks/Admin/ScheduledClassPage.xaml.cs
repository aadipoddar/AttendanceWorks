using System.Windows;
using System.Windows.Controls;

namespace AttendanceWorks.Admin;

/// <summary>
/// Interaction logic for ScheduledClassePage.xaml
/// </summary>
public partial class ScheduledClassPage : Page
{
	public ScheduledClassPage() =>
		InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
			await LoadData();

	private async Task LoadData()
	{
		var scheduledClass = await CommonData.LoadTableData<ScheduledClassModel>(TableNames.ScheduledClass);
		scheduledClassDataGrid.ItemsSource = scheduledClass;

		var course = await CommonData.LoadTableData<CourseSectionModel>(TableNames.CourseSection);
		courseSectionComboBox.ItemsSource = course;
		courseSectionComboBox.DisplayMemberPath = nameof(CourseSectionModel.Id);
		courseSectionComboBox.SelectedValuePath = nameof(CourseSectionModel.Id);
		courseSectionComboBox.SelectedIndex = 0;

		classDatePicker.SelectedDate = DateTime.Now;
	}

	private void integerTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private void scheduledClassDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
	{
		if (scheduledClassDataGrid.SelectedItem is ScheduledClassModel scheduledClass)
		{
			courseSectionComboBox.SelectedValue = scheduledClass.CourseSectionId;
			classDatePicker.SelectedDate = scheduledClass.ClassDate.ToDateTime(new TimeOnly());
			startTimeTextBox.Text = scheduledClass.StartTime.ToString("hh");
			endTimeTextBox.Text = scheduledClass.EndTime.ToString("hh");
			statusCheckBox.IsChecked = scheduledClass.Status;
		}
		else
			ClearForm();
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm())
			return;

		await ScheduledClassData.InsertScheduledClass(new ScheduledClassModel
		{
			Id = (scheduledClassDataGrid.SelectedItem as ScheduledClassModel)?.Id ?? 0,
			CourseSectionId = (int)courseSectionComboBox.SelectedValue,
			ClassDate = DateOnly.FromDateTime(classDatePicker.SelectedDate.Value),
			StartTime = new TimeOnly(int.Parse(startTimeTextBox.Text), 0),
			EndTime = new TimeOnly(int.Parse(endTimeTextBox.Text), 0),
			Status = statusCheckBox.IsChecked ?? false
		});

		await LoadData();
		ClearForm();
	}

	private void ClearForm()
	{
		classDatePicker.SelectedDate = DateTime.Now;
		startTimeTextBox.Clear();
		endTimeTextBox.Clear();
		statusCheckBox.IsChecked = true;
	}

	private bool ValidateForm()
	{
		if (courseSectionComboBox.SelectedValue == null)
		{
			MessageBox.Show("Please select a course section.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		if (classDatePicker.SelectedDate == null)
		{
			MessageBox.Show("Please select a class date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		if (string.IsNullOrWhiteSpace(startTimeTextBox.Text))
		{
			MessageBox.Show("Please enter a start time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		if (string.IsNullOrWhiteSpace(endTimeTextBox.Text))
		{
			MessageBox.Show("Please enter an end time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		return true;
	}
}
