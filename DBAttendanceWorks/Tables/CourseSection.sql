CREATE TABLE [dbo].[CourseSection]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [CourseId] INT NOT NULL,
    [SectionId] INT NOT NULL,
    [TeacherId] INT NOT NULL,
    [ClassRoomId] INT NOT NULL,
    [Status] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_CourseSection_Course] FOREIGN KEY ([CourseId]) REFERENCES [Course]([Id]),
    CONSTRAINT [FK_CourseSection_Section] FOREIGN KEY ([SectionId]) REFERENCES [Section]([Id]),
    CONSTRAINT [FK_CourseSection_Teacher] FOREIGN KEY ([TeacherId]) REFERENCES [Teacher]([Id]),
    CONSTRAINT [FK_CourseSection_ClassRoom] FOREIGN KEY ([ClassRoomId]) REFERENCES [ClassRoom]([Id])
)
