CREATE VIEW [dbo].[View_AttendanceDetails]
	AS
SELECT
	a.Id,
	a.StudentId,
	st.Name AS StudentName,
	st.Roll AS StudentRoll,
	st.Phone AS StudentPhone,
	st.Email AS StudentEmail,
	a.EntryTime,
	a.Present,
	a.MarkedBy,
	t.Name AS MarkedByName,
	a.ScheduledClassId,
	sc.ClassDate,
	sc.StartTime,
	sc.EndTime,
	cr.Id AS ClassRoomId,
	cr.Name AS ClassRoomName,
	c.Id AS CourseId,
	c.Code AS CourseCode,
	c.Name AS CourseName,
	c.Credits AS CourseCredits,
	s.Id AS SectionId,
	s.Name AS SectionName
FROM 
	Attendance a
JOIN
	ScheduledClass sc ON a.ScheduledClassId = sc.Id
JOIN
	ClassRoom cr ON sc.ClassRoomId = cr.Id
JOIN
	Course c ON sc.CourseId = c.Id
JOIN
	Section s ON sc.SectionId = s.Id
JOIN
	Student st ON a.StudentID = st.Id
LEFT JOIN
	Teacher t ON a.MarkedBy = t.Id