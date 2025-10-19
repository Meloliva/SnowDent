using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Capa_de_Datos;

namespace Capa_de_Presentacion
{
    /// <summary>
    /// Interaction logic for WindowPrincipal.xaml
    /// </summary>
    public partial class WindowPrincipal : Window
    {
        public string _rol;
        public Usuario usuarioActual;

        // Constructor recomendado: primero Usuario, luego rol
        public WindowPrincipal(string rol, Usuario usuario)
        {
            InitializeComponent();
            usuarioActual = usuario;
            _rol = rol;
            ConfigurarVisibilidadPorRol();
            
        }

        // Constructor por defecto (opcional, si lo necesitas en otros lugares)

        public void ConfigurarVisibilidadPorRol()
        {
            if (_rol == "Recepcionista")
            {
                // Muestra todos los botones
                btncasoclinico.Visibility = Visibility.Visible;
                btnHistorialclinico.Visibility = Visibility.Visible;
                btnduraciontratamiento.Visibility = Visibility.Visible;
                btnOdontologo.Visibility = Visibility.Visible;
                btnpaciente.Visibility = Visibility.Visible;
                btncita.Visibility = Visibility.Visible;
                btnreporteporgenero.Visibility = Visibility.Visible;
                btnconteocitasportratamiento.Visibility = Visibility.Visible;
                tbrangofecha.Visibility = Visibility.Visible;
                btntratamientocurso.Visibility = Visibility.Visible;
            }
            else
            {
                // Solo muestra los permitidos
                btncasoclinico.Visibility = Visibility.Visible;
                btnHistorialclinico.Visibility = Visibility.Visible;
                btnduraciontratamiento.Visibility = Visibility.Visible;

                // Oculta el resto del menú lateral
                btnOdontologo.Visibility = Visibility.Collapsed;
                btnpaciente.Visibility = Visibility.Collapsed;
                btncita.Visibility = Visibility.Collapsed;
                btnreporteporgenero.Visibility = Visibility.Collapsed;
                btnconteocitasportratamiento.Visibility = Visibility.Collapsed;
                tbrangofecha.Visibility = Visibility.Collapsed;
                btntratamientocurso.Visibility = Visibility.Collapsed;
            }
        }

        private void btnOdontologo_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new URegistroOdontologo();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UserPaciente();
        }

        private void btncita_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UserCita();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UserCantidadGenero();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UserContarcitasportratamiento();
        }

        private void tbrangofecha_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UPacientesRangoFecha();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UPacientesConTratamientos();
        }

        private void btnHistorialclinico_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UDetalleHistoriaClinica();
        }

        private void btncasoclinico_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UCasoClinico(usuarioActual);
        }

        private void btnduraciontratamiento_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UDuracionTratamiento();
        }

        private void btnsalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
