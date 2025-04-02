using ClosedXML.Excel;
using DocumentFormat.OpenXml.Vml;
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
    public partial class formReporteGeneral : Form
    {
        private ZonaLogica contextoZonas;
        private List<Zona> LstZonas;
        private List<Pago> LstPagos;
        private List<PagoPartida> LstPartidasPagos;
        private List<PagoPartida> LstPartidasPagosAux;
        private List<string> msjErr;


        public formReporteGeneral()
        {
            InitializeComponent();
        }

        private void InicializarFormulario()
        {
            contextoZonas = new ZonaLogica();
            Global.LimpiarControles(this);
            msjErr = new List<string>();
            LlenarComboRutas();
        }


        private void LlenarComboRutas()
        {
            LstZonas = contextoZonas.GetAllZonas();
            cbxZonas.DataSource = LstZonas;
            cbxZonas.DisplayMember = "Nombre";
            cbxZonas.ValueMember = "Id";
            cbxZonas.SelectedIndex = -1;
        }

        private void GenerarReporte(bool todos = false)
        {
            var lstRutasSeleccionadas = todos ? LstZonas.ToList() : LstZonas.Where(x => x.Id == (int)cbxZonas.SelectedValue).ToList();
            msjErr = new List<string>();

            using (var workbook = new XLWorkbook())
            {
                foreach (var zona in lstRutasSeleccionadas)
                {
                    // Crear una hoja de Excel
                    var worksheet = workbook.Worksheets.Add(zona.Nombre);
                    //consultar encabezados por zona
                    using (var contextoPagos = new PagoLogica())
                    {
                        LstPagos = contextoPagos.ListarPagosxZona(zona.Id);

                        if (LstPagos != null && LstPagos.Count > 0)
                        {
                            //consulta partidas por idpagosrelacionados y que noe sten eliminadas
                            using (var contextoPartidas = new PagoPartidaLogica())
                            {
                                string idsRelacionados = string.Join(",", LstPagos.Select(p => p.Id.ToString()));
                                LstPartidasPagos = contextoPartidas.ListarPartidasPagos(idsRelacionados);

                                if (LstPartidasPagos != null && LstPartidasPagos.Count > 0)
                                {
                                    int colPos = 1;
                                    foreach (var ObjPago in LstPagos)
                                    {
                                        LstPartidasPagosAux = LstPartidasPagos.Where(x => x.PagoId == ObjPago.Id).ToList();
                                        if (LstPartidasPagosAux != null && LstPartidasPagosAux.Count > 0)
                                        {
                                            //variables                                            
                                            int rowPos = 1;
                                            int noPagos = LstPartidasPagosAux.Count;
                                            decimal Acumulado = 0;

                                            // 1. Escribir el encabezado en las primeras 5 filas            
                                            worksheet.Column(colPos).Width = 4.3;
                                            worksheet.Column(colPos + 1).Width = 16.7;
                                            worksheet.Column(colPos + 2).Width = 16.7;

                                            //poste izquierdo
                                            //var rangoPosteIzdo = worksheet.Range("A1:A" + (noPagos + 8));
                                            var rangoPosteIzdo = worksheet.Range(rowPos, colPos, noPagos + 8, colPos);
                                            rangoPosteIzdo.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;

                                            //style encabezado 
                                            //var rangoEncabezado = worksheet.Range("B1:C3");
                                            var rangoEncabezado = worksheet.Range(rowPos, colPos + 2, rowPos + 2, colPos + 3);
                                            rangoEncabezado.Style.Font.Bold = true;
                                            rangoEncabezado.Style.Font.FontSize = 12;

                                            //nombre cliente
                                            //var rangoNombre = worksheet.Range("B1:C1");
                                            var rangoNombre = worksheet.Range(rowPos, colPos + 1, rowPos, colPos + 2);
                                            rangoNombre.Merge();
                                            rangoNombre.Value = ObjPago.NombreCliente;

                                            //total y meses
                                            /*worksheet.Cell(2, 2).Value = "$ " + ObjPago.Total;
                                            worksheet.Cell(2, 3).Value = ObjPago.Meses + " MESES";*/
                                            worksheet.Cell(rowPos + 1, colPos + 1).Value = "$ " + ObjPago.Total;
                                            worksheet.Cell(rowPos + 1, colPos + 2).Value = ObjPago.Meses + " MESES";

                                            //zona
                                            //var rangoZona = worksheet.Range("B3:C3");
                                            if (ObjPago.Estado == ((int)Enumeraciones.Estados.PAGADO).ToString("N0"))
                                            {

                                                worksheet.Cell(rowPos + 2, colPos + 1).Value = zona.Nombre;
                                                worksheet.Cell(rowPos + 2, colPos + 2).Value = Enumeraciones.Estados.PAGADO.ToString();
                                            }
                                            else
                                            {
                                                var rangoZona = worksheet.Range(rowPos + 2, colPos + 1, rowPos + 2, colPos + 2);
                                                rangoZona.Merge();
                                                rangoZona.Value = zona.Nombre;
                                            }


                                            //lotes //dia pago
                                          
                                            worksheet.Cell(rowPos + 3, colPos + 1).Value = ObjPago.Lotes;
                                            worksheet.Cell(rowPos + 3, colPos + 2).Value = "Día pago: " + ObjPago.DiaPago;


                                            var rangoObservaciones = worksheet.Range(rowPos + 4, colPos + 1, rowPos + 4, colPos + 2);
                                            rangoObservaciones.Merge();
                                            rangoObservaciones.Value = ObjPago.Observacion;


                                            //encabezado partidas                                       
                                            var rangoEncabezadoPartidas = worksheet.Range(rowPos + 5, colPos + 1, rowPos + 5, colPos + 2);
                                            rangoEncabezadoPartidas.Style.Fill.BackgroundColor = XLColor.ForestGreen;
                                            rangoEncabezadoPartidas.Style.Font.FontSize = 12;
                                        

                                            worksheet.Cell(rowPos + 5, colPos + 1).Value = "MONTO";
                                            worksheet.Cell(rowPos + 5, colPos + 2).Value = "FECHA";


                                            for (int pos = 0; pos < noPagos; pos++)
                                            {
                                                worksheet.Cell(rowPos + 6 + pos, colPos).Value = pos+1;
                                                worksheet.Cell(rowPos + 6 + pos, colPos + 1).Value = "$ " + LstPartidasPagosAux[pos].Monto;
                                                worksheet.Cell(rowPos + 6 + pos, colPos + 2).Value = LstPartidasPagosAux[pos].Fecha;
                                                
                                                Acumulado += Convert.ToDecimal(LstPartidasPagosAux[pos].Monto);
                                            }

                                            decimal montoCalculado = 0;
                                            PagoPartida objPagoInicial = null;
                                            objPagoInicial = LstPartidasPagosAux.OrderBy(x => x.Fecha).FirstOrDefault();
                                            DateTime fechaActual = Global.FechaServidor();
                                            int mesesTranscurridos = ((fechaActual.Year - objPagoInicial.Fecha.Year) * 12) + fechaActual.Month - objPagoInicial.Fecha.Month;
                                            decimal total = ObjPago.Total;
                                            int meses = Convert.ToInt32(ObjPago.Meses);
                                            decimal mensualidad = (total - objPagoInicial.Monto) / meses;
                                            

                                            Acumulado = Acumulado - objPagoInicial.Monto;

                                            decimal montoPreliminarmentePAgado = 0;
                                            if (mesesTranscurridos < meses)
                                            {
                                                montoPreliminarmentePAgado = mensualidad * mesesTranscurridos;
                                            }
                                            else
                                            {
                                                montoPreliminarmentePAgado = total;
                                            }

                                            worksheet.Cell(rowPos + 5 + (3 + noPagos), colPos + 1).Value = "Abonado ($):";
                                            worksheet.Cell(rowPos + 5 + (3 + noPagos), colPos + 1).Style.Font.FontSize = 12;
                                            worksheet.Cell(rowPos + 5 + (3 + noPagos), colPos + 1).Style.Font.Bold = true;

                                            worksheet.Cell(rowPos + 5 + (3 + noPagos), colPos + 2).Value = Acumulado.ToString("N2");
                                            worksheet.Cell(rowPos + 5 + (3 + noPagos), colPos + 2).Style.Font.FontSize = 12;
                                            worksheet.Cell(rowPos + 5 + (3 + noPagos), colPos + 2).Style.Font.Bold = true;


                                            worksheet.Cell(rowPos + 6 + (3 + noPagos), colPos + 1).Value = "Debería llevar ($):";
                                            worksheet.Cell(rowPos + 6 + (3 + noPagos), colPos + 1).Style.Font.FontSize = 12;
                                            worksheet.Cell(rowPos + 6 + (3 + noPagos), colPos + 1).Style.Font.Bold = true;

                                            worksheet.Cell(rowPos + 6 + (3 + noPagos), colPos + 2).Value = montoPreliminarmentePAgado.ToString("N2");
                                            worksheet.Cell(rowPos + 6 + (3 + noPagos), colPos + 2).Style.Font.FontSize = 12;
                                            worksheet.Cell(rowPos + 6 + (3 + noPagos), colPos + 2).Style.Font.Bold = true;


                                            worksheet.Cell(rowPos + 7 + (3 + noPagos), colPos + 1).Value = "Atraso ($):";
                                            worksheet.Cell(rowPos + 7 + (3 + noPagos), colPos + 1).Style.Font.FontSize = 12;
                                            worksheet.Cell(rowPos + 7 + (3 + noPagos), colPos + 1).Style.Font.Bold = true;

                                            worksheet.Cell(rowPos + 7 + (3 + noPagos), colPos + 2).Value = Math.Max(montoPreliminarmentePAgado - Acumulado, 0).ToString("N2");
                                            worksheet.Cell(rowPos + 7 + (3 + noPagos), colPos + 2).Style.Font.FontSize = 12;
                                            worksheet.Cell(rowPos + 7 + (3 + noPagos), colPos + 2).Style.Font.Bold = true;



                                            // Aplicar bordes a toda la tabla
                                            var tableRange = worksheet.RangeUsed();
                                            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                                            colPos += 3;

                                        }
                                        else
                                        {
                                            msjErr.Add("*No hay partidas registradas para la boleta del cliente " + ObjPago.NombreCliente + ", zona " + zona.Nombre);
                                        }

                                    }
                                }
                                else
                                {
                                    msjErr.Add("*Las boletas de pago de la zona " + zona.Nombre + " no tienen pagos registrados.");
                                }

                            }


                            //recorrer y e imprimir en el excel
                        }
                        else
                        {
                            msjErr.Add("*No hay pagos registrados en la zona " + zona.Nombre + ".");
                        }


                    }




                }

                //guardar
                workbook.SaveAs(saveFileDialog1.FileName);


                if (msjErr != null && msjErr.Count > 0)
                {
                    MessageBox.Show("Se han detectado los siguientes detalles al momento de exportar el reporte: \r\n" + string.Join("\r\n", msjErr), "Detalles de la generación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Reporte generado correctamente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }


            }
        }

        private void formReporteGeneral_Shown(object sender, EventArgs e)
        {
            InicializarFormulario();
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            if (!chkTodos.Checked && cbxZonas.SelectedIndex == -1)
            {
                MessageBox.Show("No se ha seleccionado la zona a consultar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                saveFileDialog1.Title = "Guardar archivo Excel";
                saveFileDialog1.Filter = "Archivos Excel (*.xlsx)|*.xlsx";
                saveFileDialog1.DefaultExt = "xlsx";
                saveFileDialog1.FileName = "reporte general.xlsx";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    GenerarReporte(chkTodos.Checked);
                }
            }

        }

        private void chkTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTodos.Checked)
            {
                cbxZonas.SelectedIndex = -1;
            }
        }

        private void cbxZonas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxZonas.SelectedIndex > -1)
            {
                chkTodos.Checked = false;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
