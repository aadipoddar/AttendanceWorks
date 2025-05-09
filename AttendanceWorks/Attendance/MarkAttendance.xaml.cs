using System.Windows;
using System.Windows.Controls;

namespace AttendanceWorks.Attendance;

/// <summary>
/// Interaction logic for MarkAttendance.xaml
/// </summary>
public partial class MarkAttendance : Window
{
	private TeacherModel _teacher;
	private readonly ActiveClassModel _activeClass;
	private readonly List<StudentAttendanceViewModel> _students = [];
	private List<StudentAttendanceViewModel> _filteredStudents = [];

	public MarkAttendance(ActiveClassModel activeClass)
	{
		InitializeComponent();
		_activeClass = activeClass;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async void refreshButton_Click(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		_teacher = await CommonData.LoadTableDataById<TeacherModel>(TableNames.Teacher, _activeClass.TeacherId);

		classInfoTextBlock.Text = $"Current Class: {_activeClass.CourseCode} - {_activeClass.CourseName} ({_activeClass.SectionName})";
		dateTimeTextBlock.Text = $"Date: {DateTime.Now:d} | Time: {_activeClass.StartTime:hh:mm tt} - {_activeClass.EndTime:hh:mm tt}";
		teacherNameTextBlock.Text = $"Teacher: {_teacher.Name}";
		classroomTextBlock.Text = $"Classroom: {_activeClass.ClassroomName}";

		var enrolledStudents = await StudentData.LoadStudentBySection(_activeClass.SectionId);
		var existingAttendance = await AttendanceData.LoadAttendanceByScheduledClass(_activeClass.ScheduledClassId);

		foreach (var student in enrolledStudents)
		{
			var attendanceRecord = existingAttendance.FirstOrDefault(a => a.StudentId == student.Id);

			_students.Add(new StudentAttendanceViewModel
			{
				Id = student.Id,
				Name = student.Name,
				Roll = student.Roll,
				Present = attendanceRecord?.Present ?? false,
				EntryTime = attendanceRecord?.EntryTime ?? DateTime.Now,
				AttendanceId = attendanceRecord?.Id ?? 0
			});
		}

		_filteredStudents = [.. _students];
		studentsDataGrid.ItemsSource = _filteredStudents;
	}

	private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		string searchText = searchTextBox.Text.ToLower().Trim();

		if (string.IsNullOrEmpty(searchText))
			_filteredStudents = [.. _students];
		else
			_filteredStudents = [.. _students.Where(
				s => s.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) ||
				s.Roll.ToString().Contains(searchText))];

		studentsDataGrid.ItemsSource = null;
		studentsDataGrid.ItemsSource = _filteredStudents;
	}

	private void markAllPresentButton_Click(object sender, RoutedEventArgs e)
	{
		foreach (var student in _filteredStudents)
		{
			student.Present = true;
			student.EntryTime = DateTime.Now;
		}

		studentsDataGrid.Items.Refresh();
	}

	private async void saveAttendanceButton_Click(object sender, RoutedEventArgs e)
	{
		foreach (var student in _students)
		{
			var attendanceModel = new AttendanceModel
			{
				Id = student.AttendanceId,
				ScheduledClassId = _activeClass.ScheduledClassId,
				StudentId = student.Id,
				Present = student.Present,
				EntryTime = student.Present ? (student.EntryTime == DateTime.Now ? DateTime.Now : student.EntryTime) : DateTime.Now,
				MarkedBy = _teacher.Id
			};

			await AttendanceData.InsertAttendance(attendanceModel);
		}

		MessageBox.Show("Attendance has been successfully saved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
		Close();
	}
}

internal class StudentAttendanceViewModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int Roll { get; set; }
	public bool Present { get; set; }
	public DateTime EntryTime { get; set; }
	public int AttendanceId { get; set; }
}