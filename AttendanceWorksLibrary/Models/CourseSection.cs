namespace AttendanceWorksLibrary.Models;

public class CourseSectionModel
{
	public int Id { get; set; }
	public int CourseId { get; set; }
	public int SectionId { get; set; }
	public int TeacherId { get; set; }
	public int ClassRoomId { get; set; }
	public bool Status { get; set; }
}
