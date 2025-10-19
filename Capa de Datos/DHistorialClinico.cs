using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_de_Datos
{
    public class DHistorialClinico
    {
        public HistorialClinico ObtenerPorCitaYGeneral(int idCita, int idHistorialGeneral)
        {
            using (var context = new SnowDentEntities7())
            {
                return context.HistorialesClinicos
                    .FirstOrDefault(hc => hc.CitaId == idCita && hc.HistorialGeneralId == idHistorialGeneral);
            }
        }

        public HistorialClinico CrearYObtener(int idCita, int idHistorialGeneral, int idTipoTratamiento)
        {
            using (var context = new SnowDentEntities7())
            {
                var hc = context.HistorialesClinicos
                    .FirstOrDefault(h => h.CitaId == idCita && h.HistorialGeneralId == idHistorialGeneral);
                if (hc != null)
                    return hc;

                hc = new HistorialClinico
                {
                    CitaId = idCita,
                    HistorialGeneralId = idHistorialGeneral,
                    IdTipoTratamiento = idTipoTratamiento,
                    FechaCreacion = DateTime.Now,
                    Observaciones = "Paciente con un seguimiento de tratamiento",
                };
                context.HistorialesClinicos.Add(hc);
                context.SaveChanges();
                return hc;
            }
        }
    }
}