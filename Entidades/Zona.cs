using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Zona
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public long ConnectionId { get; set; }
        public string Plaza { get; set; }
        public string NombreConPlaza => string.IsNullOrWhiteSpace(Plaza) ? Nombre : $"{Nombre} [{Plaza}]";
        public string UniqueKey => $"{ConnectionId}_{Id}";
    }
}
