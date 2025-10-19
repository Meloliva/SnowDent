using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_de_Datos;

namespace Capa_de_Negocio
{
    public class NLogueo
    {
        private DLogueo dLogueo = new DLogueo();
        private DLogger_Auditoria logger = new DLogger_Auditoria();

        public Usuario Login(string username, string password, string rolSeleccionado)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(rolSeleccionado))
            {
                logger.LogLoginFallido(username);
                return null;
            }

            var usuario = dLogueo.ValidarCredenciales(username, password);

            if (usuario != null && usuario.Role == rolSeleccionado)
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
