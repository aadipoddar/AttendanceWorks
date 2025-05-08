using System.Diagnostics;

namespace AttendanceWorksMaui;

public partial class MainPage : ContentPage
{
	private readonly StudentModel _student;
	private ActiveClassModel _currentClass;
	private CancellationTokenSource _cancelTokenSource;
	private bool _locationPermissionGranted;
	private bool _isInClassRange;
	private bool _isAttendanceMarked;
	private bool _isCheckingLocation;

	public MainPage(StudentModel student)
	{
		InitializeComponent();
		_student = student;
	}

	private async void ContentPage_Loaded(object sender, EventArgs e)
	{
		await LoadClassInfo();
		await LoadAttendanceStats();
		await CheckLocationPermission();
	}

	private async void RefreshButton_Clicked(object sender, EventArgs e)
	{
		RefreshButton.IsEnabled = false;
		RefreshButton.Text = "Refreshing...";

		await LoadClassInfo();
		await CheckLocationPermission();

		RefreshButton.IsEnabled = true;
		RefreshButton.Text = "Refresh Class Info";
	}

	private async Task LoadClassInfo()
	{
		DateLabel.Text = DateTime.Now.ToString("MMMM d, yyyy");
		WelcomeLabel.Text = $"Hello, {_student.Name} ({_student.Roll})";

		CurrentClassLabel.Text = "Checking for active classes...";

		var activeClasses = await CommonData.LoadTableData<ActiveClassModel>(ViewNames.ViewActiveClasses);
		_currentClass = activeClasses.Where(x => x.SectionId == _student.SectionId).FirstOrDefault();

		if (_currentClass is not null)
		{
			CurrentClassLabel.Text = $"{_currentClass.CourseCode}: {_currentClass.CourseName}";
			ClassTimeLabel.Text = $"{_currentClass.StartTime:hh\\:mm tt} - {_currentClass.EndTime:hh\\:mm tt}";
			ClassRoomLabel.Text = _currentClass.ClassroomName;
		}
		else
		{
			_currentClass = null;
			CurrentClassLabel.Text = "No active class";
			ClassTimeLabel.Text = "N/A";
			ClassRoomLabel.Text = "N/A";
		}

		await CheckAttendanceStatus();
	}

	private async Task CheckAttendanceStatus()
	{
		if (_currentClass is null)
		{
			AttendanceStatusLabel.Text = "No active class";
			AttendanceStatusLabel.TextColor = Colors.Grey;
			return;
		}

		var existingAttendance = await AttendanceData.LoadAttendanceByScheduledClass(_currentClass.ScheduledClassId);
		var attendanceRecord = existingAttendance.FirstOrDefault(a => a.StudentId == _student.Id);

		_isAttendanceMarked = attendanceRecord is not null;

		if (_isAttendanceMarked)
		{
			AttendanceStatusLabel.Text = attendanceRecord.Present ? "Marked Present" : "Marked Absent";
			AttendanceStatusLabel.TextColor = attendanceRecord.Present ? Colors.Green : Colors.Red;
		}
		else
		{
			_isAttendanceMarked = false;

			AttendanceStatusLabel.Text = "Not marked";
			AttendanceStatusLabel.TextColor = Color.FromArgb("#ff9900");
		}
	}

	private async Task LoadAttendanceStats()
	{
		var studentAttendance = await StudentData.LoadStudentAttendance(_student.Id);
		if (studentAttendance is not null)
		{
			var presentPercentage = (double)studentAttendance.Count(a => a.Present) / studentAttendance.Count * 100;
			var absentPercentage = (double)studentAttendance.Count(a => !a.Present) / studentAttendance.Count * 100;

			var lateCount = studentAttendance.Count(a =>
				(a.EntryTime.TimeOfDay - a.StartTime.ToTimeSpan()).TotalMinutes > 5);
			var latePercentage = (double)lateCount / studentAttendance.Count * 100;

			presentPercentLabel.Text = $"{presentPercentage:F2}%";
			absentPercentLabel.Text = $"{absentPercentage:F2}%";
			latePercentLabel.Text = $"{latePercentage:F2}%";
		}
	}

