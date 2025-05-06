using System.Windows;
using System.Windows.Controls;

namespace AttendanceWorks.Admin;

/// <summary>
/// Interaction logic for StudentPage.xaml
/// </summary>
public partial class StudentPage : Page
{
	public StudentPage() =>
		InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		var students = await CommonData.LoadTableData<StudentModel>(TableNames.Student);
		studentDataGrid.ItemsSource = students;

		sectionComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<SectionModel>(TableNames.Section);
		sectionComboBox.DisplayMemberPath = nameof(SectionModel.Name);
		sectionComboBox.SelectedValuePath = nameof(SectionModel.Id);
		sectionComboBox.SelectedIndex = 0;
	}

	private void studentDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
	{
		if (studentDataGrid.SelectedItem is StudentModel student)
		{
			nameTextBox.Text = student.Name;
			passwordTextBox.Password = student.Password;
			emailTextBox.Text = student.Email;
			phoneTextBox.Text = student.Phone;
			rollTextBox.Text = student.Roll.ToString();
			sectionComboBox.SelectedValue = student.SectionId;
			statusCheckBox.IsChecked = student.Status;
		}

		else
			ClearForm();
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm())
			return;

		await StudentData.InsertStudent(new StudentModel
		{
			Id = (studentDataGrid.SelectedItem as StudentModel)?.Id ?? 0,
			Name = nameTextBox.Text,
			Password = passwordTextBox.Password,
			Email = emailTextBox.Text,
			Phone = phoneTextBox.Text,
			Roll = int.Parse(rollTextBox.Text),
			SectionId = (int)sectionComboBox.SelectedValue,
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
		rollTextBox.Text = string.Empty;
		sectionComboBox.SelectedIndex = 0;
		statusCheckBox.IsChecked = true;
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

		if (string.IsNullOrWhiteSpace(rollTextBox.Text))
		{
			MessageBox.Show("Please enter a roll number.");
			rollTextBox.Focus();
			return false;
		}

		return true;
	}

	private void integerTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);
}
