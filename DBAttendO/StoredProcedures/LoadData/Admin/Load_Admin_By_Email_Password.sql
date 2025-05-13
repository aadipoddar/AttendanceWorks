CREATE PROCEDURE [dbo].[Load_Admin_By_Email_Password]
	@Email NVARCHAR(100),
	@Password NVARCHAR(10)
AS
BEGIN
	SELECT
	*
	FROM
		[dbo].[Admin]
	WHERE
		[Email] = @Email
		AND [Password] = @Password
END