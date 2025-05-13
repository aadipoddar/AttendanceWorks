namespace AttendOLibrary.Data;

public static class TeacherData
{
	public static async Task<TeacherModel> LoadTeacherByEmail(string Email) =>
		(await SqlDataAccess.LoadData<TeacherModel, dynamic>(StoredProcedureNames.LoadTeacherByEmail, new { Email })).FirstOrDefault();

	public static async Task<TeacherModel> LoadTeacherByEmailPassword(string Email, string Password) =>
		(await SqlDataAccess.LoadData<TeacherModel, dynamic>(StoredProcedureNames.LoadTeacherByEmailPassword, new { Email, Password })).FirstOrDefault();

	public static async Task InsertTeacher(TeacherModel teacher) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertTeacher, teacher);
}