using System.Collections.Generic;
using Capa_de_Datos;

namespace Capa_de_Negocio
{
    public class NTratamiento
    {
        private DTratamiento datos = new DTratamiento();

        // Registrar un tratamiento asociado a un historial clínico y cita
        public string Registrar(Tratamiento obj, string pacienteDNI)
        {
            return datos.Registrar(obj, pacienteDNI);
        }
        public List<Tratamiento> ListarActivosReporte()
        {
            return datos.ListarActivosReporte();
        }

        // Modificar tratamiento
        public string Modificar(Tratamiento obj)
        {
            return datos.Modificar(obj);
        }

        // Eliminar tratamiento (físico)
        public string Eliminar(int id)
        {
            return datos.Eliminar(id);
        }

        // Listar tratamientos activos (Inicio o En Progreso)
        public List<Tratamiento> ListarActivos()
        {
            return datos.ListarActivos();
        }

        // Listar tipos de tratamiento
        public List<TipoTratamiento> ListarTipos()
        {
            return datos.ListarTipos();
        }
    }
}