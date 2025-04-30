using System.Windows;
using System.Windows.Controls;

namespace AttendanceWorks.Admin;

/// <summary>
/// Interaction logic for SectionPage.xaml
/// </summary>
public partial class SectionPage : Page
{
	public SectionPage() =>
	InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		var sections = await CommonData.LoadTableData<SectionModel>(TableNames.Section);
		sectionDataGrid.ItemsSource = sections;
	}

	private void sectionDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
	{
		if (sectionDataGrid.SelectedItem is SectionModel section)
		{
			nameTextBox.Text = section.Name;
			statusCheckBox.IsChecked = section.Status;
		}

		else
			ClearForm();
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm())
			return;

		await SectionData.InsertSection(new SectionModel
		{
			Id = (sectionDataGrid.SelectedItem as SectionModel)?.Id ?? 0,
			Name = nameTextBox.Text,
			Status = statusCheckBox.IsChecked ?? false
		});

		await LoadData();
		ClearForm();
	}

	private void ClearForm()
	{
		nameTextBox.Text = string.Empty;
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

		return true;
	}
}
