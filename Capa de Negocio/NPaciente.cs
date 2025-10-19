using System;
using System.Collections.Generic;
using Capa_de_Datos;

namespace Capa_de_Negocio
{
    public class NPaciente
    {
        private DPaciente datos = new DPaciente();

        // Registrar un nuevo paciente
        public string Registrar(Paciente obj)
        {
            return datos.Registrar(obj);
        }

        // Modificar paciente
        public string Modificar(Paciente obj)
        {
            return datos.Modificar(obj);
        }

        // Eliminar paciente (lógico)
        public string Eliminar(string dni)
        {
            return datos.Eliminar(dni);
        }

        // Listar todos los pacientes
        public List<Paciente> ListarTodo()
        {
            return datos.ListarTodo();
        }

        // Listar pacientes activos
        public List<Paciente> ListarActivos()
        {
            return datos.ListarActivos();
        }

        // Buscar paciente por DNI
        public bool DniExiste(string dni)
        {
            return datos.DniExiste(dni);
        }
    }
}
