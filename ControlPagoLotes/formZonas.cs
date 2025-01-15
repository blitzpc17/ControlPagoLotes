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
            Global.LimpiarControles(this);
            obj = null;
            Listar();                    
        }

        private void Listar()
        {
            lista = contexto.GetAllZonas();
            listaAux = lista;
            SetearDataDgv();
         
        }
        private void SetearDataDgv()
        {
            dgvRegistros.DataSource = listaAux;
            tsTotalRegistros.Text = dgvRegistros.RowCount.ToString("N0");
            Apariencias();
        }

        private void Apariencias()
        {
            if (dgvRegistros.DataSource != null && dgvRegistros.Columns.Count > 0)
            {
                dgvRegistros.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dgvRegistros.Columns[0].Visible = false;
                dgvRegistros.Columns[1].HeaderText = "Zona";               
                dgvRegistros.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void Guardar()
        {
            if (obj != null)
            {
                obj.Nombre = txtNombreZona.Text;
                contexto.UpdateZona(obj);
            }
            else
            {
                contexto.AddZona(new Zona { Nombre = txtNombreZona.Text });
            }
            MessageBox.Show("Registro guardado correctamente.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (MessageBox.Show("Se borrarán los datos ingresados. ¿Desea continuar?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                InicializarModulo();
            };
        }

        private void txtNombreZona_KeyUp(object sender, KeyEventArgs e)
        {
            if(!string.IsNullOrEmpty(txtNombreZona.Text))
            {
                Filtrar(txtNombreZona.Text);
            }
            else
            {
                listaAux = lista;
            }
            
            SetearDataDgv();
        }

        private void Filtrar(string palabra)
        {
            listaAux = lista;
            listaAux = listaAux.Where(x => x.Nombre.Contains(palabra)).OrderBy(x => x.Nombre).ToList();
        }

        private void modificarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvRegistros.DataSource == null) return;
            SetDataRegistro((int)dgvRegistros.CurrentRow.Cells[0].Value);
        }

        private void SetDataRegistro(int id)
        {
            obj = contexto.GetZonaById(id);
            txtNombreZona.Text = obj.Nombre.ToString();
        }


    }
}
