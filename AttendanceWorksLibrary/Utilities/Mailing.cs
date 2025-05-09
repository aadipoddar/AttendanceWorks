using System.Net;
using System.Net.Mail;

namespace AttendanceWorksLibrary.Utilities;

public static class Mailing
{
	public static void ForgotPasswordEmail(string studentEmail, string password)
	{
		string fromMail = Secrets.EmailId;
		string fromPassword = Secrets.EmailPassword;

		var subject = "Password Recovery Information";

		var body = $@"
			<html>
			<body style='font-family: Arial, sans-serif; line-height: 1.6;'>
				<div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 5px;'>
					<h2 style='color: #3a3a3a; border-bottom: 1px solid #eaeaea; padding-bottom: 10px;'>Password Recovery</h2>
					<p>Dear Student,</p>
					<p>We have received a request to recover your password for the Attendance Management System.</p>
					
					<div style='background-color: #f8f8f8; border-left: 4px solid #4285f4; padding: 15px; margin: 20px 0;'>
						<p style='margin: 0;'><strong>Your Password:</strong> <span style='font-family: Consolas, monospace; background-color: #eaeaea; padding: 3px 6px; border-radius: 3px;'>{password}</span></p>
					</div>
					
					<table style='width: 100%; border-collapse: collapse; margin-top: 15px; background-color: #fff9e6; border: 1px solid #ffe0b2;'>
						<tr>
							<td style='padding: 10px; border: 1px solid #ffe0b2;'>
								<p style='color: #e65100; margin: 0;'><strong>⚠️ Security Notice</strong></p>
								<ul style='margin: 10px 0 0 0; padding-left: 20px;'>
									<li>For security reasons, we recommend changing your password after logging in.</li>
									<li>Never share your password with anyone.</li>
									<li>Ensure you're using a secure connection when accessing your account.</li>
								</ul>
							</td>
						</tr>
					</table>
					
					<p style='margin-top: 20px;'>If you did not request this password recovery, please contact the administrator immediately.</p>
					
					<div style='margin-top: 30px; padding-top: 15px; border-top: 1px solid #eaeaea; color: #777777; font-size: 0.9em;'>
						<p>This is an automated message from the Attendance Management System. Please do not reply to this email.</p>
					</div>
				</div>
			</body>
			</html>";

		MailMessage message = new()
		{
			From = new MailAddress(fromMail),
			Subject = subject,
			Body = body,
			IsBodyHtml = true
		};

		message.To.Add(studentEmail);

		var smtpClient = new SmtpClient("smtp.gmail.com", 587)
		{
			Credentials = new NetworkCredential(fromMail, fromPassword),
			EnableSsl = true
		};

		smtpClient.Send(message);
	}

	public static async Task StudentClassScheduleEmail(StudentModel student, CourseModel course, ClassRoomModel classroom, DateOnly classDate, TimeOnly startTime, TimeOnly endTime)
	{
		if (string.IsNullOrEmpty(student.Email))
			return;

		string fromMail = Secrets.EmailId;
		string fromPassword = Secrets.EmailPassword;

		var subject = $"New Class Scheduled: {course.Name}";

		var body = $@"
			<html>
			<body style='font-family: Arial, sans-serif; line-height: 1.6;'>
				<div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
					<h2>Class Schedule Notification</h2>
					<p>Dear {student.Name},</p>
					<p>This is to inform you that a new class has been scheduled:</p>
					<table style='width: 100%; border-collapse: collapse; margin-top: 15px;'>
						<tr>
							<td style='padding: 8px; border: 1px solid #ddd;'><strong>Course</strong></td>
							<td style='padding: 8px; border: 1px solid #ddd;'>{course.Name} ({course.Code})</td>
						</tr>
						<tr>
							<td style='padding: 8px; border: 1px solid #ddd;'><strong>Date</strong></td>
							<td style='padding: 8px; border: 1px solid #ddd;'>{classDate:dddd, MMMM d, yyyy}</td>
						</tr>
						<tr>
							<td style='padding: 8px; border: 1px solid #ddd;'><strong>Time</strong></td>
							<td style='padding: 8px; border: 1px solid #ddd;'>{startTime:hh:mm tt} - {endTime:hh:mm tt}</td>
						</tr>
						<tr>
							<td style='padding: 8px; border: 1px solid #ddd;'><strong>Classroom</strong></td>
							<td style='padding: 8px; border: 1px solid #ddd;'>{classroom.Name}</td>
						</tr>
					</table>
					<p style='margin-top: 20px;'>Please be present on time. Attendance will be marked.</p>
					<p>Thank you.</p>
				</div>
			</body>
			</html>";

		MailMessage message = new()
		{
			From = new MailAddress(fromMail),
			Subject = subject,
			Body = body,
			IsBodyHtml = true
		};

		message.To.Add(student.Email);

		var smtpClient = new SmtpClient("smtp.gmail.com", 587)
		{
			Credentials = new NetworkCredential(fromMail, fromPassword),
			EnableSsl = true
		};

		await Task.Run(() => smtpClient.Send(message));
	}

