using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace AttendO.Teachers;

/// <summary>
/// Interaction logic for Dashboard.xaml
/// </summary>
public partial class Dashboard : Window
{
	private readonly TeacherModel _teacher;
	private readonly DispatcherTimer _dateTimeTimer;
	private readonly TextBlock _presentPercentText;
	private readonly TextBlock _latePercentText;
	private readonly TextBlock _absentPercentText;
	private readonly StackPanel _classesStackPanel;

	public Dashboard(TeacherModel teacher)
	{
		InitializeComponent();
		_teacher = teacher;

		if (_teacher is not null)
		{
			welcomeText.Text = $"Welcome back, {_teacher.Name}!";
			teacherNameText.Text = _teacher.Name;
			teacherEmailText.Text = _teacher.Email;

			// Set teacher initial
			if (!string.IsNullOrEmpty(_teacher.Name))
			{
				teacherInitial.Text = _teacher.Name[0].ToString().ToUpper();
			}
		}

		UpdateDateTime();

		_dateTimeTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMinutes(1)
		};
		_dateTimeTimer.Tick += (s, e) => UpdateDateTime();
		_dateTimeTimer.Start();

		// Set window title with teacher name
		Title = _teacher != null ? $"AttendO - {_teacher.Name}'s Dashboard" : "AttendO - Teacher Dashboard";

		// Find the percentage TextBlocks
		_presentPercentText = (TextBlock)FindName("presentPercentText");
		_latePercentText = (TextBlock)FindName("latePercentText");
		_absentPercentText = (TextBlock)FindName("absentPercentText");

