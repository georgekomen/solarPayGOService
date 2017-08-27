using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace sunamiapi.codeIncludes
{
    public class sendMail
    {
        public sendMail(string to, string subject, string body)
        {
            /*string from = "sunamisolarkenya@gmail.com";
            string pass = "sunami2017";

            // Command line argument must the the SMTP host.
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(from, pass);

            MailMessage mm = new MailMessage(from, to, subject, body);
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);*/


        }
    }
}