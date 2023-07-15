using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MordenDoors
{
    public partial class MailTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           string UserID = ConfigurationManager.AppSettings.Get("UserID");
           string Password = ConfigurationManager.AppSettings.Get("Password");
           string SMTPPort = ConfigurationManager.AppSettings.Get("SMTPPort");
           string Host = ConfigurationManager.AppSettings.Get("Host");
           string IsSSL = ConfigurationManager.AppSettings.Get("EnableSsl");

            MailMessage mail = new MailMessage();
            mail.To.Add("tajinder@revclerx.com");
            mail.From = new MailAddress(UserID);
            mail.Subject = "Test Email";
            mail.Body = "I am testing mail";
            SmtpClient smtp = new SmtpClient();
            smtp.Host = Host;
            smtp.Port = Convert.ToInt16(SMTPPort);
            smtp.Credentials = new NetworkCredential(UserID,Password);
            smtp.EnableSsl = Convert.ToBoolean(IsSSL);
            smtp.Send(mail);
        }
    }
}