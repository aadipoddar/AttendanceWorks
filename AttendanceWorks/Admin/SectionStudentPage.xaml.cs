using System.Windows;
using System.Windows.Controls;

namespace AttendanceWorks.Admin;

/// <summary>
/// Interaction logic for SectionStudentPage.xaml
/// </summary>
public partial class SectionStudentPage : Page
{
	public SectionStudentPage() =>
	InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		var sectionStudent = await CommonData.LoadTableData<SectionStudentModel>(TableNames.SectionStudent);
		sectionStudentDataGrid.ItemsSource = sectionStudent;

		var section = await CommonData.LoadTableData<SectionModel>(TableNames.Section);
		sectionComboBox.ItemsSource = section;
		sectionComboBox.DisplayMemberPath = nameof(SectionModel.Name);
		sectionComboBox.SelectedValuePath = nameof(SectionModel.Id);
		sectionComboBox.SelectedIndex = 0;

		var student = await CommonData.LoadTableData<StudentModel>(TableNames.Student);
		studentComboBox.ItemsSource = student;
		studentComboBox.DisplayMemberPath = nameof(StudentModel.Name);
		studentComboBox.SelectedValuePath = nameof(StudentModel.Id);
		studentComboBox.SelectedIndex = 0;
	}

	private void sectionStudentDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
	{
		if (sectionStudentDataGrid.SelectedItem is SectionStudentModel sectionStudent)
		{
			sectionComboBox.SelectedValue = sectionStudent.SectionId;
			studentComboBox.SelectedValue = sectionStudent.StudentId;
			statusCheckBox.IsChecked = sectionStudent.Status;
		}

		else
			ClearForm();
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		await SectionData.InsertSectionStudent(new SectionStudentModel
		{
			Id = (sectionStudentDataGrid.SelectedItem as SectionStudentModel)?.Id ?? 0,
			SectionId = (int)sectionComboBox.SelectedValue,
			StudentId = (int)studentComboBox.SelectedValue,
			Status = statusCheckBox.IsChecked ?? false
		});

		await LoadData();
		ClearForm();
	}

	private void ClearForm()
	{
		sectionComboBox.SelectedIndex = 0;
		studentComboBox.SelectedIndex = 0;
		statusCheckBox.IsChecked = true;
	}
}
