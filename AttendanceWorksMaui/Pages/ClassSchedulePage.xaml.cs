using Microsoft.Maui.Controls.Shapes;

namespace AttendanceWorksMaui.Pages;

public partial class ClassSchedulePage : ContentPage
{
	private readonly StudentModel _student;
	private List<ScheduledClassViewModel> _allClasses = [];

	private readonly Dictionary<string, Color> _lightThemeColors = new()
					{
						{ "PresentBackground", Color.FromArgb("#dcf8c6") },
						{ "PresentBorder", Color.FromArgb("#4CAF50") },
						{ "PresentText", Color.FromArgb("#2E7D32") },
						{ "AbsentBackground", Color.FromArgb("#ffcdd2") },
						{ "AbsentBorder", Color.FromArgb("#F44336") },
						{ "AbsentText", Color.FromArgb("#c62828") },
						{ "LateBackground", Color.FromArgb("#fff9c4") },
						{ "LateBorder", Color.FromArgb("#FFC107") },
						{ "LateText", Color.FromArgb("#F57F17") },
						{ "FutureBackground", Color.FromArgb("#E3F2FD") },
						{ "FutureBorder", Color.FromArgb("#2196F3") },
						{ "FutureText", Color.FromArgb("#0D47A1") },
						{ "CardBackground", Colors.White }
					};

	private readonly Dictionary<string, Color> _darkThemeColors = new()
					{
						{ "PresentBackground", Color.FromArgb("#1B5E20") },
						{ "PresentBorder", Color.FromArgb("#388E3C") },
						{ "PresentText", Color.FromArgb("#A5D6A7") },
						{ "AbsentBackground", Color.FromArgb("#B71C1C") },
						{ "AbsentBorder", Color.FromArgb("#E53935") },
						{ "AbsentText", Color.FromArgb("#FFCDD2") },
						{ "LateBackground", Color.FromArgb("#F57F17") },
						{ "LateBorder", Color.FromArgb("#FFA000") },
						{ "LateText", Color.FromArgb("#FFF9C4") },
						{ "FutureBackground", Color.FromArgb("#0D47A1") },
						{ "FutureBorder", Color.FromArgb("#2196F3") },
						{ "FutureText", Color.FromArgb("#90CAF9") },
						{ "CardBackground", Color.FromArgb("#252525") }
					};

	public ClassSchedulePage(StudentModel student)
	{
		InitializeComponent();
		_student = student;
	}

	private async void ContentPage_Loaded(object sender, EventArgs e) =>
		await LoadScheduleData();

	#region LoadClasses
	private async Task LoadScheduleData()
	{
		ClassDatePicker.Date = DateTime.Today;

		var section = await CommonData.LoadTableDataById<SectionModel>(TableNames.Section, _student.SectionId);

		StudentNameLabel.Text = $"{_student.Name} (Roll: {_student.Roll})";
		SectionLabel.Text = $"Section: {section?.Name ?? "Unknown"}";

		LoadingLabel.IsVisible = true;

		var scheduledClasses = await ScheduledClassData.LoadScheduledClasseDetailsBySection(_student.SectionId);
		if (scheduledClasses is null || scheduledClasses.Count == 0)
		{
			LoadingLabel.Text = "No scheduled classes found.";
			return;
		}

		var studentAttendance = await StudentData.LoadStudentAttendance(_student.Id);

		_allClasses = [.. scheduledClasses.Select(cls =>
		{
			var attendanceRecord = studentAttendance?.FirstOrDefault(a =>
				a.ScheduledClassId == cls.Id &&
				a.StudentId == _student.Id);

			bool isPast = cls.ClassDate.ToDateTime(cls.StartTime) < DateTime.Now;

			return new ScheduledClassViewModel
			{
				Id = cls.Id,
				CourseId = cls.CourseId,
				CourseCode = cls.CourseCode,
				CourseName = cls.CourseName,
				ClassRoomId = cls.ClassRoomId,
				ClassRoomName = cls.ClassRoomName,
				ClassDate = cls.ClassDate,
				StartTime = cls.StartTime,
				EndTime = cls.EndTime,
				IsPastClass = isPast,
				TeacherName = cls.TeacherName,
				AttendanceStatus = attendanceRecord is not null
					? (attendanceRecord.Present
						? ((attendanceRecord.EntryTime.TimeOfDay - cls.StartTime.ToTimeSpan()).TotalMinutes > 5
							? AttendanceStatus.Late
							: AttendanceStatus.Present)
						: AttendanceStatus.Absent)
					: AttendanceStatus.NotMarked
			};
		})];

		DisplayClassesForSelectedDate();
	}

