using System;
using System.Collections.Generic;
using Capa_de_Datos;

namespace Capa_de_Negocio
{
    public class NReporteOdontologo_grafico
    {
        private DReporteOdontologo_grafico datos = new DReporteOdontologo_grafico();

        // Gráfica: Duración promedio de tratamiento por tipo
        public List<ReporteGraficaDTO> DuracionPromedioPorTipoTratamiento()
        {
            return datos.DuracionPromedioPorTipoTratamiento();
        }

        // Gráfica: Cantidad de pacientes por género
        public List<ReporteGraficaDTO> PacientesPorGenero()
        {
            return datos.PacientesPorGenero();
        }
    }
}
