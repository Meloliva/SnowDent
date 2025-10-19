using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Capa_de_Negocio;

namespace Capa_de_Presentacion
{
    /// <summary>
    /// Lógica de interacción para UPacientesRangoFecha.xaml
    /// </summary>
    public partial class UPacientesRangoFecha : UserControl
    {
        private NReporteRecepcionista nReporte = new NReporteRecepcionista();

        public UPacientesRangoFecha()
        {
            InitializeComponent();
            BtnBuscar.Click += BtnBuscar_Click;
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (DpFechaInicio.SelectedDate == null || DpFechaFin.SelectedDate == null)
            {
                MessageBox.Show("Seleccione ambas fechas.");
                return;
            }

            DateTime fechaInicio = DpFechaInicio.SelectedDate.Value.Date;
            DateTime fechaFin = DpFechaFin.SelectedDate.Value.Date;

            if (fechaFin < fechaInicio)
            {
                MessageBox.Show("La fecha final no puede ser menor que la fecha inicial.");
                return;
            }

            var pacientes = nReporte.PacientesPorRangoFecha(fechaInicio, fechaFin);
            dgPacientesxFecha.ItemsSource = null;
            dgPacientesxFecha.ItemsSource = pacientes;
        }
    }
}
