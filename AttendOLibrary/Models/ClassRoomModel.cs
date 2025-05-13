namespace AttendOLibrary.Models;

public class ClassRoomModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public decimal Latitude { get; set; }
	public decimal Longitude { get; set; }
	public bool Status { get; set; }
}