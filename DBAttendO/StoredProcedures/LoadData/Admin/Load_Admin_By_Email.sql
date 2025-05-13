CREATE PROCEDURE [dbo].[Load_Admin_By_Email]
	@Email NVARCHAR(100)
AS
BEGIN
	SELECT
	*
	FROM
		[dbo].[Admin]
	WHERE
		[Email] = @Email
END