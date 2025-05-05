CREATE VIEW [dbo].[View_ActiveClasses]
	AS
SELECT 
    sc.[Id] AS ScheduledClassId,
    c.Id AS CourseId,
    c.[Name] AS CourseName,
    c.[Code] AS CourseCode,
    s.Id AS [SectionId],
    s.[Name] AS SectionName,
    cr.[Id] AS ClassroomId,
    cr.[Name] AS ClassroomName,
    t.[Id] AS TeacherId,
    t.[Name] AS TeacherName,
    sc.[ClassDate],
    sc.[StartTime],
    sc.[EndTime]
FROM 
    [dbo].[ScheduledClass] sc
    INNER JOIN [dbo].[CourseSection] cs ON sc.[CourseSectionId] = cs.[Id]
    INNER JOIN [dbo].[Course] c ON cs.[CourseId] = c.[Id]
    INNER JOIN [dbo].[Section] s ON cs.[SectionId] = s.[Id]
    INNER JOIN [dbo].[Teacher] t ON cs.[TeacherId] = t.[Id]
    INNER JOIN [dbo].[ClassRoom] cr ON cs.[ClassRoomId] = cr.[Id]
WHERE 
    sc.[ClassDate] = CAST(GETDATE() AS DATE)
    AND sc.[StartTime] <= CAST(GETDATE() AS TIME)
    AND sc.[EndTime] > CAST(GETDATE() AS TIME)
    AND sc.[Status] = 1
    AND cs.[Status] = 1
    AND c.[Status] = 1
    AND s.[Status] = 1