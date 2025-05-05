CREATE PROCEDURE [dbo].[Load_Student_By_Roll]
	@Roll INT
AS
BEGIN
	SELECT
	*
	FROM
		[dbo].[Student]
	WHERE
		[Roll] = @Roll
END