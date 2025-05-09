CREATE PROCEDURE [dbo].[Load_Student_By_Email]
	@Email NVARCHAR(100)
AS
BEGIN
	SELECT
	*
	FROM
		[dbo].[Student]
	WHERE
		[Email] = @Email
		AND [Status] = 1
END