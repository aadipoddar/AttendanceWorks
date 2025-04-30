using System.Windows;

using AttendanceWorks.Admin;

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

	private void btnTeacher_Click(object sender, RoutedEventArgs e)
	{

	}

	private void btnAdmin_Click(object sender, RoutedEventArgs e)
	{
		AdminPanel adminPanel = new();
		adminPanel.Show();
	}
}
