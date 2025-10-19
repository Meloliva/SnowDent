using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace Capa_de_Datos
{
    public class DReporteRecepcionista
    {


        // R2: Total de citas por tipo de tratamiento
        public List<CitasPorTratamientoDTO> TotalCitasPorTipoTratamiento()
        {
            using (var context = new SnowDentEntities7())
            {
                var query = from t in context.Tratamientos
                            join c in context.Citas on t.CitaId equals c.IdCita
                            join tipo in context.TiposTratamientos on t.TipoId equals tipo.IdTipoTratamiento
                            group c by tipo.Nombre into g
                            select new CitasPorTratamientoDTO
                            {
                                NombreTratamiento = g.Key,
                                TotalCitas = g.Count()
                            };
                return query.ToList();
            }
        }

        // R3: Listar Pacientes por un rango de fecha del registro
        public List<PacienteRangoDTO> PacientesPorRangoFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            using (var context = new SnowDentEntities7())
            {
                return context.Pacientes
                    .Where(p => p.FechaRegistro >= fechaInicio && p.FechaRegistro <= fechaFin && p.Estado == "Activo")
                    .Select(p => new PacienteRangoDTO
                    {
                        Nombre = p.Nombre,
                        Apellido = p.Apellido,
                        DNI = p.DNI,
                        FechaRegistro = p.FechaRegistro
                    })
                    .ToList();
            }
        }

        // R4: Mostrar Pacientes con tratamientos en curso
        public List<PacienteTratamientoDTO> PacientesConTratamientosEnCurso()
        {
            using (var context = new SnowDentEntities7())
            {
                var query = from t in context.Tratamientos
                            where t.Estado == "En Progreso"
                            join c in context.Citas on t.CitaId equals c.IdCita
                            join p in context.Pacientes on c.PacienteDNI equals p.DNI
                            where p.Estado == "Activo"
                            select new PacienteTratamientoDTO
                            {
                                Nombre = p.Nombre,
                                Apellido = p.Apellido,
                                DNI = p.DNI,
                                EstadoTratamiento = t.Estado
                            };
                return query.Distinct().ToList();
            }
        }
    }

    // DTOs para los reportes
    public class CitasPorTratamientoDTO
    {
        public string NombreTratamiento { get; set; }
        public int TotalCitas { get; set; }
    }

    public class PacienteRangoDTO
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class PacienteTratamientoDTO
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string EstadoTratamiento { get; set; }
    }
}
