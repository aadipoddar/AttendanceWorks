CREATE VIEW [dbo].[View_ScheduleedClassDetails]
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
	CourseSection cs ON sc.[CourseSectionId] = cs.[Id]
JOIN
	Course c ON cs.[CourseId] = c.[Id]
JOIN
	Section s ON cs.[SectionId] = s.[Id]
JOIN
	Teacher t ON cs.[TeacherId] = t.[Id]
JOIN
	ClassRoom cr ON cs.[ClassRoomId] = cr.[Id]
WHERE sc.[Status] = 1
	AND cs.[Status] = 1
	AND c.[Status] = 1
	AND s.[Status] = 1	