CREATE PROCEDURE [dbo].[Insert_SeectionStudent]
	@Id INT,
	@SectionId INT,
	@StudentId INT,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[SectionStudent]
			([SectionId], [StudentId], [Status])
		VALUES
			(@SectionId, @StudentId, @Status)
	END
	ELSE
	BEGIN
		UPDATE [dbo].[SectionStudent]
		SET 
			[SectionId] = @SectionId,
			[StudentId] = @StudentId,
			[Status] = @Status
		WHERE [Id] = @Id
	END
END