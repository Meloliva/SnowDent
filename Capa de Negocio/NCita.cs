using System;
using System.Collections.Generic;
using Capa_de_Datos;

namespace Capa_de_Negocio
{
    public class NCita
    {
        private DCita datos = new DCita();

        public string Registrar(Cita obj)
        {
            return datos.Registrar(obj);
        }

        public string Modificar(Cita obj)
        {
            return datos.Modificar(obj);
        }

        public string Eliminar(int id)
        {
            return datos.Eliminar(id);
        }

        public List<CitaVistaDTO> ListarTodo()
        {
            return datos.ListarTodo();
        }

        public List<Cita> ListarProgramadas()
        {
            return datos.ListarProgramadas();
        }

        public Paciente BuscarPacientePorDNI(string dni)
        {
            return datos.BuscarPacientePorDNI(dni);
        }

        // Método para exponer los tratamientos desde la capa de negocio
        public List<Tratamiento> ListarTratamientos()
        {
            return datos.ListarTratamientos();
        }
    }
}