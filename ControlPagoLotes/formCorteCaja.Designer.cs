namespace ControlPagoLotes
{
    partial class formCorteCaja
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formCorteCaja));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tsTotalRegistros = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tsTotalDia = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel8 = new System.Windows.Forms.ToolStripLabel();
            this.tsTotalTransferencias = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.tsNuevoIngreso = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.tsMontoModificado = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel7 = new System.Windows.Forms.ToolStripLabel();
            this.tsMontoEliminado = new System.Windows.Forms.ToolStripLabel();
            this.tsCargandoInformacion = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.tsTotalMigrado = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.tsMigradosModificados = new System.Windows.Forms.ToolStripLabel();
            this.dgvRegistros = new System.Windows.Forms.DataGridView();
            this.dtpFechaContrato = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnConsultar = new System.Windows.Forms.Button();
            this.chkTodas = new System.Windows.Forms.CheckBox();
            this.cbxLotificaciones = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.panelMes = new System.Windows.Forms.Panel();
            this.numericAnioMes = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbxMeses = new System.Windows.Forms.ComboBox();
            this.panelSemana = new System.Windows.Forms.Panel();
            this.numSemana = new System.Windows.Forms.NumericUpDown();
            this.numAnioSemana = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxPeriodo = new System.Windows.Forms.ComboBox();
            this.panelDia = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.bntExportar = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegistros)).BeginInit();
            this.panel1.SuspendLayout();
            this.panelMes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericAnioMes)).BeginInit();
            this.panelSemana.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSemana)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAnioSemana)).BeginInit();
            this.panelDia.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tsTotalRegistros,
            this.toolStripSeparator1,
            this.toolStripLabel2,
            this.tsTotalDia,
            this.toolStripLabel8,
            this.tsTotalTransferencias,
            this.toolStripLabel3,
            this.tsNuevoIngreso,
            this.toolStripSeparator2,
            this.toolStripLabel5,
            this.tsMontoModificado,
            this.toolStripLabel7,
            this.tsMontoEliminado,
            this.tsCargandoInformacion,
            this.toolStripSeparator3,
            this.toolStripLabel4,
            this.tsTotalMigrado,
            this.toolStripSeparator4,
            this.toolStripLabel6,
            this.tsMigradosModificados});
            this.toolStrip1.Location = new System.Drawing.Point(0, 586);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1245, 25);
            this.toolStrip1.TabIndex = 33;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(117, 22);
            this.toolStripLabel1.Text = "Total de registros:";
            // 
            // tsTotalRegistros
            // 
            this.tsTotalRegistros.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.tsTotalRegistros.Name = "tsTotalRegistros";
            this.tsTotalRegistros.Size = new System.Drawing.Size(17, 22);
            this.tsTotalRegistros.Text = "0";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(101, 22);
            this.toolStripLabel2.Text = "Total del día($):";
            // 
            // tsTotalDia
            // 
            this.tsTotalDia.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.tsTotalDia.Name = "tsTotalDia";
            this.tsTotalDia.Size = new System.Drawing.Size(17, 22);
            this.tsTotalDia.Text = "0";
            // 
            // toolStripLabel8
            // 
            this.toolStripLabel8.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.toolStripLabel8.Name = "toolStripLabel8";
            this.toolStripLabel8.Size = new System.Drawing.Size(80, 22);
            this.toolStripLabel8.Text = "Total transf:";
            // 
            // tsTotalTransferencias
            // 
            this.tsTotalTransferencias.Font = new System.Drawing.Font("Segoe UI Black", 9.75F, System.Drawing.FontStyle.Bold);
            this.tsTotalTransferencias.Name = "tsTotalTransferencias";
            this.tsTotalTransferencias.Size = new System.Drawing.Size(16, 22);
            this.tsTotalTransferencias.Text = "0";
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(130, 22);
            this.toolStripLabel3.Text = "Nuevos Ingresos($):";
            // 
            // tsNuevoIngreso
            // 
            this.tsNuevoIngreso.Font = new System.Drawing.Font("Segoe UI Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsNuevoIngreso.Name = "tsNuevoIngreso";
            this.tsNuevoIngreso.Size = new System.Drawing.Size(16, 22);
            this.tsNuevoIngreso.Text = "0";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(152, 22);
            this.toolStripLabel5.Text = "Monto Modificados ($):";
            // 
            // tsMontoModificado
            // 
            this.tsMontoModificado.Font = new System.Drawing.Font("Segoe UI Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsMontoModificado.Name = "tsMontoModificado";
            this.tsMontoModificado.Size = new System.Drawing.Size(16, 22);
            this.tsMontoModificado.Text = "0";
            // 
            // toolStripLabel7
            // 
            this.toolStripLabel7.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.toolStripLabel7.Name = "toolStripLabel7";
            this.toolStripLabel7.Size = new System.Drawing.Size(143, 22);
            this.toolStripLabel7.Text = "Monto Eliminados ($):";
            // 
            // tsMontoEliminado
            // 
            this.tsMontoEliminado.Font = new System.Drawing.Font("Segoe UI Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsMontoEliminado.Name = "tsMontoEliminado";
            this.tsMontoEliminado.Size = new System.Drawing.Size(16, 22);
            this.tsMontoEliminado.Text = "0";
            // 
            // tsCargandoInformacion
            // 
            this.tsCargandoInformacion.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsCargandoInformacion.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsCargandoInformacion.Name = "tsCargandoInformacion";
            this.tsCargandoInformacion.Size = new System.Drawing.Size(114, 22);
            this.tsCargandoInformacion.Text = "Cargando info";
            this.tsCargandoInformacion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(132, 22);
            this.toolStripLabel4.Text = "Monto Migrados($):";
            // 
            // tsTotalMigrado
            // 
            this.tsTotalMigrado.Font = new System.Drawing.Font("Segoe UI Black", 9.75F, System.Drawing.FontStyle.Bold);
            this.tsTotalMigrado.Name = "tsTotalMigrado";
            this.tsTotalMigrado.Size = new System.Drawing.Size(16, 22);
            this.tsTotalMigrado.Text = "0";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(134, 15);
            this.toolStripLabel6.Text = "Monto migrados modif:";
            // 
            // tsMigradosModificados
            // 
            this.tsMigradosModificados.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.tsMigradosModificados.Name = "tsMigradosModificados";
            this.tsMigradosModificados.Size = new System.Drawing.Size(15, 17);
            this.tsMigradosModificados.Text = "0";
            // 
            // dgvRegistros
            // 
            this.dgvRegistros.AllowUserToAddRows = false;
            this.dgvRegistros.AllowUserToDeleteRows = false;
            this.dgvRegistros.AllowUserToResizeRows = false;
            this.dgvRegistros.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRegistros.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvRegistros.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvRegistros.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvRegistros.Location = new System.Drawing.Point(0, 118);
            this.dgvRegistros.Name = "dgvRegistros";
            this.dgvRegistros.ReadOnly = true;
            this.dgvRegistros.RowHeadersVisible = false;
            this.dgvRegistros.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRegistros.Size = new System.Drawing.Size(1245, 465);
            this.dgvRegistros.TabIndex = 32;
            // 
            // dtpFechaContrato
            // 
            this.dtpFechaContrato.CustomFormat = "dd/MM/yyyy";
            this.dtpFechaContrato.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dtpFechaContrato.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaContrato.Location = new System.Drawing.Point(95, 4);
            this.dtpFechaContrato.Name = "dtpFechaContrato";
            this.dtpFechaContrato.Size = new System.Drawing.Size(200, 26);
            this.dtpFechaContrato.TabIndex = 43;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(440, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 20);
            this.label1.TabIndex = 34;
            this.label1.Text = "Periodo:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnConsultar);
            this.panel1.Controls.Add(this.chkTodas);
            this.panel1.Controls.Add(this.cbxLotificaciones);
            this.panel1.Controls.Add(this.panelMes);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.panelSemana);
            this.panel1.Controls.Add(this.cbxPeriodo);
            this.panel1.Controls.Add(this.panelDia);
            this.panel1.Controls.Add(this.btnCancelar);
            this.panel1.Controls.Add(this.bntExportar);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1245, 115);
            this.panel1.TabIndex = 36;
            // 
            // btnConsultar
            // 
            this.btnConsultar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConsultar.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnConsultar.Image = global::ControlPagoLotes.Properties.Resources.buscar;
            this.btnConsultar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConsultar.Location = new System.Drawing.Point(872, 28);
            this.btnConsultar.Name = "btnConsultar";
            this.btnConsultar.Size = new System.Drawing.Size(148, 40);
            this.btnConsultar.TabIndex = 46;
            this.btnConsultar.Text = " CONSULTAR";
            this.btnConsultar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnConsultar.UseVisualStyleBackColor = true;
            this.btnConsultar.Click += new System.EventHandler(this.btnConsultar_Click);
            // 
            // chkTodas
            // 
            this.chkTodas.AutoSize = true;
            this.chkTodas.Checked = true;
            this.chkTodas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTodas.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.chkTodas.Location = new System.Drawing.Point(140, 70);
            this.chkTodas.Name = "chkTodas";
            this.chkTodas.Size = new System.Drawing.Size(191, 24);
            this.chkTodas.TabIndex = 45;
            this.chkTodas.Text = "Todas las lotificaciones";
            this.chkTodas.UseVisualStyleBackColor = true;
            this.chkTodas.CheckedChanged += new System.EventHandler(this.chkTodas_CheckedChanged);
            // 
            // cbxLotificaciones
            // 
            this.cbxLotificaciones.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbxLotificaciones.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbxLotificaciones.Enabled = false;
            this.cbxLotificaciones.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.cbxLotificaciones.FormattingEnabled = true;
            this.cbxLotificaciones.Location = new System.Drawing.Point(140, 35);
            this.cbxLotificaciones.Name = "cbxLotificaciones";
            this.cbxLotificaciones.Size = new System.Drawing.Size(212, 28);
            this.cbxLotificaciones.TabIndex = 44;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(28, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 20);
            this.label7.TabIndex = 43;
            this.label7.Text = "Lotificación:";
            // 
            // panelMes
            // 
            this.panelMes.Controls.Add(this.numericAnioMes);
            this.panelMes.Controls.Add(this.label4);
            this.panelMes.Controls.Add(this.label5);
            this.panelMes.Controls.Add(this.cbxMeses);
            this.panelMes.Location = new System.Drawing.Point(444, 69);
            this.panelMes.Name = "panelMes";
            this.panelMes.Size = new System.Drawing.Size(313, 32);
            this.panelMes.TabIndex = 42;
            this.panelMes.Visible = false;
            // 
            // numericAnioMes
            // 
            this.numericAnioMes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.numericAnioMes.Location = new System.Drawing.Point(253, 2);
            this.numericAnioMes.Maximum = new decimal(new int[] {
            2100,
            0,
            0,
            0});
            this.numericAnioMes.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numericAnioMes.Name = "numericAnioMes";
            this.numericAnioMes.Size = new System.Drawing.Size(60, 26);
            this.numericAnioMes.TabIndex = 43;
            this.numericAnioMes.Value = new decimal(new int[] {
            2025,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 20);
            this.label4.TabIndex = 41;
            this.label4.Text = "Mes:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(194, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 20);
            this.label5.TabIndex = 40;
            this.label5.Text = "Año:";
            // 
            // cbxMeses
            // 
            this.cbxMeses.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.cbxMeses.FormattingEnabled = true;
            this.cbxMeses.Location = new System.Drawing.Point(91, 3);
            this.cbxMeses.Name = "cbxMeses";
            this.cbxMeses.Size = new System.Drawing.Size(97, 28);
            this.cbxMeses.TabIndex = 0;
            this.cbxMeses.SelectedIndexChanged += new System.EventHandler(this.cbxMeses_SelectedIndexChanged);
            // 
            // panelSemana
            // 
            this.panelSemana.Controls.Add(this.numSemana);
            this.panelSemana.Controls.Add(this.numAnioSemana);
            this.panelSemana.Controls.Add(this.label3);
            this.panelSemana.Controls.Add(this.label2);
            this.panelSemana.Location = new System.Drawing.Point(444, 67);
            this.panelSemana.Name = "panelSemana";
            this.panelSemana.Size = new System.Drawing.Size(311, 32);
            this.panelSemana.TabIndex = 39;
            this.panelSemana.Visible = false;
            // 
            // numSemana
            // 
            this.numSemana.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.numSemana.Location = new System.Drawing.Point(91, 2);
            this.numSemana.Maximum = new decimal(new int[] {
            53,
            0,
            0,
            0});
            this.numSemana.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSemana.Name = "numSemana";
            this.numSemana.Size = new System.Drawing.Size(70, 26);
            this.numSemana.TabIndex = 43;
            this.numSemana.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numAnioSemana
            // 
            this.numAnioSemana.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.numAnioSemana.Location = new System.Drawing.Point(243, 3);
            this.numAnioSemana.Maximum = new decimal(new int[] {
            2100,
            0,
            0,
            0});
            this.numAnioSemana.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numAnioSemana.Name = "numAnioSemana";
            this.numAnioSemana.Size = new System.Drawing.Size(70, 26);
            this.numAnioSemana.TabIndex = 42;
            this.numAnioSemana.Value = new decimal(new int[] {
            2024,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(0, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 20);
            this.label3.TabIndex = 41;
            this.label3.Text = "Semana:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(191, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 20);
            this.label2.TabIndex = 40;
            this.label2.Text = "Año:";
            // 
            // cbxPeriodo
            // 
            this.cbxPeriodo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.cbxPeriodo.FormattingEnabled = true;
            this.cbxPeriodo.Location = new System.Drawing.Point(535, 32);
            this.cbxPeriodo.Name = "cbxPeriodo";
            this.cbxPeriodo.Size = new System.Drawing.Size(212, 28);
            this.cbxPeriodo.TabIndex = 39;
            this.cbxPeriodo.SelectedIndexChanged += new System.EventHandler(this.cbxPeriodo_SelectedIndexChanged);
            // 
            // panelDia
            // 
            this.panelDia.Controls.Add(this.label6);
            this.panelDia.Controls.Add(this.dtpFechaContrato);
            this.panelDia.Location = new System.Drawing.Point(444, 67);
            this.panelDia.Name = "panelDia";
            this.panelDia.Size = new System.Drawing.Size(311, 32);
            this.panelDia.TabIndex = 38;
            this.panelDia.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(0, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 20);
            this.label6.TabIndex = 42;
            this.label6.Text = "Fecha:";
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Image = global::ControlPagoLotes.Properties.Resources.boton_eliminar;
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Location = new System.Drawing.Point(872, 72);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(148, 40);
            this.btnCancelar.TabIndex = 37;
            this.btnCancelar.Text = "CANCELAR";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // bntExportar
            // 
            this.bntExportar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bntExportar.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.bntExportar.Image = global::ControlPagoLotes.Properties.Resources.sobresalir;
            this.bntExportar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bntExportar.Location = new System.Drawing.Point(1050, 28);
            this.bntExportar.Name = "bntExportar";
            this.bntExportar.Size = new System.Drawing.Size(148, 40);
            this.bntExportar.TabIndex = 36;
            this.bntExportar.Text = "EXPORTAR";
            this.bntExportar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bntExportar.UseVisualStyleBackColor = true;
            this.bntExportar.Click += new System.EventHandler(this.bntExportar_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // formCorteCaja
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1245, 611);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.dgvRegistros);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(913, 650);
            this.Name = "formCorteCaja";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Corte de caja";
            this.Load += new System.EventHandler(this.formCorteCaja_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegistros)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelMes.ResumeLayout(false);
            this.panelMes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericAnioMes)).EndInit();
            this.panelSemana.ResumeLayout(false);
            this.panelSemana.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSemana)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAnioSemana)).EndInit();
            this.panelDia.ResumeLayout(false);
            this.panelDia.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel tsTotalRegistros;
        private System.Windows.Forms.DataGridView dgvRegistros;
        private System.Windows.Forms.DateTimePicker dtpFechaContrato;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button bntExportar;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripLabel tsTotalDia;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripLabel tsNuevoIngreso;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripLabel tsMontoModificado;
        private System.Windows.Forms.ToolStripLabel toolStripLabel7;
        private System.Windows.Forms.ToolStripLabel tsMontoEliminado;
        private System.Windows.Forms.ToolStripLabel tsCargandoInformacion;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripLabel tsTotalMigrado;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripLabel tsMigradosModificados;
        private System.Windows.Forms.ToolStripLabel toolStripLabel8;
        private System.Windows.Forms.ToolStripLabel tsTotalTransferencias;
        private System.Windows.Forms.Panel panelDia;
        private System.Windows.Forms.Panel panelSemana;
        private System.Windows.Forms.ComboBox cbxPeriodo;
        private System.Windows.Forms.Panel panelMes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbxMeses;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numAnioSemana;
        private System.Windows.Forms.NumericUpDown numericAnioMes;
        private System.Windows.Forms.NumericUpDown numSemana;
        private System.Windows.Forms.ComboBox cbxLotificaciones;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkTodas;
        private System.Windows.Forms.Button btnConsultar;
    }
}