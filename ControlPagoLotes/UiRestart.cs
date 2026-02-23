using System;
using System.Linq;
using System.Windows.Forms;

namespace ControlPagoLotes
{
    public static class UiRestart
    {
        public static void GoToLogin(Form currentForm)
        {
            if (currentForm == null) return;

            // Siempre en hilo UI
            if (currentForm.InvokeRequired)
            {
                currentForm.Invoke(new Action(() => GoToLogin(currentForm)));
                return;
            }

            // Ejecuta después de que termine el cierre actual (para modales)
            currentForm.BeginInvoke(new Action(() =>
            {
                // 1) Cierra TODOS los forms abiertos (incluye modales y no modales)
                //    OJO: copiamos a lista para no modificar colección mientras iteramos
                var open = Application.OpenForms.Cast<Form>().ToList();

                foreach (var f in open)
                {
                    try
                    {
                        // Si ya es login, no lo cierres
                        if (f is formLogin) continue;

                        // Cierra
                        f.Close();
                    }
                    catch { /* ignore */ }
                }

                // 2) Si no hay login abierto, abre uno nuevo (no modal)
                var login = Application.OpenForms.Cast<Form>().FirstOrDefault(x => x is formLogin);
                if (login == null)
                {
                    new formLogin().Show();
                }
                else
                {
                    login.Show();
                    login.BringToFront();
                    login.Activate();
                }
            }));
        }
    }
}