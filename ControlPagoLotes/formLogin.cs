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
    public partial class formLogin : Form
    {
        public formLogin()
        {
            InitializeComponent();
        }

        private void btnAcceder_Click(object sender, EventArgs e)
        {
            IniciarSecion();
        }

        private void IniciarSecion()
        {
            formBusqueda bus = new formBusqueda();
            bus.ShowDialog();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            CerrarModulo();
        }

        private void CerrarModulo()
        {
            Close();
        }
    }
}
