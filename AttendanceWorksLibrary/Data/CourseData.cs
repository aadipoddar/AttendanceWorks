namespace AttendanceWorksLibrary.Data;

public static class CourseData
{
	public static async Task InsertCourse(CourseModel course) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertCourse, course);

	public static async Task InsertCourseSection(CourseSectionModel courseSection) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertCourseSection, courseSection);
}
