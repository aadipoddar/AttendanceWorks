CREATE PROCEDURE [dbo].[Insert_CourseSection]
	@Id INT,
	@CourseId INT,
	@SectionId INT,
	@TeacherId INT,
	@ClassRoomId INT,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[CourseSection]
			([CourseId], [SectionId], [TeacherId], [ClassRoomId], [Status])
		VALUES
			(@CourseId, @SectionId, @TeacherId, @ClassRoomId, @Status)
	END
	ELSE
	BEGIN
		UPDATE [dbo].[CourseSection]
		SET 
			[CourseId] = @CourseId,
			[SectionId] = @SectionId,
			[TeacherId] = @TeacherId,
			[ClassRoomId] = @ClassRoomId,
			[Status] = @Status
		WHERE [Id] = @Id
	END
END