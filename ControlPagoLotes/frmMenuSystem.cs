using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ControlPagoLotes
{
    public partial class frmMenuSystem : Form
    {
        public frmMenuSystem()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AplicarPermisos();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (AppState.MustRestartToLogin)
                this.Close();
        }

        private void AplicarPermisos()
        {
            // CONEXIONES: siempre visible
            btnConexiones.Visible = true;

            // Los demás módulos sí dependen del permiso
            bool puedeAdministrar = UsuarioPuedeAdministrarSistema();

            btnUsuarios.Visible = puedeAdministrar;
            btnZonas.Visible = puedeAdministrar;

            ReacomodarBotonesVisibles();
        }

        private bool UsuarioPuedeAdministrarSistema()
        {
            if (Global.ObjUsuario == null || string.IsNullOrWhiteSpace(Global.ObjUsuario.Usuario))
                return false;

            var usuariosPermitidos = new List<string>
            {
                "ADMIN",
                "DIANA",
                "DONATO",
                "EMMANUEL"
            };

            return usuariosPermitidos.Contains(Global.ObjUsuario.Usuario.Trim().ToUpper());
        }

        private void ReacomodarBotonesVisibles()
        {
            var botonesVisibles = new List<Button>();

            if (btnUsuarios.Visible) botonesVisibles.Add(btnUsuarios);
            if (btnZonas.Visible) botonesVisibles.Add(btnZonas);
            if (btnConexiones.Visible) botonesVisibles.Add(btnConexiones);
            if (btnSalir.Visible) botonesVisibles.Add(btnSalir);

            int left = 34;
            int top = 38;
            int width = 154;
            int height = 116;
            int espacio = 6;

            foreach (var btn in botonesVisibles)
            {
                btn.Left = left;
                btn.Top = top;
                btn.Width = width;
                btn.Height = height;

                left += width + espacio;
            }
        }

        private void NuevoUsuario()
        {
            using (var f = new formUsuarios())
                f.ShowDialog();

            if (AppState.MustRestartToLogin)
            {
                this.Close();
                return;
            }
        }

        private void NuevoConfiguracionZona()
        {
            using (var f = new frmRutasConfig())
                f.ShowDialog();

            if (AppState.MustRestartToLogin)
            {
                this.Close();
                return;
            }
        }

        private void NuevoConexionRemota()
        {
            using (var frm = new FrmConnections())
                frm.ShowDialog();

            if (AppState.MustRestartToLogin)
            {
                this.Close();
                return;
            }
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            if (!UsuarioPuedeAdministrarSistema())
            {
                MessageBox.Show(
                    "No tiene permisos para acceder a Usuarios.",
                    "Acceso denegado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            NuevoUsuario();
        }

        private void btnZonas_Click(object sender, EventArgs e)
        {
            if (!UsuarioPuedeAdministrarSistema())
            {
                MessageBox.Show(
                    "No tiene permisos para acceder a Configuración de Zonas.",
                    "Acceso denegado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            NuevoConfiguracionZona();
        }

        private void btnNuevoPago_Click(object sender, EventArgs e)
        {
            // CONEXIONES: sin validación de permisos
            NuevoConexionRemota();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "¿Desea regresar a la búsqueda general de pagos?",
                "Advertencia",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        }
    }
}