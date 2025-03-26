using Entidades;
using LOGICA;
using System;
using System.Collections;
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
    public partial class formUsuarios : Form
    {
        private UsuarioLogica contexto;
        private List<UsuarioL> Lista;
        private List<UsuarioL> ListaAux;
        private UsuarioL Obj;

        public formUsuarios()
        {
            InitializeComponent();
        }

        private void formUsuarios_Load(object sender, EventArgs e)
        {
            InicializarModulo();
        }

        private void InicializarModulo()
        {
            Global.LimpiarControles(this);
            contexto = new UsuarioLogica();
            Lista = contexto.GetAllUsuario();
            ListaAux = Lista;
            Obj = null;
            SetDataDatagridView();
           
        }

        private void Guardar()
        {

            if (Obj == null)
            {
                Obj = new UsuarioL();
            }

            Obj.Usuario = txtUsuario.Text;
            Obj.Password = txtContraseña.Text;

            if (Obj.Id == 0)
            {
                contexto.AddUsuario(Obj);
            }
            else
            {
                contexto.UpdateUsuario(Obj);
            }

            MessageBox.Show("Registro guardado correctamente.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            InicializarModulo();

        }

        private void Filtrar(string palabra)
        {
            ListaAux = Lista;
            ListaAux = ListaAux.Where(x => x.Usuario.Contains(palabra)).OrderBy(x => x.Usuario).ToList();
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
                dgvRegistros.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dgvRegistros.Columns[0].Visible = false;
                dgvRegistros.Columns[1].HeaderText = "Usuario";
                dgvRegistros.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvRegistros.Columns[2].Visible = false;
                /*dgvRegistros.Columns[2].HeaderText = "Contraseña";
                dgvRegistros.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;*/
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Guardar();            
        }     

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Se borrarán los datos ingresados. ¿Desea continuar?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question)== DialogResult.Yes)
            {
                InicializarModulo();
            }
            
        }

        private void txtUsuario_KeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUsuario.Text))
            {
                Filtrar(txtUsuario.Text);
            }
            else
            {
                ListaAux = Lista;
            }

            SetDataDatagridView();
        }

        private void modificarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvRegistros.DataSource == null) return;
            SetDataRegistro((int)dgvRegistros.CurrentRow.Cells[0].Value);
        }

        private void SetDataRegistro(int id)
        {            
            Obj = contexto.GetUsuarioById(id);
            txtContraseña.Text = Obj.Password.ToString();
            txtUsuario.Text = Obj.Usuario.ToString();
        }
    }
}
