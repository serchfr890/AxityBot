using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Text;
using System.Net;

namespace CoreBot.Utils
{
    public class SendMailToSigma
    {
        public static void sendMail(string name, string email, string phone, string comments)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add("sergio.flores@axity.com");
            mail.Subject = "Nuevo cliente interezado en los productos de Sigma";
            mail.SubjectEncoding = Encoding.UTF8;

            string msg = $"Hola Tannia, <br><br> Un nuevo cliente esta interezado en nuestros productos. Te dejo la info para que" +
                $"te puedas comunicar con él. <br><br>" +
                $"<br> Nombre: {name}" +
                $"<br> Email: {email}" +
                $"<br> Teléfono: {phone}" +
                $"<br> Comentarios: {comments}" +
                $"<br><br> Hasta pronto =) . <br>Saludos!";

            mail.Body = msg;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;

            mail.From = new MailAddress("serchfr890123@outlook.com");
            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential("serchfr890123@outlook.com", "S1E2R3g4i5o6123");
            client.Port = 587;
            client.EnableSsl = true;
            client.Host = "smtp.outlook.com";

            try
            {
                client.Send(mail);
            }
            catch(Exception e)
            {
                throw new ArgumentException("Hubo un error con la conxion: " + e.StackTrace);
            }
        }
    }
}
