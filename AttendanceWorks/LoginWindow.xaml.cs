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
			Dashboard dashboard = new(teacher);
			dashboard.Show();
			Close();

			return;
		}

		MessageBox.Show("Invalid email or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
	}
}