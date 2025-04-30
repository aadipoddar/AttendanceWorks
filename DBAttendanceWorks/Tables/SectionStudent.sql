CREATE TABLE [dbo].[SectionStudent]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SectionId] INT NOT NULL, 
    [StudentId] INT NOT NULL, 
    [Status] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_SectionStudentDetails_ToSection] FOREIGN KEY (SectionId) REFERENCES [Section](Id), 
    CONSTRAINT [FK_SectionStudentDetails_ToStudent] FOREIGN KEY (StudentId) REFERENCES [Student](Id),
    CONSTRAINT [UQ_SectionStudent] UNIQUE ([SectionId], [StudentId])
)
