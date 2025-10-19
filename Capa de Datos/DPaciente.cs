using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_de_Datos
{
    public class DPaciente
    {
        public string Registrar(Paciente obj)
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    // Validación DNI único
                    if (context.Pacientes.Any(p => p.DNI == obj.DNI))
                    {
                        return "Ya existe un paciente con ese DNI.";
                    }

                    context.Pacientes.Add(obj);
                    context.SaveChanges();
                }
                return "Registrado correctamente.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        // Verificar si DNI existe
        public bool DniExiste(string dni)
        {
            using (var context = new SnowDentEntities7())
            {
                return context.Pacientes.Any(p => p.DNI == dni);
            }
        }
        // Modificar paciente
        public string Modificar(Paciente obj)
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    Paciente temp = context.Pacientes.Find(obj.DNI);
                    if (temp == null)
                    {
                        return "Paciente no encontrado.";
                    }

                    // Campos modificables
                    temp.Nombre = obj.Nombre;
                    temp.Apellido = obj.Apellido;
                    temp.Genero = obj.Genero;
                    temp.Email = obj.Email;
                    context.SaveChanges();
                }
                return "Modificado correctamente.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        // Eliminar paciente (lógico)
        public string Eliminar(string dni)
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    Paciente temp = context.Pacientes.Find(dni);
                    if (temp == null)
                    {
                        return "Paciente no encontrado.";
                    }
                    temp.Estado = "Inactivo";
                    context.SaveChanges();
                }
                return "Eliminado correctamente.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Listar todos los pacientes
        public List<Paciente> ListarTodo()
        {
            List<Paciente> lista = new List<Paciente>();
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    lista = context.Pacientes.ToList();
                }
                return lista;
            }
            catch (Exception ex)
            {
                return lista;
            }
        }

        // Listar pacientes activos
        public List<Paciente> ListarActivos()
        {
            List<Paciente> lista = new List<Paciente>();
            try
            {
                using (var db = new SnowDentEntities7())
                {
                    lista = db.Pacientes.Where(p => p.Estado == "Activo").ToList();
                }
                return lista;
            }
            catch (Exception ex)
            {
                return lista;
            }
        }

    }
}

