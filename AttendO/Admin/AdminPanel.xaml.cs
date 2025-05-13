using System.Windows;

namespace AttendO.Admin;

/// <summary>
/// Interaction logic for AdminPanel.xaml
/// </summary>
public partial class AdminPanel : Window
{
	public AdminPanel() =>
		InitializeComponent();

	private void Window_Loaded(object sender, RoutedEventArgs e) =>
		mainFrame.Content = new StudentPage();

	private void studentsButton_Click(object sender, RoutedEventArgs e) =>
		mainFrame.Content = new StudentPage();

	private void teacherButton_Click(object sender, RoutedEventArgs e) =>
		mainFrame.Content = new TeacherPage();

	private void classRoomButton_Click(object sender, RoutedEventArgs e) =>
		mainFrame.Content = new ClassRoomPage();

	private void courseButton_Click(object sender, RoutedEventArgs e) =>
		mainFrame.Content = new CoursePage();

	private void sectionButton_Click(object sender, RoutedEventArgs e) =>
		mainFrame.Content = new SectionPage();

	private void scheduledClassButton_Click(object sender, RoutedEventArgs e) =>
		mainFrame.Content = new ScheduledClassPage();
}
