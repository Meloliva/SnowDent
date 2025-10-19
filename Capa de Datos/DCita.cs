using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Capa_de_Datos
{
    public class DCita
    {
        private SnowDentEntities7 context;

        // Agrega una referencia a DTratamiento
        private DTratamiento dTratamiento = new DTratamiento();

        public DCita()
        {
            context = new SnowDentEntities7();
            context.Configuration.LazyLoadingEnabled = false;
        }

        // Registrar una nueva cita
        public string Registrar(Cita cita)
        {
            using (var context = new SnowDentEntities7())
            {
                try
                {
                    // Obtener el odontólogo asociado a la cita
                    var odontologo = context.Odontologos.FirstOrDefault(o => o.Id == cita.OdontologoId);
                    if (odontologo == null)
                        return "Odontólogo no encontrado.";

                    // Validar que la hora de la cita esté dentro del turno del odontólogo
                    if (cita.Hora < odontologo.Turno.HoraInicio || cita.Hora > odontologo.Turno.HoraFin)
                    {
                        return "La hora seleccionada está fuera del turno del odontólogo.";
                    }

                    // Validar que no exista otra cita con el mismo odontólogo, fecha y hora
                    bool existeCita = context.Citas.Any(c =>
                        c.OdontologoId == cita.OdontologoId &&
                        c.Fecha == cita.Fecha && // Comparación directa, SQL tipo 'date'
                        c.Hora == cita.Hora);

                    if (existeCita)
                    {
                        return "Ya existe una cita registrada con este odontólogo para la fecha y hora seleccionadas.";
                    }

                    context.Citas.Add(cita);
                    context.SaveChanges();
                    return "Cita registrada correctamente.";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        // Modificar cita
        public string Modificar(Cita obj)
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    Cita temp = context.Citas.Find(obj.IdCita);
                    if (temp == null)
                        return "Cita no encontrada.";

                    var odontologo = context.Odontologos.Find(obj.OdontologoId);
                    if (odontologo == null || odontologo.Estado != "Activo")
                        return "Odontólogo no disponible.";

                    // Obtener el turno asignado al odontólogo
                    var turno = context.Turnos.Find(odontologo.TurnoId);
                    if (turno == null)
                        return "Turno no asignado o no disponible.";

                    // Validar que la hora de la cita esté dentro del rango del turno
                    if (obj.Hora < turno.HoraInicio || obj.Hora >= turno.HoraFin)
                        return "La hora seleccionada no corresponde al turno del odontólogo.";

                    // Validar que no exista otra cita con el mismo odontólogo, fecha y hora (excepto la actual)
                    bool ocupado = context.Citas.Any(c =>
                        c.OdontologoId == obj.OdontologoId &&
                        c.Fecha == obj.Fecha && // Comparación directa, SQL tipo 'date'
                        c.Hora == obj.Hora &&
                        (c.Estado == "Programada" || c.Estado == "Activa") &&
                        c.IdCita != obj.IdCita
                    );
                    if (ocupado)
                        return "El odontólogo ya tiene una cita programada a esa hora.";

                    temp.Fecha = obj.Fecha;
                    temp.Hora = obj.Hora;
                    temp.Estado = obj.Estado;
                    temp.PacienteDNI = obj.PacienteDNI;
                    temp.OdontologoId = obj.OdontologoId;

                    context.SaveChanges();
                }
                return "Cita modificada correctamente.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Eliminar cita (lógico)
        public string Eliminar(int id)
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    Cita temp = context.Citas.Find(id);
                    if (temp == null)
                        return "Cita no encontrada.";

                    temp.Estado = "Cancelada"; // O el valor que uses para inactivar
                    context.SaveChanges();
                }
                return "Cita eliminada correctamente.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Listar todas las citas
        public List<CitaVistaDTO> ListarTodo()
        {
            using (var db = new SnowDentEntities7())
            {
                var query = from cita in db.Citas
                            join paciente in db.Pacientes on cita.PacienteDNI equals paciente.DNI
                            join odontologo in db.Odontologos on cita.OdontologoId equals odontologo.Id
                            select new CitaVistaDTO
                            {
                                IdCita = cita.IdCita,
                                PacienteDNI = paciente.DNI,
                                OdontologoDNI = odontologo.DNI,
                                NombrePaciente = paciente.Nombre,
                                ApellidoPaciente = paciente.Apellido,
                                NombreOdontologo = odontologo.Nombre,
                                ApellidoOdontologo = odontologo.Apellido,
                                Hora = cita.Hora,
                                Fecha = cita.Fecha,
                                Estado = cita.Estado,
                                Especialidad = odontologo.Especialidad.Nombre,
                                EstadoPaciente = paciente.Estado // <-- Relacionado aquí
                            };
                return query.ToList();
            }
        }

        // Listar citas activas
        public List<Cita> ListarProgramadas()
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    return context.Citas
                        .Include(c => c.Paciente)
                        .Include(c => c.Odontologo)
                        .Include(c => c.Tratamiento)
                        .Where(c => c.Estado == "Programada")
                        .ToList();
                }
            }
            catch
            {
                return new  List<Cita>();
            }
        }

        // Buscar paciente por DNI
        public Paciente BuscarPacientePorDNI(string dni)
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    return context.Pacientes.FirstOrDefault(p => p.DNI == dni && p.Estado == "Activo");
                }
            }
            catch
            {
                return null;
            }
        }

        // Método para exponer los tratamientos desde la capa de datos
        public List<Tratamiento> ListarTratamientos()
        {
            return dTratamiento.ListarActivos();
        }
    }
}

