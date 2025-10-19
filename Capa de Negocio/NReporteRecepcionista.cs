using System;
using System.Collections.Generic;
using Capa_de_Datos;

namespace Capa_de_Negocio
{
    public class NReporteRecepcionista
    {
        private DReporteRecepcionista datos = new DReporteRecepcionista();

        // R2: Total de citas por tipo de tratamiento
        public List<CitasPorTratamientoDTO> TotalCitasPorTipoTratamiento()
        {
            return datos.TotalCitasPorTipoTratamiento();
        }

        // R3: Listar Pacientes por un rango de fecha del registro
        public List<PacienteRangoDTO> PacientesPorRangoFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            return datos.PacientesPorRangoFecha(fechaInicio, fechaFin);
        }

        // R4: Mostrar Pacientes con tratamientos en curso
        public List<PacienteTratamientoDTO> PacientesConTratamientosEnCurso()
        {
            return datos.PacientesConTratamientosEnCurso();
        }
    }
}
