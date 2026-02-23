using Entidades;
using LOGICA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ControlPagoLotes
{
    public partial class frmRutasConfig : Form
    {
        private readonly ZonaLogica zonaLogica;
        private readonly RutasLogica rutasLogica;
        private readonly UsuarioLogica usuariosRepo;

        private List<UsuarioL> _usuarios = new List<UsuarioL>();
        private List<Zona> _zonas = new List<Zona>();

        private int? _selectedUsuarioId = null;

        public frmRutasConfig()
        {
            InitializeComponent();

            zonaLogica = new ZonaLogica();
            rutasLogica = new RutasLogica();
            usuariosRepo = new UsuarioLogica();
        }

        private void frmRutasConfig_Load(object sender, EventArgs e)
        {
            LoadUsuarios();
            LoadZonas();

            lblUsuarioSel.Text = "Usuario: (ninguno)";
            lblInfo.Text = "Selecciona un usuario y marca las zonas permitidas.";
        }

        private void LoadUsuarios()
        {
            var usuarios = usuariosRepo.GetAllUsuario();
            _usuarios = usuarios ?? new List<UsuarioL>();

            lvUsuarios.BeginUpdate();
            lvUsuarios.Items.Clear();

            foreach (var u in _usuarios)
            {
                var item = new ListViewItem(u.Usuario ?? $"Usuario {u.Id}");
                item.Tag = u.Id;
                lvUsuarios.Items.Add(item);
            }

            lvUsuarios.EndUpdate();
        }

        private void LoadZonas()
        {
            _zonas = zonaLogica.GetAllZonas() ?? new List<Zona>();

            clbZonas.BeginUpdate();
            clbZonas.Items.Clear();

            foreach (var z in _zonas)
            {
                clbZonas.Items.Add(new ZonaItem { Id = z.Id, Nombre = z.Nombre }, false);
            }

            clbZonas.EndUpdate();
        }

        private void lvUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvUsuarios.SelectedItems.Count == 0)
            {
                _selectedUsuarioId = null;
                lblUsuarioSel.Text = "Usuario: (ninguno)";
                ClearChecks();
                return;
            }

            var id = (int)lvUsuarios.SelectedItems[0].Tag;
            _selectedUsuarioId = id;

            lblUsuarioSel.Text = $"Usuario: {lvUsuarios.SelectedItems[0].Text} (ID {id})";
            LoadUserZonas(id);
        }

        private void LoadUserZonas(int usuarioId)
        {
            var zonasAsignadas = rutasLogica.GetZonasForUser(usuarioId) ?? new List<int>();

            clbZonas.BeginUpdate();
            for (int i = 0; i < clbZonas.Items.Count; i++)
            {
                var zi = (ZonaItem)clbZonas.Items[i];
                clbZonas.SetItemChecked(i, zonasAsignadas.Contains(zi.Id));
            }
            clbZonas.EndUpdate();
        }

        private void ClearChecks()
        {
            clbZonas.BeginUpdate();
            for (int i = 0; i < clbZonas.Items.Count; i++)
                clbZonas.SetItemChecked(i, false);
            clbZonas.EndUpdate();
        }

        private List<int> GetCheckedZonas()
        {
            var list = new List<int>();
            foreach (var item in clbZonas.CheckedItems)
                list.Add(((ZonaItem)item).Id);

            return list.Distinct().OrderBy(x => x).ToList();
        }

        private void btnMarcarTodas_Click(object sender, EventArgs e)
        {
            if (_selectedUsuarioId == null)
            {
                MessageBox.Show("Selecciona un usuario primero.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            clbZonas.BeginUpdate();
            for (int i = 0; i < clbZonas.Items.Count; i++)
                clbZonas.SetItemChecked(i, true);
            clbZonas.EndUpdate();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            if (_selectedUsuarioId == null)
            {
                MessageBox.Show("Selecciona un usuario primero.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ClearChecks();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (_selectedUsuarioId == null)
            {
                MessageBox.Show("Selecciona un usuario.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var zonas = GetCheckedZonas();

            // Guardar JSON en VARIABLESGLOBALES(label='FltroZonas')
            rutasLogica.SetZonasForUser(_selectedUsuarioId.Value, zonas);

            MessageBox.Show("Rutas guardadas correctamente.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Si se modificó el mismo usuario logueado => re-login
            var currentUserId = TryGetCurrentUserId();
            if (currentUserId > 0 && currentUserId == _selectedUsuarioId.Value)
            {
                AppState.MustRestartToLogin = true;
                this.Close();
                return;
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Obtiene el Id del usuario logueado (sin asumir nombre de propiedad).
        /// Busca propiedades comunes: Id, ID, IdUsuario, UsuarioId, id_usuario, etc.
        /// </summary>
        private int TryGetCurrentUserId()
        {
            try
            {
                var obj = Global.ObjUsuario;
                if (obj == null) return 0;

                var t = obj.GetType();
                string[] props = { "Id", "ID", "IdUsuario", "UsuarioId", "Id_Usuario", "id_usuario", "ID_USUARIO" };

                foreach (var p in props)
                {
                    var pi = t.GetProperty(p, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (pi == null) continue;

                    var val = pi.GetValue(obj, null);
                    if (val == null) continue;

                    if (val is int i) return i;
                    if (int.TryParse(val.ToString(), out var parsed)) return parsed;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private class ZonaItem
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public override string ToString() => Nombre;
        }
    }
}
