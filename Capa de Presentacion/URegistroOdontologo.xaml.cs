using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Capa_de_Datos;
using Capa_de_Negocio;

namespace Capa_de_Presentacion
{
    /// <summary>
    /// Interaction logic for URegistroOdontologo.xaml
    /// </summary>
    public partial class URegistroOdontologo : UserControl
    {
        private NOdontologo nOdontologo = new NOdontologo();
        private DOdontologo dOdontologo = new DOdontologo();

        public URegistroOdontologo()
        {
            InitializeComponent();
            CargarEspecialidades();
            CargarTurnos();
            MostrarOdontologos(nOdontologo.ListarOdontologosVista());
        }

        private void MostrarOdontologos(List<OdontologoVistaDTO> listaodontologo)
        {
            dgOdontologos.ItemsSource = null;
            if (listaodontologo.Count == 0)
            {
                return;
            }
            else
            {
                dgOdontologos.ItemsSource = listaodontologo;
            }
        }

        private void CargarEspecialidades()
        {
            var especialidades = dOdontologo.ListarEspecialidades();
            cmbEspecialidad.ItemsSource = especialidades;
            cmbEspecialidad.DisplayMemberPath = "Nombre";
            cmbEspecialidad.SelectedValuePath = "EspecialidadId";
        }

