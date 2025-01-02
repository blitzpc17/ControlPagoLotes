using DAO.ADOS;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOGICA
{
    public class PagoLogica
    {
        PagosRepository contexto;
        public PagoLogica()
        {

            contexto = new PagosRepository();
        }

        // Crear Pago
        public int AddPago(Pago Pago)
        {
            return contexto.AddPagos(Pago);
        }

        // Leer Pago
        public Pago GetPagoById(int id)
        {
            return contexto.GetPagosById(id);
        }

        // Actualizar Pago
        public bool UpdatePago(Pago Pago)
        {
            return contexto.UpdatePagos(Pago);
        }

        // Eliminar Pago
        public bool DeletePago(int id)
        {
            return contexto.DeletePagos(id);
        }

        // Leer Pago
        public List<Pago> GetAllPagos()
        {
            return contexto.GetAllPagos();
        }
    }
}
