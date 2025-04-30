namespace AttendanceWorksLibrary.Models;

public class SectionStudentModel
{
	public int Id { get; set; }
	public int SectionId { get; set; }
	public int StudentId { get; set; }
	public bool Status { get; set; }
}
