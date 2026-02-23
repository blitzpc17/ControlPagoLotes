using Entidades;
using LOGICA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace ControlPagoLotes
{
    public partial class formBusqueda : Form
    {
        private PagoLogica contexto;
        private List<clsPagosBusqueda> Lista;
        private List<clsPagosBusqueda> ListaAux;
        private int opc = 1;
        private int columna = 1;

        public formBusqueda()
        {
            InitializeComponent();
        }

        // ✅ cuando el form vuelve a activarse (después de cerrar un modal hijo)
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (AppState.MustRestartToLogin)
                this.Close();
        }

        private void InicializarModulo()
        {
            contexto = new PagoLogica();
            tsCargandoInformacion.Text = "Cargando información, espere un momento...";
            bgCargadoInfo.RunWorkerAsync();
        }

        private void CerrarSesion()
        {
            if (MessageBox.Show("¿Desea cerrar la sesión?", "Advertencia",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        }

        private void NuevaZona()
        {
            using (var f = new formZonas())
                f.ShowDialog();

            if (AppState.MustRestartToLogin) { this.Close(); return; }
            InicializarModulo();
        }

        private void SeleccionarRegistro()
        {
            using (var f = new formBoleta((int)dgvRegistros.CurrentRow.Cells[0].Value))
                f.ShowDialog();

            if (AppState.MustRestartToLogin) { this.Close(); return; }
            InicializarModulo();
        }

        private void NuevoUsuario()
        {
            using (var f = new frmMenuSystem())
                f.ShowDialog();

            // ✅ importantísimo: si desde el menú cambiaron conexión, cerrar búsqueda
            if (AppState.MustRestartToLogin)
            {
                this.Close();
                return;
            }
        }

        private void NuevoPago()
        {
            using (var f = new formBoleta(null))
                f.ShowDialog();

            if (AppState.MustRestartToLogin) { this.Close(); return; }
            InicializarModulo();
        }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            using (var fr = new formReportes())
                fr.ShowDialog();

            if (AppState.MustRestartToLogin) { this.Close(); return; }
            InicializarModulo();
        }

        private void Filtrar(string palabra, int columna)
        {
            ListaAux = Lista;

            switch (columna)
            {
                case 1: ListaAux = ListaAux.Where(x => x.Cliente.Contains(palabra)).OrderBy(x => x.Cliente).ToList(); break;
                case 2: ListaAux = ListaAux.Where(x => x.Zona.Contains(palabra)).OrderBy(x => x.Cliente).ToList(); break;
                case 3: ListaAux = ListaAux.Where(x => x.Lotes.Contains(palabra)).OrderBy(x => x.Zona).ThenBy(x => x.Cliente).ThenBy(x => x.Lotes).ToList(); break;
                case 4: ListaAux = ListaAux.Where(x => x.Total.Contains(palabra)).OrderBy(x => x.Zona).ThenBy(x => x.Cliente).ThenBy(x => x.Total).ThenBy(x => x.Lotes).ToList(); break;
                case 5: ListaAux = ListaAux.Where(x => x.Fecha.Contains(palabra)).OrderBy(x => x.Fecha).ThenBy(x => x.Cliente).ThenBy(x => x.Zona).ToList(); break;
                case 7: ListaAux = ListaAux.Where(x => x.NombreEstado.Contains(palabra)).OrderBy(x => x.NombreEstado).ThenBy(x => x.Cliente).ThenBy(x => x.Zona).ThenBy(x => x.Lotes).ToList(); break;
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
                dgvRegistros.Columns[1].Width = 500;
                dgvRegistros.Columns[2].HeaderText = "Zona";
                dgvRegistros.Columns[2].Width = 350;
                dgvRegistros.Columns[3].HeaderText = "Lotes";
                dgvRegistros.Columns[3].Width = 350;
                dgvRegistros.Columns[4].HeaderText = "Total";
                dgvRegistros.Columns[4].Width = 200;
                dgvRegistros.Columns[5].HeaderText = "Fecha Pago";
                dgvRegistros.Columns[5].Width = 200;
                dgvRegistros.Columns[6].Visible = false;
                dgvRegistros.Columns[7].HeaderText = "Estado";
                dgvRegistros.Columns[7].Width = 200;
            }
        }

        private void btnNuevoPago_Click(object sender, EventArgs e) => NuevoPago();
        private void btnZonas_Click(object sender, EventArgs e) => NuevaZona();
        private void btnUsuarios_Click(object sender, EventArgs e) => NuevoUsuario();
        private void btnSalir_Click(object sender, EventArgs e) => CerrarSesion();

        private void dgvRegistros_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRegistros.DataSource == null || dgvRegistros.RowCount <= 0) return;
            SeleccionarRegistro();
        }

        private void formBusqueda_Load(object sender, EventArgs e) => InicializarModulo();

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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Lista = contexto.GetAllPagosBusqueda();
            ListaAux = Lista;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetDataDatagridView();
            tsCargandoInformacion.Text = "";
        }
    }
}