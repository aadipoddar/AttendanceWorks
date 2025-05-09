namespace AttendanceWorksLibrary.Models;

public class AttendanceModel
{
	public int Id { get; set; }
	public int ScheduledClassId { get; set; }
	public int StudentId { get; set; }
	public bool Present { get; set; }
	public DateTime EntryTime { get; set; }
	public int? MarkedBy { get; set; }
}

public class AttendanceDetailModel
{
	public int Id { get; set; }
	public int StudentId { get; set; }
	public string StudentName { get; set; }
	public int StudentRoll { get; set; }
	public string StudentPhone { get; set; }
	public string StudentEmail { get; set; }
	public DateTime EntryTime { get; set; }
	public bool Present { get; set; }
	public int? MarkedBy { get; set; }
	public string MarkedByName { get; set; }
	public int ScheduledClassId { get; set; }
	public DateOnly ClassDate { get; set; }
	public TimeOnly StartTime { get; set; }
	public TimeOnly EndTime { get; set; }
	public int ClassRoomId { get; set; }
	public string ClassRoomName { get; set; }
	public int CourseId { get; set; }
	public string CourseCode { get; set; }
	public string CourseName { get; set; }
	public int CourseCredits { get; set; }
	public int SectionId { get; set; }
	public string SectionName { get; set; }
}