using System;
using System.Collections.Generic;
using Capa_de_Datos;

namespace Capa_de_Negocio
{
    public class NUsuario
    {
        private DUsuario datos = new DUsuario();

        // Registrar un nuevo usuario
        public string Registrar(Usuario obj)
        {
            return datos.Registrar(obj);
        }

        // Modificar usuario
        public string Modificar(Usuario obj)
        {
            return datos.Modificar(obj);
        }

        // Eliminar usuario
        public string Eliminar(int id)
        {
            return datos.Eliminar(id);
        }

        // Listar todos los usuarios
        public List<Usuario> ListarTodo()
        {
            return datos.ListarTodo();
        }

        // Verificar si el username existe
        public bool UsernameExiste(string username)
        {
            return datos.UsernameExiste(username);
        }
    }
}
