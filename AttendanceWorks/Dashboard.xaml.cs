using System.Windows;

using AttendanceWorks.Admin;
using AttendanceWorks.Attendance;

namespace AttendanceWorks;

/// <summary>
/// Interaction logic for Dashboard.xaml
/// </summary>
public partial class Dashboard : Window
{
	private TeacherModel _teacher;

	public Dashboard(TeacherModel teacher)
	{
		InitializeComponent();
		_teacher = teacher;
	}

	private void btnStudent_Click(object sender, RoutedEventArgs e)
	{

	}

	private async void btnMarkAttendacne_Click(object sender, RoutedEventArgs e)
	{
		var activeClasses = await CommonData.LoadTableData<ActiveClassModel>(ViewNames.ViewActiveClasses);

		var activeTeacherClass = activeClasses
			.Where(x => x.TeacherId == _teacher.Id).FirstOrDefault();

		if (activeTeacherClass is null)
		{
			MessageBox.Show("You Dont Have any Active Classes.", "No Class Found");
			return;
		}

		MarkAttendance markAttendance = new(activeTeacherClass);
		markAttendance.ShowDialog();
	}

	private void btnAdmin_Click(object sender, RoutedEventArgs e)
	{
		AdminPanel adminPanel = new();
		adminPanel.Show();
	}
}
