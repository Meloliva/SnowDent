using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_de_Datos
{
    public class DLogger_Auditoria
    {
        public string filePath = "..\\..\\..\\LogAuditoria.txt";
        public void Log(string message)
        {
            if (File.Exists(filePath) == false)
            {
                StreamWriter sw = File.CreateText(filePath);
                sw.WriteLine(message);
                sw.Close();
            }
            else
            {
                StreamWriter sw = File.AppendText(filePath);
                sw.WriteLine(message);
                sw.Close();
            }
        }
        public void LogLoginExitoso(string username)
        {
            Log($"LOGIN EXITOSO | Usuario: {username}");
        }

        // Método específico para registrar logins fallidos
        public void LogLoginFallido(string username)
        {
            Log($"LOGIN FALLIDO | Usuario: {username}");
        }
        public void LogCreacion(string entidad, string identificador, string usuario)
        {
            Log($"CREACIÓN | Entidad: {entidad} | ID: {identificador} | Usuario: {usuario}");
        }

        public void LogModificacion(string entidad, string identificador, string usuario)
        {
            Log($"MODIFICACIÓN | Entidad: {entidad} | ID: {identificador} | Usuario: {usuario}");
        }

        public void LogEliminacion(string entidad, string identificador, string usuario)
        {
            Log($"ELIMINACIÓN | Entidad: {entidad} | ID: {identificador} | Usuario: {usuario}");
        }

    }
}
