using System.Net.Mail;

namespace iUtils
{
  public class iMail
  {
    const string MailServerName = "ms-exch1.kromestudios.com";

    public static void TestMail()
    {
      SendMail("iaincartwright@kromestudios.com", "transformers@kromestudios.com", "Test Report", "This is tyhe body of the test report mail");
    }

    public static void SendMail(string a_from, string a_to, string a_subject, string a_body)
    {
      using (var message = new MailMessage(a_from, a_to, a_subject, a_body))
      {
	      var mailClient = new SmtpClient(MailServerName) {UseDefaultCredentials = true};
				
	      mailClient.Send(message);
      }
    }
  }
}