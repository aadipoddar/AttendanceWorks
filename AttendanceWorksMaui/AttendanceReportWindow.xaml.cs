using System.Collections.ObjectModel;

namespace AttendanceWorksMaui;

public partial class AttendanceReportWindow : ContentPage
{
	public ObservableCollection<AttendanceDetailViewModel> AttendanceRecords { get; set; } = new ObservableCollection<AttendanceDetailViewModel>();
	private readonly StudentModel _student;
	private bool _isDataLoaded = false;

	public AttendanceReportWindow(StudentModel student)
	{
		InitializeComponent();
		_student = student;
		BindingContext = this;
		StudentNameLabel.Text = $"{_student.Name} ({_student.Roll})";
		DateRangeLabel.Text = DateTime.Now.ToString("MMMM yyyy");
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		if (!_isDataLoaded)
		{
			await LoadAttendanceData();
			_isDataLoaded = true;
		}
	}

	private async Task LoadAttendanceData()
	{
		try
		{
			// Fetch attendance records from the database
			var attendanceDetails = await StudentData.LoadStudentAttendance(_student.Id);

			if (attendanceDetails != null && attendanceDetails.Count > 0)
			{
				// Clear existing records
				AttendanceRecords.Clear();

				// Add records to collection
				foreach (var record in attendanceDetails)
				{
					AttendanceRecords.Add(new AttendanceDetailViewModel
					{
						Id = record.Id,
						CourseName = record.CourseName,
						CourseCode = record.CourseCode,
						ClassDate = record.ClassDate,
						StartTime = record.StartTime,
						EndTime = record.EndTime,
						ClassRoomName = record.ClassRoomName,
						Present = record.Present,
						EntryTime = record.EntryTime,
						TimeInfo = $"{record.StartTime:hh\\:mm tt} - {record.EndTime:hh\\:mm tt}"
					});
				}

				// Update summary data
				UpdateSummary(attendanceDetails);
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Failed to load attendance data: {ex.Message}", "OK");
		}
	}

	private void UpdateSummary(List<AttendanceDetailModel> records)
	{
		int totalClasses = records.Count;
		int presentCount = records.Count(r => r.Present);
		int absentCount = totalClasses - presentCount;
		double attendanceRate = totalClasses > 0 ? (double)presentCount / totalClasses * 100 : 0;

		TotalClassesLabel.Text = totalClasses.ToString();
		PresentCountLabel.Text = presentCount.ToString();
		AbsentCountLabel.Text = absentCount.ToString();
		AttendanceRateLabel.Text = $"{attendanceRate:F1}%";

		AttendanceProgressBar.WidthRequest = attendanceProgressBackground.WidthRequest * (float)(attendanceRate / 100);
	}

	private async void FilterAll_Tapped(object sender, EventArgs e) =>
		await LoadAttendanceData();

	private async void FilterPresent_Tapped(object sender, EventArgs e)
	{
		var records = await StudentData.LoadStudentAttendance(_student.Id);
		var filteredRecords = records.Where(r => r.Present).ToList();

		AttendanceRecords.Clear();
		foreach (var record in filteredRecords)
		{
			AttendanceRecords.Add(new AttendanceDetailViewModel
			{
				Id = record.Id,
				CourseName = record.CourseName,
				CourseCode = record.CourseCode,
				ClassDate = record.ClassDate,
				StartTime = record.StartTime,
				EndTime = record.EndTime,
				ClassRoomName = record.ClassRoomName,
				Present = record.Present,
				EntryTime = record.EntryTime,
				TimeInfo = $"{record.StartTime:hh\\:mm tt} - {record.EndTime:hh\\:mm tt}"
			});
		}
	}

	private async void FilterAbsent_Tapped(object sender, EventArgs e)
	{
		var records = await StudentData.LoadStudentAttendance(_student.Id);
		var filteredRecords = records.Where(r => !r.Present).ToList();

		AttendanceRecords.Clear();
		foreach (var record in filteredRecords)
		{
			AttendanceRecords.Add(new AttendanceDetailViewModel
			{
				Id = record.Id,
				CourseName = record.CourseName,
				CourseCode = record.CourseCode,
				ClassDate = record.ClassDate,
				StartTime = record.StartTime,
				EndTime = record.EndTime,
				ClassRoomName = record.ClassRoomName,
				Present = record.Present,
				EntryTime = record.EntryTime,
				TimeInfo = $"{record.StartTime:hh\\:mm tt} - {record.EndTime:hh\\:mm tt}"
			});
		}
	}

	private async void BackButton_Clicked(object sender, EventArgs e) =>
		await Navigation.PopAsync();
}

// View model for attendance details
public class AttendanceDetailViewModel
{
	public int Id { get; set; }
	public string CourseName { get; set; }
	public string CourseCode { get; set; }
	public DateOnly ClassDate { get; set; }
	public TimeOnly StartTime { get; set; }
	public TimeOnly EndTime { get; set; }
	public string ClassRoomName { get; set; }
	public bool Present { get; set; }
	public DateTime? EntryTime { get; set; }
	public string TimeInfo { get; set; }
}