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
    INNER JOIN [dbo].[Course] c ON sc.[CourseId] = c.[Id]
    INNER JOIN [dbo].[Section] s ON sc.[SectionId] = s.[Id]
    INNER JOIN [dbo].[Teacher] t ON sc.[TeacherId] = t.[Id]
    INNER JOIN [dbo].[ClassRoom] cr ON sc.[ClassRoomId] = cr.[Id]
WHERE 
    sc.[ClassDate] = CAST(DATEADD(MINUTE, 330, GETUTCDATE()) AS DATE)
    AND sc.[StartTime] <= CAST(DATEADD(MINUTE, 330, GETUTCDATE()) AS TIME)
    AND sc.[EndTime] > CAST(DATEADD(MINUTE, 330, GETUTCDATE()) AS TIME)
    --sc.[ClassDate] = CAST(GETDATE() AS DATE)
    --AND sc.[StartTime] <= CAST(GETDATE() AS TIME)
    --AND sc.[EndTime] > CAST(GETDATE() AS TIME)
    AND sc.[Status] = 1
    AND c.[Status] = 1
    AND s.[Status] = 1