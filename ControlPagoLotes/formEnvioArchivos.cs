using Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlPagoLotes
{
    public partial class formEnvioArchivos: Form
    {
        private string rutaArchivo = "";
        private int tipoEnvio = 0;
        private string msjEnvio = "";
        private string destino = "";

        public formEnvioArchivos(string ruta)
        {
            InitializeComponent();
            rutaArchivo = ruta;
        }
        

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(1, $"Proceso iniciado... 1% completado");
            string NombreArchivo = "boleta_"+Global.FechaServidor().ToString("dd-MM-yyyy_HH:mm:ss")+".pdf";
            //subiendo archivo
            if(Global.SubirArchivoFtp(rutaArchivo, "public/recibos/"+NombreArchivo))
            {
                backgroundWorker1.ReportProgress(1, $"Archivo subido... 50% completado");

                switch ((int)e.Argument)
                {
                    case 0: //correo
                        backgroundWorker1.ReportProgress(1, $"Generando y enviando correo... 25% completado");
                        List<string> correoDestino = new List<string>();
                        correoDestino.Add(destino);
                        List<string> archivosAdjuntos = new List<string>();
                        archivosAdjuntos.Add(rutaArchivo);
                        clsCorreo objCorreo = new clsCorreo
                        {
                            Asunto = "Boleta de pago JAADE",
                            Cuerpo = "<html><body>Buen dia:<br>Por medio del presente le hago envio de su boleta de pago solicitada.<br>Saludos.</body></html>",
                            CorreoDestino = correoDestino,
                            PathAttach = archivosAdjuntos 
                        };

                        if (Global.EnviarCorreo(objCorreo))
                        {
                            backgroundWorker1.ReportProgress(1, $"Correo enviado... 100% completado");
                        }
                        else
                        {
                            msjEnvio = "No se pudo enviar el correo, intente nuevamente.";
                        }

                            break;

                    case 1: //whats
                        backgroundWorker1.ReportProgress(1, $"Enviando mensaje... 50% completado");
                        clsWhatsApp objWhats = new clsWhatsApp
                        {
                            TelefonoDestino = destino,
                            Cuerpo = "Hola somos de JAADE! Le enviamos su recibo de pago. ¡Saludos!",
                            PathMediaFile = @"https://jaade.net/recibos/"+NombreArchivo,
                        };

                        if (Global.EnviarWhatsApp(objWhats))
                        {
                            backgroundWorker1.ReportProgress(1, $"WhatsApp enviado... 100% completado");
                        }
                        else {
                            msjEnvio = "Fallo en el servicio de whatsapp, intente nuevamente.";
                        }
                        break;
                }


                Thread.Sleep(1500);


                if (!Global.EliminarArchivoFtp("public/recibos/" + NombreArchivo))
                {
                    msjEnvio = "Se envió la boleta correctamente, pero no se pudo eliminar el archivo generado.";
                }
            }
            else
            {
                msjEnvio = "Ocurrió un error al subir el archivo al servidor remoto.";
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                pEnviando.Image = Properties.Resources.envioarchivo;
                lblProceso.Text = "Operación cancelada.";
            }
            else if (e.Error != null)
            {
                lblProceso.Text = $"Error: {e.Error.Message}";
                pEnviando.Image = Properties.Resources.error;
            } else if (!string.IsNullOrEmpty(msjEnvio))
            {
                lblProceso.Text = msjEnvio;
                pEnviando.Image = Properties.Resources.error;
            }
            else
            {
                pEnviando.Image = Properties.Resources.garrapata;
                lblProceso.Text = "Operación completada.";
            }


            btnCancelarEnvio.Visible = false;
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {          
            btnCancelarEnvio.Visible = true;
            pEnviando.Image = Properties.Resources.envioarchivo;
            tipoEnvio = cbxTipoEnvio.SelectedIndex;
            destino = txtDestino.Text;
            lblProceso.Text = "";

            if(cbxTipoEnvio.SelectedIndex == -1)
            {
                MessageBox.Show("No ha seleccionado el Tipo de envio",
                    "Advertencia", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;

            }

            if (string.IsNullOrEmpty(txtDestino.Text))
            {
                MessageBox.Show("No ha ingresado el destino a enviar.",
                    "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;

            }

            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync(tipoEnvio);
            }
            else
            {
                MessageBox.Show("Hay una operación de envio en proceso... espere a que termine o de clic en el boton de cancelar e intentelo de nuevamente.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

                

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblProceso.Text = e.UserState.ToString();
        }

        private void btnCancelarEnvio_Click(object sender, EventArgs e)
        {
            // Cancela la operación en segundo plano
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
                btnCancelarEnvio.Visible = false;
                lblProceso.Text = "Proceso de envio cancelado correctamente.";
                pEnviando.Image = Properties.Resources.envioarchivo;
            }
        }

        private void cbxTipoEnvio_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtDestino.Clear();

            if (cbxTipoEnvio.SelectedIndex == 1)
            {
                txtDestino.MaxLength = 13;
            }
            else
            {
                txtDestino.MaxLength = 100;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
