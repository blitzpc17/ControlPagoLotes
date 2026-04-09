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
                string errorConexion="";

                while (
                    !ConnectionStorage.HasDefaultConnection(_sqlitePath) ||
                    !ConnectionStorage.CanConnectToDefault(_sqlitePath, out errorConexion)
                )
                {
                    var hayDefault = ConnectionStorage.HasDefaultConnection(_sqlitePath);

                    var mensaje = !hayDefault
                        ? "Debes configurar una conexión principal para continuar."
                        : "No se pudo conectar con la conexión principal actual.\n\n" +
                          $"Detalle: {errorConexion}\n\n" +
                          "Selecciona o configura otra conexión.";

                    MessageBox.Show(
                        mensaje,
                        "Configuración de conexión",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    using (var cfg = new FrmConnections())
                        cfg.ShowDialog();

                    bool hayConexionValida =
                        ConnectionStorage.HasDefaultConnection(_sqlitePath) &&
                        ConnectionStorage.CanConnectToDefault(_sqlitePath, out errorConexion);

                    if (!hayConexionValida)
                    {
                        var salir = MessageBox.Show(
                            "No hay una conexión válida configurada.\n\n¿Deseas salir de la aplicación?",
                            "Conexión requerida",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (salir == DialogResult.Yes)
                        {
                            Close();
                            return;
                        }
                    }
                }

                // 1) Login
                DialogResult loginResult;
                using (var login = new formLogin())
                    loginResult = login.ShowDialog();

                if (loginResult != DialogResult.OK)
                {
                    Close();
                    return;
                }

                // 2) Pantalla principal
                using (var main = new formBusqueda())
                    main.ShowDialog();

                // 3) Si cambiaron conexión, volver al inicio
                if (AppState.MustRestartToLogin)
                {
                    AppState.MustRestartToLogin = false;
                    continue;
                }

                Close();
                return;
            }
        }
    }
}