namespace AttendanceWorksMaui.Pages;

public partial class AttendanceReportWindow : ContentPage
{
	private StudentModel _student;

	public AttendanceReportWindow(StudentModel student)
	{
		InitializeComponent();
		_student = student;
	}
}