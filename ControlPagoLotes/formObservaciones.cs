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
    public partial class formObservaciones: Form
    {
        public string observacion;
        public formObservaciones(string observacion)
        {
            InitializeComponent();
            this.observacion = observacion;
        }

        public formObservaciones()
        {
            InitializeComponent();
        }

        private void formObservaciones_Shown(object sender, EventArgs e)
        {
            InicializarFormulario();
        }

        private void InicializarFormulario()
        {
            if (!string.IsNullOrEmpty(observacion))
            {
                txtObservaciones.Text = observacion;
            }
            
            lblContador.Text = txtObservaciones.Text.Length.ToString("N0");
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("No se guardarán las observaciones. ¿Deseas salir sin confirmar?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Close();
            }
            
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtObservaciones.Text))
            {
                MessageBox.Show("No hay texto ingresado", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else if (MessageBox.Show("¿Deseas guardar las observaciones?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                observacion = txtObservaciones.Text;
                Close();
            }
        }

        private void txtObservaciones_TextChanged(object sender, EventArgs e)
        {
            lblContador.Text = txtObservaciones.Text.Length.ToString();
        }
    }
}
