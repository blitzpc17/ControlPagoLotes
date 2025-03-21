using Entidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ControlPagoLotes
{
    public static class Global
    {
        public static UsuarioL ObjUsuario;

        private static string ftpServer = "195.35.10.145";
        private static string ftpUsername = "u401313086";
        private static string ftpPassword = "@Jaade_inmobiliaria2024";

        private static int smtpPuerto = 587;
        private static string smtpPassword = "@Jaade_inmobiliaria24";
        private static string smtpHostName = "smtp.hostinger.com";
        private static string smtpEmailBase = "soporte@jaade.net";

        //{"AccountSID":"", "AuthToken":"", "TelefonoSalida":"14155238886"}

        private static string AccounSID = "ACf0a1d2dfc06e0739d4a759cb596c535b";
        private static string AuthToken = "327b1b87abab580322e89cd36d96d7a0";
        private static string TelefonoSalida = "14155238886";

        public static void LimpiarControles(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Clear();
                }
                else if (control is ComboBox comboBox)
                {
                    if (comboBox.DataSource != null)
                    {
                        comboBox.DataSource = null;
                    }
                    else
                    {
                        comboBox.Items.Clear();
                    }
                }
                else if (control is DataGridView dataGridView)
                {
                    dataGridView.DataSource = null;
                    dataGridView.Rows.Clear();
                }
                else if (control is GroupBox || control is Panel || control is TabPage || control is TabControl)
                {
                    LimpiarControles(control);
                }
            }
        }


        public static DateTime FechaServidor()
        {
            return DateTime.Now;
        }

        public static decimal FormatearPesosADecimal(string input)
        {
            return decimal.Parse(input, NumberStyles.Number, CultureInfo.InvariantCulture);
        }

        public static string FormatearFecha(string input, string formato)
        {            
            DateTime fecha;

            // Intentar convertir en el formato "dd/MM/yyyy"
            if (DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                // Convertir al formato "yyyy-MM-dd"
                return fecha.ToString(formato);
            }
            else
            {
                return fecha.ToString("yyyy-MM-dd");
            }
        }

        public static bool SubirArchivoFtp(string pathLocal, string pathRemoto)
        {

            // Ruta del archivo local que deseas subir
            string archivoLocal = pathLocal;

            // Ruta en el servidor FTP donde deseas guardar el archivo
            string rutaRemota = pathRemoto;

            // Crear una instancia de FtpWebRequest
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ftpServer}/{rutaRemota}");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

            try
            {
                // Leer el contenido del archivo local
                byte[] archivoBytes = File.ReadAllBytes(archivoLocal);

                // Obtener el flujo de salida y escribir el contenido del archivo
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(archivoBytes, 0, archivoBytes.Length);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool EliminarArchivoFtp(string pathRemoto)
        {

            // Ruta en el servidor FTP del archivo que deseas eliminar
            string rutaRemota = pathRemoto;

            // Crear una instancia de FtpWebRequest
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ftpServer}/{rutaRemota}");
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

            try
            {
                // Enviar la solicitud para eliminar el archivo
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return true;
                }
            }
            catch (WebException ex)
            {
                return false;
            }
        }
        
        public static bool EnviarWhatsApp(clsWhatsApp objWhats)
        {  

            TwilioClient.Init(AccounSID, AuthToken);

            // Número de teléfono del destinatario en formato internacional (por ejemplo, +1234567890)
            //string recipientPhoneNumber = "whatsapp:+5212381458680";
            string recipientPhoneNumber = "whatsapp:+521" + objWhats.TelefonoDestino;

            try
            {
                var message = MessageResource.Create(
                    body: objWhats.Cuerpo,
                    from: new PhoneNumber("whatsapp:+" + TelefonoSalida),
                    to: new PhoneNumber(recipientPhoneNumber),
                    mediaUrl: new List<Uri> { new Uri(objWhats.PathMediaFile) }
                );


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool EnviarCorreo( clsCorreo objCorreo)
        {
           
            // Configurar la información del correo electrónico y el servidor SMTP
            List<string> toAddress = objCorreo.CorreoDestino;
            string subject = objCorreo.Asunto;
            string body = objCorreo.Cuerpo;

            // Ruta del archivo PDF que deseas adjuntar
            List<string> pdfFilePath = objCorreo.PathAttach; //@"C:\Ruta\Al\Archivo.pdf";

            // Crear el cliente SMTP y configurar las credenciales
            using (SmtpClient smtpClient = new SmtpClient(smtpHostName, smtpPuerto))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(smtpEmailBase, smtpPassword);
                smtpClient.EnableSsl = true;

                // Crear el mensaje de correo electrónico
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(smtpEmailBase);
                    foreach (var item in toAddress)
                    {
                        mailMessage.To.Add(item);
                    }
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    // Adjuntar el archivo PDF
                    foreach (var file in pdfFilePath)
                    {
                        Attachment pdfAttachment = new Attachment(file);
                        mailMessage.Attachments.Add(pdfAttachment);
                    }

                    try
                    {
                        // Enviar el correo electrónico
                        smtpClient.Send(mailMessage);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
        }


        



    }
}
