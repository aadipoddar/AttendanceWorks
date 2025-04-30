CREATE PROCEDURE [dbo].[Insert_ScheduledClass]
	@Id INT,
	@CourseSectionId INT,
	@ClassDate DATE,
	@StartTime TIME,
	@EndTime TIME,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[ScheduledClass]
			([CourseSectionId], [ClassDate], [StartTime], [EndTime], [Status])
		VALUES
			(@CourseSectionId, @ClassDate, @StartTime, @EndTime, @Status)
	END
	ELSE
	BEGIN
		UPDATE [dbo].[ScheduledClass]
		SET 
			[CourseSectionId] = @CourseSectionId,
			[ClassDate] = @ClassDate,
			[StartTime] = @StartTime,
			[EndTime] = @EndTime,
			[Status] = @Status
		WHERE [Id] = @Id
	END
END