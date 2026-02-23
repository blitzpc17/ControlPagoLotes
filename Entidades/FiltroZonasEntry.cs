using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class FiltroZonasEntry
    {
        public int UsuarioId { get; set; }
        public List<int> ZonasId { get; set; } = new List<int>();
    }
}
