using LOGICA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

        private void btnAcceder_Click(object sender, EventArgs e)
        {
            IniciarSecion();
        }

        private void IniciarSecion()
        {
            Global.ObjUsuario = contexto.ValidarAcceso(txtUsuario.Text, txtPassword.Text);

            if(Global.ObjUsuario == null)
            {
                MessageBox.Show("No se pudo iniciar sesión, verifique su información.", "Adveretencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("¡Hola "+Global.ObjUsuario.Usuario+"!", "Bienvenido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                formBusqueda bus = new formBusqueda();
                Hide();
                bus.ShowDialog();
                InicializarModulo();
                Show();
            }          

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            CerrarModulo();
        }

        private void CerrarModulo()
        {
            Close();
        }

        private void formLogin_Load(object sender, EventArgs e)
        {
            InicializarModulo();
        }
    }
}
