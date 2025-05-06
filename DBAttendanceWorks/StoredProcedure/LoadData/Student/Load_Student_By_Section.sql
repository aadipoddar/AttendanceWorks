CREATE PROCEDURE [dbo].[Load_Student_By_Section]
	@SectionId INT
AS
BEGIN
	SELECT
	*
	FROM
		[dbo].[Student] s
	WHERE
		s.SectionId = @SectionId
		AND s.Status = 1
END