namespace AttendanceWorksLibrary.Data;

public static class ScheduledClassData
{
	public static async Task<int> InsertScheduledClass(ScheduledClassModel scheduledClassModel) =>
		(await SqlDataAccess.LoadData<int, dynamic>(StoredProcedureNames.InsertScheduledClass, scheduledClassModel)).FirstOrDefault();

	public static async Task<List<ScheduledClassDetailModel>> LoadScheduledClasseDetailsBySection(int SectionId) =>
		await SqlDataAccess.LoadData<ScheduledClassDetailModel, dynamic>(StoredProcedureNames.LoadScheduledClasseDetailsBySection, new { SectionId });
}
