using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using Capa_de_Negocio;
using Capa_de_Datos;

namespace Capa_de_Presentacion
{
    /// <summary>
    /// Lógica de interacción para UserCantidadGenero.xaml
    /// </summary>
    public partial class UserCantidadGenero : UserControl, INotifyPropertyChanged
    {
        private NReporteOdontologo_grafico nReporte = new NReporteOdontologo_grafico();

        private string _genero;
        public string Genero
        {
            get { return _genero; }
            set
            {
                if (_genero != value)
                {
                    _genero = value;
                    OnPropertyChanged("Genero");
                }
            }
        }

        public UserCantidadGenero()
        {
            InitializeComponent();
            this.DataContext = this; // Importante para el binding en XAML
            CargarGraficoPie();
        }

        private void CargarGraficoPie()
        {
            List<ReporteGraficaDTO> datos = nReporte.PacientesPorGenero();

            var series = new SeriesCollection();
            foreach (var item in datos)
            {
                // Asigna color según el género
                Brush color = Brushes.Gray; // Color por defecto
                if (item.Nombre.ToLower().Contains("fem"))
                    color = (Brush)new BrushConverter().ConvertFromString("#FFB5C0"); // Rosado
                else if (item.Nombre.ToLower().Contains("masc"))
                    color = (Brush)new BrushConverter().ConvertFromString("#90D5FF"); // Azul

                series.Add(new PieSeries
                {
                    Title = item.Nombre,
                    Values = new ChartValues<double> { item.Valor },
                    DataLabels = true,
                    Fill = color
                });
            }
            pieChartGenero.Series = series;

            // Asigna el género predominante para el fondo (opcional, puedes quitarlo si ya no lo necesitas)
            if (datos.Count > 0)
            {
                var generoPredominante = datos.OrderByDescending(x => x.Valor).First().Nombre;
                if (generoPredominante.ToLower().Contains("fem"))
                    Genero = "Femenino";
                else if (generoPredominante.ToLower().Contains("masc"))
                    Genero = "Masculino";
                else
                    Genero = "Femenino";
            }
            else
            {
                Genero = "Femenino";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
