CREATE PROCEDURE [dbo].[Insert_Student]
	@Id INT,
	@Name VARCHAR(100),
	@Roll INT,
	@Email VARCHAR(100),
	@Phone VARCHAR(10),
	@Password VARCHAR(10),
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Student]
			([Name], [Roll], [Email], [Phone], [Password], [Status])
		VALUES
			(@Name, @Roll, @Email, @Phone, @Password, @Status)
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Student]
		SET 
			[Name] = @Name,
			[Roll] = @Roll,
			[Email] = @Email,
			[Phone] = @Phone,
			[Password] = @Password,
			[Status] = @Status
		WHERE [Id] = @Id
	END
END