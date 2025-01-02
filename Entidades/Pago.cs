using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Pago
    {
        public int Id { get; set; }
        public string NombreCliente { get; set; }
        public decimal Total { get; set; }
        public string Meses { get; set; }
        public int ZonaId { get; set; }
        public string DiaPago { get; set; }
        public string Lotes { get; set; }
        public DateTime FechaRegistro { get; set; } 
    }
}
