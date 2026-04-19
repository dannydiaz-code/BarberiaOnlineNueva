using System.Net;
using System.Net.Mail;

namespace BarberiaOnlineNueva.Services
{
    public class EmailService
    {
        public void EnviarCorreo(string destino, string asunto, string mensaje)
        {
            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("dannydiaza0@gmail.com", "vprbmvmzxsbxgnjs"),
                EnableSsl = true
            };

            var mail = new MailMessage("TU_CORREO@gmail.com", destino, asunto, mensaje);

            smtp.Send(mail);
        }
    }
}