	private void DisplayClassesForSelectedDate()
	{
		ScheduleContainer.Clear();
		ScheduleContainer.Add(LoadingLabel);
		LoadingLabel.IsVisible = false;

		var selectedDateOnly = DateOnly.FromDateTime(ClassDatePicker.Date);
		var classesOnDate = _allClasses
			.Where(c => c.ClassDate == selectedDateOnly)
			.OrderBy(c => c.StartTime)
			.ToList();

		if (classesOnDate.Count == 0)
		{
			LoadingLabel.Text = $"No classes scheduled for {ClassDatePicker.Date:dddd, MMMM d, yyyy}";
			LoadingLabel.IsVisible = true;
			return;
		}

		var dayHeader = new Label
		{
			Text = ClassDatePicker.Date.ToString("dddd, MMMM d, yyyy"),
			Style = (Style)Resources["DayHeaderStyle"]
		};
		ScheduleContainer.Add(dayHeader);

		foreach (var classItem in classesOnDate)
			AddClassCard(classItem);
	}

	private void AddClassCard(ScheduledClassViewModel classItem)
	{
		var isDarkTheme = Application.Current.RequestedTheme == AppTheme.Dark;
		var colors = isDarkTheme ? _darkThemeColors : _lightThemeColors;

		// Create the card border
		var cardBorder = new Border
		{
			StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10) },
			StrokeThickness = 1,
			Stroke = GetBorderColor(classItem.AttendanceStatus, classItem.IsPastClass),
			Background = GetBackgroundColor(classItem.AttendanceStatus, classItem.IsPastClass),
			Padding = new Thickness(15),
			Margin = new Thickness(0, 5, 0, 10)
		};

		// Create the card content
		var grid = new Grid
		{
			RowDefinitions =
							{
								new RowDefinition { Height = GridLength.Auto },
								new RowDefinition { Height = GridLength.Auto },
								new RowDefinition { Height = GridLength.Auto }
							},
			ColumnDefinitions =
							{
								new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
								new ColumnDefinition { Width = GridLength.Auto }
							}
		};

		// Course name and code
		var courseStack = new VerticalStackLayout
		{
			new Label
			{
				Text = classItem.CourseName,
				FontAttributes = FontAttributes.Bold,
				FontSize = 18
			},
			new Label
			{
				Text = classItem.CourseCode,
				FontSize = 14,
				TextColor = isDarkTheme ? Color.FromArgb("#AAAAAA") : Color.FromArgb("Gray")
			}
		};
		grid.Add(courseStack, 0, 0);

		// Time indicator on right
		var timeLabel = new Label
		{
			Text = $"{classItem.StartTime:hh\\:mm tt} - {classItem.EndTime:hh\\:mm tt}",
			FontAttributes = FontAttributes.Bold,
			TextColor = GetTextColor(classItem.AttendanceStatus, classItem.IsPastClass),
			HorizontalOptions = LayoutOptions.End,
			VerticalOptions = LayoutOptions.Center
		};
		grid.Add(timeLabel, 1, 0);

		// Class details
		var detailsGrid = new Grid
		{
			ColumnDefinitions =
							{
								new ColumnDefinition { Width = GridLength.Auto },
								new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
							},
			RowDefinitions =
							{
								new RowDefinition { Height = GridLength.Auto },
								new RowDefinition { Height = GridLength.Auto }
							},
			Margin = new Thickness(0, 10, 0, 0)
		};

		detailsGrid.Add(new Label
		{
			Text = "Teacher:",
			FontAttributes = FontAttributes.Bold,
			TextColor = isDarkTheme ? Color.FromArgb("#BBBBBB") : Color.FromArgb("#555555")
		}, 0, 0);

		detailsGrid.Add(new Label
		{
			Text = classItem.TeacherName,
			Margin = new Thickness(5, 0, 0, 0)
		}, 1, 0);

		detailsGrid.Add(new Label
		{
			Text = "Room:",
			FontAttributes = FontAttributes.Bold,
			TextColor = isDarkTheme ? Color.FromArgb("#BBBBBB") : Color.FromArgb("#555555")
		}, 0, 1);

		detailsGrid.Add(new Label
		{
			Text = classItem.ClassRoomName,
			Margin = new Thickness(5, 0, 0, 0)
		}, 1, 1);

		grid.Add(detailsGrid, 0, 1);
		Grid.SetColumnSpan(detailsGrid, 2);

		// Attendance status or navigation button
		if (classItem.IsPastClass)
		{
			var statusText = classItem.AttendanceStatus switch
			{
				AttendanceStatus.Present => "Present",
				AttendanceStatus.Absent => "Absent",
				AttendanceStatus.Late => "Late",
				_ => "Not Marked"
			};

			var statusColor = classItem.AttendanceStatus switch
			{
				AttendanceStatus.Present => isDarkTheme ? Color.FromArgb("#A5D6A7") : Color.FromArgb("#2E7D32"),
				AttendanceStatus.Absent => isDarkTheme ? Color.FromArgb("#FFCDD2") : Color.FromArgb("#c62828"),
				AttendanceStatus.Late => isDarkTheme ? Color.FromArgb("#FFF9C4") : Color.FromArgb("#F57F17"),
				_ => isDarkTheme ? Color.FromArgb("#AAAAAA") : Color.FromArgb("#888888")
			};

			var statusLabel = new Label
			{
				Text = $"Status: {statusText}",
				FontAttributes = FontAttributes.Bold,
				TextColor = statusColor,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center,
				Margin = new Thickness(0, 10, 0, 0)
			};
			grid.Add(statusLabel, 0, 2);
		}
		else
		{
			var navigateButton = new Button
			{
				Text = "Navigate to Class",
				BackgroundColor = isDarkTheme ? Color.FromArgb("#2196F3") : Color.FromArgb("#2196F3"),
				TextColor = Colors.White,
				FontAttributes = FontAttributes.Bold,
				CornerRadius = 20,
				Padding = new Thickness(5),
				Margin = new Thickness(0, 10, 0, 0),
				HorizontalOptions = LayoutOptions.Fill,
			};

			navigateButton.Clicked += async (sender, e) =>
			{
				var classRoom = await CommonData.LoadTableDataById<ClassRoomModel>(TableNames.ClassRoom, classItem.ClassRoomId);
				if (classRoom is not null)
					await Navigation.PushAsync(new NavigateToClassPage(classRoom));
			};

			grid.Add(navigateButton, 0, 2);
			Grid.SetColumnSpan(navigateButton, 2);
		}

		cardBorder.Content = grid;
		ScheduleContainer.Add(cardBorder);
	}

	private Color GetBackgroundColor(AttendanceStatus status, bool isPastClass)
	{
		var isDarkTheme = Application.Current.RequestedTheme == AppTheme.Dark;
		var colors = isDarkTheme ? _darkThemeColors : _lightThemeColors;

		if (!isPastClass)
			return colors["FutureBackground"];

		return status switch
		{
			AttendanceStatus.Present => colors["PresentBackground"],
			AttendanceStatus.Absent => colors["AbsentBackground"],
			AttendanceStatus.Late => colors["LateBackground"],
			_ => isDarkTheme ? Color.FromArgb("#333333") : Colors.White
		};
	}

	private Color GetBorderColor(AttendanceStatus status, bool isPastClass)
	{
		var isDarkTheme = Application.Current.RequestedTheme == AppTheme.Dark;
		var colors = isDarkTheme ? _darkThemeColors : _lightThemeColors;

		if (!isPastClass)
			return colors["FutureBorder"];

		return status switch
		{
			AttendanceStatus.Present => colors["PresentBorder"],
			AttendanceStatus.Absent => colors["AbsentBorder"],
			AttendanceStatus.Late => colors["LateBorder"],
			_ => isDarkTheme ? Color.FromArgb("#555555") : Color.FromArgb("#DDDDDD")
		};
	}

	private Color GetTextColor(AttendanceStatus status, bool isPastClass)
	{
		var isDarkTheme = Application.Current.RequestedTheme == AppTheme.Dark;
		var colors = isDarkTheme ? _darkThemeColors : _lightThemeColors;

		if (!isPastClass)
			return colors["FutureText"];

		return status switch
		{
			AttendanceStatus.Present => colors["PresentText"],
			AttendanceStatus.Absent => colors["AbsentText"],
			AttendanceStatus.Late => colors["LateText"],
			_ => isDarkTheme ? Colors.White : Colors.Black
		};
	}
	#endregion

	#region DatePick
	private void PreviousDayButton_Clicked(object sender, EventArgs e)
	{
		ClassDatePicker.Date = ClassDatePicker.Date.AddDays(-1);
		DisplayClassesForSelectedDate();
	}

	private void NextDayButton_Clicked(object sender, EventArgs e)
	{
		ClassDatePicker.Date = ClassDatePicker.Date.AddDays(1);
		DisplayClassesForSelectedDate();
	}

	private void TodayButton_Clicked(object sender, EventArgs e)
	{
		ClassDatePicker.Date = DateTime.Today;
		DisplayClassesForSelectedDate();
	}

	private void ClassDatePicker_DateSelected(object sender, DateChangedEventArgs e)
	{
		ClassDatePicker.Date = e.NewDate;
		DisplayClassesForSelectedDate();
	}
	#endregion

	#region Events
	private async void ScheduleRefreshView_Refreshing(object sender, EventArgs e)
	{
		await LoadScheduleData();
		ScheduleRefreshView.IsRefreshing = false;
	}

	private void OnSwipedLeft(object sender, SwipedEventArgs e) =>
		NextDayButton_Clicked(sender, e);

	private void OnSwipedRight(object sender, SwipedEventArgs e) =>
		PreviousDayButton_Clicked(sender, e);

	private async void BackButton_Clicked(object sender, EventArgs e) =>
		await Navigation.PopAsync();
	#endregion
}

public class ScheduledClassViewModel
{
	public int Id { get; set; }
	public int CourseId { get; set; }
	public string CourseCode { get; set; }
	public string CourseName { get; set; }
	public int ClassRoomId { get; set; }
	public string ClassRoomName { get; set; }
	public DateOnly ClassDate { get; set; }
	public TimeOnly StartTime { get; set; }
	public TimeOnly EndTime { get; set; }
	public bool IsPastClass { get; set; }
	public string TeacherName { get; set; }
	public AttendanceStatus AttendanceStatus { get; set; }
}

public enum AttendanceStatus
{
	NotMarked,
	Present,
	Absent,
	Late
}