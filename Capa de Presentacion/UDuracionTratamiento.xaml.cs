using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using Capa_de_Negocio;
using Capa_de_Datos;
using System.Data.Entity;
using System.Windows.Media;

namespace Capa_de_Presentacion
{
    /// <summary>
    /// Lógica de interacción para UDuracionTratamiento.xaml
    /// </summary>
    public partial class UDuracionTratamiento : UserControl
    {
        private NReporteOdontologo_grafico nReporte = new NReporteOdontologo_grafico();

        public UDuracionTratamiento()
        {
            InitializeComponent();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string dni = TxtDni.Text.Trim();

            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Por favor, ingrese un DNI.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Verificar existencia del paciente
            DPaciente dPaciente = new DPaciente();
            var paciente = dPaciente.ListarTodo().FirstOrDefault(p => p.DNI == dni);
            if (paciente == null)
            {
                MessageBox.Show("No se encontró un paciente con ese DNI.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Buscar todos los tratamientos terminados asociados al paciente
            DTratamiento dTratamiento = new DTratamiento();
            var tratamientos = dTratamiento.ListarActivosReporte()
                .Where(t => t.HistorialClinico != null
                    && t.HistorialClinico.HistorialGeneral.PacienteDNI == dni
                    && t.InicioTratamiento != null
                    && t.FinTratamiento != null)
                .ToList();

            if (tratamientos.Count == 0)
            {
                MessageBox.Show("El paciente no tiene un tratamiento terminado con fecha de inicio y fin.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Mostrar solo los tratamientos terminados de ese paciente en la gráfica
            CargarHistogramaPaciente(tratamientos);
        }

        private void CargarHistogramaPaciente(List<Tratamiento> tratamientos)
        {
            // Agrupar por tipo de tratamiento y calcular duración promedio
            var datos = tratamientos
                .GroupBy(t => t.TipoTratamiento != null ? t.TipoTratamiento.Nombre : "Sin Tipo")
                .Select(g => new ReporteGraficaDTO
                {
                    Nombre = g.Key,
                    Valor = g.Average(t => (t.FinTratamiento - t.InicioTratamiento).TotalDays)
                })
                .ToList();

            barChartTratamientos.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "",
                    Values = new ChartValues<double>(datos.ConvertAll(x => x.Valor)),
                    Fill = new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                    MaxColumnWidth = 50,
                    DataLabels = false
                }
            };

            barChartTratamientos.AxisX.Clear();
            barChartTratamientos.AxisX.Add(new Axis
            {
                Title = "Tipo de Tratamiento",
                Labels = datos.ConvertAll(x => x.Nombre),
                Separator = new LiveCharts.Wpf.Separator { Step = 1, IsEnabled = false }
            });

            barChartTratamientos.AxisY.Clear();
            barChartTratamientos.AxisY.Add(new Axis
            {
                Title = "Duración Promedio (días)",
                MinValue = 0,
                Separator = new LiveCharts.Wpf.Separator { StrokeThickness = 1 }
            });
        }
    }
}
