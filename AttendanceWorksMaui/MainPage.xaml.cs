using System.Diagnostics;

using AttendanceWorksLibrary.Models;

namespace AttendanceWorksMaui;

public partial class MainPage : ContentPage
{
	private StudentModel _currentStudent;
	private ActiveClassModel _currentClass;
	private bool _locationPermissionGranted;
	private bool _isInClassRange;
	private bool _isAttendanceMarked;
	private CancellationTokenSource _cancelTokenSource;
	private bool _isCheckingLocation;

	public MainPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// Initialize UI with current date
		DateLabel.Text = DateTime.Now.ToString("MMMM d, yyyy");

		// Load student info
		await LoadStudentInfo();

		// Check for active classes
		await RefreshClassInfo();

		// Check location permissions
		await CheckLocationPermission();
	}

	private async Task LoadStudentInfo()
	{
		try
		{
			// Simulate loading student information
			await Task.Delay(500);
			_currentStudent = new StudentModel
			{
				Id = 1,
				Name = "John Smith",
				Roll = 101
			};

			WelcomeLabel.Text = $"Hello, {_currentStudent.Name}";
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error loading student info: {ex.Message}");
			await DisplayAlert("Error", "Failed to load student information.", "OK");
		}
	}

	private async Task RefreshClassInfo()
	{
		try
		{
			// Show loading state
			CurrentClassLabel.Text = "Checking for active classes...";
			StatusLabel.Text = "CHECKING";
			StatusLabel.TextColor = Colors.Orange;

			// Simulate API call to check for active classes
			await Task.Delay(1000);

			// Check if there's an active class for the student
			var hasActiveClass = DateTime.Now.Hour >= 9 && DateTime.Now.Hour < 17; // Simulate check

			if (hasActiveClass)
			{
				// Simulate active class data
				_currentClass = new ActiveClassModel
				{
					CourseName = "Introduction to Programming",
					CourseCode = "CS101",
					SectionName = "Section A",
					ClassroomName = "Room 301",
					StartTime = new TimeOnly(9, 0),
					EndTime = new TimeOnly(10, 30)
				};

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
		catch (Exception ex)
		{
			Debug.WriteLine($"Error refreshing class info: {ex.Message}");
			CurrentClassLabel.Text = "Failed to load class info";
			await DisplayAlert("Error", "Failed to refresh class information.", "OK");
		}
	}

	private async Task CheckAttendanceStatus()
	{
		if (_currentClass == null)
		{
			AttendanceStatusLabel.Text = "No active class";
			AttendanceStatusLabel.TextColor = Colors.Grey;
			return;
		}

		// Simulate checking if attendance is already marked
		await Task.Delay(500);
		bool isAttendanceMarked = false; // Simulate check

		_isAttendanceMarked = isAttendanceMarked;

		if (_isAttendanceMarked)
		{
			AttendanceStatusLabel.Text = "Marked Present";
			AttendanceStatusLabel.TextColor = Colors.Green;
			MarkAttendanceButton.IsEnabled = false;
			MarkAttendanceButton.Text = "Attendance Marked";
		}
		else
		{
			AttendanceStatusLabel.Text = "Not marked";
			AttendanceStatusLabel.TextColor = Color.FromArgb("#ff9900");
		}
	}

	private async Task CheckLocationPermission()
	{
		try
		{
			LocationInfoLabel.Text = "Checking location permissions...";

			var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

			if (status != PermissionStatus.Granted)
			{
				status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
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
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error checking location permission: {ex.Message}");
			LocationInfoLabel.Text = "Error checking location permissions";
		}
	}

	private async Task GetCurrentLocation()
	{
		try
		{
			_isCheckingLocation = true;
			_cancelTokenSource = new CancellationTokenSource();

			LocationStatusLabel.Text = "Locating...";

			var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
			var location = await Geolocation.GetLocationAsync(request, _cancelTokenSource.Token);

			if (location != null)
			{
				LocationStatusLabel.Text = "Available";

				// Check if student is in range of classroom
				if (_currentClass != null)
				{
					// Simulate classroom location check
					_isInClassRange = true; // Simulate check

					if (_isInClassRange)
					{
						LocationInfoLabel.Text = "You are within class location range";
						MarkAttendanceButton.IsEnabled = true && !_isAttendanceMarked;
					}
					else
					{
						LocationInfoLabel.Text = "You are not in the classroom location";
						MarkAttendanceButton.IsEnabled = false;
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
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error getting location: {ex.Message}");
			LocationStatusLabel.Text = "Error";
			LocationInfoLabel.Text = "Error determining your location";
			MarkAttendanceButton.IsEnabled = false;
		}
		finally
		{
			_isCheckingLocation = false;
		}
	}

	private async void MarkAttendanceButton_Clicked(object sender, EventArgs e)
	{
		if (_currentClass == null || !_locationPermissionGranted || !_isInClassRange)
		{
			await DisplayAlert("Cannot Mark Attendance",
				"Make sure you have an active class and are in the classroom location.", "OK");
			return;
		}

		try
		{
			// Disable the button while processing
			MarkAttendanceButton.IsEnabled = false;
			MarkAttendanceButton.Text = "Processing...";

			// Simulate marking attendance
			await Task.Delay(1500);

			// Update UI to show attendance marked
			AttendanceStatusLabel.Text = "Marked Present";
			AttendanceStatusLabel.TextColor = Colors.Green;
			MarkAttendanceButton.Text = "Attendance Marked";

			_isAttendanceMarked = true;

			await DisplayAlert("Success", "Your attendance has been marked successfully!", "OK");
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error marking attendance: {ex.Message}");
			MarkAttendanceButton.IsEnabled = true;
			MarkAttendanceButton.Text = "Mark My Attendance";
			await DisplayAlert("Error", "Failed to mark attendance. Please try again.", "OK");
		}
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
		{
			_cancelTokenSource?.Cancel();
		}
		base.OnDisappearing();
	}
}