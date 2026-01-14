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
using static ControlPagoLotes.Enumeraciones;
using System.Collections;
using Color = System.Drawing.Color;
using Path = System.IO.Path;
using Org.BouncyCastle.Asn1;

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
        private List<PagoPartida> ListaPartidas;

        private bool guardoEncabezado = false;
        private decimal acumulado = 0;

        private string clipboardText;
        private int indexActual = -1;
        List<clsCELDASPAGOS> ListaCeldas;
        List<clsCELDASPAGOS> ListaCeldasNuevas;
        List<clsCELDASPAGOS> ListaCeldasModificar;


        PagoPartida objPagoInicial;

        List<string> LstValidacionesmsj;
        List<string> LstValidacionesAtrasos;

        private bool pagoAtrasado = false;

        private decimal montoAtrasado = 0;
        private decimal montoMensualidad = 0;
        private decimal montoPagadoAcumulado = 0;  
        private decimal montoPreliminarmentePagado = 0; 

        private string observaciones;


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

            // btnAddPago.Enabled = true;
            listaZonas = contextoZonas.GetAllZonas();
            cbxZona.DataSource = listaZonas;
            cbxZona.DisplayMember = "Nombre";
            cbxZona.ValueMember = "Id";
            cbxZona.SelectedIndex = -1;

            var listaEstados = Enum.GetValues(typeof(Enumeraciones.Estados))
                .Cast<Enumeraciones.Estados>()
                .Select(x => new { Id = Convert.ToInt32(x), Nombre = x.ToString() })
                .ToList();

            cbxEstados.DataSource = listaEstados;
            cbxEstados.DisplayMember = "Nombre";
            cbxEstados.ValueMember = "Id";

            bool nuevo = idRegistro == null;

            if (!nuevo)
            {
                Obj = contextoPago.GetPagoById((int)idRegistro);

                txtNombreCliente.Text = Obj.NombreCliente;
                txtDiaPago.Text = Obj.DiaPago;
                cbxZona.SelectedValue = Obj.ZonaId;
                cbxEstados.SelectedValue = Convert.ToInt32(Obj.Estado);
                txtLotes.Text = Obj.Lotes;
                txtMeses.Text = Obj.Meses;
                txtTotal.Text = Obj.Total.ToString("N2");
                txtTelefono.Text = Obj.Telefonos;
                dtpFechaContrato.Value = Obj.FechaRegistro;
                //obtiene las partidas modificadas y nuevas....excluye los eliminados
                ListaPartidas = contextoPagoPartida.GetAllPagoPartidas((int)idRegistro);

                if (ListaPartidas != null && ListaPartidas.Count > 0)
                {
                    ListaCeldas = new List<clsCELDASPAGOS>();

                    foreach (var r in ListaPartidas.OrderBy(x => x.Fecha))
                    {
                        dgvRegistros.Rows.Add(r.Id, r.Monto, r.Fecha.ToShortDateString(), false, false, r.FormaPago);
                        ListaCeldas.Add(new clsCELDASPAGOS
                        {
                            Id = r.Id,
                            Monto = r.Monto.ToString("N2"),
                            Fecha = r.Fecha.ToString("dd/MM/yyyy"),
                            FormaPago = r.FormaPago
                        });
                    }

                    MostrarCeldasEnDgv();

                    // Validar atrasos y actualizar estado automáticamente
                    List<string> lstValidacionesPago = ValidarAtrasoPago();

                    // El método ValidarAtrasoPago ahora actualiza:
                    // 1. El estado en la base de datos (si cambia)
                    // 2. La variable pagoAtrasado
                    // 3. La etiqueta lblInformacionPago
                    // Solo necesitamos sincronizar el combobox
                    cbxEstados.SelectedValue = Convert.ToInt32(Obj.Estado);

                    // Mostrar mensajes de validación si existen
                    if (lstValidacionesPago.Count > 0 && Obj.Estado != ((int)Enumeraciones.Estados.PAGADO).ToString())
                    {
                        string msjAlert = string.Join("\n", lstValidacionesPago);
                        MessageBox.Show("Validaciones detectadas:\n" + msjAlert,
                            "Información del Pago",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                else
                {
                    tsTotalRegistros.Text = @"0";
                    tsAcumulado.Text = @"0.00";

                    // Si no hay partidas, mostrar información básica
                    if (Obj.Estado == ((int)Enumeraciones.Estados.PAGADO).ToString())
                    {
                        lblInformacionPago.Text = "EL CONTRATO HA SIDO LIQUIDADO.";
                    }
                    else
                    {
                        lblInformacionPago.Text = $"Mensualidad: $ {montoMensualidad.ToString("N2")}\n" +
                                                  $"Atraso: $ 0.00";
                    }
                }

            }
            else
            {
                lblInformacionPago.Text = "";
            }

            //preguntar si el restante 
            tsRestante.Text = Obj == null ? "0.00" : (Obj.Total - acumulado).ToString("N2");

            InicializarBotonObservaciones();
        }

        private void ActualizarEtiquetaInformacionPago()
        {
            if (Obj == null) return;

            string estadoActual = Obj.Estado;

            if (estadoActual == ((int)Enumeraciones.Estados.PAGADO).ToString())
            {
                lblInformacionPago.Text = "EL CONTRATO HA SIDO LIQUIDADO (PAGADO COMPLETAMENTE).";
                return;
            }

            if (estadoActual == ((int)Enumeraciones.Estados.CANCELADO).ToString())
            {
                lblInformacionPago.Text = "EL CONTRATO HA SIDO CANCELADO.";
                return;
            }

            // Normalizar atraso / saldo a favor
            decimal atraso = Math.Max(0, montoAtrasado);
            decimal saldoAFavor = Math.Max(0, montoPagadoAcumulado - montoPreliminarmentePagado);

            lblInformacionPago.Text =
                $"Mensualidad: $ {montoMensualidad:N2}\n" +
                $"Atraso: $ {atraso:N2}\n" +
                $"Saldo a favor: $ {saldoAFavor:N2}";
        }

        private void InicializarBotonObservaciones()
        {
            if (Obj != null && !string.IsNullOrEmpty(Obj.Observacion))
            {
                btnObservaciones.Image = Properties.Resources.comentariocargado;
            }
            else
            {
                btnObservaciones.Image = Properties.Resources.sincomentario;
            }
        }

        private void InicializarDgv()
        {
            indexActual = -1;
            dgvRegistros.ColumnCount = 5;//id
            dgvRegistros.Columns[0].Visible = false; //id
            dgvRegistros.Columns[3].Visible = false; //id
            dgvRegistros.Columns[4].Visible = false; //id
            dgvRegistros.Columns[1].HeaderText = "MONTO";
            dgvRegistros.Columns[2].HeaderText = "FECHA";

            dgvRegistros.AllowUserToAddRows = false;
            dgvRegistros.ReadOnly = false;

            dgvRegistros.Columns[1].Width = 200;
            dgvRegistros.Columns[2].Width = 200;

            // Crear y agregar la columna ComboBox para "FORMA PAGO"
            DataGridViewComboBoxColumn comboColumn = new DataGridViewComboBoxColumn
            {
                HeaderText = "FORMA PAGO",
                Name = "FormaPagoColumn",
                DataSource = Enum.GetValues(typeof(FormaPago))
                        .Cast<FormaPago>()
                        .Select(e => new { Id = (int)e, Nombre = e.ToString() })
                        .ToList(),
                // DataPropertyName = "FormaPago",
                DisplayMember = "Nombre",
                ValueMember = "Id",
                Width = 200
            };

            dgvRegistros.Columns.Add(comboColumn);


        }

        private void InicializarVariables()
        {
            listaZonas = null;
            Obj = null;
            ListaPartidas = null;
            guardoEncabezado = false;
            acumulado = 0;
            clipboardText = "";
            indexActual = -1;

            ListaCeldas = null;
            ListaCeldasNuevas = null;
            ListaCeldasModificar = null;

            pagoAtrasado = false;
            lblInformacionPago.Text = "";

        }

        private void GuardarPago()
        {
            DateTime fechaServidor = Global.FechaServidor();

            if (Obj == null)
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
                    Telefonos = txtTelefono.Text,
                    Observacion = observaciones
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
                Obj.Telefonos = txtTelefono.Text;
                
                guardoEncabezado = contextoPago.UpdatePago(Obj);
            }

            if (((int)cbxEstados.SelectedValue == Convert.ToInt32(Enumeraciones.Estados.ATRASADO)))
            {
                MessageBox.Show("Se guardo la información del cliente pero no se guardarón los pagos debido a que el contrato esta en estado " + Enumeraciones.Estados.ATRASADO + ".", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (guardoEncabezado && (ListaCeldas == null || ListaCeldas.Count <= 0))
            {
                MessageBox.Show("Se ha guardado correctamente la información del cliente en la boleta.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                InicializarModulo();
            }
            else
            {
                bool guardadoPartidasCorrecto = false;
                ListaCeldasNuevas = ListaCeldas.Where(x => x.Id == null).ToList();
                ListaCeldasModificar = ListaCeldas.Where(x => x.Id != null && (x.Modificar == true || x.Eliminar == true)).ToList();
                StringBuilder scriptInsert;

                if (ListaCeldasNuevas.Count > 0)
                {
                    scriptInsert = new StringBuilder("INSERT INTO PAGOSPARTIDAS (PagoId, Monto, Fecha, UsuarioId, FechaCreacion, FormaPago, MontoOriginal, FechaModificacion, UsuarioModificoId) VALUES ");


                    foreach (var item in ListaCeldasNuevas)
                    {
                        if (decimal.TryParse(item.Monto, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out decimal monto) && (DateTime.TryParse(item.Fecha, out DateTime fecha)))
                        {
                            PagoPartida partida = new PagoPartida
                            {
                                PagoId = Obj.Id,
                                Monto = monto,
                                Fecha = fecha,
                                UsuarioId = Global.ObjUsuario.Id,
                                FechaCreacion = fechaServidor,
                                FormaPago = item.FormaPago,
                                MontoOriginal = 0,
                                FechaModificacion = null,
                                UsuarioModificoId = null

                            };



                            scriptInsert.AppendFormat("({0}, {1}, '{2}', {3}, '{4}', '{5}', {6}, {7}, {8}), ",
                                    partida.PagoId,
                                    partida.Monto,
                                    partida.Fecha.ToString("yyyy-MM-dd"),
                                    partida.UsuarioId,
                                    partida.FechaCreacion.ToString("yyyy-MM-dd HH:mm:ss"),
                                    partida.FormaPago,
                                    0,
                                    "NULL",
                                    "NULL"
                                );




                        }
                    }

                    scriptInsert.Length -= 2;
                    scriptInsert.Append(";");


                    guardadoPartidasCorrecto = (ListaCeldasNuevas.Count == contextoPagoPartida.InsertarPartidasPago(scriptInsert.ToString()));
                }
                else
                {
                    guardadoPartidasCorrecto = true;
                }



                if (!guardadoPartidasCorrecto)
                {
                    MessageBox.Show("Error al guardar los pagos en la boleta, intente cerrar y abrir el módulo nuevamente.", "Error en la operación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    //continuar con las modificaciones en partidas
                    if (ListaCeldasModificar.Count > 0)
                    {
                        scriptInsert = new StringBuilder();
                        foreach (var item in ListaCeldasModificar)
                        {
                            scriptInsert.AppendLine($@"
                                UPDATE PagosPartidas
                                SET 
                                    Monto = {Global.FormatearPesosADecimal(item.Monto)}, 
                                    Fecha = '{Global.FormatearFecha(item.Fecha, "yyyy-MM-dd")}',
                                    FormaPago = '{item.FormaPago}', 
                                    MontoOriginal = {ListaPartidas.FirstOrDefault(x => x.Id == item.Id).Monto}, 
                                    FechaModificacion = '{fechaServidor:yyyy-MM-dd HH:mm:ss}', 
                                    UsuarioModificoId = '{Global.ObjUsuario.Id}',
                                    FechaBaja = {((item.Eliminar == true) ? "'" + fechaServidor.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL")},
                                    UsuarioBajaId = {((item.Eliminar == true) ? Global.ObjUsuario.Id.ToString("N0") : "NULL")}
                                WHERE Id = {item.Id};
                            ");
                        }

                        string query = scriptInsert.ToString();

                        guardadoPartidasCorrecto = (ListaCeldasModificar.Count == contextoPagoPartida.InsertarPartidasPago(scriptInsert.ToString()));
                    }

                    if (guardadoPartidasCorrecto)
                    {
                        MessageBox.Show("Los pagos de la boleta se han guardado correctamente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        InicializarModulo();
                    }



                }


            }





        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Desea regresar a la búsqueda general de pagos?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }

        }

        private void btnAddPago_Click(object sender, EventArgs e)
        {
            if (!Validaciones())
            {
                GuardarPago();
            }
            else
            {
                string msjAlert = string.Join(". \n", LstValidacionesmsj);
                MessageBox.Show("Olvido llenar la siguiente información: \n" + msjAlert, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }

        private bool Validaciones()
        {
            LstValidacionesmsj = new List<string>();

            if (string.IsNullOrEmpty(txtNombreCliente.Text))
            {
                LstValidacionesmsj.Add("*No ha ingresado el NOMBRE del CLIENTE");
            }
            if (string.IsNullOrEmpty(txtTotal.Text))
            {
                LstValidacionesmsj.Add("*No ha ingresado el MONTO del lote");
            }
            if (string.IsNullOrEmpty(txtMeses.Text))
            {
                LstValidacionesmsj.Add("*No ha ingresado el número de MESES a pagar");
            }
            if (string.IsNullOrEmpty(txtDiaPago.Text))
            {
                LstValidacionesmsj.Add("*No ha ingresado el  DÍA DE PAGO");
            }
            if (string.IsNullOrEmpty(txtLotes.Text))
            {
                LstValidacionesmsj.Add("*No ha ingresado el LOTE");
            }

            if (cbxEstados.SelectedIndex == -1)
            {
                LstValidacionesmsj.Add("*No ha seleccionado el ESTADO de la boleta");
            }
            if (cbxZona.SelectedIndex == -1)
            {
                LstValidacionesmsj.Add("*No ha seleccionado la ZONA");
            }
            if (dtpFechaContrato.Value == null)
            {
                LstValidacionesmsj.Add("*No ha seleccionado la FECHA de contrato");
            }

            return LstValidacionesmsj.Count > 0;


        }

        private void pegarContenidoToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                            ListaCeldas.Add(new clsCELDASPAGOS { Monto = cells[0], Fecha = cells[1], FormaPago = cells.Length == 2 ? 0 : Convert.ToInt32(cells[2]) });
                        }
                        else if (indexActual > -1)
                        {
                            ListaCeldas.Insert(indexActual, new clsCELDASPAGOS { Monto = cells[0], Fecha = cells[1], FormaPago = cells.Length == 2 ? 0 : Convert.ToInt32(cells[2]) });
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
            int rowIndex = 0;

            if (!ListaCeldas.Any(x => x.Fecha == null || string.IsNullOrEmpty(x.Fecha)))
            {
                ListaCeldas = ListaCeldas.OrderBy(x =>
                {
                    string[] formatos = {
                        "dd-MM-yyyy", "d-MM-yyyy", "dd-M-yyyy", "d-M-yyyy",
                        "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/M/yyyy",
                        "dd-MM-yy", "d-MM-yy", "dd-M-yy", "d-M-yy",
                        "dd/MM/yy", "d/MM/yy", "dd/M/yy", "d/M/yy",
                        "M/d/yyyy", "M/d/yy", "MM/dd/yyyy", "MM/dd/yy",
                        "yyyy-MM-dd", "yyyy/MM/dd", "yy-MM-dd", "yy/MM/dd"
                    };

                    if (DateTime.TryParseExact(x.Fecha, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
                    {
                        return fecha;
                    }

                    // Opción 1: Poner fechas no válidas al principio
                    return DateTime.MinValue;

                    // Opción 2: Poner fechas no válidas al final
                    // return DateTime.MaxValue;

                    // Opción 3: Lanzar excepción para fechas no válidas
                    // throw new FormatException($"Formato de fecha no reconocido: {x.Fecha}");
                }).ToList();

            }

            foreach (var item in ListaCeldas)
            {
                dgvRegistros.Rows.Add(item.Id, item.Monto, item.Fecha, item.Modificar, item.Eliminar, item.FormaPago);

                if ((item.Eliminar == null || item.Eliminar == false) && decimal.TryParse(item.Monto, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out decimal monto))
                {
                    acumulado += monto;
                }
                else
                {
                    acumulado += 0;
                }

                if (item.Eliminar == true)
                {
                    dgvRegistros.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Orange;
                }

                rowIndex++;

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
                //ListaCeldas.Insert(indexActual, new clsCELDASPAGOS());
                ListaCeldas.Add(new clsCELDASPAGOS());
            }          

            MostrarCeldasEnDgv();

        }

        private void EliminarPartida()
        {
            if (dgvRegistros.Rows.Count <= 0) return;

            
            if (dgvRegistros.CurrentRow.Cells[0].Value != null)
            {
                if (string.IsNullOrEmpty(dgvRegistros.CurrentRow.Cells[0].Value.ToString()))
                {
                    int index = ListaCeldas.FindIndex(x => x.Fecha == dgvRegistros.CurrentRow.Cells[2].Value.ToString());
                    ListaCeldas[index].Eliminar = ListaCeldas[index].Eliminar != true;
                }
                else
                {
                    int index = ListaCeldas.FindIndex(x => x.Id == (int)dgvRegistros.CurrentRow.Cells[0].Value);
                    ListaCeldas[index].Eliminar = ListaCeldas[index].Eliminar != true;
                }
               
            }
            else
            {
                indexActual = dgvRegistros.CurrentRow.Index;
                ListaCeldas.RemoveAt(indexActual);
            }


            MostrarCeldasEnDgv();
        }

        private void dgvRegistros_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRegistros.RowCount < 1) return;
            if (Obj == null)
            {
                MessageBox.Show("Primero debe guardar la boleta para poder modificar las partidas. Confirme el pago y cargue la boleta e intentelo nuevamente.", "´No se puede realizar la operación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else
            {
                ModificarValoresCeldas(e.RowIndex, e.ColumnIndex, dgvRegistros.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? null);
            }

        }

        private void ModificarValoresCeldas(int row, int cell, string valor)
        {
            if (dgvRegistros.RowCount < 1) return;
            if (ListaCeldas == null || ListaCeldas.Count < 1) return;            

            if (cell == 1)
            {
                ListaCeldas[row].Monto = valor;
            }
            else if (cell == 2)
            {
                ListaCeldas[row].Fecha = valor;
            }
            else if (cell == 5)
            {
                ListaCeldas[row].FormaPago = String.IsNullOrEmpty(valor) ? 0 : Convert.ToInt32(valor);
            }

            PagoPartida obj;

            obj = ListaPartidas.FirstOrDefault(x => x.Id == ListaCeldas[row].Id);

            if (obj != null && (ListaCeldas[row].Fecha != obj.Fecha.ToString("dd/MM/yyyy") || ListaCeldas[row].FormaPago != obj.FormaPago || Global.FormatearPesosADecimal(ListaCeldas[row].Monto) != obj.Monto))
            {
                ListaCeldas[row].Modificar = true;
            }

            MostrarCeldasEnDgv();

        }

        private List<string> ValidarAtrasoPago()
        {
            List<string> msj = new List<string>();

            // VALIDACIÓN PARA ESTADOS FINALES: PAGADO o CANCELADO
            if (Obj.Estado == ((int)Enumeraciones.Estados.PAGADO).ToString())
            {
                msj.Add("La boleta ha sido PAGADA completamente.");
                LstValidacionesAtrasos = msj;
                ActualizarEtiquetaInformacionPago();
                return msj;
            }

            if (Obj.Estado == ((int)Enumeraciones.Estados.CANCELADO).ToString())
            {
                msj.Add("La boleta ha sido CANCELADA.");
                LstValidacionesAtrasos = msj;
                ActualizarEtiquetaInformacionPago();
                return msj;
            }

            bool cambiarEstadoACorriente = false;
            bool cambiarEstadoAAtrasado = false;

            DateTime fechaActual = Global.FechaServidor();

            // Verificar si ya está pagado completamente
            if (ListaPartidas.Sum(x => x.Monto) >= Obj.Total && Obj.Estado != ((int)Enumeraciones.Estados.PAGADO).ToString())
            {
                Obj.Estado = ((int)Enumeraciones.Estados.PAGADO).ToString();
                contextoPago.UpdatePago(Obj);
                msj.Add("La boleta ha sido PAGADA completamente.");
                LstValidacionesAtrasos = msj;
                ActualizarEtiquetaInformacionPago();
                return msj;
            }

            PagoPartida ultimoPago = ListaPartidas.OrderByDescending(x => x.Fecha).First();
            int noPagos = Convert.ToInt32(Obj.Meses);
            decimal total = Obj.Total;

            objPagoInicial = ListaPartidas
                .OrderBy(x => x.Fecha)
                .First(x => x.FechaBaja == null);

            // USAR VARIABLES DE CLASE
            montoPagadoAcumulado = (ListaPartidas == null || ListaPartidas.Count <= 0) ? 0 : ListaPartidas.Sum(x => x.Monto);

            // Si tu lógica considera el primer pago como "enganche", lo excluimos
            montoPagadoAcumulado -= objPagoInicial.Monto;

            // ✅ Meses transcurridos HASTA EL MES ACTUAL (sin restar por día)
            int mesesTranscurridos = ((fechaActual.Year - objPagoInicial.Fecha.Year) * 12) +
                                     (fechaActual.Month - objPagoInicial.Fecha.Month);

            if (mesesTranscurridos < 0) mesesTranscurridos = 0;
            if (mesesTranscurridos > noPagos) mesesTranscurridos = noPagos;

            // Cálculos
            montoPreliminarmentePagado = 0;
            montoMensualidad = ((total - objPagoInicial.Monto) / noPagos);

            montoPreliminarmentePagado = montoMensualidad * mesesTranscurridos;

            // montoAtrasado puede ser negativo (eso significa saldo a favor)
            montoAtrasado = montoPreliminarmentePagado - montoPagadoAcumulado;

            // ✅ Meses desde el último pago HASTA EL MES ACTUAL (sin restar por día)
            int mesesDesdeUltimoPago = ((fechaActual.Year - ultimoPago.Fecha.Year) * 12) +
                                       (fechaActual.Month - ultimoPago.Fecha.Month);

            if (mesesDesdeUltimoPago < 0) mesesDesdeUltimoPago = 0;

            // Normalizados
            decimal atraso = Math.Max(0, montoAtrasado);
            decimal saldoAFavor = Math.Max(0, montoPagadoAcumulado - montoPreliminarmentePagado);

            // VALIDACIÓN PRINCIPAL
            if (saldoAFavor > 0)
            {
                if (mesesDesdeUltimoPago > 3)
                {
                    cambiarEstadoAAtrasado = true;
                    msj.Add($"*El cliente tiene saldo a favor de $ {saldoAFavor:N2}, pero lleva {mesesDesdeUltimoPago} meses sin realizar pagos.");
                    msj.Add("*Estado cambiado a ATRASADO por inactividad en pagos.");
                }
                else
                {
                    cambiarEstadoACorriente = true;
                    msj.Add($"*El pago está al corriente con saldo a favor de $ {saldoAFavor:N2}.");
                    msj.Add("*Estado cambiado a CORRIENTE (saldo positivo).");
                }
            }
            else if (atraso > 0)
            {
                msj.Add($"*El monto acumulado a la boleta $ {montoPagadoAcumulado:N2} está por debajo del estimado de $ {montoPreliminarmentePagado:N2}.");
                msj.Add($"*Atraso actual: $ {atraso:N2}");

                if (mesesDesdeUltimoPago > 3)
                {
                    msj.Add($"*No se han recibido pagos en {mesesDesdeUltimoPago} meses.");
                    cambiarEstadoAAtrasado = true;
                }
            }
            else
            {
                msj.Add("*El pago está exactamente al día (sin atraso ni saldo a favor).");

                if (mesesDesdeUltimoPago > 3)
                {
                    msj.Add($"*Aunque está al día, no se han recibido pagos en {mesesDesdeUltimoPago} meses.");
                    cambiarEstadoAAtrasado = true;
                }
                else
                {
                    cambiarEstadoACorriente = true;
                }
            }

            // Aplicar cambios de estado (solo si no es PAGADO o CANCELADO)
            if (cambiarEstadoACorriente && Obj.Estado != ((int)Enumeraciones.Estados.CORRIENTE).ToString())
            {
                Obj.Estado = ((int)Enumeraciones.Estados.CORRIENTE).ToString();
                contextoPago.UpdatePago(Obj);
                cbxEstados.SelectedValue = (int)Enumeraciones.Estados.CORRIENTE;
                pagoAtrasado = false;
            }
            else if (cambiarEstadoAAtrasado && Obj.Estado != ((int)Enumeraciones.Estados.ATRASADO).ToString())
            {
                Obj.Estado = ((int)Enumeraciones.Estados.ATRASADO).ToString();
                contextoPago.UpdatePago(Obj);
                cbxEstados.SelectedValue = (int)Enumeraciones.Estados.ATRASADO;
                pagoAtrasado = true;
            }

            // Actualizar etiqueta SIEMPRE
            ActualizarEtiquetaInformacionPago();

            if (msj.Count > 0)
                LstValidacionesAtrasos = msj;

            return msj;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private void MensajePorEstado(int estado)
        {
            string msj = null;
            string titulo = null;
            MessageBoxIcon tipo = MessageBoxIcon.None;

            switch (estado)
            {
                case 2:
                    titulo = "Aviso";
                    msj = "El contrado ha sido " + Enumeraciones.Estados.PAGADO + ".";
                    tipo = MessageBoxIcon.Information;
                    break;

                case 3:
                    titulo = "Aviso";
                    msj = "El contrado ha sido " + Enumeraciones.Estados.CANCELADO + ".";
                    tipo = MessageBoxIcon.Information;
                    break;

                case 4:
                    titulo = "ADVERTENCIA";
                    msj = string.Join(Environment.NewLine, LstValidacionesAtrasos); //"El cliente no ha presentado mensualidad en más de 3 meses consecutivos.";
                    tipo = MessageBoxIcon.Information;
                    break;

                case 5:
                    titulo = "ADVERTENCIA";
                    msj = "El pago tiene el estado " + Enumeraciones.Estados.DESCONOCIDO + ", verifique la inforción y guarde los cambios. ";
                    tipo = MessageBoxIcon.Information;
                    break;
            }


            if (!string.IsNullOrEmpty(msj))
            {
                MessageBox.Show(msj, titulo, MessageBoxButtons.OK, tipo);
            }

        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (Obj != null && ListaCeldas != null && ListaCeldas.Count > 0)
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

        private void ExportarExcel(Pago ObjPago, List<clsCELDASPAGOS> ListaPartidas, string rutaArchivo)
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

                //obsevaciones
                var rangoObservacion = worksheet.Range("B5:C5");
                rangoObservacion.Merge();
                rangoObservacion.Value = Obj.Observacion;

                //encabezado partidas
                var rangoEncabezadoPartidas = worksheet.Range("B6:C6");
                rangoEncabezadoPartidas.Style.Fill.BackgroundColor = XLColor.ForestGreen;
                rangoEncabezadoPartidas.Style.Font.FontSize = 12;
                worksheet.Cell(6, 2).Value = "MONTO";
                worksheet.Cell(6, 3).Value = "FECHA";

                for (int pos = 0; pos < noPagos; pos++)
                {
                    worksheet.Cell(6 + pos + 1, 1).Value = pos + 1;
                    worksheet.Cell(6 + pos + 1, 2).Value = "$ " + ListaCeldas[pos].Monto;
                    worksheet.Cell(6 + pos + 1, 3).Value = ListaCeldas[pos].Fecha;
                    Console.WriteLine(ListaCeldas[pos].Monto);
                    Acumulado += Convert.ToDecimal(ListaCeldas[pos].Monto);
                }

                worksheet.Cell(6 + (3 + noPagos), 2).Value = "Saldo ($):";
                worksheet.Cell(6 + (3 + noPagos), 2).Style.Font.FontSize = 12;
                worksheet.Cell(6 + (3 + noPagos), 2).Style.Font.Bold = true;

                worksheet.Cell(6 + (3 + noPagos), 3).Value = Acumulado;
                worksheet.Cell(6 + (3 + noPagos), 3).Style.Font.FontSize = 12;
                worksheet.Cell(6 + (3 + noPagos), 3).Style.Font.Bold = true;



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
                    GenerarBoletaPdf(rutaArchivo);
                }
            }
        }

        private void GenerarBoletaPdf(string rutaArchivo)
        {
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
                    tableEncabezado.AddCell(new Cell(1, 2).Add(new Paragraph(Obj.NombreCliente))).SetFont(boldFont);
                    tableEncabezado.AddCell(new Cell(2, 1).Add(new Paragraph("$ " + Obj.Total.ToString("N2")))).SetFont(boldFont);
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
                    decimal acumulado = 0;
                    for (int i = 0; i < ListaCeldas.Count; i++)
                    {
                        table.AddCell($"{i + 1}");
                        table.AddCell($"{"$ " + ListaCeldas[i].Monto}");
                        table.AddCell($"{ListaCeldas[i].Fecha}");

                        acumulado += Global.FormatearPesosADecimal(ListaCeldas[i].Monto);
                    }
                    //agregar total
                    table.AddCell(new Cell(4 + ListaCeldas.Count + 1, 1).Add(new Paragraph("Acumulado:")));
                    table.AddCell(new Cell(4 + ListaCeldas.Count + 1, 1).Add(new Paragraph("$ " + acumulado.ToString("N2"))));

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

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            EnviarRecibo();
        }

        private void EnviarRecibo()
        {

            if (MessageBox.Show("¿Desea enviar la boleta de pago?", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {


                //bucar si existe el archivo tmeporal
                string rutaMisDocumentos = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Nombre del archivo que buscas
                string nombreArchivo = "envio.pdf";

                // Combinar la ruta de "Mis Documentos" con el nombre del archivo
                string rutaCompleta = Path.Combine(rutaMisDocumentos, nombreArchivo);


                GenerarBoletaPdf(rutaCompleta);

                // Verificar si el archivo existe
                if (File.Exists(rutaCompleta))
                {
                    formEnvioArchivos frm = new formEnvioArchivos(rutaCompleta);
                    frm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("No se genero la boleta, verifique si existe en MIS DOCUMENTOS el arcvhivo envio.pdf e intentelo nuevamente.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }





            }

        }

        private void limpiarPagosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvRegistros.Rows.Count <= 0) return;

            if (MessageBox.Show("¿Estás seguro de eliminar todos los pagos que se han cargado a la boleta.?",
                "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                EliminarPagosRelacionados();
            }
        }

        private void EliminarPagosRelacionados()
        {
            ListaCeldas.ForEach(x => x.Eliminar = true);

            MostrarCeldasEnDgv();

        }

        private void dgvRegistros_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // Mostrar el número de fila comenzando desde 1
            var grid = sender as DataGridView;
            using (SolidBrush brush = new SolidBrush(grid.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString(
                    (e.RowIndex + 1).ToString(),
                    grid.RowHeadersDefaultCellStyle.Font,
                    brush,
                    e.RowBounds.Location.X + 20,
                    e.RowBounds.Location.Y + 4
                );
            }
        }

        private void btnObservaciones_Click(object sender, EventArgs e)
        {
            formObservaciones obs;
            if(Obj!=null && !string.IsNullOrEmpty(Obj.Observacion))
            {
                obs = new formObservaciones(Obj.Observacion);
            }else if(Obj==null && !string.IsNullOrEmpty(observaciones))
            {
                obs = new formObservaciones(observaciones);
            }
            else
            {
                obs = new formObservaciones();
            }

            obs.ShowDialog();

            if (!string.IsNullOrEmpty(obs.observacion))
            {
                if (Obj != null)
                {
                    Obj.Observacion = obs.observacion;
                }
                else
                {
                    observaciones = obs.observacion;
                }

                btnObservaciones.Image = Properties.Resources.comentariocargado;

            }
            else
            {
                if (Obj != null)
                {
                    Obj.Observacion = null;
                }
                else
                {
                    observaciones = null;
                }
                    
                btnObservaciones.Image = Properties.Resources.sincomentario;
            }




        }
    }
}
