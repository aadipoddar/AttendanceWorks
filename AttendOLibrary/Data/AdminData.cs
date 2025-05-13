namespace AttendOLibrary.Data;

public static class AdminData
{
	public static async Task<AdminModel> LoadAdminByEmail(string Email) =>
		(await SqlDataAccess.LoadData<AdminModel, dynamic>(StoredProcedureNames.LoadAdminByEmail, new { Email })).FirstOrDefault();

	public static async Task<AdminModel> LoadAdminByEmailPassword(string Email, string Password) =>
		(await SqlDataAccess.LoadData<AdminModel, dynamic>(StoredProcedureNames.LoadAdminByEmailPassword, new { Email, Password })).FirstOrDefault();
}
