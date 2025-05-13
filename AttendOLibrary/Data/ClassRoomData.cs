namespace AttendOLibrary.Data;

public static class ClassRoomData
{
	public static async Task InsertClassRoom(ClassRoomModel classRoom) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertClassRoom, classRoom);
}
