namespace AttendanceWorksLibrary.Data;

public static class StudentData
{
	public static async Task InsertStudent(StudentModel student) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertStudent, student);

	public static async Task<StudentModel> LoadStudentByEmailPassword(string Email, string Password) =>
		(await SqlDataAccess.LoadData<StudentModel, dynamic>(StoredProcedureNames.LoadStudentByEmailPassword, new { Email, Password })).FirstOrDefault();

	public static async Task<List<StudentModel>> LoadStudentBySection(int SectionId) =>
		await SqlDataAccess.LoadData<StudentModel, dynamic>(StoredProcedureNames.LoadStudentBySection, new { SectionId });
}
