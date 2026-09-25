using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class PeriodoConsulta
    {
        public string Tipo { get; set; } // "dia", "semana", "mes"
        public DateTime? Fecha { get; set; }
        public int? NumeroSemana { get; set; }
        public int? Anio { get; set; }
        public int? Mes { get; set; }
        public bool todas { get;set; }
        public int? LotificacionId { get; set; }
        public List<int> LotificacionesIds { get; set; }
        public bool todosUsuarios { get; set; } = true;
        public List<string> UsuariosNombres { get; set; }
        public long? ConexionPrincipalId { get; set; }
    }
}
