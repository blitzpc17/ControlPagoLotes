using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace ControlPagoLotes
{
    public partial class FrmConnections : Form
    {
        private readonly string _sqlitePath;
        private long? _selectedId = null;

        public FrmConnections()
        {
            InitializeComponent();

            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "jaadeproductions");
            Directory.CreateDirectory(dir);

            _sqlitePath = Path.Combine(dir, "db_conexiones.sqlite");
        }

        private void FrmConnections_Load(object sender, EventArgs e)
        {
            LoadGrid();
            ClearForm();
        }

        // =============================
        // SQLite local storage
        // =============================
        private SQLiteConnection OpenSqlite()
        {
            var cn = new SQLiteConnection($"Data Source={_sqlitePath};Version=3;");
            cn.Open();
            return cn;
        }

        private void LoadGrid()
        {
            using (var cn = OpenSqlite())
            using (var da = new SQLiteDataAdapter(
                "SELECT id, label, is_default, conn_string, updated_at FROM connections ORDER BY is_default DESC, label;",
                cn))
            {
                var dt = new DataTable();
                da.Fill(dt);

                dgv.DataSource = dt;

                if (dgv.Columns["conn_string"] != null)
                    dgv.Columns["conn_string"].Visible = false;

                if (dgv.Columns["id"] != null)
                    dgv.Columns["id"].FillWeight = 15;

                if (dgv.Columns["label"] != null)
                    dgv.Columns["label"].HeaderText = "Etiqueta";

                if (dgv.Columns["is_default"] != null)
                    dgv.Columns["is_default"].HeaderText = "Principal";

                if (dgv.Columns["updated_at"] != null)
                    dgv.Columns["updated_at"].HeaderText = "Actualizado";
            }
        }

        // =============================
        // Helpers
        // =============================
        private string BuildConnString()
        {
            var server = (txtServer.Text ?? "").Trim();
            var db = (txtDatabase.Text ?? "").Trim();
            var port = (int)numPort.Value;
            var winAuth = chkWindowsAuth.Checked;
            var user = (txtUser.Text ?? "").Trim();
            var pass = txtPassword.Text ?? "";

            if (string.IsNullOrWhiteSpace(server)) throw new Exception("Servidor requerido.");
            if (string.IsNullOrWhiteSpace(db)) throw new Exception("Base de datos requerida.");

            var b = new SqlConnectionStringBuilder
            {
                DataSource = $"{server},{port}",
                InitialCatalog = db,
                IntegratedSecurity = winAuth,
                Encrypt = chkEncrypt.Checked,
                TrustServerCertificate = chkTrustCert.Checked,
                ConnectTimeout = (int)numTimeout.Value
            };

            if (!winAuth)
            {
                if (string.IsNullOrWhiteSpace(user)) throw new Exception("Usuario requerido (SQL Login).");
                b.UserID = user;
                b.Password = pass;
            }

            return b.ConnectionString;
        }

        private (bool ok, string msg) TestSql(string connString)
        {
            try
            {
                using (var cn = new SqlConnection(connString))
                {
                    cn.Open();
                    return (true, $"OK: {cn.DataSource} / {cn.Database} / v{cn.ServerVersion}");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private void ClearForm()
        {
            _selectedId = null;
            lblId.Text = "ID: (nuevo)";
            txtLabel.Text = "";
            txtServer.Text = "";
            numPort.Value = 1433;
            txtDatabase.Text = "";
            chkWindowsAuth.Checked = false;
            txtUser.Text = "";
            txtPassword.Text = "";
            chkEncrypt.Checked = true;
            chkTrustCert.Checked = true;
            numTimeout.Value = 30;
            chkDefault.Checked = false;

            ToggleAuthInputs();
            lblStatus.Text = "Listo.";
        }

        private void ToggleAuthInputs()
        {
            var win = chkWindowsAuth.Checked;
            txtUser.Enabled = !win;
            txtPassword.Enabled = !win;
        }

        private void FillFormFromRow(DataRowView row)
        {
            _selectedId = Convert.ToInt64(row["id"]);
            lblId.Text = $"ID: {_selectedId}";
            txtLabel.Text = Convert.ToString(row["label"]);
            chkDefault.Checked = Convert.ToInt32(row["is_default"]) == 1;

            var cs = Convert.ToString(row["conn_string"]);
            var b = new SqlConnectionStringBuilder(cs);

            txtServer.Text = b.DataSource.Contains(",") ? b.DataSource.Split(',')[0] : b.DataSource;

            int port = 1433;
            if (b.DataSource.Contains(","))
                int.TryParse(b.DataSource.Split(',')[1], out port);

            if (port < 1) port = 1433;
            numPort.Value = Math.Min(65535, Math.Max(1, port));

            txtDatabase.Text = b.InitialCatalog;

            chkEncrypt.Checked = b.Encrypt;
            chkTrustCert.Checked = b.TrustServerCertificate;
            numTimeout.Value = Math.Min(300, Math.Max(1, b.ConnectTimeout));

            chkWindowsAuth.Checked = b.IntegratedSecurity;
            txtUser.Text = b.UserID ?? "";
            txtPassword.Text = b.Password ?? "";

            ToggleAuthInputs();
        }

        private void TriggerRestartToLogin()
        {
            AppState.MustRestartToLogin = true;
            this.DialogResult = DialogResult.OK; // opcional
            this.Close();
        }

        // =============================
        // Events
        // =============================
        private void chkWindowsAuth_CheckedChanged(object sender, EventArgs e)
        {
            ToggleAuthInputs();
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null || dgv.CurrentRow.DataBoundItem == null) return;

            var drv = dgv.CurrentRow.DataBoundItem as DataRowView;
            if (drv == null) return;

            FillFormFromRow(drv);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                var cs = BuildConnString();
                var r = TestSql(cs);

                lblStatus.Text = r.ok ? r.msg : $"ERROR: {r.msg}";
                MessageBox.Show(r.msg, r.ok ? "Conexión OK" : "Error",
                    MessageBoxButtons.OK,
                    r.ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"ERROR: {ex.Message}";
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var label = (txtLabel.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(label)) throw new Exception("Etiqueta requerida.");

                var cs = BuildConnString();

                var test = TestSql(cs);
                if (!test.ok)
                {
                    var c = MessageBox.Show(
                        $"No se pudo conectar:\n{test.msg}\n\n¿Aun así deseas guardar?",
                        "Conexión fallida",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (c != DialogResult.Yes) return;
                }

                using (var cn = OpenSqlite())
                using (var tx = cn.BeginTransaction())
                {
                    if (chkDefault.Checked)
                    {
                        using (var clear = cn.CreateCommand())
                        {
                            clear.Transaction = tx;
                            clear.CommandText = "UPDATE connections SET is_default = 0 WHERE is_default = 1;";
                            clear.ExecuteNonQuery();
                        }
                    }

                    if (_selectedId == null)
                    {
                        using (var cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText = @"
INSERT INTO connections(label, conn_string, is_default)
VALUES(@label,@cs,@def);";
                            cmd.Parameters.AddWithValue("@label", label);
                            cmd.Parameters.AddWithValue("@cs", cs);
                            cmd.Parameters.AddWithValue("@def", chkDefault.Checked ? 1 : 0);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (var cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText = @"
UPDATE connections
SET label=@label, conn_string=@cs, is_default=@def, updated_at=datetime('now')
WHERE id=@id;";
                            cmd.Parameters.AddWithValue("@id", _selectedId.Value);
                            cmd.Parameters.AddWithValue("@label", label);
                            cmd.Parameters.AddWithValue("@cs", cs);
                            cmd.Parameters.AddWithValue("@def", chkDefault.Checked ? 1 : 0);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                }

                if (chkDefault.Checked)
                {
                    MessageBox.Show(
                        "Conexión principal actualizada. Se regresará al login.",
                        "Listo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    TriggerRestartToLogin();
                    return;
                }

                lblStatus.Text = "Guardado.";
                LoadGrid();
                ClearForm();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"ERROR: {ex.Message}";
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedId == null)
            {
                MessageBox.Show("Selecciona una conexión.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show("¿Eliminar esta conexión?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using (var cn = OpenSqlite())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM connections WHERE id=@id;";
                    cmd.Parameters.AddWithValue("@id", _selectedId.Value);
                    cmd.ExecuteNonQuery();
                }

                lblStatus.Text = "Eliminado.";
                LoadGrid();
                ClearForm();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"ERROR: {ex.Message}";
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSetDefault_Click(object sender, EventArgs e)
        {
            if (_selectedId == null)
            {
                MessageBox.Show("Selecciona una conexión.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var cn = OpenSqlite())
                using (var tx = cn.BeginTransaction())
                {
                    using (var clear = cn.CreateCommand())
                    {
                        clear.Transaction = tx;
                        clear.CommandText = "UPDATE connections SET is_default = 0 WHERE is_default = 1;";
                        clear.ExecuteNonQuery();
                    }

                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = "UPDATE connections SET is_default = 1, updated_at=datetime('now') WHERE id=@id;";
                        cmd.Parameters.AddWithValue("@id", _selectedId.Value);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }

                MessageBox.Show(
                    "Conexión principal actualizada. Se regresará al login.",
                    "Listo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                lblStatus.Text = "Marcada como principal.";
                LoadGrid();

                TriggerRestartToLogin();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"ERROR: {ex.Message}";
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}