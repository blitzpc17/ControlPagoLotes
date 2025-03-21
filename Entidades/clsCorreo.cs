using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class clsCorreo
    {   
        public string Asunto { get; set; }
        public string Cuerpo { get; set; }
        public List<string> CorreoDestino { get; set; }
        public List<string> PathAttach { get; set; }
    }
}
