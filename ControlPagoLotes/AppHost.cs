using System;
using System.Windows.Forms;

namespace ControlPagoLotes
{
    public class AppHost : Form
    {
        private readonly string _sqlitePath;

        public AppHost(string sqlitePath)
        {
            _sqlitePath = sqlitePath;

            // Host invisible
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            Opacity = 0;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RunFlow();
        }

        private void RunFlow()
        {
            while (true)
            {
                // 0) Si no hay conexión principal, obligar a configurar
                if (!ConnectionStorage.HasDefaultConnection(_sqlitePath))
                {
                    using (var cfg = new FrmConnections())
                        cfg.ShowDialog();

                    if (!ConnectionStorage.HasDefaultConnection(_sqlitePath))
                    {
                        MessageBox.Show(
                            "Debes configurar una conexión principal para continuar.",
                            "Conexión requerida",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        Close();
                        return;
                    }
                }

                // 1) Login (MODAL)
                DialogResult loginResult;
                using (var login = new formLogin())
                    loginResult = login.ShowDialog();

                // Si el usuario cerró login => salir
                if (loginResult != DialogResult.OK)
                {
                    Close();
                    return;
                }

                // 2) Principal (MODAL) - tu pantalla: formBusqueda
                using (var main = new formBusqueda())
                    main.ShowDialog();

                // 3) Si alguien cambió conexión, regresamos al login
                if (AppState.MustRestartToLogin)
                {
                    AppState.MustRestartToLogin = false;
                    continue; // vuelve al inicio del while => login
                }

                // Si no hay restart, aquí decides si sales o vuelves a login.
                // Yo cierro la app cuando se cierre formBusqueda.
                Close();
                return;
            }
        }
    }
}