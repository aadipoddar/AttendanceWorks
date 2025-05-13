CREATE PROCEDURE [dbo].[Insert_Course]
	@Id INT,
	@Code VARCHAR(20),
	@Name VARCHAR(100),
	@Credits INT,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Course]
			([Code], [Name], [Credits], [Status])
		VALUES
			(@Code, @Name, @Credits, @Status)
	END
	ELSE
	BEGIN
		UPDATE [dbo].[Course]
		SET 
			[Code] = @Code,
			[Name] = @Name,
			[Credits] = @Credits,
			[Status] = @Status
		WHERE [Id] = @Id
	END
END