namespace AttendanceWorksLibrary.Data;

public static class ScheduledClassData
{
	public static async Task<int> InsertScheduledClass(ScheduledClassModel scheduledClassModel) =>
		(await SqlDataAccess.LoadData<int, dynamic>(StoredProcedureNames.InsertScheduledClass, scheduledClassModel)).FirstOrDefault();
}
