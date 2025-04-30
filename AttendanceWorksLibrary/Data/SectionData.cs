namespace AttendanceWorksLibrary.Data;

public static class SectionData
{
	public static async Task InsertSection(SectionModel section) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertSection, section);

	public static async Task InsertSectionStudent(SectionStudentModel sectionStudent) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertSectionStudent, sectionStudent);
}
