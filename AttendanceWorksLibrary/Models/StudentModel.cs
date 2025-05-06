namespace AttendanceWorksLibrary.Models;

public class StudentModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int Roll { get; set; }
	public string Email { get; set; }
	public string Phone { get; set; }
	public string Password { get; set; }
	public int SectionId { get; set; }
	public bool Status { get; set; }
}
