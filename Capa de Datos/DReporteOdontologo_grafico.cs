using System;
using System.Collections.Generic;
using System.Linq;

namespace Capa_de_Datos
{
    public class DReporteOdontologo_grafico
    {
        // Duración promedio de tratamiento por tipo de tratamiento (para gráfica de barras)
        public List<ReporteGraficaDTO> DuracionPromedioPorTipoTratamiento()
        {
            using (var context = new SnowDentEntities7())
            {
                var query = from t in context.Tratamientos
                            where t.FinTratamiento != null
                                  && t.InicioTratamiento != null
                                  && t.Estado == "Terminado"
                                  && t.Cita != null
                                  && t.Cita.Paciente != null
                                  && t.Cita.Paciente.Estado == "Activo"
                            group t by t.TipoTratamiento.Nombre into g
                            select new ReporteGraficaDTO
                            {
                                Nombre = g.Key,
                                Valor = g.Average(x =>
                                    System.Data.Entity.SqlServer.SqlFunctions.DateDiff("day", x.InicioTratamiento, x.FinTratamiento) ?? 0)
                            };
                return query.ToList();
            }
        }

        // Cantidad de pacientes por género (para gráfica circular)
        public List<ReporteGraficaDTO> PacientesPorGenero()
        {
            using (var context = new SnowDentEntities7())
            {
                return context.Pacientes
                    .Where(p=>p.Estado=="Activo")
                    .GroupBy(p => p.Genero)
                    .Select(g => new ReporteGraficaDTO
                    {
                        Nombre = g.Key,
                        Valor = g.Count()
                    })
                    .ToList();
            }
        }
    }

    // DTO genérico para gráficos
    public class ReporteGraficaDTO
    {
        public string Nombre { get; set; }
        public double Valor { get; set; }
    }
}
