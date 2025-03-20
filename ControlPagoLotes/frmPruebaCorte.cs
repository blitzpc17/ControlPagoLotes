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
    public partial class frmPruebaCorte: Form
    {
        public frmPruebaCorte()
        {
            InitializeComponent();
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            using (var contexto = new PagoPartidaLogica())
            {
                DateTime fecha = dtpFechaContrato.Value;
                var ListaPagosDiarios = contexto.ListarPagoPorFecha(fecha);

                var montoNuevos = ListaPagosDiarios.Where(x => x.FechaModifico == null && x.FechaElimino == null).ToList();
                    //(ListaPagosDiarios != null && ListaPagosDiarios.Count > 0) ? (ListaPagosDiarios.Where(x => x.UsuarioModifico == null && x.FechaModifico == null && x.FechaElimino == null && x.UsuarioElimino == null).Sum(x => x.Monto)) : 0; ;
                Console.WriteLine(montoNuevos);
                decimal montoModificados = (ListaPagosDiarios != null && ListaPagosDiarios.Count > 0) ? (ListaPagosDiarios.Where(x => (x.FechaElimino == null && x.UsuarioElimino == null) && (x.UsuarioModifico != null && x.FechaModifico != null)).Sum(x => (x.CantidadOriginal - x.Monto))) : 0;
                Console.WriteLine(montoModificados);
                decimal montoEliminados = (ListaPagosDiarios != null && ListaPagosDiarios.Count > 0) ? (ListaPagosDiarios.Where(x => x.UsuarioModifico == null && x.FechaModifico == null && x.FechaElimino != null && x.UsuarioElimino != null).Sum(x => (x.Monto))) : 0;
                Console.WriteLine(montoEliminados);
            }
                
        }
    }
}
