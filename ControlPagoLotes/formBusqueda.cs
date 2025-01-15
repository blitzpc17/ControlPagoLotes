using Entidades;
using LOGICA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlPagoLotes
{
    public partial class formBusqueda : Form
    {
        private PagoLogica contexto;
        private List<clsPagosBusqueda> Lista;
        private List<clsPagosBusqueda> ListaAux;
        private int opc = 1;//default nombre cliente
        private int columna = 1;//default por nombre del cliente
        public formBusqueda()
        {
            InitializeComponent();
        }
        private void InicializarModulo()
        {
            contexto = new PagoLogica();
            Lista = contexto.GetAllPagosBusqueda();
            ListaAux = Lista;
            SetDataDatagridView();
        }

        private void CerrarSesion()
        {
            if (MessageBox.Show("¿Desea cerrar la sesión?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question)== DialogResult.Yes)
            {
                Close();
            }
         
        }
        private void NuevaZona()
        {
            formZonas formZonas = new formZonas();
            formZonas.ShowDialog();
            InicializarModulo();
        }
        
        private void SeleccionarRegistro()
        {
            formBoleta formBoleta = new formBoleta((int)dgvRegistros.CurrentRow.Cells[0].Value);
            formBoleta.ShowDialog();
            InicializarModulo();
        }
        private void NuevoUsuario()
        {
            formUsuarios formUsuarios = new formUsuarios();
            formUsuarios.ShowDialog();
            InicializarModulo();
        }

        private void NuevoPago()
        {
            formBoleta formBoleta = new formBoleta(null);
            formBoleta.ShowDialog();
            InicializarModulo();
        }
        private void btnReportes_Click(object sender, EventArgs e)
        {
            formReportes fr = new formReportes();
            fr.ShowDialog();
            InicializarModulo();
        }

        private void Filtrar(string palabra, int columna)
        {
            ListaAux = Lista;

            switch (columna)
            {
                case 1: //cliente
                    ListaAux = ListaAux.Where(x => x.Cliente.Contains(palabra)).OrderBy(x => x.Cliente).ToList();
                    break;

                case 2: // zona
                    ListaAux = ListaAux.Where(x => x.Zona.Contains(palabra)).OrderBy(x => x.Cliente).ToList();
                    break;

                case 3: //lotes
                    ListaAux = ListaAux.Where(x => x.Lotes.Contains(palabra)).OrderBy(x => x.Zona).ThenBy(x=>x.Cliente).ThenBy(x=>x.Lotes).ToList();
                    break;

                case 4: //total
                    ListaAux = ListaAux.Where(x => x.Total.Contains(palabra)).OrderBy(x => x.Zona).ThenBy(x=>x.Cliente).ThenBy(x=>x.Total).ThenBy(x=>x.Lotes).ToList();
                    break;

                case 5: //fecha pago
                    ListaAux = ListaAux.Where(x => x.Fecha.Contains(palabra)).OrderBy(x => x.Fecha).ThenBy(x=>x.Cliente).ThenBy(x=>x.Zona).ToList();
                    break;

                case 7: //estado
                    ListaAux = ListaAux.Where(x => x.ClaveEstado.Contains(palabra)).OrderBy(x => x.ClaveEstado).ThenBy(x=>x.Cliente).ThenBy(x=>x.Zona).ThenBy(x=>x.Lotes).ToList();
                    break;
            }

            
        }

        private void SetDataDatagridView()
        {
            dgvRegistros.DataSource = ListaAux;
            tsTotalRegistros.Text = ListaAux.Count.ToString("N0");
            Apariencias();
        }

        private void Apariencias()
        {
            if (dgvRegistros.DataSource != null && dgvRegistros.Columns.Count > 0)
            {
                dgvRegistros.Columns[0].Visible = false;
                dgvRegistros.Columns[1].HeaderText = "Cliente";
                dgvRegistros.Columns[1].Width = 300;
                dgvRegistros.Columns[2].HeaderText = "Zona";
                dgvRegistros.Columns[2].Width = 210;
                dgvRegistros.Columns[3].HeaderText = "Lotes";
                dgvRegistros.Columns[3].Width = 150;
                dgvRegistros.Columns[4].HeaderText = "Total";
                dgvRegistros.Columns[4].Width = 110;
                dgvRegistros.Columns[5].HeaderText = "Fecha Pago";
                dgvRegistros.Columns[5].Width = 110;
                dgvRegistros.Columns[6].Visible = false;
                dgvRegistros.Columns[7].HeaderText = "Estado";
                dgvRegistros.Columns[7].Width = 150;
            }
        }

        private void btnNuevoPago_Click(object sender, EventArgs e)
        {
            NuevoPago();
        }       

        private void btnZonas_Click(object sender, EventArgs e)
        {
            NuevaZona();
        }        

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            NuevoUsuario();
        }   

        private void dgvRegistros_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRegistros.DataSource == null || dgvRegistros.RowCount <= 0) return;
            SeleccionarRegistro();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            CerrarSesion();
        }

        private void formBusqueda_Load(object sender, EventArgs e)
        {
            InicializarModulo();
        }   

        private void txtBusqueda_KeyUp(object sender, KeyEventArgs e)
        {
            Filtrar(txtBusqueda.Text, columna);
            SetDataDatagridView();
        }

        private void dgvRegistros_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (columna == e.ColumnIndex) return;
            columna = e.ColumnIndex;
            txtBusqueda.Clear();
            ListaAux = Lista;
            SetDataDatagridView();

        }

        private void cargarBoletaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvRegistros.DataSource == null || dgvRegistros.RowCount <= 0) return;
            SeleccionarRegistro();
        }
    }
}
