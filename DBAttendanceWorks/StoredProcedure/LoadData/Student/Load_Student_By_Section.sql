CREATE PROCEDURE [dbo].[Load_Student_By_Section]
	@SectionId INT
AS
BEGIN
	SELECT 
		s.[Id],
		s.[Name],
		s.[Roll],
		s.[Email],
		s.[Phone],
		s.[Password],
		s.[Status]
	FROM 
		[dbo].[SectionStudent] ss
	JOIN 
		[dbo].[Student] s ON ss.[StudentId] = s.[Id]
	WHERE 
		ss.[SectionId] = @SectionId AND ss.[Status] = 1 AND s.[Status] = 1;
END