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
        private decimal montoNuevos = 0;
        private decimal montoTransferencias = 0;
        private decimal montoNuevosMismoDiaModificados = 0;
        private decimal montoModificados = 0;
        private decimal montoModificadosMigrados = 0;
        private decimal montoEliminados = 0;
        private decimal montoMigrado = 0;
        private Enumeraciones.Periodo periodoSeleccionado;
        public formCorteCaja()
        {
            InitializeComponent();
        }

        private void InicializarFormulario()
        {
            contexto = new PagoPartidaLogica();
           
            InicializarControles();
           
        }

        private void InicializarControles()
        {
            Global.LimpiarControles(this);
            ComboBoxHelper.LlenarComboBox<Enumeraciones.Periodo>(cbxPeriodo, true);
            cbxPeriodo.SelectedIndex = -1;
            ComboBoxHelper.LlenarComboBox<Enumeraciones.Meses>(cbxMeses, true);
            cbxMeses.SelectedIndex = -1;
            CargarLotificaciones();
            
           
        }

        private void CargarLotificaciones()
        {
            contexto.ListarLotificaciones();
            cbxLotificaciones.DataSource = contexto.LstZona;
            cbxLotificaciones.DisplayMember = "Nombre";
            cbxLotificaciones.ValueMember = "Id";   
            cbxLotificaciones.SelectedIndex = -1;
        }

        private void dtpFechaContrato_ValueChanged(object sender, EventArgs e)
        {
            tsCargandoInformacion.Text = "Cargando información...";
            backgroundWorker1.RunWorkerAsync();
        }

        private void ListarPagosPorFecha()
        {
            ListaPagosDiarios = contexto.ListarPagoPorFecha(contexto.objConsulta);
          
            //agregar monto migrados
           montoNuevos = (ListaPagosDiarios != null && ListaPagosDiarios.Count > 0) ? (ListaPagosDiarios
                    .Where(x => x.FormaPagoTipo == 1   //se cambia  a puro efectivo
                        && (x.FechaElimino == null && x.UsuarioElimino == null)
                        && (x.UsuarioModifico == null && x.FechaModifico == null)                           
                    ).Sum(x => x.Monto)) : 0; 

            montoTransferencias = (ListaPagosDiarios != null && ListaPagosDiarios.Count > 0) ? (ListaPagosDiarios
                    .Where(x => x.FormaPagoTipo == 2   //se agrego para transferencias
                        && (x.FechaElimino == null && x.UsuarioElimino == null)
                        && (x.UsuarioModifico == null && x.FechaModifico == null)
                    ).Sum(x => x.Monto)) : 0;

            montoNuevosMismoDiaModificados = (ListaPagosDiarios != null && ListaPagosDiarios.Count > 0) ? (ListaPagosDiarios
                    .Where(x => x.FormaPagoTipo != 0
                           && (x.FechaElimino == null && x.UsuarioElimino == null)
                           && (x.UsuarioModifico != null && x.FechaModifico != null && Convert.ToDateTime(x.FechaModifico).Date == x.FechaMovimiento.Date)                           
                           ).Sum(x => x.Monto)) : 0;


            Console.WriteLine(montoNuevos);
            montoModificados = (ListaPagosDiarios != null && ListaPagosDiarios.Count > 0) ? 
                (ListaPagosDiarios.Where(x =>
                   x.FormaPagoTipo != 0
                    &&(x.FechaElimino == null && x.UsuarioElimino == null) 
                    && (x.UsuarioModifico != null && x.FechaModifico != null)
                    && (Convert.ToDateTime(x.FechaModifico).Date != x.FechaMovimiento.Date)
                    )
                    .Sum(x => ((x.CantidadOriginal > x.Monto)?((x.CantidadOriginal-x.Monto) *-1):(x.Monto - x.CantidadOriginal))   )) : 0;

            Console.WriteLine(montoModificados);

         

            montoEliminados = (ListaPagosDiarios != null && ListaPagosDiarios.Count > 0) ? 
                (ListaPagosDiarios.Where(x => x.FechaElimino != null && x.UsuarioElimino != null).Sum(x => (x.Monto))) : 0;
            Console.WriteLine(montoEliminados);

            /*montoMigrado = (ListaPagosDiarios != null && ListaPagosDiarios.Count > 0) ? 
                (ListaPagosDiarios.Where(
                    x => x.FormaPagoTipo == 0 
                    && x.FechaElimino==null&& x.UsuarioElimino==null
                    && ( (x.FechaModifico==null && x.UsuarioModifico ==null) || (x.FechaModifico!=null && Convert.ToDateTime(x.FechaModifico).Date == x.FechaMovimiento.Date)) )
                ).Sum(x => x.Monto) : 0; */

            montoMigrado = ListaPagosDiarios?
                                .Where(x => x.FormaPagoTipo == 0
                                            && x.FechaElimino == null
                                            && x.UsuarioElimino == null
                                            && (x.FechaModifico == null && x.UsuarioModifico == null
                                                || x.FechaModifico?.Date == x.FechaMovimiento.Date))
                                .Sum(x => x.Monto) ?? 0;

            montoModificadosMigrados = (ListaPagosDiarios != null && ListaPagosDiarios.Count > 0) ?
             (ListaPagosDiarios.Where(x =>
                x.FormaPagoTipo == 0
                 && (x.FechaElimino == null && x.UsuarioElimino == null)
                 && (x.UsuarioModifico != null && x.FechaModifico != null)
                 && (Convert.ToDateTime(x.FechaModifico).Date != x.FechaMovimiento.Date)
                 )
                 .Sum(x => ((x.CantidadOriginal > x.Monto) ? ((x.CantidadOriginal - x.Monto) * -1) : (x.Monto - x.CantidadOriginal)))) : 0;

        }

        private void Apariencias()
        {
            dgvRegistros.Columns[0].HeaderText = "CLIENTE";
            dgvRegistros.Columns[1].HeaderText = "ZONA";
            dgvRegistros.Columns[2].HeaderText = "LOTE(S)";
            dgvRegistros.Columns[3].HeaderText = "MONTO ($)";
            dgvRegistros.Columns[4].HeaderText = "MONTO ORIGINAL ($)";
            dgvRegistros.Columns[5].HeaderText = "FECHA PAGO";
            dgvRegistros.Columns[6].HeaderText = "FECHA REGISTRO";
            dgvRegistros.Columns[7].HeaderText = "RECIBIÓ";
            dgvRegistros.Columns[8].HeaderText = "FECHA MODIFICACIÓN";
            dgvRegistros.Columns[9].HeaderText = "MODIFICO";
            dgvRegistros.Columns[10].HeaderText = "FECHA Elimino";
            dgvRegistros.Columns[11].HeaderText = "ELIMINO";
            dgvRegistros.Columns[12].Visible = false;
            dgvRegistros.Columns[13].HeaderText = "FORMA PAGO";

            //apariencias

            dgvRegistros.Columns[0].Width = 280;
            dgvRegistros.Columns[1].Width = 210;
            dgvRegistros.Columns[2].Width = 210;
            dgvRegistros.Columns[3].Width = 110;
            dgvRegistros.Columns[4].Width = 110;
            dgvRegistros.Columns[5].Width = 110;
            dgvRegistros.Columns[6].Width = 110;
            dgvRegistros.Columns[7].Width = 210;
            dgvRegistros.Columns[8].Width = 110;
            dgvRegistros.Columns[9].Width = 210;
            dgvRegistros.Columns[10].Width = 110;
            dgvRegistros.Columns[11].Width = 210;
            dgvRegistros.Columns[11].Width = 180;

        }

        private void bntExportar_Click(object sender, EventArgs e)
        {
            if (dgvRegistros.DataSource == null)
            {
                MessageBox.Show("No hay registros para exportar.", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                return;
            }

            saveFileDialog1.Title = "Guardar archivo Excel";
            saveFileDialog1.Filter = "Archivos Excel (*.xlsx)|*.xlsx";
            saveFileDialog1.DefaultExt = "xlsx";
            saveFileDialog1.FileName = "corte_de_caja.xlsx";
            saveFileDialog1.ShowDialog();
            if (string.IsNullOrEmpty(saveFileDialog1.FileName))
            {
                MessageBox.Show("Debe confirmar el pago para poder exportar la información.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string rutaArchivo = saveFileDialog1.FileName;


            using (var workbook = new XLWorkbook())
            {
                //variables
                int noPagos = ListaPagosDiarios .Count;
                decimal Acumulado = 0;

                // Crear una hoja de Excel
                var worksheet = workbook.Worksheets.Add("Corte de caja "+ dtpFechaContrato.Value.ToString("dd-MM-yyyy"));

                // 1. Escribir el encabezado en las primeras 5 filas            
                worksheet.Column(1).Width = 4.3;
                worksheet.Column(2).Width = 40;
                worksheet.Column(3).Width = 20;
                worksheet.Column(4).Width = 35;
                worksheet.Column(5).Width = 15;
                worksheet.Column(6).Width = 15;
                worksheet.Column(7).Width = 25;
                worksheet.Column(8).Width = 20;
                worksheet.Column(9).Width = 30;
                worksheet.Column(10).Width = 20;
                worksheet.Column(11).Width = 30;
                worksheet.Column(12).Width = 20;
                worksheet.Column(13).Width = 30;
                worksheet.Column(14).Width = 30;

                //poste izquierdo
                var rangoPosteIzdo = worksheet.Range("A1:A" + (noPagos+1));
                rangoPosteIzdo.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;

                int rowReporte = 1;
                //encabezados
                worksheet.Cell(rowReporte, 1).Value = "No.";
                worksheet.Cell(rowReporte, 2).Value = "Cliente";
                worksheet.Cell(rowReporte, 3).Value = "Zona";
                worksheet.Cell(rowReporte, 4).Value = "Lotes";
                worksheet.Cell(rowReporte, 5).Value = "Monto($)";
                worksheet.Cell(rowReporte, 6).Value = "Monto Original($)";
                worksheet.Cell(rowReporte, 7).Value = "Fecha Pago";
                worksheet.Cell(rowReporte, 8).Value = "Fecha Movimiento";
                worksheet.Cell(rowReporte, 9).Value = "Recibió";
                worksheet.Cell(rowReporte, 10).Value = "Fecha Módifico";
                worksheet.Cell(rowReporte, 11).Value = "Módifico";
                worksheet.Cell(rowReporte, 12).Value = "Fecha Eliminó";
                worksheet.Cell(rowReporte, 13).Value = "Eliminó";
                worksheet.Cell(rowReporte, 14).Value = "Forma Pago";

                rowReporte++;

                for (int i=0; i<ListaPagosDiarios.Count; i++)
                {
                    worksheet.Cell(rowReporte, 1).Value = (i + 1);
                    worksheet.Cell(rowReporte, 2).Value = ListaPagosDiarios[i].NombreCliente;
                    worksheet.Cell(rowReporte, 3).Value = ListaPagosDiarios[i].Zona;
                    worksheet.Cell(rowReporte, 4).Value = ListaPagosDiarios[i].Lotes;
                    worksheet.Cell(rowReporte, 5).Value = ListaPagosDiarios[i].Monto;//.ToString("N2");
                    worksheet.Cell(rowReporte, 6).Value = ListaPagosDiarios[i].CantidadOriginal;//.ToString("N2");
                    worksheet.Cell(rowReporte, 7).Value = ListaPagosDiarios[i].FechaPago.ToString("dd/MM/yyyy");
                    worksheet.Cell(rowReporte, 8).Value = ListaPagosDiarios[i].FechaMovimiento.ToString("dd/MM/yyyy HH:mm:ss");
                    worksheet.Cell(rowReporte, 9).Value = ListaPagosDiarios[i].UsuarioRecibe;

                    if (ListaPagosDiarios[i].FechaModifico != null)
                    {
                        worksheet.Cell(rowReporte, 10).Value = Convert.ToDateTime(ListaPagosDiarios[i].FechaModifico).ToString("dd/MM/yyyy HH:mm:ss");
                    }
                    else
                    {
                        worksheet.Cell(rowReporte, 10).Value = "";
                    }

                    worksheet.Cell(rowReporte, 11).Value = ListaPagosDiarios[i].UsuarioModifico;
                    
                    if(ListaPagosDiarios[i].FechaElimino != null)
                    {
                        worksheet.Cell(rowReporte, 12).Value = Convert.ToDateTime(ListaPagosDiarios[i].FechaElimino).ToString("dd/MM/yyyy HH:mm:ss");

                    }
                    else
                    {
                        worksheet.Cell(rowReporte, 12).Value = "";
                    }

                    worksheet.Cell(rowReporte, 13).Value = ListaPagosDiarios[i].UsuarioElimino;
                    worksheet.Cell(rowReporte, 14).Value = ListaPagosDiarios[i].FormaPago;

                    rowReporte++;
                }

                // Aplicar bordes a toda la tabla
                var tableRange = worksheet.RangeUsed();
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                //guardar
                workbook.SaveAs(rutaArchivo);
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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ListarPagosPorFecha();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dgvRegistros.DataSource = ListaPagosDiarios;
            tsTotalRegistros.Text = dgvRegistros.RowCount.ToString("N0");
            tsNuevoIngreso.Text = (montoNuevos+montoNuevosMismoDiaModificados).ToString("N2");
            tsMontoModificado.Text = montoModificados.ToString("N2");
            tsMontoEliminado.Text = montoEliminados.ToString("N2");
            tsTotalDia.Text = (montoNuevos + montoModificados+montoNuevosMismoDiaModificados).ToString("N2");
            tsTotalMigrado.Text = montoMigrado.ToString("N2");
            tsMigradosModificados.Text = montoModificadosMigrados.ToString("N2");

            tsTotalTransferencias.Text = montoTransferencias.ToString("N2");
            Apariencias();

            tsCargandoInformacion.Text = "";
        }

        private void cbxPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ( cbxPeriodo.Items.Count<=0)
            {
                MostrarPanelPeriodo(0);
            }
            else
            {
                periodoSeleccionado = ComboBoxHelper.ObtenerValorSeleccionado<Enumeraciones.Periodo>(cbxPeriodo);
                MostrarPanelPeriodo((int)periodoSeleccionado);
            }

              
        }

        private void MostrarPanelPeriodo(int pos)
        {
            switch (pos)
            {
                case 0: //ninguno
                    panelDia.Visible = false;
                    panelSemana.Visible = false;
                    panelMes.Visible = false;
                    break;

                case 1: //DIA
                    panelDia.Visible = true;
                    panelSemana.Visible = false;
                    panelMes.Visible = false;
                    break;

                case 2: //semana
                    panelDia.Visible = false;
                    panelSemana.Visible = true;
                    panelMes.Visible = false;
                    break;

                case 3: //mes
                    panelDia.Visible = false;
                    panelSemana.Visible = false;
                    panelMes.Visible = true;
                    break;
            }
        }

        private void chkTodas_CheckedChanged(object sender, EventArgs e)
        {
            cbxLotificaciones.Enabled = !chkTodas.Checked;
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            contexto.InicializarObjConsulta();
            contexto.objConsulta.todas = chkTodas.Checked;
            if (!chkTodas.Checked)
            {
                if(cbxLotificaciones.SelectedIndex == -1)                 {
                    MessageBox.Show("Debe seleccionar una lotificación para realizar la consulta.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                contexto.objConsulta.LotificacionId = (int?)cbxLotificaciones.SelectedValue;

            }

            if (cbxPeriodo.SelectedIndex == -1) { 
                MessageBox.Show("Debe seleccionar un periodo para realizar la consulta.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);            
            }
            else
            {
                switch (periodoSeleccionado)
                {
                    case Enumeraciones.Periodo.DIA:
                        contexto.objConsulta.Tipo = Enumeraciones.Periodo.DIA.ToString();
                        contexto.objConsulta.Fecha = dtpFechaContrato.Value;
                        break;

                    case Enumeraciones.Periodo.SEMANA:
                        contexto.objConsulta.Tipo = Enumeraciones.Periodo.SEMANA.ToString();
                        contexto.objConsulta.NumeroSemana = (int?)numSemana.Value;
                        contexto.objConsulta.Anio = (int?)numSemana.Value;
                        break;

                    case Enumeraciones.Periodo.MES:
                        contexto.objConsulta.Tipo = Enumeraciones.Periodo.MES.ToString();
                        contexto.objConsulta.Mes = (int?)cbxMeses.SelectedValue;
                        contexto.objConsulta.Anio = (int?)numAnioMes.Value;
                        break;

                    default:
                        contexto.objConsulta.Tipo = "SIN PERIODO";
                        break;
                }


                tsCargandoInformacion.Text = "Cargando información...";
                backgroundWorker1.RunWorkerAsync();
            }

               
        }
    }
}
