using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Capa_de_Datos;

namespace Capa_de_Presentacion
{
    /// <summary>
    /// Lógica de interacción para UDetalleHistoriaClinica.xaml
    /// </summary>
    public partial class UDetalleHistoriaClinica : UserControl
    {
        private DHistorialGeneral dHistorialGeneral = new DHistorialGeneral();

        public UDetalleHistoriaClinica()
        {
            InitializeComponent();
        }

        // Ahora acepta List<HistorialGeneralVista>
        public void MostrarCasosClinicos(List<HistorialGeneralVista> casosClinicosVista)
        {
            dgHistorialesClinicos.ItemsSource = casosClinicosVista;
        }
        
        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string dni = TxtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Por favor, ingrese un DNI.");
                return;
            }

            // Obtiene el historial general y muestra los casos clínicos en formato HistorialGeneralVista
            var historialGeneral = dHistorialGeneral.ObtenerPorPaciente(dni);
            if (historialGeneral == null)
            {
                MessageBox.Show("No se encontró historial general para el paciente.");
                return;
            }

            List<HistorialGeneralVista> historiasClinicasVista = dHistorialGeneral.ListarHistorialesClinicos(historialGeneral.IdHistoriaGeneral);
            MostrarCasosClinicos(historiasClinicasVista);
        }
    }
}
