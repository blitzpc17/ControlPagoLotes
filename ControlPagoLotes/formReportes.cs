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
    public partial class formReportes : Form
    {
        public formReportes()
        {
            InitializeComponent();
        }

        private void btnGeneral_Click(object sender, EventArgs e)
        {
            ExportarExcelGeneral();
        }

        private void ExportarExcelGeneral()
        {
            //generar excel

        }

        private void btnCorteCaja_Click(object sender, EventArgs e)
        {
            CorteCaja();
        }

        private void CorteCaja()
        {
            formCorteCaja formCorte = new formCorteCaja();
            formCorte.ShowDialog();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Desea regresar a la búsqueda general de pagos?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        }
    }
}