	public static async Task TeacherClassScheduleEmail(TeacherModel teacher, CourseModel course, ClassRoomModel classroom, SectionModel section, DateOnly classDate, TimeOnly startTime, TimeOnly endTime)
	{
		if (string.IsNullOrEmpty(teacher.Email))
			return;

		string fromMail = Secrets.EmailId;
		string fromPassword = Secrets.EmailPassword;

		var subject = $"You've Been Assigned to Teach: {course.Name}";

		var body = $@"
			<html>
			<body style='font-family: Arial, sans-serif; line-height: 1.6;'>
			    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
			        <h2>Class Assignment Notification</h2>
			        <p>Dear {teacher.Name},</p>
			        <p>You have been assigned to teach the following class:</p>
			        <table style='width: 100%; border-collapse: collapse; margin-top: 15px;'>
			            <tr>
			                <td style='padding: 8px; border: 1px solid #ddd;'><strong>Course</strong></td>
			                <td style='padding: 8px; border: 1px solid #ddd;'>{course.Name} ({course.Code})</td>
			            </tr>
			            <tr>
			                <td style='padding: 8px; border: 1px solid #ddd;'><strong>Section</strong></td>
			                <td style='padding: 8px; border: 1px solid #ddd;'>{section.Name}</td>
			            </tr>
			            <tr>
			                <td style='padding: 8px; border: 1px solid #ddd;'><strong>Date</strong></td>
			                <td style='padding: 8px; border: 1px solid #ddd;'>{classDate:dddd, MMMM d, yyyy}</td>
			            </tr>
			            <tr>
			                <td style='padding: 8px; border: 1px solid #ddd;'><strong>Time</strong></td>
			                <td style='padding: 8px; border: 1px solid #ddd;'>{startTime:hh:mm tt} - {endTime:hh:mm tt}</td>
			            </tr>
			            <tr>
			                <td style='padding: 8px; border: 1px solid #ddd;'><strong>Classroom</strong></td>
			                <td style='padding: 8px; border: 1px solid #ddd;'>{classroom.Name}</td>
			            </tr>
			        </table>
			        <p style='margin-top: 20px;'>Please be prepared and arrive on time to mark student attendance.</p>
			        <p>Thank you for your dedication to teaching.</p>
			    </div>
			</body>
			</html>";

		MailMessage message = new()
		{
			From = new MailAddress(fromMail),
			Subject = subject,
			Body = body,
			IsBodyHtml = true
		};

		message.To.Add(teacher.Email);

		var smtpClient = new SmtpClient("smtp.gmail.com", 587)
		{
			Credentials = new NetworkCredential(fromMail, fromPassword),
			EnableSsl = true
		};

		await Task.Run(() => smtpClient.Send(message));
	}

