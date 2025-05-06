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

public class StudentAttendanceViewModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int Roll { get; set; }
	public bool Present { get; set; }
	public DateTime EntryTime { get; set; }
	public int AttendanceId { get; set; }
}