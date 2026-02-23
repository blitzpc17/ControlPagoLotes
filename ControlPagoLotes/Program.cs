//using System;
//using System.IO;
//using System.Windows.Forms;

//namespace ControlPagoLotes
//{
//    static class Program
//    {
//        [STAThread]
//        static void Main()
//        {
//            var dir = Path.Combine(
//                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
//                "jaadeproductions");
//            Directory.CreateDirectory(dir);

//            var sqlitePath = Path.Combine(dir, "db_conexiones.sqlite");            

//            // ✅ IMPORTANTÍSIMO: crea tabla antes de consultar
//            ConnectionsDb.EnsureCreated(sqlitePath);

//            Application.EnableVisualStyles();
//            Application.SetCompatibleTextRenderingDefault(false);

//            if (!ConnectionStorage.HasDefaultConnection(sqlitePath))
//            {
//                using (var frm = new FrmConnections())
//                    frm.ShowDialog();

//                if (!ConnectionStorage.HasDefaultConnection(sqlitePath))
//                {
//                    MessageBox.Show(
//                        "Debes configurar una conexión principal para continuar.",
//                        "Conexión requerida",
//                        MessageBoxButtons.OK,
//                        MessageBoxIcon.Warning);
//                    return;
//                }
//            }

//            Application.Run(new formLogin()); // o tu form principal
//        }
//    }
//}

using System;
using System.IO;
using System.Windows.Forms;

namespace ControlPagoLotes
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "jaadeproductions");
            Directory.CreateDirectory(dir);

            var sqlitePath = Path.Combine(dir, "db_conexiones.sqlite");

            // crea tabla antes de consultar
            ConnectionsDb.EnsureCreated(sqlitePath);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // En lugar de correr login directo, corremos un Host
            Application.Run(new AppHost(sqlitePath));
        }
    }
}