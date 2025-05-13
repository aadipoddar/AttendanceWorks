CREATE PROCEDURE [dbo].[Load_Teacher_By_Email]
	@Email NVARCHAR(100)
AS
BEGIN
	SELECT
	*
	FROM
		[dbo].[Teacher]
	WHERE
		[Email] = @Email
END