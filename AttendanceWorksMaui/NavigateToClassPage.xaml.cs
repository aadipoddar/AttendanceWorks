using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace AttendanceWorksMaui;

public partial class NavigateToClassPage : ContentPage
{
	private readonly ClassRoomModel _classRoom;
	private Location _studentLocation;
	private double _zoomLevel = 16.0; // Default zoom level
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

		// Update UI with classroom info
		ClassNameLabel.Text = $"Finding your way to {_classRoom.Name}";
		ClassroomDetailLabel.Text = _classRoom.Name;

		// Create classroom pin
		Pin classroomPin = new()
		{
			Label = _classRoom.Name,
			Address = "Your Classroom",
			Type = PinType.Place,
			Location = new Location((double)_classRoom.Latitude, (double)_classRoom.Longitude)
		};
		map.Pins.Add(classroomPin);

		// Check location permission and get current location
		await CheckLocationPermissionAndGetLocation();
	}

	private async Task CheckLocationPermissionAndGetLocation()
	{
		try
		{
			// Show loading indicators
			DistanceDetailLabel.Text = "Checking location...";
			ETADetailLabel.Text = "Checking location...";

			var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

			if (status != PermissionStatus.Granted)
			{
				status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
			}

			_locationPermissionGranted = (status == PermissionStatus.Granted);

			if (_locationPermissionGranted)
			{
				await GetCurrentLocation();
			}
			else
			{
				await DisplayAlert("Location Permission Required",
					"Navigation requires access to your location. Please enable location services.", "OK");

				// Center the map on classroom location only
				map.MoveToRegion(MapSpan.FromCenterAndRadius(
					new Location((double)_classRoom.Latitude, (double)_classRoom.Longitude),
					Distance.FromKilometers(0.5)));

				DistanceDetailLabel.Text = "Unknown";
				ETADetailLabel.Text = "Unknown";
			}
		}
		catch (Exception ex)
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

			if (_studentLocation != null)
			{
				// Add student location pin
				Pin studentPin = new()
				{
					Label = "Your Location",
					Type = PinType.Generic,
					Location = _studentLocation
				};
				map.Pins.Add(studentPin);

				// Get classroom location
				var classroomLocation = new Location((double)_classRoom.Latitude, (double)_classRoom.Longitude);

				// Draw a route line (polyline) between student and classroom
				var routeCoordinates = new List<Location>
				{
					_studentLocation,
					classroomLocation
				};

				var polyline = new Polyline
				{
					StrokeColor = Color.FromArgb("#4285F4"),  // Google Maps blue color
					StrokeWidth = 6,
				};

				foreach (var position in routeCoordinates)
				{
					polyline.Geopath.Add(position);
				}

				map.MapElements.Add(polyline);

				// Center map to include both points
				map.MoveToRegion(MapSpan.FromCenterAndRadius(
					new Location(
						(_studentLocation.Latitude + (double)_classRoom.Latitude) / 2,
						(_studentLocation.Longitude + (double)_classRoom.Longitude) / 2),
					Distance.FromKilometers(
						CalculateDistance(_studentLocation, classroomLocation) * 2)));

				// Calculate and update distance
				double distanceInKm = CalculateDistance(_studentLocation, classroomLocation);

				DistanceDetailLabel.Text = $"{distanceInKm:F2} km";

				// Estimate ETA (assuming average walking speed of 5 km/h)
				double timeInMinutes = (distanceInKm / 5.0) * 60;
				ETADetailLabel.Text = $"{timeInMinutes:F0} minutes (walking)";
			}
			else
			{
				await DisplayAlert("Location Not Found", "Unable to determine your current location.", "OK");
				DistanceDetailLabel.Text = "Unknown";
				ETADetailLabel.Text = "Unknown";

				// Center the map on classroom location only
				map.MoveToRegion(MapSpan.FromCenterAndRadius(
					new Location((double)_classRoom.Latitude, (double)_classRoom.Longitude),
					Distance.FromKilometers(0.5)));
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Location Error", "Error determining your location.", "OK");
			DistanceDetailLabel.Text = "Error";
			ETADetailLabel.Text = "Error";
		}
		finally
		{
			_isCheckingLocation = false;
		}
	}

	private static double CalculateDistance(Location loc1, Location loc2)
	{
		return Location.CalculateDistance(loc1, loc2, DistanceUnits.Kilometers);
	}

	private void ZoomInButton_Clicked(object sender, EventArgs e)
	{
		_zoomLevel = Math.Min(_zoomLevel + 1, 20);
		UpdateMapZoom();
	}

	private void ZoomOutButton_Clicked(object sender, EventArgs e)
	{
		_zoomLevel = Math.Max(_zoomLevel - 1, 1);
		UpdateMapZoom();
	}

	private void UpdateMapZoom()
	{
		var center = map.VisibleRegion?.Center;
		if (center != null)
		{
			map.MoveToRegion(MapSpan.FromCenterAndRadius(
				center,
				Distance.FromKilometers(20 / Math.Pow(2, _zoomLevel - 1))));
		}
	}

	private void RecenterButton_Clicked(object sender, EventArgs e)
	{
		if (_studentLocation != null && _classRoom != null)
		{
			var classroomLocation = new Location((double)_classRoom.Latitude, (double)_classRoom.Longitude);

			// Redraw route when recentering
			var routeCoordinates = NavigateToClassPage.CreateSimplifiedRoute(_studentLocation, classroomLocation);

			// Draw the route
			var polyline = new Polyline
			{
				StrokeColor = Color.FromArgb("#4285F4"),
				StrokeWidth = 6
			};

			foreach (var position in routeCoordinates)
			{
				polyline.Geopath.Add(position);
			}

			// Clear and add new route
			map.MapElements.Clear();
			map.MapElements.Add(polyline);

			// Center map to include both points
			map.MoveToRegion(MapSpan.FromCenterAndRadius(
				new Location(
					(_studentLocation.Latitude + (double)_classRoom.Latitude) / 2,
					(_studentLocation.Longitude + (double)_classRoom.Longitude) / 2),
				Distance.FromKilometers(
					CalculateDistance(_studentLocation, classroomLocation) * 2)));
		}
		else if (_classRoom != null)
		{
			// If student location is not available, center on classroom
			map.MoveToRegion(MapSpan.FromCenterAndRadius(
				new Location((double)_classRoom.Latitude, (double)_classRoom.Longitude),
				Distance.FromKilometers(0.5)));
		}
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
		if (_classRoom != null)
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
		{
			_cancelTokenSource?.Cancel();
		}
		base.OnDisappearing();
	}
	private static List<Location> CreateSimplifiedRoute(Location start, Location end)
	{
		var route = new List<Location> { start };

		// Calculate midpoint with slight offset to make the route look more natural
		// This simulates the route following roads rather than a straight line
		double latMid = (start.Latitude + end.Latitude) / 2;
		double lonMid = (start.Longitude + end.Longitude) / 2;

		// Add slight randomness to make the path curve
		// The 0.0001 factor can be adjusted based on the typical distance
		double offsetFactor = 0.0001;
		double latOffset = (start.Longitude - end.Longitude) * offsetFactor;
		double lonOffset = (end.Latitude - start.Latitude) * offsetFactor;

		// Create intermediary points for a more natural curve
		route.Add(new Location(latMid + latOffset, lonMid + lonOffset));

		// Add the destination
		route.Add(end);

		return route;
	}
}
