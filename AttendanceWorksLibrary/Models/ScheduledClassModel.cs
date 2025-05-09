namespace AttendanceWorksLibrary.Models;

public class ScheduledClassModel
{
	public int Id { get; set; }
	public int SectionId { get; set; }
	public int CourseId { get; set; }
	public int TeacherId { get; set; }
	public int ClassRoomId { get; set; }
	public DateOnly ClassDate { get; set; }
	public TimeOnly StartTime { get; set; }
	public TimeOnly EndTime { get; set; }
	public bool Status { get; set; }
}

public class ScheduledClassDetailModel
{
	public int Id { get; set; }
	public int CourseId { get; set; }
	public string CourseName { get; set; }
	public string CourseCode { get; set; }
	public int SectionId { get; set; }
	public string SectionName { get; set; }
	public int ClassRoomId { get; set; }
	public string ClassRoomName { get; set; }
	public int TeacherId { get; set; }
	public string TeacherName { get; set; }
	public DateOnly ClassDate { get; set; }
	public TimeOnly StartTime { get; set; }
	public TimeOnly EndTime { get; set; }
}