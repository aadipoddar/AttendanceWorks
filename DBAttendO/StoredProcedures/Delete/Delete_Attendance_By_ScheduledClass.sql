CREATE PROCEDURE [dbo].[Delete_Attendance_By_ScheduledClass]
	@ScheduledClassId INT
AS
BEGIN
	DELETE
	FROM
		dbo.Attendance
	WHERE
		ScheduledClassId = @ScheduledClassId;
END