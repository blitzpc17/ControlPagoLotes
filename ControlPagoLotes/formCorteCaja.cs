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
        private Enumeraciones.Meses mesSeleccionado;
        private List<long> _targetConnections = null;
        private DataGridView dgvConexiones;
        private CheckedListBox clbLotificaciones;
        private Label lblUsuarios;
        private CheckedListBox clbUsuarios;
        private CheckBox chkTodosUsuarios;
        private List<Zona> _zonasDisponibles;
        private HashSet<int> _zonasSeleccionadas = new HashSet<int>();
        private HashSet<string> _usuariosSeleccionados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public formCorteCaja()
        {
            InitializeComponent();
        }

        private async void InicializarFormulario()
        {
            contexto = new PagoPartidaLogica();
           
            InicializarControles();
           
            // Verify offline connections
            var offlineConnections = await contexto.CheckAndDisableOfflineConnectionsAsync();
            if (offlineConnections.Count > 0)
            {
                Label lblOfflineWarning = new Label
                {
                    Text = "ADVERTENCIA: Las siguientes plazas están desconectadas y no se incluyen en el corte: " + string.Join(", ", offlineConnections),
                    ForeColor = Color.Red,
                    Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                    Dock = DockStyle.Bottom,
                    Height = 25,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                this.panel1.Controls.Add(lblOfflineWarning);
                this.panel1.Height += 25;
                this.dgvRegistros.Top += 25;
                this.dgvRegistros.Height -= 25;
            }
        }

        private void InicializarControles()
        {
            Global.LimpiarControles(this);
            ComboBoxHelper.LlenarComboBox<Enumeraciones.Periodo>(cbxPeriodo, true);
            cbxPeriodo.SelectedIndex = -1;
            ComboBoxHelper.LlenarComboBox<Enumeraciones.Meses>(cbxMeses, true);
            cbxMeses.SelectedIndex = -1;

            numericAnioMes.Value = DateTime.Now.Year;
            numAnioSemana.Value = DateTime.Now.Year;

            AgregarFiltrosAvanzados();
        }

        public Dictionary<long, string> GetReglasActuales()
        {
            var reglas = new Dictionary<long, string>();
            if (dgvConexiones != null)
            {
                foreach (DataGridViewRow row in dgvConexiones.Rows)
                {
                    long id = (long)row.Cells["Id"].Value;
                    string regla = row.Cells["Regla"].Value.ToString();
                    reglas.Add(id, regla);
                }
            }
            return reglas;
        }

        private void AgregarFiltrosAvanzados()
        {
            cbxLotificaciones.Visible = false;
            chkTodas.Visible = false; // Hide original checkbox
            label7.Visible = false; // Hide original label "Lotificación:"

            // --- CONEXIONES GRID ---
            int conX = 12;
            var labelConexion = new Label { AutoSize = true, Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold), Location = new Point(conX, 5), Text = "Conexiones:" };
            dgvConexiones = new DataGridView { Location = new Point(conX, 25), Size = new Size(270, 85), AllowUserToAddRows = false, AllowUserToDeleteRows = false, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, BackgroundColor = Color.White };
            
            var colId = new DataGridViewTextBoxColumn { Name = "Id", Visible = false };
            var colPlaza = new DataGridViewTextBoxColumn { Name = "Plaza", HeaderText = "Plaza", ReadOnly = true, Width = 110 };
            var colRegla = new DataGridViewComboBoxColumn { Name = "Regla", HeaderText = "Filtro", Width = 130 };
            colRegla.Items.AddRange("OMITIR", "TODAS", "ASIGNADAS", "MANUAL");
            
            dgvConexiones.Columns.AddRange(colId, colPlaza, colRegla);

            var conexionesDict = LOGICA.PagoPartidaLogica.GetConexionesDisponibles();
            if (conexionesDict != null)
            {
                foreach (var kvp in conexionesDict)
                {
                    int rowIndex = dgvConexiones.Rows.Add();
                    dgvConexiones.Rows[rowIndex].Cells["Id"].Value = kvp.Key;
                    dgvConexiones.Rows[rowIndex].Cells["Plaza"].Value = kvp.Value;
                    dgvConexiones.Rows[rowIndex].Cells["Regla"].Value = "TODAS"; // Por defecto
                }
            }
            
            // --- LOTIFICACIONES ---
            int lotiX = 290;
            var labelLoti = new Label { AutoSize = true, Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold), Location = new Point(lotiX, 5), Text = "Zonas (Manual):" };
            var txtFiltroZonas = new TextBox { Location = new Point(lotiX, 25), Size = new Size(190, 22) };
            txtFiltroZonas.TextChanged += (s, e) => ActualizarLotificacionesUI(false, txtFiltroZonas.Text);

            clbLotificaciones = new CheckedListBox { Font = new Font("Microsoft Sans Serif", 10F), Location = new Point(lotiX, 50), Size = new Size(190, 60), CheckOnClick = true };
            clbLotificaciones.ItemCheck += (s, e) => {
                var z = (Zona)clbLotificaciones.Items[e.Index];
                if (e.NewValue == CheckState.Checked) _zonasSeleccionadas.Add(z.Id);
                else _zonasSeleccionadas.Remove(z.Id);
            };
            
            dgvConexiones.CellValueChanged += (s, e) => { if (e.ColumnIndex == colRegla.Index) ActualizarLotificacionesUI(true, txtFiltroZonas.Text); };
            dgvConexiones.CurrentCellDirtyStateChanged += (s, e) => { if (dgvConexiones.IsCurrentCellDirty) dgvConexiones.CommitEdit(DataGridViewDataErrorContexts.Commit); };

            // --- USUARIOS ---
            int userX = 490;
            lblUsuarios = new Label { AutoSize = true, Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold), Location = new Point(userX, 5), Text = "Usuarios:" };
            var txtFiltroUsuarios = new TextBox { Location = new Point(userX, 25), Size = new Size(180, 22) };
            txtFiltroUsuarios.TextChanged += (s, e) => ActualizarUsuariosUI(txtFiltroUsuarios.Text);

            clbUsuarios = new CheckedListBox { Font = new Font("Microsoft Sans Serif", 10F), Location = new Point(userX, 50), Size = new Size(180, 60), Enabled = false, CheckOnClick = true };
            chkTodosUsuarios = new CheckBox { AutoSize = true, Checked = true, Font = new Font("Microsoft Sans Serif", 10F), Location = new Point(userX + 185, 25), Text = "Todos" };
            chkTodosUsuarios.CheckedChanged += (s, e) => { clbUsuarios.Enabled = !chkTodosUsuarios.Checked; };
            clbUsuarios.ItemCheck += (s, e) => {
                var u = (UsuarioL)clbUsuarios.Items[e.Index];
                if (e.NewValue == CheckState.Checked) _usuariosSeleccionados.Add(u.Usuario);
                else _usuariosSeleccionados.Remove(u.Usuario);
            };

            // --- REUBICACION DE CONTROLES ORIGINALES ---
            int perX = 750;
            label1.Location = new Point(perX, 5); // Label Periodo:
            label1.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            cbxPeriodo.Location = new Point(perX, 25);
            cbxPeriodo.Size = new Size(150, 26);
            panelMes.Location = new Point(perX, 55);
            panelSemana.Location = new Point(perX, 55);
            panelDia.Location = new Point(perX, 55);

            // Botones
            int btnX = 1080;
            btnConsultar.Location = new Point(btnX, 5);
            btnConsultar.Size = new Size(140, 32);
            bntExportar.Location = new Point(btnX, 42);
            bntExportar.Size = new Size(140, 32);
            btnCancelar.Location = new Point(btnX, 79);
            btnCancelar.Size = new Size(140, 32);

            panel1.Controls.Add(labelConexion);
            panel1.Controls.Add(dgvConexiones);
            panel1.Controls.Add(labelLoti);
            panel1.Controls.Add(txtFiltroZonas);
            panel1.Controls.Add(clbLotificaciones);
            panel1.Controls.Add(lblUsuarios);
            panel1.Controls.Add(txtFiltroUsuarios);
            panel1.Controls.Add(clbUsuarios);
            panel1.Controls.Add(chkTodosUsuarios);

            ActualizarLotificacionesUI(true, "");
            ActualizarUsuariosUI("");

            panel1.Height = 115; // Mantenemos la altura original
        }

        private void ActualizarLotificacionesUI(bool refetch, string filtroText = "")
        {
            if (refetch || _zonasDisponibles == null)
            {
                var zonaLogic = new ZonaLogica();
                _zonasDisponibles = zonaLogic.GetAllZonas(null, null, true); // Traemos TODO para que elijan MANUALMENTE.
            }

            var reglas = GetReglasActuales();
            List<Zona> zonasFiltradas = new List<Zona>();

            foreach (var z in _zonasDisponibles)
            {
                if (reglas.ContainsKey(z.ConnectionId) && reglas[z.ConnectionId] == "MANUAL")
                {
                    zonasFiltradas.Add(z);
                }
            }

            if (!string.IsNullOrWhiteSpace(filtroText))
            {
                zonasFiltradas = zonasFiltradas.Where(z => z.NombreConPlaza.IndexOf(filtroText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            ((ListBox)clbLotificaciones).DataSource = null;
            ((ListBox)clbLotificaciones).DataSource = zonasFiltradas;
            ((ListBox)clbLotificaciones).DisplayMember = "NombreConPlaza";
            ((ListBox)clbLotificaciones).ValueMember = "Id";

            for (int i = 0; i < clbLotificaciones.Items.Count; i++)
            {
                var z = (Zona)clbLotificaciones.Items[i];
                if (_zonasSeleccionadas.Contains(z.Id)) clbLotificaciones.SetItemChecked(i, true);
            }
            
            clbLotificaciones.Enabled = (zonasFiltradas.Count > 0);
        }

        private void ActualizarUsuariosUI(string filtroText)
        {
            var userLogic = new UsuarioLogica();
            var allUsers = userLogic.GetAllUsuario(true);

            if (!string.IsNullOrWhiteSpace(filtroText))
            {
                allUsers = allUsers.Where(u => u.Usuario.IndexOf(filtroText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            ((ListBox)clbUsuarios).DataSource = null;
            ((ListBox)clbUsuarios).DataSource = allUsers;
            ((ListBox)clbUsuarios).DisplayMember = "Usuario";
            ((ListBox)clbUsuarios).ValueMember = "Id";

            for (int i = 0; i < clbUsuarios.Items.Count; i++)
            {
                var u = (UsuarioL)clbUsuarios.Items[i];
                if (_usuariosSeleccionados.Contains(u.Usuario)) clbUsuarios.SetItemChecked(i, true);
            }
        }

        private void dtpFechaContrato_ValueChanged(object sender, EventArgs e)
        {
            tsCargandoInformacion.Text = "Cargando información...";
            backgroundWorker1.RunWorkerAsync();
        }

        private void ListarPagosPorFecha()
        {
            ListaPagosDiarios = contexto.ListarPagoPorFecha(contexto.objConsulta, Global.ObjUsuario.Id, Global.ObjUsuario.Usuario, _targetConnections);
          
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
            
            contexto.objConsulta.ReglasPorConexion = GetReglasActuales();

            // --- LOTIFICACIONES ---
            contexto.objConsulta.todas = false; 
            contexto.objConsulta.LotificacionesIds = _zonasSeleccionadas.ToList();

            // --- USUARIOS ---
            contexto.objConsulta.todosUsuarios = chkTodosUsuarios.Checked;
            if (!chkTodosUsuarios.Checked)
            {
                if (_usuariosSeleccionados.Count == 0)
                {
                    MessageBox.Show("Debe seleccionar al menos un usuario para realizar la consulta.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                contexto.objConsulta.UsuariosNombres = _usuariosSeleccionados.ToList();
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
                        contexto.objConsulta.Anio = (int?)numAnioSemana.Value;
                        break;

                    case Enumeraciones.Periodo.MES:
                        contexto.objConsulta.Tipo = Enumeraciones.Periodo.MES.ToString();

                        contexto.objConsulta.Mes = (int)mesSeleccionado;//(int?)cbxMeses.SelectedValue;
                        contexto.objConsulta.Anio = (int?)numericAnioMes.Value;
                        break;

                    default:
                        contexto.objConsulta.Tipo = "SIN PERIODO";
                        break;
                }

                contexto.objConsulta.ConexionPrincipalId = null;
                // Siempre consultamos todas las conexiones para poder traer las "extras"
                _targetConnections = null;

                tsCargandoInformacion.Text = "Cargando información...";
                backgroundWorker1.RunWorkerAsync();
            }

               
        }

        private void cbxMeses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxMeses.Items.Count <= 0) return;
            mesSeleccionado = ComboBoxHelper.ObtenerValorSeleccionado<Enumeraciones.Meses>(cbxMeses);
        }
    }
}
