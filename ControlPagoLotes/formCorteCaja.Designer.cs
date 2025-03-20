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
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.tsNuevoIngreso = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.tsMontoModificado = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel7 = new System.Windows.Forms.ToolStripLabel();
            this.tsMontoEliminado = new System.Windows.Forms.ToolStripLabel();
            this.dgvRegistros = new System.Windows.Forms.DataGridView();
            this.dtpFechaContrato = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.bntExportar = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.tsCargandoInformacion = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.tsTotalMigrado = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegistros)).BeginInit();
            this.panel1.SuspendLayout();
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
            this.tsTotalMigrado});
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
            this.toolStripLabel5.Size = new System.Drawing.Size(158, 22);
            this.toolStripLabel5.Text = "Montos Modificados ($):";
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
            this.toolStripLabel7.Size = new System.Drawing.Size(149, 22);
            this.toolStripLabel7.Text = "Montos Eliminados ($):";
            // 
            // tsMontoEliminado
            // 
            this.tsMontoEliminado.Font = new System.Drawing.Font("Segoe UI Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsMontoEliminado.Name = "tsMontoEliminado";
            this.tsMontoEliminado.Size = new System.Drawing.Size(16, 22);
            this.tsMontoEliminado.Text = "0";
            // 
            // dgvRegistros
            // 
            this.dgvRegistros.AllowUserToAddRows = false;
            this.dgvRegistros.AllowUserToDeleteRows = false;
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
            this.dgvRegistros.Location = new System.Drawing.Point(0, 97);
            this.dgvRegistros.Name = "dgvRegistros";
            this.dgvRegistros.ReadOnly = true;
            this.dgvRegistros.RowHeadersVisible = false;
            this.dgvRegistros.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRegistros.Size = new System.Drawing.Size(1245, 486);
            this.dgvRegistros.TabIndex = 32;
            // 
            // dtpFechaContrato
            // 
            this.dtpFechaContrato.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dtpFechaContrato.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dtpFechaContrato.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaContrato.Location = new System.Drawing.Point(129, 39);
            this.dtpFechaContrato.Name = "dtpFechaContrato";
            this.dtpFechaContrato.Size = new System.Drawing.Size(217, 26);
            this.dtpFechaContrato.TabIndex = 35;
            this.dtpFechaContrato.ValueChanged += new System.EventHandler(this.dtpFechaContrato_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(32, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 20);
            this.label1.TabIndex = 34;
            this.label1.Text = "Consultar:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancelar);
            this.panel1.Controls.Add(this.bntExportar);
            this.panel1.Controls.Add(this.dtpFechaContrato);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1245, 100);
            this.panel1.TabIndex = 36;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Image = global::ControlPagoLotes.Properties.Resources.boton_eliminar;
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Location = new System.Drawing.Point(1060, 35);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(130, 40);
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
            this.bntExportar.Location = new System.Drawing.Point(924, 35);
            this.bntExportar.Name = "bntExportar";
            this.bntExportar.Size = new System.Drawing.Size(130, 40);
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
            this.toolStripLabel4.Size = new System.Drawing.Size(138, 22);
            this.toolStripLabel4.Text = "Montos Migrados($):";
            // 
            // tsTotalMigrado
            // 
            this.tsTotalMigrado.Font = new System.Drawing.Font("Segoe UI Black", 9.75F, System.Drawing.FontStyle.Bold);
            this.tsTotalMigrado.Name = "tsTotalMigrado";
            this.tsTotalMigrado.Size = new System.Drawing.Size(16, 22);
            this.tsTotalMigrado.Text = "0";
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
    }
}