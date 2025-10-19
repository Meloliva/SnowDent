using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Capa_de_Datos;
using Capa_de_Negocio;

namespace Capa_de_Presentacion
{
    /// <summary>
    /// Lógica de interacción para UCasoClinico.xaml
    /// </summary>
    public partial class UCasoClinico : UserControl
    {
        private DTratamiento dTratamiento = new DTratamiento();
        private DCita dCita = new DCita();
        private DHistorialGeneral dHistorialGeneral = new DHistorialGeneral();
        private DOdontologo dOdontologo = new DOdontologo();

        // Declarar los objetos de negocio al inicio de la clase
        private NTratamiento nTratamiento = new NTratamiento();
        private NHistorialGeneral nHistorialGeneral = new NHistorialGeneral();
        private NHistorialClinico nHistorialClinico = new NHistorialClinico();
        private NCita nCita = new NCita();

        private Usuario usuarioActual;
        public UCasoClinico(Usuario usuario)
        {
            InitializeComponent();
            usuarioActual = usuario;
            CargarTiposTratamiento();
            BtnBuscar.Click += BtnBuscar_Click;
            btnguardar.Click += BtnAgregarTratamiento_Click;
            btneditar.Click += BtnModificarTratamiento_Click;
            btneliminar.Click += BtnEliminarTratamiento_Click;
            CbTipoTratamiento.SelectionChanged += CbTipoTratamiento_SelectionChanged;

            btneliminar.Visibility = (usuarioActual != null &&
                usuarioActual.Role != null &&
                usuarioActual.Role.Trim().ToLower() == "recepcionista")
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        private void LimpiarCampos()
        {
            TxtDni.Text = "";
            TxtNombre.Text = "";
            TxtNombreOdontologo.Text = "";
            CbTipoTratamiento.SelectedIndex = -1;
            DpFechaInicio.SelectedDate = null;
            DpFechaFin.SelectedDate = null;
            DpProximaCita.SelectedDate = null;
            TxtHoraProximaCita.Text = "";
            cbEstado.SelectedIndex = -1;
            TxtDiagnostico.Text = "";
        }

        private void MostrarTratamientosActivos(List<Tratamiento> tratamientosactivos)
        {
            dgCasosClinicos.ItemsSource = null;
            if (tratamientosactivos == null || tratamientosactivos.Count == 0)
            {
                MessageBox.Show("No hay tratamientos para mostrar.");
                return;
            }

            var lista = tratamientosactivos.Select(t => new
            {
                t.IdTratamiento,
                t.InicioTratamiento,
                t.FinTratamiento,
                t.FechaProximaCita,
                HoraProximaCita = t.HoraProximaCita.ToString(@"hh\:mm"),
                Paciente = t.Cita?.Paciente != null ? $"{t.Cita.Paciente.Nombre} {t.Cita.Paciente.Apellido}" : "",
                PacienteDNI = t.Cita?.PacienteDNI ?? "",
                Odontologo = t.Cita?.Odontologo != null ? $"{t.Cita.Odontologo.Nombre} {t.Cita.Odontologo.Apellido}" : "",
                TipoTratamiento = t.TipoTratamiento?.Nombre ?? "",
                t.CitaId,
                Diagnostico = t.Diagnostico ?? "",
                Estado = t.Estado ?? ""
            }).ToList();

            dgCasosClinicos.ItemsSource = lista;
        }

        private void CargarTiposTratamiento()
        {
            var tipos = dTratamiento.ListarTipos();
            if (tipos == null || tipos.Count == 0)
            {
                MessageBox.Show("No hay tipos de tratamiento disponibles. Por favor, registre al menos uno.");
                CbTipoTratamiento.ItemsSource = null;
                return;
            }
            CbTipoTratamiento.ItemsSource = tipos;
            CbTipoTratamiento.DisplayMemberPath = "Nombre";
            CbTipoTratamiento.SelectedValuePath = "IdTipoTratamiento";
        }

        private void MostrarTratamientosFiltrados()
        {
            string dni = TxtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni) || !(CbTipoTratamiento.SelectedItem is TipoTratamiento tipoSeleccionado))
            {
                dgCasosClinicos.ItemsSource = null;
                return;
            }

            int tipoId = tipoSeleccionado.IdTipoTratamiento;
            var tratamientosFiltrados = dTratamiento.ListarActivos()
                .Where(t => t.Cita != null
                         && t.Cita.PacienteDNI == dni
                         && t.TipoId == tipoId)
                .ToList();

            MostrarTratamientosActivos(tratamientosFiltrados);
        }

