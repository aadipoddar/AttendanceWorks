using AttendanceWorksLibrary.Data;

namespace AttendanceWorksMaui;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
		BindingContext = this;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// Check if user has saved credentials
		await LoadSavedCredentials();
	}

	private async Task LoadSavedCredentials()
	{
		var username = await SecureStorage.GetAsync("email");
		var password = await SecureStorage.GetAsync("password");

		if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
		{
			var student = await StudentData.LoadStudentByEmail(username);
			if (student is not null)
			{
				emailEntry.Text = student.Email;
				passwordEntry.Text = student.Password;
				RememberMeCheckbox.IsChecked = true;

				await LoginUser();
			}

			else
			{
				emailEntry.Text = string.Empty;
				passwordEntry.Text = string.Empty;
				RememberMeCheckbox.IsChecked = false;

				SecureStorage.Remove("email");
				SecureStorage.Remove("password");
			}
		}
	}

	private async void LoginButton_Clicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(emailEntry.Text) ||
			string.IsNullOrWhiteSpace(passwordEntry.Text))
		{
			await DisplayAlert("Login Failed", "Please enter both Email and Password", "OK");
			return;
		}

		// Show loading indicator
		IsBusy = true;

		await LoginUser();

		emailEntry.Text = "";
		passwordEntry.Text = "";

		IsBusy = false;
	}

	private async Task LoginUser()
	{
		bool isAuthenticated = emailEntry.Text.Contains('@') ?
			await CheckPassword(emailEntry.Text) :
			await CheckPassword(int.Parse(emailEntry.Text));

		if (isAuthenticated)
		{
			var student = emailEntry.Text.Contains('@') ?
				await StudentData.LoadStudentByEmail(emailEntry.Text) :
				await StudentData.LoadStudentByRoll(int.Parse(emailEntry.Text));

			if (RememberMeCheckbox.IsChecked)
			{
				await SecureStorage.SetAsync("email", student.Email);
				await SecureStorage.SetAsync("password", BCrypt.Net.BCrypt.EnhancedHashPassword(student.Password, 13));
			}
			else
			{
				SecureStorage.Remove("email");
				SecureStorage.Remove("password");
			}

			await Navigation.PushAsync(new MainPage());
		}
		else
		{
			await DisplayAlert("Login Failed", "Invalid Email or Password", "OK");
		}
	}

	private async Task<bool> CheckPassword(string email)
	{
		var student = await StudentData.LoadStudentByEmail(email);

		if (student is not null)
			return passwordEntry.Text == student.Password;

		return false;
	}

	private async Task<bool> CheckPassword(int roll)
	{
		var student = await StudentData.LoadStudentByRoll(roll);

		if (student is not null)
			return passwordEntry.Text == student.Password;

		return false;
	}

	private async void ForgotPassword_Tapped(object sender, EventArgs e)
	{
		await DisplayAlert("Reset Password",
			"Please contact your administrator or use the password reset feature on the web portal.", "OK");
	}
}
