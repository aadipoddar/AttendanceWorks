CREATE PROCEDURE [dbo].[Insert_ScheduledClass]
	@Id INT OUTPUT,
	@SectionId INT,
	@TeacherId INT,
	@CourseId INT,
	@ClassRoomId INT,
	@ClassDate DATE,
	@StartTime TIME,
	@EndTime TIME,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[ScheduledClass]
			([SectionId], [TeacherId], [CourseId], [ClassRoomId], [ClassDate], [StartTime], [EndTime], [Status])
		OUTPUT INSERTED.Id
		VALUES
			(@SectionId, @TeacherId, @CourseId, @ClassRoomId, @ClassDate, @StartTime, @EndTime, @Status)
	END
	ELSE
	BEGIN
		UPDATE [dbo].[ScheduledClass]
		SET 
			[SectionId] = @SectionId,
			[TeacherId] = @TeacherId,
			[CourseId] = @CourseId,
			[ClassRoomId] = @ClassRoomId,
			[ClassDate] = @ClassDate,
			[StartTime] = @StartTime,
			[EndTime] = @EndTime,
			[Status] = @Status
		WHERE [Id] = @Id

		SELECT @Id = @Id
	END
END