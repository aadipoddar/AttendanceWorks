using System.Windows;
using System.Windows.Controls;

namespace AttendO.Admin;

/// <summary>
/// Interaction logic for ClassRoomPage.xaml
/// </summary>
public partial class ClassRoomPage : Page
{
	public ClassRoomPage() =>
		InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		var classRooms = await CommonData.LoadTableData<ClassRoomModel>(TableNames.ClassRoom);
		classRoomDataGrid.ItemsSource = classRooms;
	}

	private void decimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);

	private void classRoomDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
	{
		if (classRoomDataGrid.SelectedItem is ClassRoomModel classRoom)
		{
			nameTextBox.Text = classRoom.Name;
			latitudeTextBox.Text = classRoom.Latitude.ToString();
			longitudeTextBox.Text = classRoom.Longitude.ToString();
			statusCheckBox.IsChecked = classRoom.Status;
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

		if (string.IsNullOrWhiteSpace(latitudeTextBox.Text))
		{
			MessageBox.Show("Please enter a latitude.");
			latitudeTextBox.Focus();
			return false;
		}

		if (string.IsNullOrWhiteSpace(longitudeTextBox.Text))
		{
			MessageBox.Show("Please enter a longitude.");
			longitudeTextBox.Focus();
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm())
			return;

		await ClassRoomData.InsertClassRoom(new ClassRoomModel
		{
			Id = (classRoomDataGrid.SelectedItem as ClassRoomModel)?.Id ?? 0,
			Name = nameTextBox.Text,
			Latitude = decimal.Parse(latitudeTextBox.Text),
			Longitude = decimal.Parse(longitudeTextBox.Text),
			Status = statusCheckBox.IsChecked ?? false
		});

		await LoadData();
		ClearForm();
	}

	private void ClearForm()
	{
		nameTextBox.Text = string.Empty;
		latitudeTextBox.Text = string.Empty;
		longitudeTextBox.Text = string.Empty;
		statusCheckBox.IsChecked = true;
	}
}
