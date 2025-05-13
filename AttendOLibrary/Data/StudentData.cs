namespace AttendOLibrary.Data;

public static class StudentData
{
	public static async Task InsertStudent(StudentModel student) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertStudent, student);

	public static async Task<StudentModel> LoadStudentByEmail(string Email) =>
		(await SqlDataAccess.LoadData<StudentModel, dynamic>(StoredProcedureNames.LoadStudentByEmail, new { Email })).FirstOrDefault();

	public static async Task<StudentModel> LoadStudentByRoll(int Roll) =>
		(await SqlDataAccess.LoadData<StudentModel, dynamic>(StoredProcedureNames.LoadStudentByRoll, new { Roll })).FirstOrDefault();

	public static async Task<List<StudentModel>> LoadStudentBySection(int SectionId) =>
		await SqlDataAccess.LoadData<StudentModel, dynamic>(StoredProcedureNames.LoadStudentBySection, new { SectionId });

	public static async Task<List<AttendanceDetailModel>> LoadStudentAttendance(int StudentId) =>
		await SqlDataAccess.LoadData<AttendanceDetailModel, dynamic>(StoredProcedureNames.LoadStudentAttendance, new { StudentId });
}
