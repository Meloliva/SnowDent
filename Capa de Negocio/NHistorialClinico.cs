using Capa_de_Datos;

namespace Capa_de_Negocio
{
    public class NHistorialClinico
    {
        private DHistorialClinico datos = new DHistorialClinico();

        // Obtener historial clínico por cita e historial general
        public HistorialClinico ObtenerPorCitaYGeneral(int idCita, int idHistorialGeneral)
        {
            return datos.ObtenerPorCitaYGeneral(idCita, idHistorialGeneral);
        }

        // Crear y obtener historial clínico (crea si no existe)
        public HistorialClinico CrearYObtener(int idCita, int idHistorialGeneral, int idTipoTratamiento)
        {
            return datos.CrearYObtener(idCita, idHistorialGeneral, idTipoTratamiento);
        }
    }
}