CREATE PROCEDURE [dbo].[Load_Attendance_By_ScheduledClass]
	@ScheduledClassId INT
AS
BEGIN
	SELECT
		*
	FROM [dbo].[Attendance]
	WHERE
		[ScheduledClassId] = @ScheduledClassId
END