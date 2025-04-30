CREATE TABLE [dbo].[ScheduledClass]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [CourseSectionId] INT NOT NULL,
    [ClassDate] DATE NOT NULL,
    [StartTime] TIME(2) NOT NULL,
    [EndTime] TIME(2) NOT NULL,
    [Status] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_ScheduledClass_CourseSection] FOREIGN KEY ([CourseSectionId]) REFERENCES [CourseSection]([Id])
)