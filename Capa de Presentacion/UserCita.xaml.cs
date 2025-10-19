using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Capa_de_Datos;
using Capa_de_Negocio;
using System.Data.Entity;

namespace Capa_de_Presentacion
{
    /// <summary>
    /// Lógica de interacción para UserCita.xaml
    /// </summary>
    public partial class UserCita : UserControl
    {
        private NCita nCita = new NCita();
        private NOdontologo nOdontologo = new NOdontologo();
        private DOdontologo dOdontologo = new DOdontologo();

        public UserCita()
        {
            InitializeComponent();
            CargarEspecialidades();
            CargarCitas();
        }

        private void CargarEspecialidades()
        {
            var especialidades = dOdontologo.ListarEspecialidades();
            cbespecialidad.ItemsSource = especialidades;
            cbespecialidad.DisplayMemberPath = "Nombre";
            cbespecialidad.SelectedValuePath = "EspecialidadId";
            cbespecialidad.SelectionChanged += cbespecialidad_SelectionChanged;
        }

        private void cbespecialidad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarOdontologos();
        }

        private void CargarOdontologos()
        {
            var odontologos = nOdontologo.ListarTodo();

            if (cbespecialidad.SelectedItem is Especialidad especialidadSeleccionada)
            {
                odontologos = odontologos
                    .Where(o => o.EspecialidadId == especialidadSeleccionada.EspecialidadId)
                    .ToList();
            }

            cbodontologo.ItemsSource = odontologos;
            cbodontologo.DisplayMemberPath = "NombreCompleto";
            cbodontologo.SelectedValuePath = "Id";
        }

