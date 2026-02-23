namespace ControlPagoLotes
{
    partial class frmRutasConfig
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lvUsuarios = new System.Windows.Forms.ListView();
            this.colUsuarios = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clbZonas = new System.Windows.Forms.CheckedListBox();
            this.grpUsuarios = new System.Windows.Forms.GroupBox();
            this.grpZonas = new System.Windows.Forms.GroupBox();
            this.lblUsuarioSel = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnMarcarTodas = new System.Windows.Forms.Button();
            this.grpUsuarios.SuspendLayout();
            this.grpZonas.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvUsuarios
            // 
            this.lvUsuarios.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colUsuarios});
            this.lvUsuarios.FullRowSelect = true;
            this.lvUsuarios.HideSelection = false;
            this.lvUsuarios.Location = new System.Drawing.Point(12, 22);
            this.lvUsuarios.MultiSelect = false;
            this.lvUsuarios.Name = "lvUsuarios";
            this.lvUsuarios.Size = new System.Drawing.Size(300, 430);
            this.lvUsuarios.TabIndex = 0;
            this.lvUsuarios.UseCompatibleStateImageBehavior = false;
            this.lvUsuarios.View = System.Windows.Forms.View.Details;
            this.lvUsuarios.SelectedIndexChanged += new System.EventHandler(this.lvUsuarios_SelectedIndexChanged);
            // 
            // colUsuarios
            // 
            this.colUsuarios.Text = "Usuarios";
            this.colUsuarios.Width = 280;
            // 
            // clbZonas
            // 
            this.clbZonas.CheckOnClick = true;
            this.clbZonas.FormattingEnabled = true;
            this.clbZonas.Location = new System.Drawing.Point(12, 82);
            this.clbZonas.Name = "clbZonas";
            this.clbZonas.Size = new System.Drawing.Size(430, 346);
            this.clbZonas.TabIndex = 1;
            // 
            // grpUsuarios
            // 
            this.grpUsuarios.Controls.Add(this.lvUsuarios);
            this.grpUsuarios.Location = new System.Drawing.Point(12, 12);
            this.grpUsuarios.Name = "grpUsuarios";
            this.grpUsuarios.Size = new System.Drawing.Size(326, 470);
            this.grpUsuarios.TabIndex = 2;
            this.grpUsuarios.TabStop = false;
            this.grpUsuarios.Text = "Selecciona usuario";
            // 
            // grpZonas
            // 
            this.grpZonas.Controls.Add(this.btnMarcarTodas);
            this.grpZonas.Controls.Add(this.btnLimpiar);
            this.grpZonas.Controls.Add(this.lblInfo);
            this.grpZonas.Controls.Add(this.lblUsuarioSel);
            this.grpZonas.Controls.Add(this.clbZonas);
            this.grpZonas.Location = new System.Drawing.Point(344, 12);
            this.grpZonas.Name = "grpZonas";
            this.grpZonas.Size = new System.Drawing.Size(456, 470);
            this.grpZonas.TabIndex = 3;
            this.grpZonas.TabStop = false;
            this.grpZonas.Text = "Zonas permitidas";
            // 
            // lblUsuarioSel
            // 
            this.lblUsuarioSel.AutoSize = true;
            this.lblUsuarioSel.Location = new System.Drawing.Point(9, 22);
            this.lblUsuarioSel.Name = "lblUsuarioSel";
            this.lblUsuarioSel.Size = new System.Drawing.Size(118, 15);
            this.lblUsuarioSel.TabIndex = 2;
            this.lblUsuarioSel.Text = "Usuario: (ninguno)";
            // 
            // lblInfo
            // 
            this.lblInfo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblInfo.Location = new System.Drawing.Point(9, 41);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(433, 36);
            this.lblInfo.TabIndex = 3;
            this.lblInfo.Text = "Info";
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(579, 492);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(105, 32);
            this.btnGuardar.TabIndex = 4;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(337, 434);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(105, 28);
            this.btnLimpiar.TabIndex = 5;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(695, 492);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(105, 32);
            this.btnCerrar.TabIndex = 6;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // btnMarcarTodas
            // 
            this.btnMarcarTodas.Location = new System.Drawing.Point(226, 434);
            this.btnMarcarTodas.Name = "btnMarcarTodas";
            this.btnMarcarTodas.Size = new System.Drawing.Size(105, 28);
            this.btnMarcarTodas.TabIndex = 6;
            this.btnMarcarTodas.Text = "Marcar todas";
            this.btnMarcarTodas.UseVisualStyleBackColor = true;
            this.btnMarcarTodas.Click += new System.EventHandler(this.btnMarcarTodas_Click);
            // 
            // frmRutasConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 536);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.grpZonas);
            this.Controls.Add(this.grpUsuarios);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRutasConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuración de rutas (FiltroZonas)";
            this.Load += new System.EventHandler(this.frmRutasConfig_Load);
            this.grpUsuarios.ResumeLayout(false);
            this.grpZonas.ResumeLayout(false);
            this.grpZonas.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvUsuarios;
        private System.Windows.Forms.ColumnHeader colUsuarios;
        private System.Windows.Forms.CheckedListBox clbZonas;
        private System.Windows.Forms.GroupBox grpUsuarios;
        private System.Windows.Forms.GroupBox grpZonas;
        private System.Windows.Forms.Label lblUsuarioSel;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnMarcarTodas;
    }
}