namespace AttendanceWorksLibrary.DataAccess;

public static class TableNames
{
	public static string Student => "Student";
	public static string Teacher => "Teacher";
	public static string ClassRoom => "ClassRoom";
	public static string Course => "Course";
	public static string Section => "Section";
	public static string CourseSection => "CourseSection";
	public static string ScheduledClass => "ScheduledClass";
	public static string Attendance => "Attendance";
}

public static class StoredProcedureNames
{
	public static string LoadTableData => "Load_TableData";
	public static string LoadTableDataById => "Load_TableData_By_Id";
	public static string LoadTableDataByStatus => "Load_TableData_By_Status";

	public static string LoadTeacherByEmailPassword => "Load_Teacher_By_Email_Password";
	public static string LoadStudentByEmail => "Load_Student_By_Email";
	public static string LoadStudentByRoll => "Load_Student_By_Roll";
	public static string LoadStudentBySection => "Load_Student_By_Section";
	public static string LoadStudentAttendance => "Load_Student_Attendance";

	public static string LoadAttendanceByScheduledClass => "Load_Attendance_By_ScheduledClass";

	public static string InsertStudent => "Insert_Student";
	public static string InsertTeacher => "Insert_Teacher";
	public static string InsertClassRoom => "Insert_ClassRoom";
	public static string InsertCourse => "Insert_Course";
	public static string InsertSection => "Insert_Section";
	public static string InsertCourseSection => "Insert_CourseSection";
	public static string InsertScheduledClass => "Insert_ScheduledClass";
	public static string InsertAttendance => "Insert_Attendance";

	public static string DeleteAttendanceByScheduledClass => "Delete_Attendance_By_ScheduledClass";
}

public static class ViewNames
{
	public static string ViewActiveClasses => "View_ActiveClasses";
	public static string ViewAttendanceDetails => "View_AttendanceDetails";
}