using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Entidades;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using LOGICA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PageSize = iText.Kernel.Geom.PageSize;
using Document = iText.Layout.Document;
using Paragraph = iText.Layout.Element.Paragraph;
using Table = iText.Layout.Element.Table;
using Cell = iText.Layout.Element.Cell;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Font;

namespace ControlPagoLotes
{
    public partial class formBoleta : Form
    {

        private ZonaLogica contextoZonas;
        private PagoLogica contextoPago;
        private PagoPartidaLogica contextoPagoPartida;

        private List<Zona> listaZonas;
        private Pago Obj;
        private int? idRegistro;
        private PagoPartida ObjPartida;
        private List<PagoPartida> ListaPartidas;
        private List<PagoPartida> ListaPartidasAux;

        private bool isPasting = false;
        private bool guardoEncabezado = false;
        private decimal acumulado = 0;

        private string clipboardText;
        private int indexActual = -1;
        List<clsCELDASPAGOS> ListaCeldas;
        List<string> rows;

        private bool pagoAtrasado = false;


        public formBoleta(int? idRegistro)
        {
            InitializeComponent();
            this.idRegistro = idRegistro;
        }

        private void formBoleta_Load(object sender, EventArgs e)
        {
            InicializarModulo();
            if (pagoAtrasado)
            {
                MensajePorEstado((int)Enumeraciones.Estados.ATRASADO);
            }
            else
            {
                MensajePorEstado((int)cbxEstados.SelectedValue);
            }
        }

        private void InicializarModulo()
        {
            contextoZonas = new ZonaLogica();
            contextoPago = new PagoLogica();
            contextoPagoPartida = new PagoPartidaLogica();

            InicializarVariables();

            Global.LimpiarControles(this);

            InicializarDgv();

            btnAddPago.Enabled = true;
            listaZonas = contextoZonas.GetAllZonas();
            cbxZona.DataSource = listaZonas;
            cbxZona.DisplayMember = "Nombre";
            cbxZona.ValueMember = "Id";
            cbxZona.SelectedIndex = -1;

            var listaEstados = Enum.GetValues(typeof(Enumeraciones.Estados))
                .Cast<Enumeraciones.Estados>()
                .Select(x => new { Id = Convert.ToInt32(x), Nombre = x.ToString()})
                .ToList();

            cbxEstados.DataSource = listaEstados;
            cbxEstados.DisplayMember = "Nombre";
            cbxEstados .ValueMember = "Id";

            bool nuevo = idRegistro == null;

            if (!nuevo)
            {
                Obj= contextoPago.GetPagoById((int)idRegistro);

                txtNombreCliente.Text = Obj.NombreCliente;
                txtDiaPago.Text = Obj.DiaPago;
                cbxZona.SelectedValue = Obj.ZonaId;
                cbxEstados.SelectedValue = Convert.ToInt32(Obj.Estado);
                txtLotes.Text = Obj.Lotes;
                txtMeses.Text = Obj.Meses;
                txtTotal.Text = Obj.Total.ToString("N2");
                dtpFechaContrato.Value = Obj.FechaRegistro;

                ListaPartidas = contextoPagoPartida.GetAllPagoPartidas((int)idRegistro);

                if (ListaPartidas != null && ListaPartidas.Count > 0)
                {
                    ListaCeldas = new List<clsCELDASPAGOS> ();

                    foreach (var r in ListaPartidas)
                    {
                        dgvRegistros.Rows.Add(r.Monto, r.Fecha.ToShortDateString());
                        ListaCeldas.Add(new clsCELDASPAGOS
                        {
                            Monto = r.Monto.ToString("N2"),
                            Fecha = r.Fecha.ToString("dd/MM/yyyy")
                        });
                    }
                    MostrarCeldasEnDgv();
                    if (Obj.Estado == ((int)Enumeraciones.Estados.ATRASADO).ToString() 
                        || Obj.Estado == ((int)Enumeraciones.Estados.CORRIENTE).ToString() && !ValidarAtrasoPago())
                    {                                               
                        pagoAtrasado = true;
                        cbxEstados.SelectedValue = (int)Enumeraciones.Estados.ATRASADO;
                    }
                }
                else
                {
                    tsTotalRegistros.Text = @"0";
                    tsAcumulado.Text = @"0.00";
                } 

            }


        }

