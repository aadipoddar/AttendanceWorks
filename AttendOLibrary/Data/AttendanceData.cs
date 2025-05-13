namespace AttendOLibrary.Data;

public static class AttendanceData
{
	public static async Task InsertAttendance(AttendanceModel attendanceModel) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertAttendance, attendanceModel);

	public static async Task<List<AttendanceModel>> LoadAttendanceByScheduledClass(int ScheduledClassId) =>
		await SqlDataAccess.LoadData<AttendanceModel, dynamic>(StoredProcedureNames.LoadAttendanceByScheduledClass, new { ScheduledClassId });

	public static async Task DeleteAttendanceByScheduledClass(int ScheduledClassId) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.DeleteAttendanceByScheduledClass, new { ScheduledClassId });
}