CREATE VIEW [dbo].[View_ScheduledClassDetails]
	AS
SELECT
	sc.[Id],
	c.Id AS CourseId,
	c.[Name] AS CourseName,
	c.[Code] AS CourseCode,
	s.Id AS [SectionId],
	s.[Name] AS SectionName,
	cr.[Id] AS ClassRoomId,
	cr.[Name] AS ClassRoomName,
	t.[Id] AS TeacherId,
	t.[Name] AS TeacherName,
	sc.[ClassDate],
	sc.[StartTime],
	sc.[EndTime]
FROM
	ScheduledClass sc
JOIN
	Course c ON sc.[CourseId] = c.[Id]
JOIN
	Section s ON sc.[SectionId] = s.[Id]
JOIN
	Teacher t ON sc.[TeacherId] = t.[Id]
JOIN
	ClassRoom cr ON sc.[ClassRoomId] = cr.[Id]
WHERE sc.[Status] = 1
	AND c.[Status] = 1
	AND s.[Status] = 1	