using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace AttendanceWorksMaui.Pages;

public partial class NavigateToClassPage : ContentPage
{
	private readonly ClassRoomModel _classRoom;
	private Location _studentLocation;
	private CancellationTokenSource _cancelTokenSource;
	private bool _isCheckingLocation;
	private bool _locationPermissionGranted;

	public NavigateToClassPage(ClassRoomModel classRoom)
	{
		InitializeComponent();
		_classRoom = classRoom;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		LoadClassroomPin();
		await CheckLocationPermission();
	}

	private void LoadClassroomPin()
	{
		ClassroomDetailLabel.Text = _classRoom.Name;

		Pin classroomPin = new()
		{
			Label = _classRoom.Name,
			Address = "Your Classroom",
			Type = PinType.Place,
			Location = new Location((double)_classRoom.Latitude, (double)_classRoom.Longitude)
		};
		map.Pins.Add(classroomPin);
	}

	private async Task CheckLocationPermission()
	{
		try
		{
			DistanceDetailLabel.Text = "Checking location...";
			ETADetailLabel.Text = "Checking location...";

			var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

			if (status != PermissionStatus.Granted)
				status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

			_locationPermissionGranted = status == PermissionStatus.Granted;

			if (_locationPermissionGranted)
				await GetCurrentLocation();
			else
			{
				await DisplayAlert("Location Permission Required",
					"Navigation requires access to your location. Please enable location services.", "OK");

				map.MoveToRegion(MapSpan.FromCenterAndRadius(
					new Location((double)_classRoom.Latitude, (double)_classRoom.Longitude),
					Distance.FromKilometers(0.5)));

				DistanceDetailLabel.Text = "Location Permission Denied";
				ETADetailLabel.Text = "Location Permission Denied";
				DistanceDetailLabel.TextColor = Colors.Red;
				ETADetailLabel.TextColor = Colors.Red;
			}
		}
		catch (Exception)
		{
			await DisplayAlert("Location Error", "Unable to access location services.", "OK");
		}
	}

	private async Task GetCurrentLocation()
	{
		try
		{
			_isCheckingLocation = true;
			_cancelTokenSource = new CancellationTokenSource();

			var request = new GeolocationRequest(GeolocationAccuracy.Best);
			_studentLocation = await Geolocation.GetLocationAsync(request, _cancelTokenSource.Token);

			if (_studentLocation is not null)
			{
				AddPolyLine();

				var classroomLocation = new Location((double)_classRoom.Latitude, (double)_classRoom.Longitude);

				double distanceInKm = Location.CalculateDistance(_studentLocation, classroomLocation, DistanceUnits.Kilometers);
				DistanceDetailLabel.Text = $"{distanceInKm:F2} km";

				map.MoveToRegion(MapSpan.FromCenterAndRadius(
					new Location(
							(_studentLocation.Latitude + (double)_classRoom.Latitude) / 2,
							(_studentLocation.Longitude + (double)_classRoom.Longitude) / 2),
					Distance.FromKilometers(distanceInKm * 2)));

				// Estimate ETA (Walking speed = 5 km/h)
				double timeInMinutes = distanceInKm / 5.0 * 60;
				ETADetailLabel.Text = $"{timeInMinutes:F0} minutes (walking)";
			}
			else
			{
				await DisplayAlert("Location Not Found", "Unable to determine your current location.", "OK");
				DistanceDetailLabel.Text = "Location Not Found";
				ETADetailLabel.Text = "Location Not Found";
				DistanceDetailLabel.TextColor = Colors.Red;
				ETADetailLabel.TextColor = Colors.Red;

				map.MoveToRegion(MapSpan.FromCenterAndRadius(
					new Location((double)_classRoom.Latitude, (double)_classRoom.Longitude),
					Distance.FromKilometers(0.5)));
			}
		}
		catch (Exception)
		{
			await DisplayAlert("Location Error", "Error determining your location.", "OK");
			DistanceDetailLabel.Text = "Location Error";
			ETADetailLabel.Text = "Location Error";
			DistanceDetailLabel.TextColor = Colors.Red;
			ETADetailLabel.TextColor = Colors.Red;
		}
		finally
		{
			_isCheckingLocation = false;
		}
	}

	private void AddPolyLine()
	{
		var classroomLocation = new Location((double)_classRoom.Latitude, (double)_classRoom.Longitude);
		List<Location> routeCoordinates = [_studentLocation, classroomLocation];

		var polyline = new Polyline
		{
			StrokeColor = Color.FromArgb("#4285F4"), // Google Maps blue color
			StrokeWidth = 6,
		};

		foreach (var position in routeCoordinates)
			polyline.Geopath.Add(position);

		map.MapElements.Add(polyline);
	}

	private void MapTypeButton_Clicked(object sender, EventArgs e)
	{
		map.MapType = map.MapType switch
		{
			MapType.Street => MapType.Satellite,
			MapType.Satellite => MapType.Hybrid,
			MapType.Hybrid => MapType.Street,
			_ => MapType.Street
		};
	}

	private async void StartNavigationButton_Clicked(object sender, EventArgs e)
	{
		if (_classRoom is not null)
		{
			var location = new Location((double)_classRoom.Latitude, (double)_classRoom.Longitude);
			var options = new MapLaunchOptions
			{
				Name = _classRoom.Name,
				NavigationMode = NavigationMode.Walking
			};

			await Microsoft.Maui.ApplicationModel.Map.Default.OpenAsync(location, options);
		}
	}

	protected override void OnDisappearing()
	{
		if (_isCheckingLocation)
			_cancelTokenSource?.Cancel();

		base.OnDisappearing();
	}
}
