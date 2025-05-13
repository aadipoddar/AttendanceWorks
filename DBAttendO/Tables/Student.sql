CREATE TABLE [dbo].[Student]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(100) NOT NULL, 
    [Roll] INT NOT NULL UNIQUE,
    [Email] VARCHAR(100) NOT NULL UNIQUE, 
    [Phone] VARCHAR(10) NOT NULL UNIQUE, 
    [Password] VARCHAR(10) NOT NULL, 
    [SectionId] INT NOT NULL,
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_Student_ToSection] FOREIGN KEY (SectionId) REFERENCES [Section](Id)
)
