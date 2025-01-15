using Entidades;
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


        public formBoleta(int? idRegistro)
        {
            InitializeComponent();
            this.idRegistro = idRegistro;
        }

        private void formBoleta_Load(object sender, EventArgs e)
        {
            InicializarModulo();
        }

        private void InicializarModulo()
        {
            contextoZonas = new ZonaLogica();
            contextoPago = new PagoLogica();
            contextoPagoPartida = new PagoPartidaLogica();

            InicializarVariables();

            Global.LimpiarControles(this);
         
            dgvRegistros.AllowUserToDeleteRows = true;
            dgvRegistros.AllowUserToAddRows = true;
            dgvRegistros.ReadOnly = false;

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

           /* txtNombreCliente.ReadOnly = !nuevo;
            txtDiaPago.ReadOnly = !nuevo;
            txtLotes.ReadOnly = !nuevo;
            txtMeses.ReadOnly = !nuevo; 
            txtTotal.ReadOnly = !nuevo;
            cbxZona.Enabled = nuevo;*/

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
                    tsAcumulado.Text = ListaPartidas.Sum(x => x.Monto).ToString("N2");

                    if (dgvRegistros.Columns.Count > 0)
                    {
                        dgvRegistros.Rows.Clear();
                        dgvRegistros.Columns.Clear();
                    }

                    dgvRegistros.Columns.Add("Monto", "MONTO");
                    dgvRegistros.Columns.Add("Fecha", "FECHA");

                    dgvRegistros.Columns["Monto"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvRegistros.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;

                    foreach (var r in ListaPartidas)
                    {
                        dgvRegistros.Rows.Add(r.Monto, r.Fecha.ToShortDateString());
                    }
                    tsTotalRegistros.Text = ListaPartidas.Count.ToString("N0");
                }
                else
                {
                    tsTotalRegistros.Text = @"0";
                    tsAcumulado.Text = @"0";
                }          


            }


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
        }

        private void GuardarPago()
        {           
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
                    FechaRegistro = dtpFechaContrato.Value,//DateTime.ParseExact(fechaString, "dd/MM/yyyy", CultureInfo.InvariantCulture);,
                    FechaCreacion = DateTime.Now,
                    Estado = "1"
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
                Obj.FechaRegistro = dtpFechaContrato.Value;//DateTime.ParseExact(fechaString, "dd/MM/yyyy", CultureInfo.InvariantCulture);                
                Obj.Estado = cbxEstados.SelectedValue.ToString();
                

                guardoEncabezado = contextoPago.UpdatePago(Obj);
            } 

            if (guardoEncabezado && dgvRegistros.Rows.Count > 0)
            {
                ListaPartidasAux = new List<PagoPartida>();

                foreach (DataGridViewRow item in dgvRegistros.Rows)
                {

                    if (item.IsNewRow) continue;

                    ListaPartidasAux.Add(new PagoPartida
                    {
                        Monto = Convert.ToDecimal(item.Cells[0].Value),
                        Fecha = Convert.ToDateTime(item.Cells[1].Value),
                        UsuarioId = 1,
                        PagoId = Obj.Id

                    });
                }
                //borrar anteriores

                int eliminados = contextoPagoPartida.EliminarPartidasAnteriores(Obj.Id);

                //guardar partida

                if (ListaPartidas == null || eliminados == ListaPartidas.Count)
                {
                    //insertar
                    string query = "";


                    query += string.Join(",", ListaPartidasAux.Select(item =>
                    $"({Obj.Id}, {item.Monto}, '{item.Fecha.ToString("yyyy-MM-dd")}', 1)"));

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
            PegarCeldas();
        }


        private void PegarCeldas()
        {
            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();

                // Dividir el texto en filas
                string[] rows = clipboardText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                int selectedRowIndex = dgvRegistros.CurrentCell?.RowIndex ?? 0;
                int selectedColIndex = dgvRegistros.CurrentCell?.ColumnIndex ?? 0;

                // Asegurar que haya suficientes filas y columnas en el DataGridView
                int requiredRows = selectedRowIndex + rows.Length;
                int requiredColumns = selectedColIndex + rows[0].Split('\t').Length;

                
                dgvRegistros.Columns.Add("Monto", "MONTO");
                dgvRegistros.Columns.Add("Fecha", "FECHA");

                dgvRegistros.Columns["Monto"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRegistros.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;

                // Agregar filas si es necesario
                while (dgvRegistros.RowCount < requiredRows)
                {
                    dgvRegistros.Rows.Add();
                }

                try
                {
                    // Pegar el contenido en el DataGridView
                    for (int i = 0; i < rows.Length; i++)
                    {
                        if (rows[i] == "") continue; // Saltar filas vacías

                        // Dividir cada fila en columnas
                        string[] cells = rows[i].Split('\t');

                        for (int j = 0; j < cells.Length; j++)
                        {
                            int targetRowIndex = selectedRowIndex + i;
                            int targetColIndex = selectedColIndex + j;

                            // Verificar que no se exceda el límite de filas y columnas
                            if (targetRowIndex < dgvRegistros.RowCount && targetColIndex < dgvRegistros.ColumnCount)
                            {
                                //si es monto limpia caracteres no numericos
                                if(j == 0)
                                {
                                    // Limpiar el contenido de caracteres no deseados
                                    string cleanedValue = LimpiarCaracteres(cells[j]);
                                    dgvRegistros[targetColIndex, targetRowIndex].Value = cleanedValue;
                                }
                                else
                                {
                                    dgvRegistros[targetColIndex, targetRowIndex].Value = cells[j];
                                }
                               
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al pegar los datos: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("No hay datos en el portapapeles para pegar.");
            }
        }
        
        private string LimpiarCaracteres(string input)
        {
            // Usar expresiones regulares para eliminar caracteres no deseados
            return System.Text.RegularExpressions.Regex.Replace(input, @"[^0-9.,]", "");
        }

        private void eliminarFilaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarRow();
        }

        private void EliminarRow()
        {
            // Verificar si hay alguna fila seleccionada
            if (dgvRegistros.CurrentRow != null && !dgvRegistros.CurrentRow.IsNewRow)
            {
                // Eliminar la fila seleccionada
                dgvRegistros.Rows.RemoveAt(dgvRegistros.CurrentRow.Index);
            }
            else
            {
                MessageBox.Show("Seleccione una fila válida para eliminar.");
            }
        } 


        private void dgvRegistros_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (isPasting || e.RowIndex == -1)
                return;
            // Validar solo para la columna específica
            if (e.ColumnIndex == 0)
            {
                string userInput = e.FormattedValue.ToString();

                // Validar si es un número decimal
                if (!decimal.TryParse(userInput, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    // Agregar el error al estilo de la celda
                    dgvRegistros.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Debe ingresar un número decimal válido.";
                    e.Cancel = true; // Cancelar la edición
                    return;
                }
                else
                {
                    // Limpiar el error si el dato es válido
                    dgvRegistros.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = string.Empty;
                   // acumulado += Convert.ToDecimal(dgvRegistros.Rows[e.RowIndex].Cells[0].Value);
                    ActualizarAcumuladoPagos();        
                }
            }

            ActualizarTotalRegistros();

        }

        private void ActualizarTotalRegistros()
        {
            if (dgvRegistros.RowCount<=0 || dgvRegistros.Columns.Count <= 0) return;

            tsTotalRegistros.Text = (dgvRegistros.RowCount - 1).ToString("N0");
        }

        private void ActualizarAcumuladoPagos()
        {
            if (dgvRegistros.RowCount<=0 || dgvRegistros.Columns.Count <= 0) return;

            tsAcumulado.Text = acumulado.ToString("N2");
        }

        private void dgvRegistros_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (isPasting || e.RowIndex == -1)
                return;
            dgvRegistros.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = string.Empty;
        }



    }
}
