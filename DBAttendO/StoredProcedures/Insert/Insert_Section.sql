CREATE PROCEDURE [dbo].[Insert_Section]
	@Id INT,
	@Name VARCHAR(20),
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Section]
			([Name], [Status])
		VALUES
			(@Name, @Status)
	END
	ELSE
	BEGIN
		UPDATE [dbo].[Section]
		SET 
			[Name] = @Name,
			[Status] = @Status
		WHERE [Id] = @Id
	END
END