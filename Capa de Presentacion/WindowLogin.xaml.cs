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
using Capa_de_Negocio;

namespace Capa_de_Presentacion
{
    /// <summary>
    /// Interaction logic for WindowLogin.xaml
    /// </summary>
    
    public partial class WindowLogin : Window
    {
        private NLogueo logueo = new NLogueo();

        public WindowLogin()
        {
            InitializeComponent();
        }

        private void tbusuario_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void tbcontraseña_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnlogin_Click(object sender, RoutedEventArgs e)
        {
            string username = tbusuario.Text;
            string password = tbcontraseña.Password;
            string rolSeleccionado = (cbrol.SelectedItem as ComboBoxItem)?.Content.ToString();

            var nLogueo = new NLogueo();
            var usuario = nLogueo.Login(username, password, rolSeleccionado);

            // Asignar el rol seleccionado si el usuario es válido y el rol está vacío o nulo
            if (usuario != null)
            {
                if (string.IsNullOrEmpty(usuario.Role))
                {
                    usuario.Role = rolSeleccionado;
                }
                var ventanaPrincipal = new WindowPrincipal(rolSeleccionado,usuario);
                ventanaPrincipal.Show();

                if (rolSeleccionado != "Recepcionista")
                {
                    ventanaPrincipal.ConfigurarVisibilidadPorRol();
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Credenciales incorrectas, rol no coincide o campos incompletos.");
            }
        }

        private void btnsalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
