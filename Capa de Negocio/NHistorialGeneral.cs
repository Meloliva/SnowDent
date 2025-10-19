using System.Collections.Generic;
using Capa_de_Datos;

namespace Capa_de_Negocio
{
    public class NHistorialGeneral
    {
        private DHistorialGeneral datos = new DHistorialGeneral();

        // Obtener historial general por paciente
        public HistorialGeneral ObtenerPorPaciente(string pacienteDNI)
        {
            return datos.ObtenerPorPaciente(pacienteDNI);
        }

        // Crear y obtener historial general por paciente (crea si no existe)
        public HistorialGeneral CrearYObtenerPorPaciente(string pacienteDNI, string titulo)
        {
            return datos.CrearYObtenerPorPaciente(pacienteDNI, titulo);
        }

        // Listar historiales clínicos asociados a un historial general (ahora retorna HistorialGeneralVista)
        public List<HistorialGeneralVista> ListarHistorialesClinicos(int idHistorialGeneral)
        {
            return datos.ListarHistorialesClinicos(idHistorialGeneral);
        }
    }
}