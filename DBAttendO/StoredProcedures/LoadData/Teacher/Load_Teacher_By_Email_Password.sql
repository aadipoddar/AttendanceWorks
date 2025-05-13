CREATE PROCEDURE [dbo].[Load_Teacher_By_Email_Password]
	@Email NVARCHAR(100),
	@Password NVARCHAR(10)
AS
BEGIN
	SELECT
	*
	FROM
		[dbo].[Teacher]
	WHERE
		[Email] = @Email
		AND [Password] = @Password
END