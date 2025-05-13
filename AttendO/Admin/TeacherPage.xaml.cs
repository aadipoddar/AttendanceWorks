using System.Windows;
using System.Windows.Controls;

namespace AttendO.Admin;

/// <summary>
/// Interaction logic for TeacherPage.xaml
/// </summary>
public partial class TeacherPage : Page
{
	public TeacherPage() =>
	InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		var teachers = await CommonData.LoadTableData<TeacherModel>(TableNames.Teacher);
		teacherDataGrid.ItemsSource = teachers;
	}

	private void integerTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private void teacherDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
	{
		if (teacherDataGrid.SelectedItem is TeacherModel teacher)
		{
			nameTextBox.Text = teacher.Name;
			passwordTextBox.Password = teacher.Password;
			emailTextBox.Text = teacher.Email;
			phoneTextBox.Text = teacher.Phone;
			statusCheckBox.IsChecked = teacher.Status;
		}

		else
			ClearForm();
	}

	private bool ValidateForm()
	{
		if (string.IsNullOrWhiteSpace(nameTextBox.Text))
		{
			MessageBox.Show("Please enter a name.");
			nameTextBox.Focus();
			return false;
		}

		if (string.IsNullOrWhiteSpace(passwordTextBox.Password))
		{
			MessageBox.Show("Please enter a password.");
			passwordTextBox.Focus();
			return false;
		}

		if (string.IsNullOrWhiteSpace(emailTextBox.Text))
		{
			MessageBox.Show("Please enter an email.");
			emailTextBox.Focus();
			return false;
		}

		if (string.IsNullOrWhiteSpace(phoneTextBox.Text))
		{
			MessageBox.Show("Please enter a phone number.");
			phoneTextBox.Focus();
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm())
			return;

		await TeacherData.InsertTeacher(new TeacherModel
		{
			Id = (teacherDataGrid.SelectedItem as TeacherModel)?.Id ?? 0,
			Name = nameTextBox.Text,
			Password = passwordTextBox.Password,
			Email = emailTextBox.Text,
			Phone = phoneTextBox.Text,
			Status = statusCheckBox.IsChecked ?? false
		});

		await LoadData();
		ClearForm();
	}

	private void ClearForm()
	{
		nameTextBox.Text = string.Empty;
		passwordTextBox.Password = string.Empty;
		emailTextBox.Text = string.Empty;
		phoneTextBox.Text = string.Empty;
		statusCheckBox.IsChecked = true;
	}
}
