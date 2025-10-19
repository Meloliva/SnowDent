using System;
using System.Collections.Generic;
using Capa_de_Datos;

namespace Capa_de_Negocio
{
    public class NOdontologo
    {
        // Hacer el campo de solo lectura para cumplir con IDE0044  
        private readonly DOdontologo datos = new DOdontologo();

        // Registrar un nuevo odontólogo  
        public string Registrar(Odontologo obj)
        {
            return datos.Registrar(obj);
        }

        // Modificar odontólogo  
        public string Modificar(Odontologo obj)
        {
            return datos.Modificar(obj);
        }

        // Eliminar odontólogo (lógico)  
        public string Eliminar(int id)
        {
            return datos.Eliminar(id);
        }

        // Listar todos los odontólogos  
        public List<Odontologo> ListarTodo()
        {
            return datos.ListarTodo();
        }

        public List<Turno> ListarTurnos()
        {
            return datos.ListarTurnos();
        }
        public List<OdontologoVistaDTO> ListarOdontologosVista()
        {
            return datos.ListarOdontologosVista();
        }
    }
}