		// Find classes container
		_classesStackPanel = FindName("classesContainer") as StackPanel;
		if (_classesStackPanel == null)
		{
			// If not found by name, try to find by traversing the visual tree
			var borders = FindVisualChildren<Border>(this);
			foreach (var border in borders)
			{
				if (border.Child is StackPanel stackPanel)
				{
					var headerTextBlock = stackPanel.Children.OfType<TextBlock>().FirstOrDefault();
					if (headerTextBlock != null && headerTextBlock.Text == "Today's Classes")
					{
						// Look for the stackpanel that will contain our class items
						foreach (var child in stackPanel.Children)
						{
							if (child is StackPanel childPanel)
							{
								_classesStackPanel = childPanel;
								break;
							}
						}
						if (_classesStackPanel != null) break;
					}
				}
			}
		}

	}

	// Helper method to find visual children of specified type
	private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
	{
		if (depObj == null) yield break;

		for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
		{
			var child = VisualTreeHelper.GetChild(depObj, i);

			if (child is T typedChild)
				yield return typedChild;

			foreach (var childOfChild in FindVisualChildren<T>(child))
				yield return childOfChild;
		}
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		try
		{
			await LoadActiveClassAttendanceSummary();
			await LoadTeacherClasses();
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	private async Task LoadActiveClassAttendanceSummary()
	{
		// Load active class
		var activeClasses = await CommonData.LoadTableData<ActiveClassModel>(ViewNames.ViewActiveClasses);

		var activeTeacherClass = activeClasses
			.Where(x => x.TeacherId == _teacher.Id).FirstOrDefault();

		if (activeTeacherClass is null)
		{
			attendanceSummaryTextBlock.Text = "No Active Classes";
			return;
		}

		// Load section for this active class
		var section = await CommonData.LoadTableDataById<SectionModel>(TableNames.Section, activeTeacherClass.SectionId);
		if (section is null)
		{
			attendanceSummaryTextBlock.Text = "Section not found";
			return;
		}

		// Update the attendance summary header
		attendanceSummaryTextBlock.Text = $"Attendance Summary - {section.Name}";

		// Load attendance data for this scheduled class
		var attendanceRecords = await AttendanceData.LoadAttendanceByScheduledClass(activeTeacherClass.ScheduledClassId);

		if (attendanceRecords is null || attendanceRecords.Count == 0)
		{
			_presentPercentText.Text = "0%";
			_latePercentText.Text = "0%";
			_absentPercentText.Text = "0%";
			return;
		}

		// Calculate attendance statistics
		await CalculateAndDisplayAttendanceStats(activeTeacherClass, attendanceRecords);
	}

	private async Task LoadTeacherClasses()
	{
		if (_classesStackPanel == null)
		{
			MessageBox.Show("Classes container not found in UI", "UI Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		// Clear existing class items
		_classesStackPanel.Children.Clear();

		// Get today's date
		var today = DateOnly.FromDateTime(DateTime.Today);

		// Load all scheduled classes for this teacher for today
		var allScheduledClasses = await CommonData.LoadTableData<ScheduledClassDetailModel>(ViewNames.ViewScheduledClassDetails);
		var teacherClasses = allScheduledClasses
			.Where(c => c.TeacherId == _teacher.Id && c.ClassDate == today)
			.OrderBy(c => c.StartTime)
			.ToList();

		if (teacherClasses.Count == 0)
		{
			// Add a message when no classes are scheduled
			var noclassesText = new TextBlock
			{
				Text = "No classes scheduled for today",
				Foreground = (SolidColorBrush)FindResource("MutedTextColor"),
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(0, 20, 0, 10),
				FontStyle = FontStyles.Italic
			};
			_classesStackPanel.Children.Add(noclassesText);
			return;
		}

		// Current time
		var currentTime = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay);

		foreach (var classItem in teacherClasses)
		{
			// Determine class status
			string status;
			string statusColor;

			if (currentTime < classItem.StartTime)
			{
				status = "Upcoming";
				statusColor = "#F5F5F5"; // Grey background
			}
			else if (currentTime >= classItem.StartTime && currentTime <= classItem.EndTime)
			{
				status = "Ongoing";
				statusColor = "#E3F2FD"; // Blue background
			}
			else
			{
				status = "Completed";
				statusColor = "#E8F5E9"; // Green background
			}

			// Create UI for class
			var classUI = CreateClassUI(classItem, status, statusColor);
			_classesStackPanel.Children.Add(classUI);

			// Now fetch attendance data for this class
			if (status == "Completed" || status == "Ongoing")
			{
				await UpdateClassAttendanceInfo(classItem, classUI);
			}
		}
	}

	private Border CreateClassUI(ScheduledClassDetailModel classItem, string status, string statusColor)
	{
		// Main border
		var border = new Border
		{
			Margin = new Thickness(0, 8, 0, 8),
			Padding = new Thickness(16, 12, 16, 12),
			Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F7FB")),
			CornerRadius = new CornerRadius(8)
		};

		// Main grid
		var grid = new Grid();
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		// Class info
		var infoPanel = new StackPanel();

		// Class name and section
		var classNameText = new TextBlock
		{
			Text = $"{classItem.CourseName} - {classItem.SectionName}",
			FontWeight = FontWeights.SemiBold,
			Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#36454F"))
		};
		infoPanel.Children.Add(classNameText);

		// Time and room
		var detailsPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 4, 0, 0) };

		var timeText = new TextBlock
		{
			Text = $"{classItem.StartTime:hh\\:mm tt} - {classItem.EndTime:hh\\:mm tt}",
			Foreground = (SolidColorBrush)FindResource("MutedTextColor"),
			FontSize = 13
		};
		detailsPanel.Children.Add(timeText);

		var separator = new TextBlock
		{
			Text = " • ",
			Foreground = (SolidColorBrush)FindResource("MutedTextColor"),
			FontSize = 13
		};
		detailsPanel.Children.Add(separator);

		var roomText = new TextBlock
		{
			Text = classItem.ClassRoomName,
			Foreground = (SolidColorBrush)FindResource("MutedTextColor"),
			FontSize = 13
		};
		detailsPanel.Children.Add(roomText);

		infoPanel.Children.Add(detailsPanel);

		// Attendance info (will be updated later)
		var attendanceText = new TextBlock
		{
			Name = $"attendance_{classItem.Id}",
			Margin = new Thickness(0, 6, 0, 0),
			FontSize = 12
		};
		infoPanel.Children.Add(attendanceText);

		Grid.SetColumn(infoPanel, 0);
		grid.Children.Add(infoPanel);

		// Status indicator
		var statusBorder = new Border
		{
			Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(statusColor)),
			CornerRadius = new CornerRadius(4),
			Padding = new Thickness(8, 4, 8, 4)
		};

		string foregroundColor = "#616161"; // Default grey text

		if (status == "Ongoing")
			foregroundColor = "#0D47A1"; // Blue text
		else if (status == "Completed")
			foregroundColor = "#2E7D32"; // Green text

		var statusText = new TextBlock
		{
			Text = status,
			Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(foregroundColor)),
			FontWeight = FontWeights.Medium
		};

		statusBorder.Child = statusText;
		Grid.SetColumn(statusBorder, 1);
		grid.Children.Add(statusBorder);

		border.Child = grid;
		return border;
	}

	private async Task UpdateClassAttendanceInfo(ScheduledClassDetailModel classItem, Border classBorder)
	{
		try
		{
			// Get attendance records for this class
			var attendanceRecords = await AttendanceData.LoadAttendanceByScheduledClass(classItem.Id);
			if (attendanceRecords == null || attendanceRecords.Count == 0) return;

			// Get students in section
			var students = await StudentData.LoadStudentBySection(classItem.SectionId);
			if (students == null || students.Count == 0) return;

			int totalStudents = students.Count;
			int presentCount = attendanceRecords.Count(a => a.Present);
			int lateCount = attendanceRecords.Count(a =>
				a.Present &&
				(a.EntryTime.TimeOfDay - classItem.StartTime.ToTimeSpan()).TotalMinutes > 5);
			int absentCount = totalStudents - presentCount;

			// Calculate percentages
			double presentPercentage = (double)presentCount / totalStudents * 100;
			double latePercentage = (double)lateCount / totalStudents * 100;
			double absentPercentage = (double)absentCount / totalStudents * 100;

			// Find the attendance text block
			TextBlock attendanceText = null;
			if (classBorder.Child is Grid grid)
			{
				foreach (var child in grid.Children)
				{
					if (child is StackPanel panel)
					{
						foreach (var panelChild in panel.Children)
						{
							if (panelChild is TextBlock textBlock && textBlock.Name == $"attendance_{classItem.Id}")
							{
								attendanceText = textBlock;
								break;
							}
						}
					}
				}
			}

			if (attendanceText != null)
			{
				attendanceText.Text = $"Present: {presentCount}/{totalStudents} ({presentPercentage:F1}%) | " +
								 $"Late: {lateCount}/{totalStudents} ({latePercentage:F1}%) | " +
								 $"Absent: {absentCount}/{totalStudents} ({absentPercentage:F1}%)";
				attendanceText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#555555"));
			}
		}
		catch (Exception ex)
		{
			// Just log the error, but don't crash the UI
			Console.WriteLine($"Error updating class attendance: {ex.Message}");
		}
	}

	private async Task CalculateAndDisplayAttendanceStats(ActiveClassModel activeClass, List<AttendanceModel> attendanceRecords)
	{
		try
		{
			// Get all students in the section
			var enrolledStudents = await StudentData.LoadStudentBySection(activeClass.SectionId);

			if (enrolledStudents is null || enrolledStudents.Count == 0)
			{
				_presentPercentText.Text = "0%";
				_latePercentText.Text = "0%";
				_absentPercentText.Text = "0%";
				return;
			}

			int totalStudents = enrolledStudents.Count;

			// Count present students
			int presentCount = attendanceRecords.Count(a => a.Present);

			// Count late students (present but entered after start time + 5 minutes)
			int lateCount = attendanceRecords.Count(a =>
				a.Present &&
				(a.EntryTime.TimeOfDay - activeClass.StartTime.ToTimeSpan()).TotalMinutes > 5);

			// Count absent students
			int absentCount = totalStudents - presentCount;

			// Calculate percentages
			double presentPercentage = totalStudents > 0 ? (double)presentCount / totalStudents * 100 : 0;
			double latePercentage = totalStudents > 0 ? (double)lateCount / totalStudents * 100 : 0;
			double absentPercentage = totalStudents > 0 ? (double)absentCount / totalStudents * 100 : 0;

			// Update UI with calculated values
			_presentPercentText.Text = $"{presentPercentage:F1}%";
			_latePercentText.Text = $"{latePercentage:F1}%";
			_absentPercentText.Text = $"{absentPercentage:F1}%";
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error calculating attendance statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	private void UpdateDateTime() =>
		dateTimeText.Text = DateTime.Now.ToString("dddd, MMMM d, yyyy");

	private void btnStudent_Click(object sender, RoutedEventArgs e)
	{
		StudentInfoWindow studentInfoWindow = new();
		studentInfoWindow.ShowDialog();
	}

	private async void btnMarkAttendance_Click(object sender, RoutedEventArgs e)
	{
		var activeClasses = await CommonData.LoadTableData<ActiveClassModel>(ViewNames.ViewActiveClasses);

		var activeTeacherClass = activeClasses
			.Where(x => x.TeacherId == _teacher.Id).FirstOrDefault();

		if (activeTeacherClass is null)
		{
			MessageBox.Show("You Don't Have any Active Classes.", "No Class Found");
			return;
		}

		MarkAttendance markAttendance = new(activeTeacherClass);
		markAttendance.ShowDialog();

		await LoadActiveClassAttendanceSummary();
		await LoadTeacherClasses();
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		_dateTimeTimer?.Stop();
	}
}
