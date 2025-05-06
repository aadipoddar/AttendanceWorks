namespace AttendanceWorksLibrary.Data;

public static class SectionData
{
	public static async Task InsertSection(SectionModel section) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertSection, section);
}
