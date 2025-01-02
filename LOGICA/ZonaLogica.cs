using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOGICA
{
    public class ZonaLogica
    {
        ZonasRepository contexto;
        public ZonaLogica() {
        
            contexto = new ZonasRepository();
        }

        // Crear Zona
        public int AddZona(Zona zona)
        {
            return contexto.AddZona(zona);
        }

        // Leer Zona
        public Zona GetZonaById(int id)
        {
            return contexto.GetZonaById(id);
        }

        // Actualizar Zona
        public bool UpdateZona(Zona zona)
        {
            return contexto.UpdateZona(zona);
        }

        // Eliminar Zona
        public bool DeleteZona(int id)
        {
            return contexto.DeleteZona(id);
        }

        // Leer Zona
        public List<Zona> GetAllZonas()
        {
            return contexto.GetAllZonas();
        }
    }
}
