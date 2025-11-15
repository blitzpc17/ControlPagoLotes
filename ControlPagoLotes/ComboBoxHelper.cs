using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

public static class ComboBoxHelper
{
    /// <summary>
    /// Llena un ComboBox con los valores de un Enum
    /// </summary>
    /// <typeparam name="T">Tipo del Enum</typeparam>
    /// <param name="comboBox">ComboBox a llenar</param>
    /// <param name="incluirDescripcion">Si usa DescriptionAttribute para mostrar texto</param>
    /// <param name="valorPorDefecto">Valor seleccionado por defecto (-1 para ninguno)</param>
    public static void LlenarComboBox<T>(this ComboBox comboBox, bool incluirDescripcion = false, int valorPorDefecto = 0) where T : Enum
    {
        if (comboBox == null)
            throw new ArgumentNullException(nameof(comboBox));

        comboBox.Items.Clear();
        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

        var valores = Enum.GetValues(typeof(T)).Cast<T>();

        foreach (var valor in valores)
        {
            string texto = incluirDescripcion ? ObtenerDescripcion(valor) : valor.ToString();
            comboBox.Items.Add(new ComboBoxItem<T>(valor, texto));
        }

        // Seleccionar valor por defecto
        if (comboBox.Items.Count > 0 && valorPorDefecto >= 0)
        {
            int indice = valorPorDefecto < comboBox.Items.Count ? valorPorDefecto : 0;
            comboBox.SelectedIndex = indice;
        }
    }

    /// <summary>
    /// Obtiene el valor seleccionado del Enum desde el ComboBox
    /// </summary>
    public static T ObtenerValorSeleccionado<T>(this ComboBox comboBox) where T : Enum
    {
        if (comboBox?.SelectedItem is ComboBoxItem<T> item)
            return item.Valor;

        return default(T);
    }

    /// <summary>
    /// Selecciona un valor específico del Enum en el ComboBox
    /// </summary>
    public static void SeleccionarValor<T>(this ComboBox comboBox, T valor) where T : Enum
    {
        if (comboBox == null) return;

        for (int i = 0; i < comboBox.Items.Count; i++)
        {
            if (comboBox.Items[i] is ComboBoxItem<T> item && item.Valor.Equals(valor))
            {
                comboBox.SelectedIndex = i;
                return;
            }
        }
    }

    private static string ObtenerDescripcion<T>(T valor) where T : Enum
    {
        var field = valor.GetType().GetField(valor.ToString());
        var attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                               .FirstOrDefault() as DescriptionAttribute;

        return attribute?.Description ?? valor.ToString();
    }
}

// Clase auxiliar para almacenar el valor y texto del ComboBox
public class ComboBoxItem<T>
{
    public T Valor { get; set; }
    public string Texto { get; set; }

    public ComboBoxItem(T valor, string texto)
    {
        Valor = valor;
        Texto = texto;
    }

    public override string ToString()
    {
        return Texto;
    }
}