	private async Task CheckLocationPermission()
	{
		try
		{
			LocationInfoLabel.Text = "Checking location permissions...";

			var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

			if (status != PermissionStatus.Granted)
				status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

			_locationPermissionGranted = status == PermissionStatus.Granted;

			if (_locationPermissionGranted)
			{
				LocationInfoLabel.Text = "Location permission granted";
				await GetCurrentLocation();
			}
			else
			{
				LocationStatusLabel.Text = "Not available";
				LocationInfoLabel.Text = "Location permission denied. Cannot mark attendance.";
				await MarkAbsent();
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error checking location permission: {ex.Message}");
			LocationInfoLabel.Text = "Error checking location permissions";
			await MarkAbsent();
		}
	}

	private async Task GetCurrentLocation()
	{
		try
		{
			_isCheckingLocation = true;
			_cancelTokenSource = new CancellationTokenSource();

			LocationStatusLabel.Text = "Locating...";
			StudentCoordinatesLabel.Text = "Locating...";
			ClassCoordinatesLabel.Text = "Unknown";
			DistanceLabel.Text = "Calculating...";

			LocationStatusLabel.TextColor = Colors.Orange;
			DistanceLabel.TextColor = Colors.Orange;

			var request = new GeolocationRequest(GeolocationAccuracy.Best);
			var location = await Geolocation.GetLocationAsync(request, _cancelTokenSource.Token);

			if (location is not null)
			{
				LocationStatusLabel.Text = "Available";
				LocationStatusLabel.TextColor = Color.FromRgba("#C8A2C8");
				StudentCoordinatesLabel.Text = $"Lat: {location.Latitude:F6}, Long: {location.Longitude:F6}";

				if (_currentClass is not null)
				{
					var classRoom = await CommonData.LoadTableDataById<ClassRoomModel>(TableNames.ClassRoom, _currentClass.ClassroomId);

					if (classRoom is not null)
					{
						ClassCoordinatesLabel.Text = $"Lat: {classRoom.Latitude:F6}, Long: {classRoom.Longitude:F6}";

						double differenceKilometeres = Location.CalculateDistance(
							location,
							new Location((double)classRoom.Latitude, (double)classRoom.Longitude),
							DistanceUnits.Kilometers);

						DistanceLabel.Text = $"{differenceKilometeres:F3} km";
						_isInClassRange = differenceKilometeres < 0.05;
						//_isInClassRange = differenceKilometeres < 0.001;
					}
					else
					{
						ClassCoordinatesLabel.Text = "Unknown";
						DistanceLabel.Text = "Unknown";
						_isInClassRange = false;
					}

					if (_isInClassRange)
					{
						LocationInfoLabel.Text = "You are within class location range";
						LocationInfoLabel.TextColor = Colors.Green;
						await MarkAttendance();
						NavigateToClassButton.IsVisible = false;
					}
					else
					{
						LocationInfoLabel.Text = "You are not in the classroom location";
						LocationInfoLabel.TextColor = Colors.IndianRed;
						await MarkAbsent();
						NavigateToClassButton.IsVisible = true;
					}
				}
				else
				{
					LocationInfoLabel.Text = "No active class to check location against";
					ClassCoordinatesLabel.Text = "No active class";
					DistanceLabel.Text = "N/A";
				}
			}
			else
			{
				LocationStatusLabel.Text = "Unknown";
				StudentCoordinatesLabel.Text = "Unknown";
				DistanceLabel.Text = "Unknown";
				LocationInfoLabel.Text = "Unable to determine your location";
				await MarkAbsent();
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error getting location: {ex.Message}");
			LocationStatusLabel.Text = "Error";
			StudentCoordinatesLabel.Text = "Error";
			DistanceLabel.Text = "Error";
			LocationInfoLabel.Text = "Error determining your location";
			await MarkAbsent();
		}
		finally
		{
			_isCheckingLocation = false;
		}
	}

	private async Task MarkAttendance()
	{
		if (_currentClass is null || !_locationPermissionGranted || !_isInClassRange)
		{
			await DisplayAlert("Cannot Mark Attendance",
				"Make sure you have an active class and are in the classroom location.", "OK");
			return;
		}

		if (await CheckMock())
		{
			await DisplayAlert("Mock Location Detected", "Please disable mock locations to use this app.", "OK");
			await MarkAbsent();
			return;
		}

		var existingAttendance = await AttendanceData.LoadAttendanceByScheduledClass(_currentClass.ScheduledClassId);
		var attendanceRecord = existingAttendance.FirstOrDefault(a => a.StudentId == _student.Id);

		if (attendanceRecord is not null && attendanceRecord.MarkedBy is not null)
		{
			await DisplayAlert("Attendance Already Marked",
				"Your attendance has already been marked by the Teacher.", "OK");
			return;
		}

		await AttendanceData.InsertAttendance(new AttendanceModel
		{
			Id = attendanceRecord?.Id ?? 0,
			ScheduledClassId = _currentClass.ScheduledClassId,
			StudentId = _student.Id,
			Present = true,
			EntryTime = DateTime.Now,
			MarkedBy = null
		});

		AttendanceStatusLabel.Text = "Marked Present";
		AttendanceStatusLabel.TextColor = Colors.Green;

		_isAttendanceMarked = true;
	}

	private async Task MarkAbsent()
	{
		if (_currentClass is null)
			return;

		var existingAttendance = await AttendanceData.LoadAttendanceByScheduledClass(_currentClass.ScheduledClassId);
		var attendanceRecord = existingAttendance.FirstOrDefault(a => a.StudentId == _student.Id && a.MarkedBy is null);
		if (attendanceRecord is not null)
		{
			await AttendanceData.InsertAttendance(new AttendanceModel
			{
				Id = attendanceRecord.Id,
				ScheduledClassId = _currentClass.ScheduledClassId,
				StudentId = _student.Id,
				Present = false,
				EntryTime = DateTime.Now,
				MarkedBy = null
			});

			AttendanceStatusLabel.Text = "Marked Absent";
			AttendanceStatusLabel.TextColor = Colors.Red;
		}
	}

	public static async Task<bool> CheckMock()
	{
		var request = new GeolocationRequest(GeolocationAccuracy.Best);
		var location = await Geolocation.GetLocationAsync(request);

		if (location is null || location.IsFromMockProvider)
			return true;

		return false;
	}

	private async void NavigateToClassButton_Clicked(object sender, EventArgs e)
	{
		var classRoom = await CommonData.LoadTableDataById<ClassRoomModel>(TableNames.ClassRoom, _currentClass.ClassroomId);
		await Navigation.PushAsync(new NavigateToClassPage(classRoom));
	}

	private async void ViewHistoryButton_Clicked(object sender, EventArgs e) =>
		await Navigation.PushAsync(new AttendanceReportWindow(_student));

	private async void LogoutButton_Clicked(object sender, EventArgs e)
	{
		SecureStorage.Remove("email");
		SecureStorage.Remove("password");

		await Navigation.PopAsync();
	}

	protected override void OnDisappearing()
	{
		if (_isCheckingLocation)
			_cancelTokenSource?.Cancel();

		base.OnDisappearing();
	}

	private void ViewScheduleButton_Clicked(object sender, EventArgs e)
	{

	}
}