        private void CargarTurnos()
        {
            var turnos = dOdontologo.ListarTurnos();
            cmbTurno.ItemsSource = turnos;
            cmbTurno.DisplayMemberPath = "RangoHorario"; // Propiedad calculada en Turno
            cmbTurno.SelectedValuePath = "TurnoId";
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validaciones de campos obligatorios
                if (txtDNI.Text.Length != 8)
                {
                    MessageBox.Show("El DNI debe tener exactamente 8 dígitos numéricos.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDNI.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtDNI.Text))
                {
                    MessageBox.Show("El campo DNI es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDNI.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtNombres.Text))
                {
                    MessageBox.Show("El campo Nombres es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtNombres.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtApellidos.Text))
                {
                    MessageBox.Show("El campo Apellidos es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtApellidos.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    MessageBox.Show("El campo Usuario es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtUsername.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtContrasena.Password))
                {
                    MessageBox.Show("El campo Contraseña es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtContrasena.Focus();
                    return;
                }
                if (cmbEspecialidad.SelectedItem == null)
                {
                    MessageBox.Show("Debe seleccionar una especialidad.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    cmbEspecialidad.Focus();
                    return;
                }
                if (cmbTurno.SelectedItem == null)
                {
                    MessageBox.Show("Debe seleccionar un turno.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    cmbTurno.Focus();
                    return;
                }

                var turnoSeleccionado = cmbTurno.SelectedItem as Turno;
                var especialidadSeleccionada = cmbEspecialidad.SelectedItem as Especialidad;

                // Crear el odontólogo y el usuario asociado dentro del inicializador
                var odontologo = new Odontologo
                {
                    DNI = txtDNI.Text.Trim(),
                    Nombre = txtNombres.Text.Trim(),
                    Apellido = txtApellidos.Text.Trim(),
                    FechaRegistro = DateTime.Now,
                    EspecialidadId = especialidadSeleccionada.EspecialidadId,
                    TurnoId = turnoSeleccionado.TurnoId,
                    Estado = "Activo",
                    Usuario = new Usuario
                    {
                        Username = txtUsername.Text.Trim(),
                        PasswordHash = txtContrasena.Password, // Solo aquí se asigna la contraseña
                        Role = "Odontólogo"
                    }
                };

                string resultado = nOdontologo.Registrar(odontologo);

                MessageBox.Show(resultado, "Registro de Odontólogo", MessageBoxButton.OK, MessageBoxImage.Information);
                Limpiar();
                MostrarOdontologos(nOdontologo.ListarOdontologosVista());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error crítico al guardar", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (dgOdontologos.SelectedItem is OdontologoVistaDTO odontologoSeleccionado)
            {
                try
                {
                    // Buscar el odontólogo real por Id (más seguro que por DNI)
                    var odontologo = dOdontologo.ObtenerPorId(
                        dOdontologo.ListarTodo()
                            .FirstOrDefault(o => o.DNI == odontologoSeleccionado.DNI)?.Id ?? 0);

                    if (odontologo == null)
                    {
                        MessageBox.Show("No se encontró el odontólogo en la base de datos.", "Modificar Odontólogo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    odontologo.DNI = txtDNI.Text.Trim();
                    odontologo.Nombre = txtNombres.Text.Trim();
                    odontologo.Apellido = txtApellidos.Text.Trim();
                    odontologo.EspecialidadId = (cmbEspecialidad.SelectedItem as Especialidad)?.EspecialidadId ?? odontologo.EspecialidadId;
                    odontologo.TurnoId = (cmbTurno.SelectedItem as Turno)?.TurnoId ?? odontologo.TurnoId;
                    odontologo.Usuario.Username = txtUsername.Text.Trim();

                    string resultado = nOdontologo.Modificar(odontologo);
                    MessageBox.Show(resultado, "Modificar Odontólogo", MessageBoxButton.OK, MessageBoxImage.Information);
                    MostrarOdontologos(nOdontologo.ListarOdontologosVista());
                    Limpiar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error al modificar", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Seleccione un odontólogo para modificar.", "Modificar Odontólogo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgOdontologos.SelectedItem is OdontologoVistaDTO odontologoSeleccionado)
            {
                // Buscar el odontólogo real por DNI o Id
                var odontologo = dOdontologo.ListarTodo()
                    .FirstOrDefault(o => o.DNI == odontologoSeleccionado.DNI);

                if (odontologo == null)
                {
                    MessageBox.Show("No se encontró el odontólogo en la base de datos.", "Eliminar Odontólogo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var confirm = MessageBox.Show("¿Está seguro que desea eliminar este odontólogo?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        string resultado = nOdontologo.Eliminar(odontologo.Id);
                        MessageBox.Show(resultado, "Eliminar Odontólogo", MessageBoxButton.OK, MessageBoxImage.Information);
                        Limpiar();
                        MostrarOdontologos(nOdontologo.ListarOdontologosVista());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Error al eliminar", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un odontólogo para eliminar.", "Eliminar Odontólogo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void dgOdontologos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgOdontologos.SelectedItem is OdontologoVistaDTO odontologo)
            {
                txtDNI.Text = odontologo.DNI;
                txtNombres.Text = odontologo.NombreCompleto?.Split(' ').FirstOrDefault() ?? string.Empty;
                txtApellidos.Text = odontologo.NombreCompleto?.Contains(" ") == true
                    ? odontologo.NombreCompleto.Substring(odontologo.NombreCompleto.IndexOf(' ') + 1)
                    : string.Empty;
                txtUsername.Text = odontologo.Username ?? string.Empty;
                // No se puede obtener la contraseña desde el DTO por seguridad

                // Selecciona la especialidad en el ComboBox
                if (cmbEspecialidad.ItemsSource is IEnumerable<Especialidad> especialidades)
                {
                    cmbEspecialidad.SelectedItem = especialidades.FirstOrDefault(es => es.Nombre == odontologo.Especialidad);
                }
                else
                {
                    cmbEspecialidad.SelectedIndex = -1;
                }

                // Selecciona el turno en el ComboBox
                if (cmbTurno.ItemsSource is IEnumerable<Turno> turnos)
                {
                    cmbTurno.SelectedItem = turnos.FirstOrDefault(tu =>
                        $"{tu.HoraInicio} - {tu.HoraFin}" == odontologo.Turno);
                }
                else
                {
                    cmbTurno.SelectedIndex = -1;
                }
            }
        }

        public void Limpiar()
        {
            txtDNI.Clear();
            txtNombres.Clear();
            txtApellidos.Clear();
            txtUsername.Clear();
            txtContrasena.Clear();
            cmbEspecialidad.SelectedIndex = -1;
            cmbTurno.SelectedIndex = -1;
            dgOdontologos.UnselectAll();
        }
    }
}
