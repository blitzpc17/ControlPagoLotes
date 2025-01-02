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
using Entidades;

namespace ControlPagoLotes
{
    public partial class formZonas : Form
    {

        private ZonaLogica contexto;
        private List<Zona> lista;
        private List<Zona> listaAux;
        private Zona obj;


        public formZonas()
        {
            InitializeComponent();
        }

        private void InicializarModulo()
        {
            contexto= new ZonaLogica();
            lista =  contexto.GetAllZonas();
            dgvRegistros.DataSource = lista;    
            tsTotalRegistros.Text = lista.Count.ToString("N0"); 
            
        }
        private void Guardar()
        {
            if (obj != null)
            {

            }
            else
            {
                contexto.AddZona(new Zona { Nombre = txtNombreZona.Text });
            }

            InicializarModulo();

           
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Guardar();
        }

        private void formZonas_Load(object sender, EventArgs e)
        {
            InicializarModulo();    
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            InicializarModulo();
        }
    }
}
