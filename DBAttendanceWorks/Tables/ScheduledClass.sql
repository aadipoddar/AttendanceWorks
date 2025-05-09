CREATE TABLE [dbo].[ScheduledClass]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [SectionId] INT NOT NULL,
    [CourseId] INT NOT NULL,
    [TeacherId] INT NOT NULL,
    [ClassRoomId] INT NOT NULL,
    [ClassDate] DATE NOT NULL,
    [StartTime] TIME(2) NOT NULL,
    [EndTime] TIME(2) NOT NULL,
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_ScheduledClass_ToSection] FOREIGN KEY (SectionId) REFERENCES [Section](Id), 
    CONSTRAINT [FK_ScheduledClass_ToTeacher] FOREIGN KEY (TeacherId) REFERENCES Teacher(Id),
    CONSTRAINT [FK_ScheduledClass_ToCourse] FOREIGN KEY (CourseId) REFERENCES Course(Id),
    CONSTRAINT [FK_ScheduledClass_ToClassRoom] FOREIGN KEY (ClassRoomId) REFERENCES ClassRoom(Id),
)