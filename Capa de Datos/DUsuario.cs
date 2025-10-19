using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_de_Datos
{
    public class DUsuario
    {
        // Registrar un nuevo usuario
        public virtual string Registrar(Usuario obj)
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    if (context.Usuarios.Any(u => u.Username == obj.Username))
                        return "Ya existe un usuario con ese nombre de usuario.";

                    context.Usuarios.Add(obj);
                    context.SaveChanges();
                }
                return "Registrado correctamente.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Verificar si el username existe
        public bool UsernameExiste(string username)
        {
            using (var context = new SnowDentEntities7())
            {
                return context.Usuarios.Any(u => u.Username == username);
            }
        }

        // Modificar usuario
        public virtual string Modificar(Usuario obj)
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    Usuario temp = context.Usuarios.Find(obj.Id);
                    if (temp == null)
                        return "Usuario no encontrado.";

                    temp.Username = obj.Username;
                    temp.PasswordHash = obj.PasswordHash;
                    temp.Role = obj.Role;
                    context.SaveChanges();
                }
                return "Modificado correctamente.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Eliminar usuario (físico, ya que no hay campo Estado)
        public virtual string Eliminar(int id)
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    Usuario temp = context.Usuarios.Find(id);
                    if (temp == null)
                        return "Usuario no encontrado.";

                    context.Usuarios.Remove(temp);
                    context.SaveChanges();
                }
                return "Eliminado correctamente.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Listar todos los usuarios
        public List<Usuario> ListarTodo()
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    return context.Usuarios.ToList();
                }
            }
            catch
            {
                return new List<Usuario>();
            }
        }
    }
}