        // Evento para mostrar tratamientos al seleccionar tipo de tratamiento
        private void CbTipoTratamiento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MostrarTratamientosFiltrados();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string dni = TxtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Debe ingresar un DNI válido.");
                return;
            }

            // Validar si el paciente existe
            var paciente = dCita.BuscarPacientePorDNI(dni);
            if (paciente == null)
            {
                TxtNombre.Text = "";
                TxtNombreOdontologo.Text = "";
                dgCasosClinicos.ItemsSource = null;
                MessageBox.Show("El paciente no existe.");
                return;
            }
            TxtNombre.Text = $"{paciente.Nombre} {paciente.Apellido}";

            // Validar si el paciente tiene al menos una cita
            var citasPaciente = dCita.ListarProgramadas()
                .Where(c => c.PacienteDNI == dni)
                .ToList();

            if (citasPaciente.Count == 0)
            {
                TxtNombreOdontologo.Text = "";
                dgCasosClinicos.ItemsSource = null;
                MessageBox.Show("El paciente no tiene citas registradas.");
                return;
            }

            // Autocompletar nombre y apellido del odontólogo de la última cita
            var cita = citasPaciente.OrderByDescending(c => c.IdCita).FirstOrDefault();
            if (cita != null && cita.Odontologo != null)
                TxtNombreOdontologo.Text = $"{cita.Odontologo.Nombre} {cita.Odontologo.Apellido}";
            else
                TxtNombreOdontologo.Text = "";

            // Mostrar tratamientos filtrados
            MostrarTratamientosFiltrados();
        }

        private void BtnAgregarTratamiento_Click(object sender, RoutedEventArgs e)
        {
            string dni = TxtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Debe ingresar un DNI válido.");
                return;
            }

            if (!(CbTipoTratamiento.SelectedItem is TipoTratamiento tipo))
            {
                MessageBox.Show("Seleccione un tipo de tratamiento y un caso clínico.");
                return;
            }
            string titulo = tipo.Nombre;

            var historialGeneral = dHistorialGeneral.CrearYObtenerPorPaciente(dni, titulo);
            if (historialGeneral == null)
            {
                MessageBox.Show("No se pudo crear o encontrar el historial general para el paciente.");
                return;
            }

            var citas = dCita.ListarProgramadas().Where(c => c.PacienteDNI == dni).OrderByDescending(c => c.IdCita).ToList();
            if (citas.Count == 0)
            {
                MessageBox.Show("El paciente no tiene citas registradas.");
                return;
            }
            var cita = citas.First();

            // Validar que no exista ya un tratamiento de este tipo para esta cita
            var tratamientosDeLaCita = dTratamiento.ListarActivos()
                .Where(t => t.CitaId == cita.IdCita)
                .ToList();

            bool existeTratamientoMismoTipo = tratamientosDeLaCita
                .Any(t => t.TipoId == tipo.IdTipoTratamiento);

            if (existeTratamientoMismoTipo)
            {
                MessageBox.Show("Ya existe un tratamiento de este tipo registrado para esta cita. Seleccione otro tipo o cita.");
                return;
            }

            var dHistorialClinico = new DHistorialClinico();
            var historialClinico = dHistorialClinico.CrearYObtener(cita.IdCita, historialGeneral.IdHistoriaGeneral, tipo.IdTipoTratamiento);

            // Obtener fechas y hora desde los controles
            DateTime? fechaInicio = DpFechaInicio.SelectedDate;
            DateTime? fechaFin = DpFechaFin.SelectedDate;
            DateTime? fechaProximaCita = DpProximaCita.SelectedDate;
            TimeSpan horaProximaCita;
            if (!TimeSpan.TryParse(TxtHoraProximaCita.Text, out horaProximaCita))
            {
                MessageBox.Show("Ingrese una hora válida para la próxima cita (formato HH:mm).");
                return;
            }

            // Validar que no exista una cita en la misma fecha y hora para el odontólogo
            int odontologoId = cita.OdontologoId;
            var citasOcupadas = nCita.ListarProgramadas()
                .Where(c => c.OdontologoId == odontologoId
                         && c.Fecha == fechaProximaCita
                         && c.Hora == horaProximaCita)
                .ToList();

            if (citasOcupadas.Count > 0)
            {
                MessageBox.Show("El odontólogo ya tiene una cita programada en esa fecha y hora.");
                return; // No permitir guardar la cita
            }

            // Obtener el turno del odontólogo
            var odontologo = dOdontologo.ObtenerPorId(odontologoId);
            if (odontologo == null)
            {
                MessageBox.Show("No se encontró el odontólogo.");
                return;
            }
            var turno = odontologo.Turno;
            if (turno == null)
            {
                MessageBox.Show("No se encontró el turno del odontólogo.");
                return;
            }

            // Validar que la hora esté dentro del turno
            if (horaProximaCita < turno.HoraInicio || horaProximaCita >= turno.HoraFin)
            {
                MessageBox.Show("La hora seleccionada no corresponde al turno del odontólogo.");
                return;
            }

            // Validar que no exista ya un tratamiento con el mismo odontólogo, fecha y hora
            var tratamientos = dTratamiento.ListarActivos();
            bool existeTratamiento = tratamientos.Any(t =>
                t.FechaProximaCita.Date == (fechaProximaCita ?? DateTime.MinValue).Date &&
                t.HoraProximaCita == horaProximaCita &&
                t.Cita != null &&
                t.Cita.OdontologoId == odontologoId
            );

            if (existeTratamiento)
            {
                MessageBox.Show("Ya existe un tratamiento registrado con este odontólogo para la fecha y hora seleccionadas.");
                return;
            }

            // Obtener el diagnóstico desde el TextBox
            string diagnostico = TxtDiagnostico.Text?.Replace("\r", " ").Replace("\n", " ").Trim() ?? "";

            // Crear el objeto tratamiento SOLO con los IDs, NO con las entidades de navegación
            var tratamiento = new Tratamiento
            {
                InicioTratamiento = fechaInicio ?? DateTime.Now,
                FinTratamiento = fechaFin ?? DateTime.Now,
                FechaProximaCita = fechaProximaCita ?? DateTime.Now,
                HoraProximaCita = horaProximaCita,
                TipoId = tipo.IdTipoTratamiento,
                IdHistorialClinico = historialClinico.IdHistoriaClinica,
                CitaId = cita.IdCita,
                Estado = "Inicio",
                Diagnostico = diagnostico
            };

            string resultado = dTratamiento.Registrar(tratamiento, dni);
            MessageBox.Show(resultado);

            // Mostrar tratamientos filtrados después de registrar
            MostrarTratamientosFiltrados();
            LimpiarCampos();
        }

        private void BtnModificarTratamiento_Click(object sender, RoutedEventArgs e)
        {
            if (dgCasosClinicos.SelectedItem != null)
            {
                // Recupera el objeto anónimo seleccionado
                dynamic seleccionado = dgCasosClinicos.SelectedItem;

                // Busca el tratamiento real por IdTratamiento
                int idTratamiento = seleccionado.IdTratamiento;
                var tratamientoReal = dTratamiento.ListarActivos().FirstOrDefault(t => t.IdTratamiento == idTratamiento);

                if (tratamientoReal == null)
                {
                    MessageBox.Show("No se encontró el tratamiento seleccionado.");
                    return;
                }

                if (CbTipoTratamiento.SelectedItem is TipoTratamiento tipo)
                {
                    // Validar que no exista ya un tratamiento de este tipo para esta cita (excluyendo el actual)
                    var tratamientosDeLaCita = dTratamiento.ListarActivos()
                        .Where(t => t.CitaId == tratamientoReal.CitaId && t.IdTratamiento != tratamientoReal.IdTratamiento)
                        .ToList();

                    bool existeTratamientoMismoTipo = tratamientosDeLaCita
                        .Any(t => t.TipoId == tipo.IdTipoTratamiento);

                    if (existeTratamientoMismoTipo)
                    {
                        MessageBox.Show("Ya existe un tratamiento de este tipo registrado para esta cita. Seleccione otro tipo o cita.");
                        return;
                    }

                    // Validar que no exista una cita en la misma fecha y hora para el odontólogo (excluyendo la cita actual)
                    int odontologoId = tratamientoReal.Cita.OdontologoId;
                    DateTime? fechaProximaCita = DpProximaCita.SelectedDate;
                    TimeSpan horaProximaCita;
                    if (!TimeSpan.TryParse(TxtHoraProximaCita.Text, out horaProximaCita))
                    {
                        MessageBox.Show("Ingrese una hora válida para la próxima cita (formato HH:mm).");
                        return;
                    }

                    var citasOcupadas = nCita.ListarProgramadas()
                        .Where(c => c.OdontologoId == odontologoId
                                 && c.Fecha == fechaProximaCita
                                 && c.Hora == horaProximaCita
                                 && c.IdCita != tratamientoReal.CitaId) // Excluir la cita actual
                        .ToList();

                    if (citasOcupadas.Count > 0)
                    {
                        MessageBox.Show("El odontólogo ya tiene una cita programada en esa fecha y hora.");
                        return; // No permitir guardar la modificación
                    }

                    // Obtener el turno del odontólogo
                    var odontologo = dOdontologo.ObtenerPorId(odontologoId);
                    if (odontologo == null)
                    {
                        MessageBox.Show("No se encontró el odontólogo.");
                        return;
                    }
                    var turno = odontologo.Turno;
                    if (turno == null)
                    {
                        MessageBox.Show("No se encontró el turno del odontólogo.");
                        return;
                    }

                    // Validar que la hora esté dentro del turno
                    if (horaProximaCita < turno.HoraInicio || horaProximaCita >= turno.HoraFin)
                    {
                        MessageBox.Show("La hora seleccionada no corresponde al turno del odontólogo.");
                        return;
                    }

                    // Actualiza los datos del tratamiento real
                    tratamientoReal.InicioTratamiento = DpFechaInicio.SelectedDate ?? tratamientoReal.InicioTratamiento;
                    tratamientoReal.FinTratamiento = DpFechaFin.SelectedDate ?? tratamientoReal.FinTratamiento;
                    tratamientoReal.FechaProximaCita = DpProximaCita.SelectedDate ?? tratamientoReal.FechaProximaCita;
                    tratamientoReal.HoraProximaCita = horaProximaCita;
                    tratamientoReal.TipoId = tipo.IdTipoTratamiento;
                    tratamientoReal.Estado = cbEstado.Text ?? tratamientoReal.Estado;
                    tratamientoReal.Diagnostico = TxtDiagnostico.Text?.Replace("\r", " ").Replace("\n", " ").Trim() ?? tratamientoReal.Diagnostico;

                    string resultado = nTratamiento.Modificar(tratamientoReal);
                    MessageBox.Show(resultado);

                    // Refresca el DataGrid y vuelve a seleccionar el tratamiento modificado
                    MostrarTratamientosFiltrados();

                    // Selecciona el tratamiento modificado en el DataGrid y autocompleta los campos
                    foreach (var item in dgCasosClinicos.Items)
                    {
                        dynamic anon = item;
                        if (anon.IdTratamiento == idTratamiento)
                        {
                            dgCasosClinicos.SelectedItem = item;
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione un tipo de tratamiento.");
                }
            }
            else
            {
                MessageBox.Show("Seleccione un tratamiento para modificar.");
            }
            // No limpiar campos aquí, solo modificar y mantener la selección
        }

        private void BtnEliminarTratamiento_Click(object sender, RoutedEventArgs e)
        {
            if (dgCasosClinicos.SelectedItem != null)
            {
                dynamic seleccionado = dgCasosClinicos.SelectedItem;
                int idTratamiento = seleccionado.IdTratamiento;

                var confirm = MessageBox.Show("¿Está seguro que desea eliminar el tratamiento seleccionado?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm == MessageBoxResult.Yes)
                {
                    string resultado = nTratamiento.Eliminar(idTratamiento);
                    MessageBox.Show(resultado);

                    // Refresca el DataGrid después de eliminar
                    MostrarTratamientosFiltrados();

                    // Si quedan tratamientos, selecciona el primero y autocompleta los campos
                    if (dgCasosClinicos.Items.Count > 0)
                    {
                        dgCasosClinicos.SelectedIndex = 0;
                    }
                    else
                    {
                        LimpiarCampos();
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un tratamiento para eliminar.");
            }
        }

        // Al mostrar el diagnóstico en el TextBox, también reemplaza los saltos de línea por espacios.
        private void DgCasosClinicos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCasosClinicos.SelectedItem != null)
            {
                dynamic seleccionado = dgCasosClinicos.SelectedItem;

                // Autocompletar campos con los valores del tratamiento seleccionado
                DpFechaInicio.SelectedDate = seleccionado.InicioTratamiento;
                DpFechaFin.SelectedDate = seleccionado.FinTratamiento;
                DpProximaCita.SelectedDate = seleccionado.FechaProximaCita;
                TxtHoraProximaCita.Text = seleccionado.HoraProximaCita ?? "";
                TxtNombreOdontologo.Text = seleccionado.Odontologo ?? "";
                TxtNombre.Text = seleccionado.Paciente ?? "";
                TxtDni.Text = seleccionado.PacienteDNI ?? "";
                cbEstado.Text = seleccionado.Estado ?? "";
                TxtDiagnostico.Text = (seleccionado.Diagnostico ?? "")
                    .Replace("\r", " ")
                    .Replace("\n", " ")
                    .Trim();

                // Seleccionar el tipo de tratamiento en el ComboBox
                if (!string.IsNullOrEmpty(seleccionado.TipoTratamiento))
                {
                    foreach (var item in CbTipoTratamiento.Items)
                    {
                        if (item is TipoTratamiento tipo && tipo.Nombre == seleccionado.TipoTratamiento)
                        {
                            CbTipoTratamiento.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            else
            {
                CbTipoTratamiento.SelectedIndex = -1;
                DpFechaInicio.SelectedDate = null;
                DpFechaFin.SelectedDate = null;
                DpProximaCita.SelectedDate = null;
                TxtHoraProximaCita.Text = "";
                TxtNombreOdontologo.Text = "";
                TxtNombre.Text = "";
                TxtDni.Text = "";
                cbEstado.SelectedIndex = -1;
                TxtDiagnostico.Text = "";
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarCampos();
            dgCasosClinicos.ItemsSource = null;
        }
    }
}
