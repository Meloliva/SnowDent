using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_de_Datos
{
    public partial class Odontologo
    {
        public string NombreCompleto => Nombre + " " + Apellido;
    }

    public partial class Turno
    {
        public string RangoHorario
        {
            get
            {
                try
                {
                    DateTime inicio = DateTime.Today.Add(HoraInicio);
                    DateTime fin = DateTime.Today.Add(HoraFin);
                    return $"{inicio:hh\\:mm tt} - {fin:hh\\:mm tt}";
                }
                catch (Exception ex)
                {
                    return $"Error en horario: {ex.Message}";
                }
            }
        }
    }

    public class OdontologoVistaDTO
    {
        public string Username { get; set; }
        public string Estado { get; set; }
        public string NombreCompleto { get; set; }
        public string DNI { get; set; }
        public string Especialidad { get; set; }
        public string Turno { get; set; }
    }
    public class CitaVistaDTO
    {
        public int IdCita { get; set; }
        public string PacienteDNI { get; set; }
        public string OdontologoDNI { get; set; }
        public string NombrePaciente { get; set; }
        public string ApellidoPaciente { get; set; }
        public string NombreOdontologo { get; set; }
        public string ApellidoOdontologo { get; set; }
        public TimeSpan Hora { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public string Especialidad { get; set; }
        // Nuevo: EstadoPaciente para filtrar por pacientes activos
        public string EstadoPaciente { get; set; }
    }

    public class HistorialGeneralVista
    {
        public int IdHistorialGeneral { get; set; }
        public int IdCasoClinico { get; set; }
        public int IdCita { get; set; }
        public string TipoTratamiento { get; set; }
        public string EstadoTratamiento { get; set; }
        public string DNIPaciente { get; set; }
        public string NombrePaciente { get; set; }
        public string DNIOdontologo { get; set; }
        public string Especialidad { get; set; }
    }
}
