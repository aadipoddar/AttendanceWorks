using System.Windows;

namespace AttendanceWorks;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : Window
{
	public LoginWindow()
	{
		Dapper.SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
		InitializeComponent();
	}

	private async void loginButton_Click(object sender, RoutedEventArgs e)
	{
		var teacher = await TeacherData.LoadTeacherByEmailPassword(emailTextBox.Text, passwordBox.Password);
		if (teacher is not null)
		{
			Dashboard dashboard = new(this);
			dashboard.Show();
			Hide();

			return;
		}

		var student = await StudentData.LoadStudentByEmailPassword(emailTextBox.Text, passwordBox.Password);
		if (student is not null)
		{
			Dashboard studentDashboard = new(this);
			studentDashboard.Show();
			Hide();

			return;
		}

		MessageBox.Show("Invalid email or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
	}
}