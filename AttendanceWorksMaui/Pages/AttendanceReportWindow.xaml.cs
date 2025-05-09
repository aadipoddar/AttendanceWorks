using System.Collections.ObjectModel;

namespace AttendanceWorksMaui.Pages;

public partial class AttendanceReportWindow : ContentPage
{
	private readonly StudentModel _student;
	private List<AttendanceDetailModel> _attendanceDetails;

	public AttendanceReportWindow(StudentModel student)
	{
		InitializeComponent();
		_student = student;

		CoursesCollectionView.EmptyView = "Loading attendance data...";
	}

	private async void AttendanceReportWindow_Loaded(object sender, EventArgs e) =>
		await LoadAttendanceData();

	private async Task LoadAttendanceData()
	{
		var section = await CommonData.LoadTableDataById<SectionModel>(TableNames.Section, _student.SectionId);

		HeaderLabel.Text = "Attendance Report";
		StudentNameLabel.Text = $"{_student.Name} (Roll: {_student.Roll})";
		SectionLabel.Text = $"Section: {section?.Name ?? "Unknown"}";

		_attendanceDetails = await StudentData.LoadStudentAttendance(_student.Id);
		_attendanceDetails = [.. _attendanceDetails.Where(x => x.ClassDate >= DateOnly.FromDateTime(DateTime.Now))];

		if (_attendanceDetails is null || _attendanceDetails.Count == 0)
		{
			CoursesCollectionView.EmptyView = "No attendance records found.";
			return;
		}

		UpdateOverallStatistics();

		CoursesCollectionView.ItemsSource = CalculateCourseWiseAttendance();
	}

	private void UpdateOverallStatistics()
	{
		int totalClasses = _attendanceDetails.Count;
		int presentCount = _attendanceDetails.Count(a => a.Present);
		int absentCount = totalClasses - presentCount;
		int lateCount = _attendanceDetails.Count(a =>
			a.Present && (a.EntryTime.TimeOfDay - a.StartTime.ToTimeSpan()).TotalMinutes > 5);

		double presentPercentage = totalClasses > 0 ? (double)presentCount / totalClasses * 100 : 0;
		double absentPercentage = totalClasses > 0 ? (double)absentCount / totalClasses * 100 : 0;
		double latePercentage = totalClasses > 0 ? (double)lateCount / totalClasses * 100 : 0;

		OverallPresentPercentLabel.Text = $"{presentPercentage:F1}%";
		OverallAbsentPercentLabel.Text = $"{absentPercentage:F1}%";
		OverallLatePercentLabel.Text = $"{latePercentage:F1}%";

		OverallPresentCountLabel.Text = $"{presentCount}/{totalClasses} Classes";
		OverallAbsentCountLabel.Text = $"{absentCount}/{totalClasses} Classes";
		OverallLateCountLabel.Text = $"{lateCount}/{totalClasses} Classes";
	}

	private ObservableCollection<CourseAttendanceViewModel> CalculateCourseWiseAttendance()
	{
		var groupedAttendance = _attendanceDetails
			.GroupBy(a => new { a.CourseId, a.CourseCode, a.CourseName, a.CourseCredits })
			.Select(g => new CourseAttendanceViewModel
			{
				CourseId = g.Key.CourseId,
				CourseCode = g.Key.CourseCode,
				CourseName = g.Key.CourseName,
				CourseCredits = g.Key.CourseCredits,
				TotalClasses = g.Count(),
				PresentCount = g.Count(a => a.Present),
				AbsentCount = g.Count(a => !a.Present),
				LateCount = g.Count(a => a.Present &&
					(a.EntryTime.TimeOfDay - a.StartTime.ToTimeSpan()).TotalMinutes > 5)
			})
			.ToList();

		foreach (var course in groupedAttendance)
		{
			course.PresentPercentage = course.TotalClasses > 0
				? (double)course.PresentCount / course.TotalClasses * 100
				: 0;

			course.AbsentPercentage = course.TotalClasses > 0
				? (double)course.AbsentCount / course.TotalClasses * 100
				: 0;

			course.LatePercentage = course.TotalClasses > 0
				? (double)course.LateCount / course.TotalClasses * 100
				: 0;

			if (course.PresentPercentage >= 75)
			{
				course.StatusColor = Color.FromArgb("#dcf8c6");
				course.BorderColor = Color.FromArgb("#4CAF50");
				course.TextColor = Color.FromArgb("#2E7D32");
			}
			else if (course.PresentPercentage >= 60)
			{
				course.StatusColor = Color.FromArgb("#fff9c4");
				course.BorderColor = Color.FromArgb("#FFC107");
				course.TextColor = Color.FromArgb("#F57F17");
			}
			else
			{
				course.StatusColor = Color.FromArgb("#ffcdd2");
				course.BorderColor = Color.FromArgb("#F44336");
				course.TextColor = Color.FromArgb("#c62828");
			}
		}

		return [.. groupedAttendance.OrderBy(c => c.CourseCode)];
	}

	private async void BackButton_Clicked(object sender, EventArgs e) =>
		await Navigation.PopAsync();
}

public class CourseAttendanceViewModel
{
	public int CourseId { get; set; }
	public string CourseCode { get; set; }
	public string CourseName { get; set; }
	public int CourseCredits { get; set; }
	public int TotalClasses { get; set; }
	public int PresentCount { get; set; }
	public int AbsentCount { get; set; }
	public int LateCount { get; set; }
	public double PresentPercentage { get; set; }
	public double AbsentPercentage { get; set; }
	public double LatePercentage { get; set; }
	public Color StatusColor { get; set; }
	public Color BorderColor { get; set; }
	public Color TextColor { get; set; }
}