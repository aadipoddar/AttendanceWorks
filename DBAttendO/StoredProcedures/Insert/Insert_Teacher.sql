CREATE PROCEDURE [dbo].[Insert_Teacher]
	@Id INT,
	@Name VARCHAR(100),
	@Email VARCHAR(100),
	@Phone VARCHAR(10),
	@Password VARCHAR(10),
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Teacher]
			([Name], [Email], [Phone], [Password], [Status])
		VALUES
			(@Name, @Email, @Phone, @Password, @Status)
	END
	ELSE
	BEGIN
		UPDATE [dbo].[Teacher]
		SET 
			[Name] = @Name,
			[Email] = @Email,
			[Phone] = @Phone,
			[Password] = @Password,
			[Status] = @Status
		WHERE [Id] = @Id
	END
END