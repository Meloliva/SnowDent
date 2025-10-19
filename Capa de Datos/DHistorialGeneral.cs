using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Capa_de_Datos;

namespace Capa_de_Datos
{
    public class DHistorialGeneral
    {
        private SnowDentEntities7 context;

        public DHistorialGeneral()
        {
            context = new SnowDentEntities7();
            context.Configuration.LazyLoadingEnabled = false;
        }
        public HistorialGeneral ObtenerPorPaciente(string pacienteDNI)
        {
            using (var context = new SnowDentEntities7())
            {
                return context.HistorialesGenerales
                    .Include(h => h.Paciente)
                    .FirstOrDefault(h => h.PacienteDNI == pacienteDNI);
            }
        }

        public HistorialGeneral CrearYObtenerPorPaciente(string pacienteDNI, string titulo)
        {
            using (var context = new SnowDentEntities7())
            {
                var historial = context.HistorialesGenerales.FirstOrDefault(hg => hg.PacienteDNI == pacienteDNI);
                if (historial != null)
                    return historial;

                var paciente = context.Pacientes.Find(pacienteDNI);
                if (paciente == null)
                    return null;

                historial = new HistorialGeneral { PacienteDNI = pacienteDNI, Titulo= titulo };
                context.HistorialesGenerales.Add(historial);
                context.SaveChanges();
                return historial;
            }
        }

        public List<HistorialGeneralVista> ListarHistorialesClinicos(int idHistorialGeneral)
        {
            using (var context = new SnowDentEntities7())
            {
                var query = context.HistorialesClinicos
                    .Include(hc => hc.Cita.Odontologo.Especialidad)
                    .Include(hc => hc.Cita.Paciente)
                    .Include(hc => hc.TipoTratamiento)
                    .Where(hc => hc.HistorialGeneralId == idHistorialGeneral)
                    .Select(hc => new HistorialGeneralVista
                    {
                        IdHistorialGeneral = hc.HistorialGeneralId,
                        IdCasoClinico = hc.IdHistoriaClinica,
                        IdCita = hc.CitaId,
                        TipoTratamiento = hc.TipoTratamiento != null ? hc.TipoTratamiento.Nombre : null,
                        EstadoTratamiento = hc.Tratamiento.FirstOrDefault() != null ? hc.Tratamiento.FirstOrDefault().Estado : null,
                        DNIPaciente = hc.Cita.PacienteDNI,
                        NombrePaciente = hc.Cita.Paciente != null ? hc.Cita.Paciente.Nombre + " " + hc.Cita.Paciente.Apellido : null,
                        DNIOdontologo = hc.Cita.Odontologo != null ? hc.Cita.Odontologo.DNI : null,
                        Especialidad = hc.Cita.Odontologo != null && hc.Cita.Odontologo.Especialidad != null ? hc.Cita.Odontologo.Especialidad.Nombre : null
                    });

                return query.ToList();
            }
        }
    }
}

