using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class PagoPartida
    {
        public int Id { get; set; }
        public int PagoId { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? FormaPago { get; set; }
        public decimal? MontoOriginal { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioModificoId { get; set; }
        public DateTime? FechaBaja { get; set; }
        public int? UsuarioBajaId { get; set; }

    }
}
