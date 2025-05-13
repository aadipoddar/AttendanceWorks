namespace AttendOLibrary.Data;

public static class CourseData
{
	public static async Task InsertCourse(CourseModel course) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertCourse, course);
}
