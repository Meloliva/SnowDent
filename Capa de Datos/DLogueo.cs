using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_de_Datos
{
    public class DLogueo
    {
        private DLogger_Auditoria logger = new DLogger_Auditoria();
        public Usuario ValidarCredenciales(string username, string password)
        {
            using (var context = new SnowDentEntities7())
            {
                // Busca el usuario que coincida con el username y password
                var usuario = context.Usuarios
                    .FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

                if (usuario != null)
                {
                    logger.LogLoginExitoso(username);
                    return usuario;
                }
                else
                {
                    logger.LogLoginFallido(username);
                    return null;
                }
            }
        }
    }
}
