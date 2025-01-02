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
    public partial class formBusqueda : Form
    {
        private PagoLogica contexto;
        private List<Pago> Lista;
        private List<Pago> ListaAux;
        public formBusqueda()
        {
            InitializeComponent();
        }
        private void InicializarModulo()
        {
            contexto = new PagoLogica();
            Lista = contexto.GetAllPagos();
            dgvRegistros.DataSource = Lista;
            tsTotalRegistros.Text = Lista.Count.ToString("N0");
        }

        private void CerrarSesion()
        {
            Close();
        }
        private void NuevaZona()
        {
            formZonas formZonas = new formZonas();
            formZonas.ShowDialog();
        }
        
        private void SeleccionarRegistro()
        {
            formBoleta formBoleta = new formBoleta((int)dgvRegistros.CurrentRow.Cells[0].Value);
            formBoleta.ShowDialog();
        }
        private void NuevoUsuario()
        {
            formUsuarios formUsuarios = new formUsuarios();
            formUsuarios.ShowDialog();
        }

        private void NuevoPago()
        {
            formBoleta formBoleta = new formBoleta(null);
            formBoleta.ShowDialog();
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
    }
}
