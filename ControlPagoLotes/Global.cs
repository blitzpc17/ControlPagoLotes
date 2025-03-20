using Entidades;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlPagoLotes
{
    public static class Global
    {
        public static UsuarioL ObjUsuario;
        
        public static void LimpiarControles(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Clear();
                }
                else if (control is ComboBox comboBox)
                {
                    if (comboBox.DataSource != null)
                    {
                        comboBox.DataSource = null;
                    }
                    else
                    {
                        comboBox.Items.Clear();
                    }
                }
                else if (control is DataGridView dataGridView)
                {
                    dataGridView.DataSource = null;
                    dataGridView.Rows.Clear();
                }
                else if (control is GroupBox || control is Panel || control is TabPage || control is TabControl)
                {
                    LimpiarControles(control);
                }
            }
        }


        public static DateTime FechaServidor()
        {
            return DateTime.Now;
        }

        public static decimal FormatearPesosADecimal(string input)
        {
            return decimal.Parse(input, NumberStyles.Number, CultureInfo.InvariantCulture);
        }

        public static string FormatearFecha(string input, string formato)
        {            
            DateTime fecha;

            // Intentar convertir en el formato "dd/MM/yyyy"
            if (DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                // Convertir al formato "yyyy-MM-dd"
                return fecha.ToString(formato);
            }
            else
            {
                return fecha.ToString("yyyy-MM-dd");
            }
        }


    }
}
