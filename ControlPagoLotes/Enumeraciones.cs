using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlPagoLotes
{
    public static class Enumeraciones
    {

        public enum Estados
        {
            CORRIENTE = 1,
            PAGADO = 2,
            CANCELADO =3,
            ATRASADO = 4,
            DESCONOCIDO = 5

        }
        public enum FormaPago
        {
            MIGRADO = 0,
            EFECTIVO = 1,
            TRANSFERENCIA = 2,
        }

        public enum Periodo
        {
            DIA = 1,
            SEMANA = 2,
            MES = 3 
        }

        public enum Meses
        {
            NERO = 1,
            FEBRERO = 2,
            MARZO = 3,
            ABRIL = 4,
            MAYO = 5,
            JUNIO = 6,
            JULIO = 7,
            AGOSTO = 8,
            SEPTIEMBRE = 9,
            OCTUBRE = 10,
            NOVIEMBRE = 11,
            DICIEMBRE = 12
        }

        





    }
}
