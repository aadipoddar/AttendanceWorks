using System.Net;
using System.Net.Mail;

namespace AttendanceWorksLibrary.Utilities;

public static class Mailing
{
	public static void MailPassword(string studentEmail, string password)
	{
		string fromMail = Secrets.EmailId;
		string fromPassword = Secrets.EmailPassword;

		MailMessage message = new()
		{
			From = new MailAddress(fromMail),
			Subject = "Your Password Request",
			Body = $"<html><body> Your Password is - {password} Please do not share it with anyone <html><body>",
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
}
