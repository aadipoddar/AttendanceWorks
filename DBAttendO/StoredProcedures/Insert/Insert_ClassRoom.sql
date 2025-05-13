CREATE PROCEDURE [dbo].[Insert_ClassRoom]
	@Id INT,
	@Name VARCHAR(10),
	@Latitude DECIMAL(10, 5),
	@Longitude DECIMAL(10, 5),
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[ClassRoom]
			([Name], [Latitude], [Longitude], [Status])
		VALUES
			(@Name, @Latitude, @Longitude, @Status)
	END
	ELSE
	BEGIN
		UPDATE [dbo].[ClassRoom]
		SET 
			[Name] = @Name,
			[Latitude] = @Latitude,
			[Longitude] = @Longitude,
			[Status] = @Status
		WHERE [Id] = @Id
	END
END