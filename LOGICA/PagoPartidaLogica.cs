using DAO.ADOS;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOGICA
{
    public class PagoPartidaLogica:IDisposable
    {
        PagoPartidasRepository contexto;
        ZonasRepository contextoZonas;

        public List<Zona> LstZona;
        public PeriodoConsulta objConsulta;

        public PagoPartidaLogica()
        {

            contexto = new PagoPartidasRepository();
            contextoZonas = new ZonasRepository();
        }

        public void InicializarObjConsulta()
        {
            objConsulta = new PeriodoConsulta();
        }

        // Crear PagoPartida
        public int AddPagoPartida(PagoPartida PagoPartida)
        {
            return contexto.AddPagoPartida(PagoPartida);
        }

        // Leer PagoPartida
        public PagoPartida GetPagoPartidaById(int id)
        {
            return contexto.GetPagoPartidaById(id);
        }

        // Actualizar PagoPartida
        public bool UpdatePagoPartida(PagoPartida PagoPartida)
        {
            return contexto.UpdatePagoPartida(PagoPartida);
        }

        // Eliminar PagoPartida
        public bool DeletePagoPartida(int id)
        {
            return contexto.DeletePagoPartida(id);
        }

        // Leer PagoPartida
        public List<PagoPartida> GetAllPagoPartidas(int idPago)
        {
            return contexto.GetAllPAGOSPARTIDAS(idPago);
        }

        public int EliminarPartidasAnteriores(int id)
        {
            return contexto.EliminarPartidasAnteriores(id);
        }

        public int InsertarPartidasPago(string query)
        {
            return contexto.InsertarPartidasPago(query);
        }
        public List<clsDATACORTE> ListarPagoPorFecha(PeriodoConsulta obj, int usuarioId)
        {
            return contexto.ListarPagoPorFecha(obj, usuarioId);
        }

        public void Dispose()
        {
            contexto.Dispose();
        }

        public List<PagoPartida> ListarPartidasPagos(string idsRelacionados)
        {
            return contexto.ListarPartidasPagos(idsRelacionados);
        }

        public void ListarLotificaciones()
        {
            LstZona = contextoZonas.GetAllZonas();
        }
    }
}
