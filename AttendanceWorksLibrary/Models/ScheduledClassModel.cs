namespace AttendanceWorksLibrary.Models;

public class ScheduledClassModel
{
	public int Id { get; set; }
	public int CourseSectionId { get; set; }
	public DateOnly ClassDate { get; set; }
	public int StartTime { get; set; }
	public int EndTime { get; set; }
	public bool Status { get; set; }
}