        private void InicializarDgv()
        {
            indexActual = -1;
            dgvRegistros.ColumnCount = 2;
            dgvRegistros.Columns[0].HeaderText = "MONTO";
            dgvRegistros.Columns[1].HeaderText = "FECHA";
            dgvRegistros.AllowUserToAddRows = false;
            dgvRegistros.ReadOnly = false;

            //apariencias

            dgvRegistros.Columns[0].Width = 200;
            dgvRegistros.Columns[1].Width = 200;
        }

        private void InicializarVariables()
        {
            listaZonas = null;
            Obj = null;
            ObjPartida = null;
            ListaPartidas = null;
            ListaPartidasAux = null;
            isPasting = false;
            guardoEncabezado = false;
            acumulado = 0;
            pagoAtrasado = false;
        }

        private void GuardarPago()
        {
            DateTime fechaServidor = Global.FechaServidor();

            if (Obj==null)
            {
                Obj = new Pago
                {
                    NombreCliente = txtNombreCliente.Text,
                    Total = decimal.Parse(txtTotal.Text),
                    Meses = txtMeses.Text,
                    ZonaId = (int)cbxZona.SelectedValue,
                    DiaPago = txtDiaPago.Text,
                    Lotes = txtLotes.Text,
                    FechaRegistro = dtpFechaContrato.Value,
                    Estado = cbxEstados.SelectedValue.ToString(),
                    FechaCreacion = fechaServidor,
                };

                Obj.Id = contextoPago.AddPago(Obj);
                guardoEncabezado = Obj.Id > 0;

            }
            else
            {
                Obj.NombreCliente = txtNombreCliente.Text;
                Obj.Total = decimal.Parse(txtTotal.Text);
                Obj.Meses = txtMeses.Text;
                Obj.ZonaId = (int)cbxZona.SelectedValue;
                Obj.DiaPago = txtDiaPago.Text;
                Obj.Lotes = txtLotes.Text;
                Obj.FechaRegistro = dtpFechaContrato.Value;                
                Obj.Estado = cbxEstados.SelectedValue.ToString();
                guardoEncabezado = contextoPago.UpdatePago(Obj);
            }

            if (guardoEncabezado && dgvRegistros.Rows.Count > 0 && ((int)cbxEstados.SelectedValue == Convert.ToInt32(Enumeraciones.Estados.CORRIENTE)))
            {
                ListaPartidasAux = new List<PagoPartida>();

                foreach (var item in ListaCeldas)
                {                 
                    if(decimal.TryParse(item.Monto, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out decimal monto)  && (DateTime.TryParse(item.Fecha, out DateTime fecha)))
                    {
                        ListaPartidasAux.Add(new PagoPartida
                        {
                            PagoId = Obj.Id,
                            Monto = monto,
                            Fecha = fecha,
                            UsuarioId = 1,
                            FechaCreacion = fechaServidor,
                        });
                    }
                  
                }
                //borrar anteriores

                int eliminados = contextoPagoPartida.EliminarPartidasAnteriores(Obj.Id);

                //guardar partida

                if (ListaPartidas == null || eliminados == ListaPartidas.Count)
                {
                    //insertar
                    string query = "";


                    query += string.Join(",", ListaPartidasAux.Select(item =>
                    $"({Obj.Id}, {item.Monto}, '{item.Fecha.ToString("yyyy-MM-dd")}','{Global.ObjUsuario.Id}', '{item.FechaCreacion.ToString("yyyy-MM-dd HH:mm:ss")}')"));

                    if (ListaPartidasAux.Count == contextoPagoPartida.InsertarPartidasPago(query))
                    {
                        MessageBox.Show("Se han registrado los cambios correctamente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        InicializarModulo();
                    }
                    else
                    {
                        MessageBox.Show("Error al guardar los registros, intente cargando el Pago nuevamente.", "Error en la operación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Error al intentar realizar la operación de guarddado, intente cargando el Pago nuevamente.", "Error en la operación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }

            if (guardoEncabezado)
            {
                MessageBox.Show("Registro guardado correctamente.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }





        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("¿Desea regresar a la búsqueda general de pagos?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
            
        }

        private void btnAddPago_Click(object sender, EventArgs e)
        {
            GuardarPago();
        }

        private void pegarContenidoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //PegarCeldas();
            PegadoEspecial();
        }

        private void PegadoEspecial()
        {
            clipboardText = Clipboard.GetText();
            if (!string.IsNullOrWhiteSpace(clipboardText))
            {
                if (dgvRegistros.Rows.Count > 0)
                {
                    indexActual = dgvRegistros.CurrentRow.Index;
                }

                if (ListaCeldas == null)
                {
                    ListaCeldas = new List<clsCELDASPAGOS>();
                }

                string[] lines = clipboardText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);                 

                foreach (var line in lines)
                {
                    string[] cells = line.Split('\t');
                    if (cells.Length >= 2) 
                    {
                        if (indexActual == -1)
                        {
                            ListaCeldas.Add(new clsCELDASPAGOS { Monto = cells[0], Fecha = cells[1] });
                        }else if (indexActual > -1)
                        {
                            ListaCeldas.Insert(indexActual, new clsCELDASPAGOS { Monto = cells[0], Fecha = cells[1] });
                            indexActual++;
                        }
                    }
                }

                MostrarCeldasEnDgv();

            }


        }

        private void MostrarCeldasEnDgv()
        {
            dgvRegistros.EndEdit();
            dgvRegistros.Rows.Clear();
            acumulado = 0;

            foreach (var item in ListaCeldas)
            {
                dgvRegistros.Rows.Add(item.Monto, item.Fecha);
                if (decimal.TryParse(item.Monto, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out decimal monto))
                {
                    acumulado += monto;
                }
                else
                {
                    acumulado += 0; 
                }

            }

            tsTotalRegistros.Text = dgvRegistros.RowCount.ToString("N0");
            tsAcumulado.Text = acumulado.ToString("N2");
        }

        private void eliminarFilaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarPartida();
        }

        private void nuevaPartidaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CrearPartida();
        }

        private void CrearPartida()
        {
            if (ListaCeldas == null)
            {
                ListaCeldas = new List<clsCELDASPAGOS>();
            }

            if (indexActual < 0)
            {
                ListaCeldas.Add(new clsCELDASPAGOS());
            }
            else
            {
                indexActual = dgvRegistros.CurrentRow.Index;
                ListaCeldas.Insert(indexActual, new clsCELDASPAGOS());
            }

            MostrarCeldasEnDgv();

        }

        private void EliminarPartida()
        {
            if (dgvRegistros.Rows.Count <= 0) return;

            indexActual = dgvRegistros.CurrentRow.Index;

            ListaCeldas.RemoveAt(indexActual);

            MostrarCeldasEnDgv();
        }

        private void dgvRegistros_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRegistros.RowCount < 1) return;
            ModificarValoresCeldas(e.RowIndex,e.ColumnIndex, dgvRegistros.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? null);
        }

        private void ModificarValoresCeldas(int row, int cell, string valor)
        {
            if (dgvRegistros.RowCount < 1) return;
            if (ListaCeldas == null || ListaCeldas.Count < 1) return;

            if(cell == 0)
            {
                ListaCeldas[row].Monto = valor;
            }
            else
            {
                ListaCeldas[row].Fecha = valor;
            }

            MostrarCeldasEnDgv(); 

        }

        private bool ValidarAtrasoPago()
        {   
            DateTime fechaActual = Global.FechaServidor();
            PagoPartida ultimoPago = ListaPartidas.OrderByDescending(x => x.Fecha).First();
            return (ultimoPago.Fecha >= fechaActual.AddMonths(-3) && ultimoPago.Fecha <= fechaActual);

        }

        private void formBoleta_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Obj != null && pagoAtrasado)
                {
                    Obj.Estado = ((int)Enumeraciones.Estados.ATRASADO).ToString();
                    contextoPago.UpdatePago(Obj);
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

        private void MensajePorEstado(int estado)
        {
            string msj=null;
            string titulo=null;
            MessageBoxIcon tipo = MessageBoxIcon.None;

            switch (estado)
            {
                case 2:
                    titulo = "Aviso";
                    msj = "El contrado ha sido "+Enumeraciones.Estados.PAGADO+".";
                    tipo = MessageBoxIcon.Information;
                    btnAddPago.Enabled = false;
                    break;

                case 3:
                    titulo = "Aviso";
                    msj = "El contrado ha sido "+Enumeraciones.Estados.CANCELADO+".";
                    tipo = MessageBoxIcon.Information;
                    break;

                case 4:
                    titulo = "ADVERTENCIA";
                    msj = "El cliente no ha presentado mensualidad en más de 3 meses consecutivos.";
                    tipo = MessageBoxIcon.Information;
                    break;

                case 5:
                    titulo = "ADVERTENCIA";
                    msj = "El pago tiene el estado "+ Enumeraciones.Estados.DESCONOCIDO + ", verifique la inforción y guarde los cambios. ";
                    tipo = MessageBoxIcon.Information;
                    break;
            }


            if (!string.IsNullOrEmpty(msj))
            {
                MessageBox.Show(msj, titulo, MessageBoxButtons.OK, tipo );
            }

        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if(Obj!=null && ListaCeldas!=null && ListaCeldas.Count > 0)
            {
                saveFileDialog1.Title = "Guardar archivo Excel";
                saveFileDialog1.Filter = "Archivos Excel (*.xlsx)|*.xlsx";
                saveFileDialog1.DefaultExt = "xlsx";
                saveFileDialog1.FileName = "boleta de pago.xlsx";
                saveFileDialog1.ShowDialog();
                if (string.IsNullOrEmpty(saveFileDialog1.FileName))
                {
                    MessageBox.Show("Debe confirmar el pago para poder exportar la información.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string rutaArchivo = saveFileDialog1.FileName;
                
                ExportarExcel(Obj, ListaCeldas, rutaArchivo);
            }
            else
            {
                MessageBox.Show("No hay un pago cargado para realizar la operación.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void ExportarExcel(Pago ObjPago, List<clsCELDASPAGOS>ListaPartidas, string rutaArchivo)
        {
            using (var workbook = new XLWorkbook())
            {
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
                var rangoPosteIzdo = worksheet.Range("A1:A"+(noPagos+8));
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
                worksheet.Cell(2, 3).Value = Obj.Meses + " MESES" ;                
        
                //zona
                var rangoZona = worksheet.Range("B3:C3");
                rangoZona.Merge();
                rangoZona.Value = listaZonas.First(x=>x.Id ==ObjPago.ZonaId).Nombre;

                //lotes //dia pago
                worksheet.Cell(4, 2).Value = Obj.Lotes;
                worksheet.Cell(4, 3).Value = "Día pago: "+Obj.DiaPago;

                //encabezado partidas
                var rangoEncabezadoPartidas = worksheet.Range("B5:C5");
                rangoEncabezadoPartidas.Style.Fill.BackgroundColor = XLColor.ForestGreen;
                rangoEncabezadoPartidas.Style.Font.FontSize = 12;
                worksheet.Cell(5, 2).Value = "MONTO";
                worksheet.Cell(5, 3).Value = "FECHA";               

                for(int pos = 1; pos<=noPagos; pos++)
                {
                    worksheet.Cell(5+pos,1).Value = pos;
                    worksheet.Cell(5 + pos, 2).Value = "$ "+ListaCeldas[pos-1].Monto;
                    worksheet.Cell(5 + pos, 3).Value = ListaCeldas[pos-1].Fecha;
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
                workbook.SaveAs(rutaArchivo);
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (Obj != null && ListaCeldas != null && ListaCeldas.Count > 0)
            {
                GenerarReportViewer();
            }
            else
            {
                MessageBox.Show("No hay un pago cargado para realizar la operación.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }    
        }

        private void GenerarReportViewer()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Guardar archivo PDF";
                saveFileDialog.Filter = "Archivos PDF (*.pdf)|*.pdf";
                saveFileDialog.DefaultExt = "pdf";
                saveFileDialog.FileName = "Boleta_de_pago.pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string rutaArchivo = saveFileDialog.FileName;

                    try
                    {
                        // Crear un archivo PDF con tamaño de media carta
                        using (var pdfWriter = new PdfWriter(rutaArchivo))
                        using (var pdfDoc = new PdfDocument(pdfWriter))
                        {
                            // Establecer tamaño de página a media carta
                            var mediaCarta = new PageSize(396, 612); // Ancho x Alto en puntos
                            pdfDoc.SetDefaultPageSize(mediaCarta);

                            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                            var document = new Document(pdfDoc);

                            // Agregar título
                            document.Add(new Paragraph("Boleta de pago")
                                .SetFontSize(18)
                                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                            // document.Add(new Paragraph("Este documento asegura que las tablas largas y el contenido se dividan correctamente entre las páginas.\n\n"));

                            var tableEncabezado = new Table(2)
                            .SetFontSize(14)
                            .SetWidth(UnitValue.CreatePercentValue(100)); // Ajustar al ancho del documento

                            // Agregar las celdas de encabezado con texto personalizado
                            tableEncabezado.AddCell(new Cell(1,2).Add(new Paragraph(Obj.NombreCliente))).SetFont(boldFont);
                            tableEncabezado.AddCell(new Cell(2,1).Add(new Paragraph("$ " + Obj.Total.ToString("N2")))).SetFont(boldFont);
                            tableEncabezado.AddCell(new Cell(2, 2).Add(new Paragraph(Obj.Meses + " meses"))).SetFont(boldFont);
                            tableEncabezado.AddCell(new Cell(3, 2).Add(new Paragraph(listaZonas.First(x => x.Id == Obj.ZonaId).Nombre))).SetFont(boldFont); // Valor de la ruta
                            tableEncabezado.AddCell(new Cell(4, 1).Add(new Paragraph(Obj.Lotes))).SetFont(boldFont);
                            tableEncabezado.AddCell(new Cell(4, 2).Add(new Paragraph("Día pago: " + Obj.DiaPago))).SetFont(boldFont);
                            // Agregar la tabla al documento
                            document.Add(tableEncabezado);

                            // Crear una tabla con varias filas para probar saltos de página
                            var table = new Table(3); // 4 columnas
                            table.SetWidth(UnitValue.CreatePercentValue(100)); // Usar todo el ancho disponible

                            // Encabezados de la tabla
                            table.AddHeaderCell(new Cell().Add(new Paragraph("No."))).SetFont(boldFont);
                            table.AddHeaderCell(new Cell().Add(new Paragraph("MONTO"))).SetFont(boldFont);
                            table.AddHeaderCell(new Cell().Add(new Paragraph("FECHA"))).SetFont(boldFont);

                            // Agregar muchas filas para forzar el salto de página
                            for (int i = 0; i < ListaCeldas.Count; i++)
                            {
                                table.AddCell($"{i+1}");
                                table.AddCell($"{"$ "+ ListaCeldas[i].Monto}");
                                table.AddCell($"{ListaCeldas[i].Fecha}");
                            }

                            // Agregar la tabla al documento
                            document.Add(table);

                            // Finalizar el documento
                            document.Close();
                        }

                        MessageBox.Show("Boleta de pago generada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ocurrió un error al generar la boleta de pago: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            EnviarRecibo();
        }

        private void EnviarRecibo()
        {
            throw new NotImplementedException();
        }
    }
}
