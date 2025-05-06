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

			passwordEntry.Text = "";
			emailEntry.Text = "";

			await Navigation.PushAsync(new MainPage(student));
		}

		else
			await DisplayAlert("Login Failed", "Invalid Email or Password", "OK");

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
		if (string.IsNullOrWhiteSpace(emailEntry.Text))
		{
			await DisplayAlert("Enter Email or Roll Number", "Please enter your Email", "OK");
			return;
		}

		var student = new StudentModel();

		if (emailEntry.Text.Contains('@'))
		{
			student = await StudentData.LoadStudentByEmail(emailEntry.Text);
			if (student is null)
			{
				await DisplayAlert("Email Not Found", "Please enter a valid Email", "OK");
				return;
			}
		}

		else
		{
			student = await StudentData.LoadStudentByRoll(int.Parse(emailEntry.Text));
			if (student is null)
			{
				await DisplayAlert("Roll Number Not Found", "Please enter a valid Roll Number", "OK");
				return;
			}
		}

		Mailing.MailPassword(student.Email, student.Password);

		await DisplayAlert("Password Mailed", "Your Password has been mailed to you to your Email Address", "OK");
	}

	private void TogglePasswordVisibility(object sender, EventArgs e)
	{
		passwordEntry.IsPassword = !passwordEntry.IsPassword;
		((ImageButton)sender).Source = passwordEntry.IsPassword ? "view.png" : "hide.png";
	}
}
