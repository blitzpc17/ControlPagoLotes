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

        private UsuarioL _selectedUsuario = null;

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
            lblInfo.Text = "Selecciona un usuario y marca las zonas permitidas de cualquier plaza.";
        }

        private void LoadUsuarios()
        {
            var usuarios = usuariosRepo.GetAllUsuario(unificarTodas: true);
            _usuarios = usuarios ?? new List<UsuarioL>();

            lvUsuarios.BeginUpdate();
            lvUsuarios.Items.Clear();

            foreach (var u in _usuarios)
            {
                var item = new ListViewItem(u.Usuario ?? $"Usuario {u.Id}");
                item.Tag = u;
                lvUsuarios.Items.Add(item);
            }

            lvUsuarios.EndUpdate();
        }

        private void LoadZonas()
        {
            _zonas = zonaLogica.GetAllZonas(unificarTodas: true) ?? new List<Zona>();

            clbZonas.BeginUpdate();
            clbZonas.Items.Clear();

            foreach (var z in _zonas)
            {
                clbZonas.Items.Add(new ZonaItem 
                { 
                    Id = z.Id, 
                    ConnectionId = z.ConnectionId, 
                    Plaza = z.Plaza, 
                    Nombre = z.NombreConPlaza ?? z.Nombre 
                }, false);
            }

            clbZonas.EndUpdate();
        }

        private void lvUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvUsuarios.SelectedItems.Count == 0)
            {
                _selectedUsuario = null;
                lblUsuarioSel.Text = "Usuario: (ninguno)";
                ClearChecks();
                return;
            }

            var u = lvUsuarios.SelectedItems[0].Tag as UsuarioL;
            _selectedUsuario = u;

            lblUsuarioSel.Text = $"Usuario: {u.Usuario}";
            LoadUserZonas(u);
        }

        private void LoadUserZonas(UsuarioL user)
        {
            ClearChecks();
            if (user == null || string.IsNullOrWhiteSpace(user.Usuario)) return;

            var assignedSet = rutasLogica.GetZonasAsignadasPorUsuario(user.Usuario);

            clbZonas.BeginUpdate();
            for (int i = 0; i < clbZonas.Items.Count; i++)
            {
                var zi = (ZonaItem)clbZonas.Items[i];
                string key = $"{zi.ConnectionId}_{zi.Id}";
                clbZonas.SetItemChecked(i, assignedSet.Contains(key));
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

        private void btnMarcarTodas_Click(object sender, EventArgs e)
        {
            if (_selectedUsuario == null)
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
            if (_selectedUsuario == null)
            {
                MessageBox.Show("Selecciona un usuario primero.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ClearChecks();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (_selectedUsuario == null)
            {
                MessageBox.Show("Selecciona un usuario.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Agrupar zonas marcadas por ConnectionId
            var checkedItems = clbZonas.CheckedItems.Cast<ZonaItem>().ToList();
            var checkedByConn = checkedItems
                .GroupBy(x => x.ConnectionId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Id).Distinct().OrderBy(x => x).ToList());

            var savedPlazas = rutasLogica.SaveZonasAsignadasPorUsuario(
                _selectedUsuario.Usuario,
                _selectedUsuario.Password,
                checkedByConn
            );

            MessageBox.Show($"Rutas guardadas correctamente en plazas: {string.Join(", ", savedPlazas)}.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Si se modificó el mismo usuario logueado => re-login
            if (Global.ObjUsuario != null && string.Equals(Global.ObjUsuario.Usuario, _selectedUsuario.Usuario, StringComparison.OrdinalIgnoreCase))
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

        private class ZonaItem
        {
            public int Id { get; set; }
            public long ConnectionId { get; set; }
            public string Plaza { get; set; }
            public string Nombre { get; set; }
            public override string ToString() => Nombre;
        }
    }
}