        private void CargarCitas()
        {
            try
            {
                var citas = nCita.ListarTodo(); // List<CitaVistaDTO>
                // No es necesario poner dgCitas.ItemsSource = null aquí
                if (citas != null && citas.Count > 0)
                {
                    var citasFiltradas = citas
                        .Where(c => c.Estado == "Programada"
                                 && c.EstadoPaciente == "Activo")
                        .ToList();
                    dgCitas.ItemsSource = citasFiltradas;
                }
                else
                {
                    dgCitas.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                if (ex.InnerException != null)
                    mensaje += "\n" + ex.InnerException.Message;
                MessageBox.Show("Error al cargar citas: " + mensaje);
            }
        }

        private void dgCitas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCitas.SelectedItem is CitaVistaDTO seleccionada)
            {
                tbdni.Text = seleccionada.PacienteDNI;
                tbnombre.Text = seleccionada.NombrePaciente ?? "";
                tbapellido.Text = seleccionada.ApellidoPaciente ?? "";

                // Selecciona la especialidad y odontólogo correspondientes
                var odontologo = nOdontologo.ListarTodo().FirstOrDefault(o => o.DNI == seleccionada.OdontologoDNI);
                if (odontologo != null)
                {
                    cbespecialidad.SelectedValue = odontologo.EspecialidadId;
                    CargarOdontologos();
                    cbodontologo.SelectedValue = odontologo.Id;
                }
                else
                {
                    cbodontologo.SelectedIndex = -1;
                }

                dtfecha.SelectedDate = seleccionada.Fecha;
                tbhora.Text = seleccionada.Hora.ToString(@"hh\:mm");
            }
        }

        private void btnbuscar_Click(object sender, RoutedEventArgs e)
        {
            string dni = tbdni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Ingrese un DNI para buscar.");
                return;
            }

            Paciente paciente = nCita.BuscarPacientePorDNI(dni);

            if (paciente != null)
            {
                tbnombre.Text = paciente.Nombre;
                tbapellido.Text = paciente.Apellido;
            }
            else
            {
                tbnombre.Text = "";
                tbapellido.Text = "";
                MessageBox.Show("Paciente no encontrado o inactivo.");
            }
        }

        private void btnguardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbdni.Text) ||
                cbodontologo.SelectedValue == null ||
                dtfecha.SelectedDate == null ||
                string.IsNullOrWhiteSpace(tbhora.Text))
            {
                MessageBox.Show("Complete todos los campos obligatorios.");
                return;
            }

            if (!TimeSpan.TryParse(tbhora.Text, out TimeSpan horaSpan))
            {
                MessageBox.Show("Formato de hora incorrecto. Usa HH:mm (ej: 18:00)");
                return;
            }

            var odontologo = cbodontologo.SelectedItem as Odontologo;
            if (odontologo == null)
            {
                MessageBox.Show("Seleccione un odontólogo válido.");
                return;
            }

            var turno = dOdontologo.ListarTurnos().Find(t => t.TurnoId == odontologo.TurnoId);
            if (turno == null)
            {
                MessageBox.Show("No se encontró el turno del odontólogo.");
                return;
            }

            if (horaSpan < turno.HoraInicio || horaSpan >= turno.HoraFin)
            {
                MessageBox.Show("La hora seleccionada no corresponde al turno del odontólogo.");
                return;
            }

            var fechaSeleccionada = dtfecha.SelectedDate.Value.Date;

            // Validar si el odontólogo ya tiene una cita en ese horario
            var citasMismoHorario = nCita.ListarProgramadas()
                .Where(c => c.OdontologoId == odontologo.Id
                         && c.Fecha == fechaSeleccionada
                         && c.Hora == horaSpan)
                .ToList();

            if (citasMismoHorario.Count > 0)
            {
                MessageBox.Show("El odontólogo ya tiene una cita programada en ese horario.");
                return;
            }

            // Validar si el odontólogo tiene un tratamiento con próxima cita en ese horario
            var tratamientos = nCita.ListarTratamientos();
            var tratamientoMismoHorario = tratamientos
                .Where(t => t.Cita.OdontologoId == odontologo.Id
                         && t.FechaProximaCita == fechaSeleccionada
                         && t.HoraProximaCita == horaSpan)
                .ToList();

            if (tratamientoMismoHorario.Count > 0)
            {
                MessageBox.Show("El odontólogo ya tiene un tratamiento programado en ese horario.");
                return;
            }

            var cita = new Cita
            {
                PacienteDNI = tbdni.Text,
                OdontologoId = odontologo.Id,
                Fecha = fechaSeleccionada,
                Hora = horaSpan,
                Estado = "Programada"
            };

            // Usar el objeto odontologo para obtener el turno
            var turnoCita = dOdontologo.ListarTurnos().Find(t => t.TurnoId == odontologo.TurnoId);
            if (turnoCita == null)
            {
                MessageBox.Show("No se encontró el turno del odontólogo.");
                return;
            }
            if (horaSpan < turnoCita.HoraInicio || horaSpan >= turnoCita.HoraFin)
            {
                MessageBox.Show("La hora de la próxima cita no corresponde al turno del odontólogo.");
                return;
            }

            try
            {
                string resultado = nCita.Registrar(cita);
                MessageBox.Show(resultado);
                CargarCitas();

                var envioCorreos = new DEnvioCorreos();
                string resultadoCorreo = envioCorreos.EnviarConfirmacionCita(
                    tbdni.Text,
                    fechaSeleccionada,
                    horaSpan
                );
                MessageBox.Show(resultadoCorreo, "Envío de correo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                Exception inner = ex.InnerException;
                while (inner != null)
                {
                    mensaje += "\n" + inner.Message;
                    inner = inner.InnerException;
                }
                mensaje += "\nStackTrace:\n" + ex.StackTrace;
                MessageBox.Show("Error al registrar cita: " + mensaje);
            }
        }

        private void btnmodificar_Click(object sender, RoutedEventArgs e)
        {
            if (dgCitas.SelectedItem is CitaVistaDTO seleccionada)
            {
                if (string.IsNullOrWhiteSpace(tbdni.Text) ||
                    cbodontologo.SelectedValue == null ||
                    dtfecha.SelectedDate == null ||
                    string.IsNullOrWhiteSpace(tbhora.Text))
                {
                    MessageBox.Show("Complete todos los campos obligatorios.");
                    return;
                }

                if (!TimeSpan.TryParse(tbhora.Text, out TimeSpan horaSpan))
                {
                    MessageBox.Show("Formato de hora incorrecto. Usa HH:mm (ej: 18:00)");
                    return;
                }

                var odontologo = cbodontologo.SelectedItem as Odontologo;
                if (odontologo == null)
                {
                    MessageBox.Show("Seleccione un odontólogo válido.");
                    return;
                }

                var turno = dOdontologo.ListarTurnos().Find(t => t.TurnoId == odontologo.TurnoId);
                if (turno == null)
                {
                    MessageBox.Show("No se encontró el turno del odontólogo.");
                    return;
                }

                if (horaSpan < turno.HoraInicio || horaSpan >= turno.HoraFin)
                {
                    MessageBox.Show("La hora seleccionada no corresponde al turno del odontólogo.");
                    return;
                }

                var fechaSeleccionada = dtfecha.SelectedDate.Value.Date;

                var citasMismoHorario = nCita.ListarProgramadas()
                    .Where(c => c.OdontologoId == odontologo.Id
                             && c.Fecha == fechaSeleccionada
                             && c.Hora == horaSpan
                             && c.IdCita != seleccionada.IdCita)
                    .ToList();

                if (citasMismoHorario.Count > 0)
                {
                    MessageBox.Show("El odontólogo ya tiene una cita programada en ese horario.");
                    return;
                }

                var tratamientos = nCita.ListarTratamientos();
                var tratamientoMismoHorario = tratamientos
                    .Where(t => t.Cita.OdontologoId == odontologo.Id
                             && t.FechaProximaCita == fechaSeleccionada
                             && t.HoraProximaCita == horaSpan
                             && t.CitaId != seleccionada.IdCita)
                    .ToList();

                if (tratamientoMismoHorario.Count > 0)
                {
                    MessageBox.Show("El odontólogo ya tiene un tratamiento programado en ese horario.");
                    return;
                }

                var cita = new Cita
                {
                    IdCita = seleccionada.IdCita,
                    PacienteDNI = tbdni.Text,
                    OdontologoId = odontologo.Id,
                    Fecha = fechaSeleccionada,
                    Hora = horaSpan,
                    Estado = seleccionada.Estado
                };

                string resultado = nCita.Modificar(cita);
                MessageBox.Show(resultado);
                CargarCitas();
            }
            else
            {
                MessageBox.Show("Seleccione una cita para modificar.");
            }
        }

        private void btnEliminar(object sender, RoutedEventArgs e)
        {
            if (dgCitas.SelectedItem is CitaVistaDTO seleccionada)
            {
                string resultado = nCita.Eliminar(seleccionada.IdCita);
                MessageBox.Show(resultado);
                CargarCitas();
            }
            else
            {
                MessageBox.Show("Seleccione una cita para eliminar.");
            }
        }
    }
}
