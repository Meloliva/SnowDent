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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Capa_de_Datos;
using Capa_de_Negocio;

namespace Capa_de_Presentacion
{
    /// <summary>
    /// Lógica de interacción para UserPaciente.xaml
    /// </summary>
    public partial class UserPaciente : UserControl
    {
        private NPaciente nPaciente = new NPaciente();
        public UserPaciente()
        {
            InitializeComponent();
            MostrarPacientesActivos(nPaciente.ListarActivos());
        }
        private void MostrarPacientesActivos(List<Paciente> pacientesactivos)
        {
            dgPacientes.ItemsSource = null;
            if (pacientesactivos.Count == 0)
            {
                return;
            }
            else
            {
                dgPacientes.ItemsSource = pacientesactivos;
            }
        }

        //private void btnGuardar(object sender, RoutedEventArgs e)
        //{

        //}
        private void dgPacientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPacientes.SelectedItem is Paciente seleccionado)
            {
                txtDNI.Text = seleccionado.DNI;
                txtNombre.Text = seleccionado.Nombre;
                txtApellido.Text = seleccionado.Apellido;
                txtCelular.Text = seleccionado.Celular;
                cbGenero.Text = seleccionado.Genero;
                txtEmail.Text = seleccionado.Email;
            }
        }
        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones de campos vacíos
            if (string.IsNullOrWhiteSpace(txtDNI.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtCelular.Text) ||
                string.IsNullOrWhiteSpace(cbGenero.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos obligatorios.");
                return;
            }

            if (txtDNI.Text.Length != 8 || !txtDNI.Text.All(char.IsDigit))
            {
                MessageBox.Show("El DNI debe tener exactamente 8 dígitos numéricos.");
                return;
            }

            // Validar si el DNI ya existe
            if (nPaciente.DniExiste(txtDNI.Text))
            {
                MessageBox.Show("El DNI ya está registrado.");
                return;
            }

            var paciente = new Paciente
            {
                DNI = txtDNI.Text,
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                Celular = txtCelular.Text,
                Genero = cbGenero.Text,
                Email = txtEmail.Text,
                FechaRegistro = DateTime.Now,
                Estado = "Activo"
            };

            string resultado = nPaciente.Registrar(paciente);
            MessageBox.Show(resultado);

            // Limpiar campos después de registrar
            txtDNI.Text = "";
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtCelular.Text = "";
            cbGenero.SelectedIndex = -1;
            txtEmail.Text = "";

            // Refrescar el DataGrid
            MostrarPacientesActivos(nPaciente.ListarActivos());
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (dgPacientes.SelectedItem is Paciente seleccionado)
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtApellido.Text) ||
                    string.IsNullOrWhiteSpace(txtCelular.Text) ||
                    string.IsNullOrWhiteSpace(cbGenero.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos obligatorios.");
                    return;
                }

                // Actualizar los datos del paciente seleccionado con los valores de los campos
                seleccionado.Nombre = txtNombre.Text;
                seleccionado.Apellido = txtApellido.Text;
                seleccionado.Celular = txtCelular.Text;
                seleccionado.Genero = cbGenero.Text;
                seleccionado.Email = txtEmail.Text;

                string resultado = nPaciente.Modificar(seleccionado);
                MessageBox.Show(resultado);
                //limpiar campos después de modificar
                txtDNI.Text = "";
                txtNombre.Text = "";
                txtApellido.Text = "";
                txtCelular.Text = "";
                cbGenero.SelectedIndex = -1;
                txtEmail.Text = "";
                // Refrescar el DataGrid
                MostrarPacientesActivos(nPaciente.ListarActivos());
            }
            else
            {
                MessageBox.Show("Seleccione un paciente para modificar.");
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgPacientes.SelectedItem is Paciente seleccionado)
            {
                // Eliminación lógica: cambia el estado a "Inactivo"
                string resultado = nPaciente.Eliminar(seleccionado.DNI);
                MessageBox.Show(resultado);
                MostrarPacientesActivos(nPaciente.ListarActivos());
            }
            else
            {
                MessageBox.Show("Seleccione un paciente para eliminar.");
            }
        }
    }
}
