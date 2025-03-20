using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class clsCELDASPAGOS
    {
        public int? Id {  get; set; }
        public string Monto {  get; set; }
        public string Fecha { get; set; }
        public int? FormaPago { get; set; }
        public bool? Modificar { get; set; }
        public bool? Eliminar { get; set; }

    }
}