	public static async Task ClassStatusChangeEmail(StudentModel student, CourseModel course, ClassRoomModel classroom, DateOnly classDate, TimeOnly startTime, TimeOnly endTime, bool isActive)
	{
		if (string.IsNullOrEmpty(student.Email))
			return;

		string fromMail = Secrets.EmailId;
		string fromPassword = Secrets.EmailPassword;

		string statusText = isActive ? "Activated" : "Cancelled";
		string statusColor = isActive ? "#28a745" : "#dc3545";

		var subject = $"Class {statusText}: {course.Name}";

		var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: {statusColor};'>Class Status Change Notification</h2>
                    <p>Dear {student.Name},</p>
                    <p>This is to inform you that the following class has been <strong style='color: {statusColor};'>{statusText.ToLower()}</strong>:</p>
                    <table style='width: 100%; border-collapse: collapse; margin-top: 15px;'>
                        <tr>
                            <td style='padding: 8px; border: 1px solid #ddd;'><strong>Course</strong></td>
                            <td style='padding: 8px; border: 1px solid #ddd;'>{course.Name} ({course.Code})</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px; border: 1px solid #ddd;'><strong>Date</strong></td>
                            <td style='padding: 8px; border: 1px solid #ddd;'>{classDate:dddd, MMMM d, yyyy}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px; border: 1px solid #ddd;'><strong>Time</strong></td>
                            <td style='padding: 8px; border: 1px solid #ddd;'>{startTime:hh:mm tt} - {endTime:hh:mm tt}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px; border: 1px solid #ddd;'><strong>Classroom</strong></td>
                            <td style='padding: 8px; border: 1px solid #ddd;'>{classroom.Name}</td>
                        </tr>
                    </table>
                    {(isActive ?
					  "<p style='margin-top: 20px;'>Please be present on time. Attendance will be marked.</p>" :
					  "<p style='margin-top: 20px;'>Please note that this class will not be held as scheduled. We apologize for any inconvenience.</p>")}
                    <p>Thank you.</p>
                </div>
            </body>
            </html>";

		MailMessage message = new()
		{
			From = new MailAddress(fromMail),
			Subject = subject,
			Body = body,
			IsBodyHtml = true
		};

		message.To.Add(student.Email);

		var smtpClient = new SmtpClient("smtp.gmail.com", 587)
		{
			Credentials = new NetworkCredential(fromMail, fromPassword),
			EnableSsl = true
		};

		await Task.Run(() => smtpClient.Send(message));
	}

	public static async Task TeacherClassStatusChangeEmail(TeacherModel teacher, CourseModel course, ClassRoomModel classroom, SectionModel section, DateOnly classDate, TimeOnly startTime, TimeOnly endTime, bool isActive)
	{
		if (string.IsNullOrEmpty(teacher.Email))
			return;

		string fromMail = Secrets.EmailId;
		string fromPassword = Secrets.EmailPassword;

		string statusText = isActive ? "Activated" : "Cancelled";
		string statusColor = isActive ? "#28a745" : "#dc3545";

		var subject = $"Teaching Assignment {statusText}: {course.Name}";

		var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: {statusColor};'>Class Status Change Notification</h2>
                    <p>Dear {teacher.Name},</p>
                    <p>This is to inform you that your teaching assignment has been <strong style='color: {statusColor};'>{statusText.ToLower()}</strong>:</p>
                    <table style='width: 100%; border-collapse: collapse; margin-top: 15px;'>
                        <tr>
                            <td style='padding: 8px; border: 1px solid #ddd;'><strong>Course</strong></td>
                            <td style='padding: 8px; border: 1px solid #ddd;'>{course.Name} ({course.Code})</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px; border: 1px solid #ddd;'><strong>Section</strong></td>
                            <td style='padding: 8px; border: 1px solid #ddd;'>{section.Name}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px; border: 1px solid #ddd;'><strong>Date</strong></td>
                            <td style='padding: 8px; border: 1px solid #ddd;'>{classDate:dddd, MMMM d, yyyy}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px; border: 1px solid #ddd;'><strong>Time</strong></td>
                            <td style='padding: 8px; border: 1px solid #ddd;'>{startTime:hh:mm tt} - {endTime:hh:mm tt}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px; border: 1px solid #ddd;'><strong>Classroom</strong></td>
                            <td style='padding: 8px; border: 1px solid #ddd;'>{classroom.Name}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px; border: 1px solid #ddd;'><strong>Status</strong></td>
                            <td style='padding: 8px; border: 1px solid #ddd; color: {statusColor};'><strong>{statusText}</strong></td>
                        </tr>
                    </table>
                    {(isActive ?
					  "<p style='margin-top: 20px;'>Please be prepared to conduct this class as scheduled and mark student attendance.</p>" :
					  "<p style='margin-top: 20px;'>You are no longer required to conduct this class. We apologize for any inconvenience.</p>")}
                    <p>Thank you for your understanding.</p>
                </div>
            </body>
            </html>";

		MailMessage message = new()
		{
			From = new MailAddress(fromMail),
			Subject = subject,
			Body = body,
			IsBodyHtml = true
		};

		message.To.Add(teacher.Email);

		var smtpClient = new SmtpClient("smtp.gmail.com", 587)
		{
			Credentials = new NetworkCredential(fromMail, fromPassword),
			EnableSsl = true
		};

		await Task.Run(() => smtpClient.Send(message));
	}
}
