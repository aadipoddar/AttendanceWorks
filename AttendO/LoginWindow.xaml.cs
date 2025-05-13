using System.Windows;

using AttendO.Admin;
using AttendO.Teachers;

namespace AttendO;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : Window
{
	public LoginWindow()
	{
		Dapper.SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());
		Dapper.SqlMapper.AddTypeHandler(new SqlTimeOnlyTypeHandler());

		InitializeComponent();
		emailTextBox.Focus();
	}

	private async void loginButton_Click(object sender, RoutedEventArgs e)
	{
		// Check Admin First
		var admin = await AdminData.LoadAdminByEmailPassword(emailTextBox.Text, passwordBox.Password);
		if (admin is not null)
		{
			if (admin.Status)
			{
				AdminPanel adminPanel = new();
				adminPanel.Show();
				Close();
				return;
			}

			MessageBox.Show("Your account is not active. Please contact the administrator.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		var teacher = await TeacherData.LoadTeacherByEmailPassword(emailTextBox.Text, passwordBox.Password);
		if (teacher is not null)
		{
			if (teacher.Status)
			{
				Dashboard dashboard = new(teacher);
				dashboard.Show();
				Close();
				return;
			}

			MessageBox.Show("Your account is not active. Please contact the administrator.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		MessageBox.Show("Invalid email or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
	}

	private async void ForgotPassword_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrWhiteSpace(emailTextBox.Text))
		{
			MessageBox.Show("Please enter your email", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		var admin = await AdminData.LoadAdminByEmail(emailTextBox.Text);
		if (admin is not null)
		{
			MessageBox.Show("No Forgot Password functionality available for admin", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		var teacher = await TeacherData.LoadTeacherByEmail(emailTextBox.Text);
		if (teacher is not null)
		{
			Mailing.ForgotPasswordEmail(emailTextBox.Text, teacher.Password);
			MessageBox.Show("An email has been sent to your registered email address with instructions to reset your password.", "Forgot Password", MessageBoxButton.OK, MessageBoxImage.Information);
			passwordBox.Clear();
			passwordBox.Focus();
			return;
		}

		MessageBox.Show("Email not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
	}
}
