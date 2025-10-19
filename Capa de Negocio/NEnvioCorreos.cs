using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_de_Datos;

namespace Capa_de_Negocio
{
    public class NEnvioCorreos
    {
        private DEnvioCorreos datos = new DEnvioCorreos();

        // Envía un recordatorio de cita al paciente si faltan exactamente 3 días
        public string EnviarRecordatorioCita(string pacienteDNI, int idCita)
        {
            return datos.EnviarRecordatorioCita(pacienteDNI, idCita);
        }
        public string EnviarConfirmacionCita(string pacienteDNI, DateTime fecha, TimeSpan hora)
        {
            return datos.EnviarConfirmacionCita(pacienteDNI, fecha, hora);
        }
    }
}
