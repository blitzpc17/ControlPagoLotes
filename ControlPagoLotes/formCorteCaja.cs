using ClosedXML.Excel;
using Entidades;
using LOGICA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlPagoLotes
{
    public partial class formCorteCaja : Form
    {
        private PagoPartidaLogica contexto;
        private List<clsDATACORTE> ListaPagosDiarios;
        public formCorteCaja()
        {
            InitializeComponent();
        }

        private void InicializarFormulario()
        {
            contexto = new PagoPartidaLogica();
            Global.LimpiarControles(this);
            
        }

        private void dtpFechaContrato_ValueChanged(object sender, EventArgs e)
        {
            ListarPagosPorFecha(dtpFechaContrato.Value);
        }

        private void ListarPagosPorFecha(DateTime fecha)
        {
            ListaPagosDiarios = contexto.ListarPagoPorFecha(fecha);
            dgvRegistros.DataSource = ListaPagosDiarios;
            tsTotalRegistros.Text = dgvRegistros.RowCount.ToString("N0");
            tsTotalDia.Text = (ListaPagosDiarios != null && ListaPagosDiarios.Count > 0) ?(ListaPagosDiarios.Sum(x=>x.Monto).ToString("N2")): "0.00";
            Apariencias();
        }

        private void Apariencias()
        {
            dgvRegistros.Columns[0].HeaderText = "CLIENTE";
            dgvRegistros.Columns[1].HeaderText = "ZONA";
            dgvRegistros.Columns[2].HeaderText = "LOTE(S)";
            dgvRegistros.Columns[3].HeaderText = "MONTO ($)";
            dgvRegistros.Columns[4].HeaderText = "FECHA PAGO";
            dgvRegistros.Columns[5].HeaderText = "FECHA REGISTRO";
            dgvRegistros.Columns[6].HeaderText = "RECIBIÓ";

            //apariencias

            dgvRegistros.Columns[0].Width = 280;
            dgvRegistros.Columns[1].Width = 210;
            dgvRegistros.Columns[2].Width = 210;
            dgvRegistros.Columns[3].Width = 110;
            dgvRegistros.Columns[4].Width = 110;
            dgvRegistros.Columns[5].Width = 110;
            dgvRegistros.Columns[6].Width = 210;

        }

        private void bntExportar_Click(object sender, EventArgs e)
        {
            if (dgvRegistros.DataSource == null)
            {
                MessageBox.Show("No hay registros para exportar.", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                return;
            }

            using (var workbook = new XLWorkbook())
            {/*
                //variables
                int noPagos = ListaCeldas.Count;
                decimal Acumulado = 0;

                // Crear una hoja de Excel
                var worksheet = workbook.Worksheets.Add("Boleta");

                // 1. Escribir el encabezado en las primeras 5 filas            
                worksheet.Column(1).Width = 4.3;
                worksheet.Column(2).Width = 16.7;
                worksheet.Column(3).Width = 16.7;

                //poste izquierdo
                var rangoPosteIzdo = worksheet.Range("A1:A" + (noPagos + 8));
                rangoPosteIzdo.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;

                //style encabezado 
                var rangoEncabezado = worksheet.Range("B1:C3");
                rangoEncabezado.Style.Font.Bold = true;
                rangoEncabezado.Style.Font.FontSize = 12;

                //nombre cliente
                var rangoNombre = worksheet.Range("B1:C1");
                rangoNombre.Merge();
                rangoNombre.Value = ObjPago.NombreCliente;

                //total y meses
                worksheet.Cell(2, 2).Value = "$ " + Obj.Total;
                worksheet.Cell(2, 3).Value = Obj.Meses + " MESES";

                //zona
                var rangoZona = worksheet.Range("B3:C3");
                rangoZona.Merge();
                rangoZona.Value = listaZonas.First(x => x.Id == ObjPago.ZonaId).Nombre;

                //lotes //dia pago
                worksheet.Cell(4, 2).Value = Obj.Lotes;
                worksheet.Cell(4, 3).Value = "Día pago: " + Obj.DiaPago;

                //encabezado partidas
                var rangoEncabezadoPartidas = worksheet.Range("B5:C5");
                rangoEncabezadoPartidas.Style.Fill.BackgroundColor = XLColor.ForestGreen;
                rangoEncabezadoPartidas.Style.Font.FontSize = 12;
                worksheet.Cell(5, 2).Value = "MONTO";
                worksheet.Cell(5, 3).Value = "FECHA";

                for (int pos = 1; pos <= noPagos; pos++)
                {
                    worksheet.Cell(5 + pos, 1).Value = pos;
                    worksheet.Cell(5 + pos, 2).Value = "$ " + ListaCeldas[pos - 1].Monto;
                    worksheet.Cell(5 + pos, 3).Value = ListaCeldas[pos - 1].Fecha;
                    Console.WriteLine(ListaCeldas[pos - 1].Monto);
                    Acumulado += Convert.ToDecimal(ListaCeldas[pos - 1].Monto);
                }

                worksheet.Cell(5 + (3 + noPagos), 2).Value = "Saldo ($):";
                worksheet.Cell(5 + (3 + noPagos), 2).Style.Font.FontSize = 12;
                worksheet.Cell(5 + (3 + noPagos), 2).Style.Font.Bold = true;

                worksheet.Cell(5 + (3 + noPagos), 3).Value = Acumulado;
                worksheet.Cell(5 + (3 + noPagos), 3).Style.Font.FontSize = 12;
                worksheet.Cell(5 + (3 + noPagos), 3).Style.Font.Bold = true;



                // Aplicar bordes a toda la tabla
                var tableRange = worksheet.RangeUsed();
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                //guardar
                workbook.SaveAs(rutaArchivo);*/
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Desea regresar a la búsqueda general de pagos?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        }

        private void formCorteCaja_Load(object sender, EventArgs e)
        {
            InicializarFormulario();
        }
    }
}
