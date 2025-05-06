using System.Diagnostics;

namespace AttendanceWorksMaui;

public partial class MainPage : ContentPage
{
	private readonly StudentModel _student;
	private ActiveClassModel _currentClass;
	private bool _locationPermissionGranted;
	private bool _isInClassRange;
	private bool _isAttendanceMarked;
	private CancellationTokenSource _cancelTokenSource;
	private bool _isCheckingLocation;

	public MainPage(StudentModel student)
	{
		InitializeComponent();
		_student = student;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// Initialize UI with current date
		DateLabel.Text = DateTime.Now.ToString("MMMM d, yyyy");
		WelcomeLabel.Text = $"Hello, {_student.Name} ({_student.Roll})";

		// Check for active classes
		await RefreshClassInfo();

		// Check location permissions
		await CheckLocationPermission();
	}

	private async Task RefreshClassInfo()
	{
		// Show loading state
		CurrentClassLabel.Text = "Checking for active classes...";
		StatusLabel.Text = "CHECKING";
		StatusLabel.TextColor = Colors.Orange;

		var activeClasses = await CommonData.LoadTableData<ActiveClassModel>(ViewNames.ViewActiveClasses);
		_currentClass = activeClasses.Where(x => x.SectionId == _student.SectionId).FirstOrDefault();

		if (_currentClass is not null)
		{
			CurrentClassLabel.Text = $"{_currentClass.CourseCode}: {_currentClass.CourseName}";
			ClassTimeLabel.Text = $"{_currentClass.StartTime:hh\\:mm tt} - {_currentClass.EndTime:hh\\:mm tt}";
			ClassRoomLabel.Text = _currentClass.ClassroomName;
			StatusLabel.Text = "ACTIVE";
			StatusLabel.TextColor = Colors.Green;

			// Enable mark attendance button
			MarkAttendanceButton.IsEnabled = true;
		}
		else
		{
			_currentClass = null;
			CurrentClassLabel.Text = "No active class";
			ClassTimeLabel.Text = "N/A";
			ClassRoomLabel.Text = "N/A";
			StatusLabel.Text = "INACTIVE";
			StatusLabel.TextColor = Color.FromArgb("#ff6b6b");

			// Disable mark attendance button
			MarkAttendanceButton.IsEnabled = false;
		}

		// Check if attendance is already marked
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
			MarkAttendanceButton.IsEnabled = false;
			MarkAttendanceButton.Text = "Attendance Marked";
		}
		else
		{
			_isAttendanceMarked = false;

			AttendanceStatusLabel.Text = "Not marked";
			AttendanceStatusLabel.TextColor = Color.FromArgb("#ff9900");
		}
	}

	private async Task CheckLocationPermission()
	{
		try
		{
			LocationInfoLabel.Text = "Checking location permissions...";

			var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

			if (status != PermissionStatus.Granted)
			{
				status = await Permissions.RequestAsync<Permissions.LocationAlways>();
			}

			_locationPermissionGranted = (status == PermissionStatus.Granted);

			if (_locationPermissionGranted)
			{
				LocationInfoLabel.Text = "Location permission granted";
				await GetCurrentLocation();
			}
			else
			{
				LocationStatusLabel.Text = "Not available";
				LocationInfoLabel.Text = "Location permission denied. Cannot mark attendance.";
				MarkAttendanceButton.IsEnabled = false;
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

			var request = new GeolocationRequest(GeolocationAccuracy.Best);
			var location = await Geolocation.GetLocationAsync(request, _cancelTokenSource.Token);

			if (location is not null)
			{
				LocationStatusLabel.Text = "Available";

				// Check if student is in range of classroom
				if (_currentClass is not null)
				{
					var classRoom = await CommonData.LoadTableDataById<ClassRoomModel>(TableNames.ClassRoom, _currentClass.ClassroomId);

					if (classRoom is not null)
					{
						double differenceKilometeres = Location.CalculateDistance(location, new Location((double)classRoom.Latitude, (double)classRoom.Longitude), DistanceUnits.Kilometers);
						_isInClassRange = differenceKilometeres < 0.05;
						//_isInClassRange = differenceKilometeres < 0.001;
					}
					else
						_isInClassRange = false;

					if (_isInClassRange)
					{
						LocationInfoLabel.Text = "You are within class location range";
						MarkAttendanceButton.IsEnabled = true && !_isAttendanceMarked;
						await MarkAttendance();
					}
					else
					{
						LocationInfoLabel.Text = "You are not in the classroom location";
						MarkAttendanceButton.IsEnabled = false;
						await MarkAbsent();
					}
				}
				else
				{
					LocationInfoLabel.Text = "No active class to check location against";
				}
			}
			else
			{
				LocationStatusLabel.Text = "Unknown";
				LocationInfoLabel.Text = "Unable to determine your location";
				MarkAttendanceButton.IsEnabled = false;
				await MarkAbsent();
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error getting location: {ex.Message}");
			LocationStatusLabel.Text = "Error";
			LocationInfoLabel.Text = "Error determining your location";
			MarkAttendanceButton.IsEnabled = false;
			await MarkAbsent();
		}
		finally
		{
			_isCheckingLocation = false;
		}
	}

	private async void MarkAttendanceButton_Clicked(object sender, EventArgs e) =>
		await MarkAttendance();

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

		// Disable the button while processing
		MarkAttendanceButton.IsEnabled = false;
		MarkAttendanceButton.Text = "Processing...";

		var existingAttendance = await AttendanceData.LoadAttendanceByScheduledClass(_currentClass.ScheduledClassId);
		var attendanceRecord = existingAttendance.FirstOrDefault(a => a.StudentId == _student.Id);

		if (attendanceRecord is not null && attendanceRecord.MarkedBy is not null)
		{
			await DisplayAlert("Attendance Already Marked",
				"Your attendance has already been marked by the Teacher.", "OK");

			MarkAttendanceButton.IsEnabled = true;
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

		// Update UI to show attendance marked
		AttendanceStatusLabel.Text = "Marked Present";
		AttendanceStatusLabel.TextColor = Colors.Green;
		MarkAttendanceButton.Text = "Attendance Marked";

		_isAttendanceMarked = true;
	}

	private async void RefreshButton_Clicked(object sender, EventArgs e)
	{
		RefreshButton.IsEnabled = false;
		RefreshButton.Text = "Refreshing...";

		await RefreshClassInfo();
		await GetCurrentLocation();

		RefreshButton.IsEnabled = true;
		RefreshButton.Text = "Refresh Class Info";
	}

	protected override void OnDisappearing()
	{
		if (_isCheckingLocation)
			_cancelTokenSource?.Cancel();

		base.OnDisappearing();
	}

	private async Task MarkAbsent()
	{
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
			AttendanceStatusLabel.TextColor = Color.FromArgb("#ff9900");
		}
	}

	public async Task<bool> CheckMock()
	{
		var request = new GeolocationRequest(GeolocationAccuracy.Best);
		var location = await Geolocation.GetLocationAsync(request);

		if (location is null || location.IsFromMockProvider)
			return true;

		return false;
	}
}