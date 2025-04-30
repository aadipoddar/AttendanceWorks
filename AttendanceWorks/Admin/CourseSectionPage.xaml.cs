using System.Windows;
using System.Windows.Controls;

namespace AttendanceWorks.Admin;

/// <summary>
/// Interaction logic for CourseSectionPage.xaml
/// </summary>
public partial class CourseSectionPage : Page
{
	public CourseSectionPage() =>
		InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		var courseSection = await CommonData.LoadTableData<CourseSectionModel>(TableNames.CourseSection);
		courseSectionDataGrid.ItemsSource = courseSection;

		var course = await CommonData.LoadTableData<CourseModel>(TableNames.Course);
		courseComboBox.ItemsSource = course;
		courseComboBox.DisplayMemberPath = nameof(CourseModel.Name);
		courseComboBox.SelectedValuePath = nameof(CourseModel.Id);
		courseComboBox.SelectedIndex = 0;

		var section = await CommonData.LoadTableData<SectionModel>(TableNames.Section);
		sectionComboBox.ItemsSource = section;
		sectionComboBox.DisplayMemberPath = nameof(SectionModel.Name);
		sectionComboBox.SelectedValuePath = nameof(SectionModel.Id);
		sectionComboBox.SelectedIndex = 0;

		var teacher = await CommonData.LoadTableData<TeacherModel>(TableNames.Teacher);
		teacherComboBox.ItemsSource = teacher;
		teacherComboBox.DisplayMemberPath = nameof(TeacherModel.Name);
		teacherComboBox.SelectedValuePath = nameof(TeacherModel.Id);
		teacherComboBox.SelectedIndex = 0;

		var classRoom = await CommonData.LoadTableData<ClassRoomModel>(TableNames.ClassRoom);
		classRoomComboBox.ItemsSource = classRoom;
		classRoomComboBox.DisplayMemberPath = nameof(ClassRoomModel.Name);
		classRoomComboBox.SelectedValuePath = nameof(ClassRoomModel.Id);
		classRoomComboBox.SelectedIndex = 0;
	}

	private void courseSectionDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
	{
		if (courseSectionDataGrid.SelectedItem is CourseSectionModel courseSection)
		{
			courseComboBox.SelectedValue = courseSection.CourseId;
			sectionComboBox.SelectedValue = courseSection.SectionId;
			teacherComboBox.SelectedValue = courseSection.TeacherId;
			classRoomComboBox.SelectedValue = courseSection.ClassRoomId;
			statusCheckBox.IsChecked = courseSection.Status;
		}

		else
			ClearForm();
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		await CourseData.InsertCourseSection(new CourseSectionModel
		{
			Id = (courseSectionDataGrid.SelectedItem as CourseSectionModel)?.Id ?? 0,
			CourseId = (int)courseComboBox.SelectedValue,
			SectionId = (int)sectionComboBox.SelectedValue,
			TeacherId = (int)teacherComboBox.SelectedValue,
			ClassRoomId = (int)classRoomComboBox.SelectedValue,
			Status = statusCheckBox.IsChecked ?? false
		});

		await LoadData();
		ClearForm();
	}

	private void ClearForm()
	{
		courseComboBox.SelectedIndex = 0;
		sectionComboBox.SelectedIndex = 0;
		teacherComboBox.SelectedIndex = 0;
		classRoomComboBox.SelectedIndex = 0;
		statusCheckBox.IsChecked = true;
	}
}
