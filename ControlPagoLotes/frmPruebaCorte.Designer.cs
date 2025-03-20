namespace ControlPagoLotes
{
    partial class frmPruebaCorte
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
            this.btnConsultar = new System.Windows.Forms.Button();
            this.dtpFechaContrato = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // btnConsultar
            // 
            this.btnConsultar.Location = new System.Drawing.Point(300, 213);
            this.btnConsultar.Name = "btnConsultar";
            this.btnConsultar.Size = new System.Drawing.Size(75, 23);
            this.btnConsultar.TabIndex = 0;
            this.btnConsultar.Text = "Consultar";
            this.btnConsultar.UseVisualStyleBackColor = true;
            this.btnConsultar.Click += new System.EventHandler(this.btnConsultar_Click);
            // 
            // dtpFechaContrato
            // 
            this.dtpFechaContrato.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dtpFechaContrato.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dtpFechaContrato.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaContrato.Location = new System.Drawing.Point(233, 119);
            this.dtpFechaContrato.Name = "dtpFechaContrato";
            this.dtpFechaContrato.Size = new System.Drawing.Size(217, 26);
            this.dtpFechaContrato.TabIndex = 36;
            // 
            // frmPruebaCorte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dtpFechaContrato);
            this.Controls.Add(this.btnConsultar);
            this.Name = "frmPruebaCorte";
            this.Text = "frmPruebaCorte";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConsultar;
        private System.Windows.Forms.DateTimePicker dtpFechaContrato;
    }
}