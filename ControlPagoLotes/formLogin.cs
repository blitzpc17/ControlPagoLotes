using LOGICA;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlPagoLotes
{
    public partial class formLogin : Form
    {
        private LoginLogica contexto;

        public formLogin()
        {
            InitializeComponent();
        }

        private void InicializarModulo()
        {
            Global.LimpiarControles(this);
            contexto = new LoginLogica();
        }

        private void formLogin_Load(object sender, EventArgs e)
        {
            InicializarModulo();
        }

        private async void btnAcceder_Click(object sender, EventArgs e)
        {
            await IniciarSecionAsync();
        }

        private async Task IniciarSecionAsync()
        {
            try
            {
                btnAcceder.Enabled = false;
                string originalText = btnAcceder.Text;
                btnAcceder.Text = "Validando...";

                var offlineConnections = await contexto.ValidarConexionesAsync();
                if (offlineConnections.Count > 0)
                {
                    string msj = "Las siguientes sucursales no están disponibles en este momento:\n\n- " 
                                 + string.Join("\n- ", offlineConnections) 
                                 + "\n\nEl sistema omitirá estas conexiones durante esta sesión.";
                    MessageBox.Show(msj, "Aviso de Conexiones", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                btnAcceder.Text = originalText;
                btnAcceder.Enabled = true;

                Global.ObjUsuario = contexto.ValidarAcceso(txtUsuario.Text, txtPassword.Text);

                if (Global.ObjUsuario == null)
                {
                    MessageBox.Show(
                        "No se pudo iniciar sesión, verifique su información.",
                        "Advertencia",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    // Opcional: focus al usuario
                    txtUsuario.Focus();
                    return;
                }

                MessageBox.Show(
                    "¡Hola " + Global.ObjUsuario.Usuario + "!",
                    "Bienvenido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // ✅ IMPORTANTE: el Host abrirá el siguiente form
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error de inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            CerrarModulo();
        }

        private void CerrarModulo()
        {
            // ✅ para que el Host sepa que se canceló
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}