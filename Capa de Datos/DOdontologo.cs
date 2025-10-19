using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Capa_de_Datos
{
    public class DOdontologo : DUsuario
    {

        private SnowDentEntities7 context;

        public DOdontologo()
        {
            context = new SnowDentEntities7();
            context.Configuration.LazyLoadingEnabled = false;
        }



        // Registrar odontólogo (incluye usuario)
        public string Registrar(Odontologo obj)
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    if (context.Usuarios.Any(u => u.Username == obj.Usuario.Username))
                        return "Ya existe un usuario con ese nombre de usuario.";

                    if (context.Odontologos.Any(o => o.DNI == obj.DNI))
                        return "Ya existe un odontólogo con ese DNI.";

                    // 1. Agregar el usuario y guardar para obtener el Id generado
                    context.Usuarios.Add(obj.Usuario);
                    context.SaveChanges();

                    // 2. Asignar el Id generado al odontólogo
                    obj.Id = obj.Usuario.Id;

                    // 3. Agregar el odontólogo
                    context.Odontologos.Add(obj);
                    context.SaveChanges();
                }
                return "Odontólogo registrado correctamente.";
            }
            catch (Exception ex)
            {
                // Devuelve el mensaje interno si existe para mejor diagnóstico
                return ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
        }

        // Modificar odontólogo y usuario asociado
        public string Modificar(Odontologo obj)
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    var temp = context.Odontologos
                        .Include(o => o.Usuario)
                        .FirstOrDefault(o => o.Id == obj.Id);

                    if (temp == null)
                        return "Odontólogo no encontrado.";

                    temp.DNI = obj.DNI;
                    temp.Nombre = obj.Nombre;
                    temp.Apellido = obj.Apellido;
                    temp.EspecialidadId = obj.EspecialidadId;
                    temp.TurnoId = obj.TurnoId;

                    // Si quieres reemplazar el usuario completo:
                    if (obj.Usuario != null)
                    {
                        // Elimina el usuario anterior si quieres reemplazarlo (no recomendado)
                        // context.Usuarios.Remove(temp.Usuario);
                        // context.Usuarios.Add(obj.Usuario);

                        // O actualiza los valores primitivos:
                        temp.Usuario.Username = obj.Usuario.Username;
                        temp.Usuario.Role = obj.Usuario.Role;
                        // No cambies PasswordHash aquí si no quieres permitirlo
                    }

                    context.SaveChanges();
                }
                return "Odontólogo modificado correctamente.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Eliminar odontólogo (lógico)
        public string Eliminar(int id)
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    Odontologo temp = context.Odontologos.Find(id);
                    if (temp == null)
                        return "Odontólogo no encontrado.";

                    temp.Estado = "Inactivo";
                    context.SaveChanges();
                }
                return "Odontólogo eliminado correctamente.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public List<OdontologoVistaDTO> ListarOdontologosVista()
        {
            using (var context = new SnowDentEntities7())
            {
                var odontologos = context.Odontologos
                    .Include(o => o.Usuario)
                    .Include(o => o.Especialidad)
                    .Include(o => o.Turno)
                    .Where(o => o.Estado == "Activo")
                    .ToList(); // Fetch data into memory

                return odontologos.Select(o => new OdontologoVistaDTO
                {
                    Username = o.Usuario.Username,
                    Estado = o.Estado,
                    NombreCompleto = o.Nombre + " " + o.Apellido,
                    DNI = o.DNI,
                    Especialidad = o.Especialidad.Nombre,
                    Turno = $"{o.Turno.HoraInicio} - {o.Turno.HoraFin}" // Compute RangoHorario
                }).ToList();
            }
        }

        // Listar todos los odontólogos
        public List<Odontologo> ListarTodo()
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    return context.Odontologos
                        .Include(o => o.Usuario)
                        .Where(o => o.Estado == "Activo")
                        .ToList();
                }
            }
            catch
            {
                return new List<Odontologo>();
            }
        
        }

        // Listar turnos
        public List<Turno> ListarTurnos()
        {
            using (var context = new SnowDentEntities7())
            {
                return context.Turnos.ToList();
            }
        }
        public Odontologo ObtenerPorId(int id)
        {
            using (var context = new SnowDentEntities7())
            {
                // Incluye las relaciones necesarias para evitar lazy loading fuera del contexto
                return context.Odontologos
                    .Include(o => o.Usuario)
                    .Include(o => o.Especialidad)
                    .Include(o => o.Turno)
                    .FirstOrDefault(o => o.Id == id);
            }
        }
        public List<Especialidad> ListarEspecialidades()
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    return context.Especialidades.ToList();
                }
            }
            catch
            {
                return new List<Especialidad>();
            }
        }
    }
}