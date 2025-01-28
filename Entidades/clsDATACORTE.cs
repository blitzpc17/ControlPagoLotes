using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class clsDATACORTE
    {
        public string NombreCliente { get; set; }
        public string Zona {  get; set; }
        public string Lotes { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string UsuarioRecibe {  get; set; }

    }
}
