namespace AttendanceWorksLibrary.Models;

public class ActiveClassModel
{
	public int ScheduledClassId { get; set; }
	public int CourseId { get; set; }
	public string CourseName { get; set; }
	public string CourseCode { get; set; }
	public int SectionId { get; set; }
	public string SectionName { get; set; }
	public int ClassroomId { get; set; }
	public string ClassroomName { get; set; }
	public int TeacherId { get; set; }
	public string TeacherName { get; set; }
	public DateOnly ClassDate { get; set; }
	public TimeOnly StartTime { get; set; }
	public TimeOnly EndTime { get; set; }
}