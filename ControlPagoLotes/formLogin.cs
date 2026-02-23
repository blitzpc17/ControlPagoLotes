using LOGICA;
using System;
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

        private void btnAcceder_Click(object sender, EventArgs e)
        {
            IniciarSecion();
        }

        private void IniciarSecion()
        {
            try
            {
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