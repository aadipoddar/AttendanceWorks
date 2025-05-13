using System.Windows;
using System.Windows.Controls;

namespace AttendO.Admin;

/// <summary>
/// Interaction logic for ScheduledClassPage.xaml
/// </summary>
public partial class ScheduledClassPage : Page
{
	private bool _originalStatus;

	public ScheduledClassPage() =>
		InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		var scheduledClass = await CommonData.LoadTableData<ScheduledClassModel>(TableNames.ScheduledClass);
		scheduledClassDataGrid.ItemsSource = scheduledClass;

		classDatePicker.SelectedDate = DateTime.Now;

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

	private void integerTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private void scheduledClassDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
	{
		if (scheduledClassDataGrid.SelectedItem is ScheduledClassModel scheduledClass)
		{
			courseComboBox.SelectedValue = scheduledClass.CourseId;
			sectionComboBox.SelectedValue = scheduledClass.SectionId;
			teacherComboBox.SelectedValue = scheduledClass.TeacherId;
			classRoomComboBox.SelectedValue = scheduledClass.ClassRoomId;
			classDatePicker.SelectedDate = scheduledClass.ClassDate.ToDateTime(new TimeOnly());
			startTimeTextBox.Text = scheduledClass.StartTime.ToString("HH");
			endTimeTextBox.Text = scheduledClass.EndTime.ToString("HH");
			statusCheckBox.IsChecked = scheduledClass.Status;

			_originalStatus = scheduledClass.Status;
		}
		else
			ClearForm();
	}

	private bool ValidateForm()
	{
		if (courseComboBox.SelectedValue is null)
		{
			MessageBox.Show("Please select a course.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		if (sectionComboBox.SelectedValue is null)
		{
			MessageBox.Show("Please select a section.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		if (teacherComboBox.SelectedValue is null)
		{
			MessageBox.Show("Please select a teacher.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		if (classRoomComboBox.SelectedValue is null)
		{
			MessageBox.Show("Please select a classroom.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
		if (classDatePicker.SelectedDate is null)
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

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm())
			return;

		saveButton.IsEnabled = false;

		bool isNewClass = scheduledClassDataGrid.SelectedItem == null;
		bool statusChanged = false;

		if (!isNewClass && scheduledClassDataGrid.SelectedItem is ScheduledClassModel)
			statusChanged = _originalStatus != (statusCheckBox.IsChecked ?? false);


		if (scheduledClassDataGrid.SelectedItem is ScheduledClassModel scheduledClass)
			await AttendanceData.DeleteAttendanceByScheduledClass(scheduledClass.Id);

		int scheduledClassId = await ScheduledClassData.InsertScheduledClass(new ScheduledClassModel
		{
			Id = (scheduledClassDataGrid.SelectedItem as ScheduledClassModel)?.Id ?? 0,
			CourseId = (int)courseComboBox.SelectedValue,
			SectionId = (int)sectionComboBox.SelectedValue,
			TeacherId = (int)teacherComboBox.SelectedValue,
			ClassRoomId = (int)classRoomComboBox.SelectedValue,
			ClassDate = DateOnly.FromDateTime(classDatePicker.SelectedDate.Value),
			StartTime = new TimeOnly(int.Parse(startTimeTextBox.Text), 0),
			EndTime = new TimeOnly(int.Parse(endTimeTextBox.Text), 0),
			Status = statusCheckBox.IsChecked ?? false
		});

		var students = await StudentData.LoadStudentBySection((int)sectionComboBox.SelectedValue);

		if (isNewClass || (statusChanged && (statusCheckBox.IsChecked ?? false)))
		{
			foreach (var student in students)
				await AttendanceData.InsertAttendance(new AttendanceModel
				{
					Id = 0,
					ScheduledClassId = (scheduledClassDataGrid.SelectedItem as ScheduledClassModel)?.Id ?? scheduledClassId,
					StudentId = student.Id,
					Present = false,
					EntryTime = DateTime.Now,
					MarkedBy = null
				});
		}

		if (isNewClass)
			await SendMails(students);

		else if (statusChanged)
			await SendStatusChangeMails(students, statusCheckBox.IsChecked ?? false);

		MessageBox.Show("All Notifications Sent to students and teacher",
			"Success", MessageBoxButton.OK, MessageBoxImage.Information);

		await LoadData();
		ClearForm();
	}

	private async Task SendMails(List<StudentModel> students)
	{
		var course = await CommonData.LoadTableDataById<CourseModel>(TableNames.Course, (int)courseComboBox.SelectedValue);
		var classroom = await CommonData.LoadTableDataById<ClassRoomModel>(TableNames.ClassRoom, (int)classRoomComboBox.SelectedValue);
		var section = await CommonData.LoadTableDataById<SectionModel>(TableNames.Section, (int)sectionComboBox.SelectedValue);
		var teacher = await CommonData.LoadTableDataById<TeacherModel>(TableNames.Teacher, (int)teacherComboBox.SelectedValue);

		if (teacher is not null)
			await Mailing.TeacherClassScheduleEmail(teacher, course, classroom, section,
				DateOnly.FromDateTime(classDatePicker.SelectedDate.Value),
				new TimeOnly(int.Parse(startTimeTextBox.Text), 0),
				new TimeOnly(int.Parse(endTimeTextBox.Text), 0));

		foreach (var student in students)
			await Mailing.StudentClassScheduleEmail(student, course, classroom,
				DateOnly.FromDateTime(classDatePicker.SelectedDate.Value),
				new TimeOnly(int.Parse(startTimeTextBox.Text), 0),
				new TimeOnly(int.Parse(endTimeTextBox.Text), 0));
	}

	private async Task SendStatusChangeMails(List<StudentModel> students, bool newStatus)
	{
		var course = await CommonData.LoadTableDataById<CourseModel>(TableNames.Course, (int)courseComboBox.SelectedValue);
		var classroom = await CommonData.LoadTableDataById<ClassRoomModel>(TableNames.ClassRoom, (int)classRoomComboBox.SelectedValue);
		var section = await CommonData.LoadTableDataById<SectionModel>(TableNames.Section, (int)sectionComboBox.SelectedValue);
		var teacher = await CommonData.LoadTableDataById<TeacherModel>(TableNames.Teacher, (int)teacherComboBox.SelectedValue);

		if (teacher is not null)
			await Mailing.TeacherClassStatusChangeEmail(teacher, course, classroom, section,
				DateOnly.FromDateTime(classDatePicker.SelectedDate.Value),
				new TimeOnly(int.Parse(startTimeTextBox.Text), 0),
				new TimeOnly(int.Parse(endTimeTextBox.Text), 0),
				newStatus);

		foreach (var student in students)
			await Mailing.ClassStatusChangeEmail(student, course, classroom,
				DateOnly.FromDateTime(classDatePicker.SelectedDate.Value),
				new TimeOnly(int.Parse(startTimeTextBox.Text), 0),
				new TimeOnly(int.Parse(endTimeTextBox.Text), 0),
				newStatus);
	}

	private void ClearForm()
	{
		classDatePicker.SelectedDate = DateTime.Now;
		startTimeTextBox.Clear();
		endTimeTextBox.Clear();
		statusCheckBox.IsChecked = true;
		saveButton.IsEnabled = true;
		_originalStatus = true;
	}
}
