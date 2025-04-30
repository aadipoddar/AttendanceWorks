CREATE TABLE [dbo].[ScheduledClass]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [CourseSectionId] INT NOT NULL,
    [ClassDate] DATE NOT NULL,
    [StartTime] TIME NOT NULL,
    [EndTime] TIME NOT NULL,
    [Status] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_ScheduledClass_CourseSection] FOREIGN KEY ([CourseSectionId]) REFERENCES [CourseSection]([Id]),
    CONSTRAINT [UQ_ScheduledClass] UNIQUE ([CourseSectionId], [ClassDate], [StartTime])
)