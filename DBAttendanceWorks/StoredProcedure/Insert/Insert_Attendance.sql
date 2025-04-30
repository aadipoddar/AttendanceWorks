CREATE PROCEDURE [dbo].[Insert_Attendance]
	@Id INT,
	@ScheduledClassId INT,
	@StudentId INT,
	@Present BIT,
	@EntryTime DATETIME,
	@MarkedBy INT NULL
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Attendance]
			([ScheduledClassId], [StudentId], [Present], [MarkedBy])
		VALUES
			(@ScheduledClassId, @StudentId, @Present, @MarkedBy)
	END
	ELSE
	BEGIN
		UPDATE [dbo].[Attendance]
		SET 
			[ScheduledClassId] = @ScheduledClassId,
			[StudentId] = @StudentId,
			[Present] = @Present,
			[MarkedBy] = @MarkedBy
		WHERE [Id] = @Id
	END
END