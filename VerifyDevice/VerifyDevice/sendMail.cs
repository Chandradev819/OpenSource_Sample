using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerifyDevice.Helper
{
    public class sendMail
    {
        // Note: To send email you need to add actual email id and credential so that it will work as expected  
        //public static readonly string EMAIL_SENDER = "mailer@adzgeek.com"; // change it to actual sender email id or get it from UI input  
        //public static readonly string EMAIL_RECEIVER = "srimathi89@gmail.com,socratesponnusamy@yahoo.com";//murali@adzgeek.com,
        //public static readonly string EMAIL_CREDENTIALS = "mailer@adzgeek.com"; // Provide credentials
        //public static readonly string EMAIL_SUBJECT = "P@ssw0rd1!";
        //public static readonly string EMAIL_PWD = "P@ssw0rd1!";// Provide subject   
        //public static readonly string SMTP_CLIENT = "smtp.1and1.com"; // as we are using outlook so we have provided smtp-mail.outlook.com   
        public static readonly string EMAIL_BODY = "Reset your Password <a href='http://{0}.safetychain.com/api/Account/forgotPassword?{1}'>Here.</a>";

        public static readonly string EMAIL_SENDER = System.Configuration.ConfigurationSettings.AppSettings["EmailSender"].ToString();
        public static readonly string EMAIL_RECEIVER = System.Configuration.ConfigurationSettings.AppSettings["EmailReceiver"].ToString();
        public static readonly string EMAIL_CREDENTIALS = System.Configuration.ConfigurationSettings.AppSettings["EmailUserName"].ToString();
        public static readonly string EMAIL_PWD = System.Configuration.ConfigurationSettings.AppSettings["EmailPassword"].ToString();
        public static readonly string SMTP_CLIENT = System.Configuration.ConfigurationSettings.AppSettings["EmailClient"].ToString();

        private string senderAddress;
        private string clientAddress;
        private string netPassword;
        public sendMail(string sender, string Password, string client)
        {
            senderAddress = sender;
            netPassword = Password;
            clientAddress = client;
        }
        public bool SendEMail(string messagetype,string subject, string messagedescription)
        {
            bool isMessageSent = false;
            //Intialise Parameters  
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(SMTP_CLIENT);
            client.Port = 587;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(EMAIL_CREDENTIALS, EMAIL_PWD);
            client.EnableSsl = true;
            client.Credentials = credentials;
            try
            {
                var mail = new System.Net.Mail.MailMessage(EMAIL_SENDER, EMAIL_RECEIVER);
                mail.Subject = messagetype +":"+ subject;
                mail.Body = messagedescription;
                mail.IsBodyHtml = true;
                client.Send(mail);
                isMessageSent = true;
            }
            catch (Exception ex)
            {
                isMessageSent = false;
            }
            return isMessageSent;
        }
    }
}