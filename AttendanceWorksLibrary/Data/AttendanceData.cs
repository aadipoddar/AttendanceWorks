namespace AttendanceWorksLibrary.Data;

public static class AttendanceData
{
	public static async Task InsertAttendance(AttendanceModel attendanceModel) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertAttendance, attendanceModel);
}