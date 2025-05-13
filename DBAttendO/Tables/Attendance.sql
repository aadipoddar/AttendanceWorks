CREATE TABLE [dbo].[Attendance]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [ScheduledClassId] INT NOT NULL,
    [StudentId] INT NOT NULL,
    [Present] BIT NOT NULL DEFAULT 0,
    [EntryTime] DATETIME NOT NULL DEFAULT (((getdate() AT TIME ZONE 'UTC') AT TIME ZONE 'India Standard Time')),
    [MarkedBy] INT NULL, -- Teacher ID who marked attendance
    CONSTRAINT [FK_Attendance_ScheduledClass] FOREIGN KEY ([ScheduledClassId]) REFERENCES [ScheduledClass]([Id]),
    CONSTRAINT [FK_Attendance_Student] FOREIGN KEY ([StudentId]) REFERENCES [Student]([Id]),
    CONSTRAINT [FK_Attendance_Teacher] FOREIGN KEY ([MarkedBy]) REFERENCES [Teacher]([Id])
)