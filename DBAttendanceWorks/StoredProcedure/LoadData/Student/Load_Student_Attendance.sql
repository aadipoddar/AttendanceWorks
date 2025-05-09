CREATE PROCEDURE [dbo].[Load_Student_Attendance]
	@StudentID INT
AS
BEGIN
	SELECT
	*
	FROM 
		View_AttendanceDetails
	WHERE 
		StudentId = @StudentID
END