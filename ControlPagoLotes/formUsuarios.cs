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
            contexto = new UsuarioLogica();

            Lista = contexto.GetAllUsuario();
            dgvRegistros.DataSource = Lista;   
            tsTotalRegistros.Text = Lista.Count.ToString("N0"); 
        }

        private void Guardar()
        {
            if (Obj != null)
            {

            }
            else
            {
                Obj = new UsuarioL
                {
                    Usuario = txtUsuario.Text,
                    Password = txtContraseña.Text,
                };

                contexto.AddUsuario(Obj);   
            }

            InicializarModulo();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Guardar();
        }     

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            InicializarModulo();
        }
    }
}
