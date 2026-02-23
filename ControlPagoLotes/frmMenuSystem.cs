using System;
using System.Windows.Forms;

namespace ControlPagoLotes
{
    public partial class frmMenuSystem : Form
    {
        public frmMenuSystem()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            // ✅ si se pidió reinicio, este modal debe cerrarse
            if (AppState.MustRestartToLogin)
                this.Close();
        }

        private void NuevoUsuario()
        {
            using (var f = new formUsuarios())
                f.ShowDialog();

            if (AppState.MustRestartToLogin) { this.Close(); return; }
        }

        private void NuevoConfiguracionZona()
        {
            using (var f = new formUsuarios())
                f.ShowDialog();

            if (AppState.MustRestartToLogin) { this.Close(); return; }
        }

        private void NuevoConexionRemota()
        {
            using (var frm = new FrmConnections())
                frm.ShowDialog();

            // ✅ si cambió conexión, cerrar este menú para que suba al padre
            if (AppState.MustRestartToLogin)
            {
                this.Close();
                return;
            }
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            NuevoUsuario();
        }

        private void btnZonas_Click(object sender, EventArgs e)
        {
            NuevoConfiguracionZona();
        }

        private void btnNuevoPago_Click(object sender, EventArgs e)
        {
            NuevoConexionRemota();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Desea regresar a la búsqueda general de pagos?",
                "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        }
    }
}