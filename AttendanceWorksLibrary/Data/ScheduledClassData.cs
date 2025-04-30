namespace AttendanceWorksLibrary.Data;

public static class ScheduledClassData
{
	public static async Task InsertScheduledClass(ScheduledClassModel scheduledClassModel) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertScheduledClass, scheduledClassModel);
}
