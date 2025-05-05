namespace AttendanceWorksLibrary.Models;

public class ScheduledClassModel
{
	public int Id { get; set; }
	public int CourseSectionId { get; set; }
	public DateOnly ClassDate { get; set; }
	public TimeOnly StartTime { get; set; }
	public TimeOnly EndTime { get; set; }
	public bool Status { get; set; }
}
