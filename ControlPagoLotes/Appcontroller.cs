using System;
using System.Linq;
using System.Windows.Forms;

namespace ControlPagoLotes
{
    public static class AppController
    {
        public static void RestartToLogin(Form currentForm, Func<Form> loginFactory)
        {
            // Ejecuta en el hilo UI
            if (currentForm.InvokeRequired)
            {
                currentForm.Invoke(new Action(() => RestartToLogin(currentForm, loginFactory)));
                return;
            }

            // Cierra todos los forms excepto el actual (para no romper el enumerador)
            var openForms = Application.OpenForms.Cast<Form>().ToList();
            foreach (var f in openForms)
            {
                if (f != null && f != currentForm)
                    f.Close();
            }

            // Oculta y cierra el actual al final
            currentForm.Hide();

            // Abre login
            var login = loginFactory();
            login.Show();

            // Ya que el login está abierto, cerramos el form actual
            currentForm.Close();
        }
    }
}
