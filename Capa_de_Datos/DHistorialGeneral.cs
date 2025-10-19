using System.Collections.Generic;
using System.Linq;

public class DHistorialGeneral
{
    public HistorialGeneral ObtenerPorPaciente(string pacienteDNI)
    {
        using (var context = new SnowDentEntities6())
        {
            return context.HistorialesGenerales.FirstOrDefault(hg => hg.PacienteDNI == pacienteDNI);
        }
    }

    public HistorialGeneral CrearYObtenerPorPaciente(string pacienteDNI)
    {
        using (var context = new SnowDentEntities6())
        {
            var historial = context.HistorialesGenerales.FirstOrDefault(hg => hg.PacienteDNI == pacienteDNI);
            if (historial != null)
                return historial;

            var paciente = context.Pacientes.Find(pacienteDNI);
            if (paciente == null)
                return null;

            historial = new HistorialGeneral { PacienteDNI = pacienteDNI };
            context.HistorialesGenerales.Add(historial);
            context.SaveChanges();
            return historial;
        }
    }

    public List<HistorialClinico> ListarHistorialesClinicos(int idHistorialGeneral)
    {
        using (var context = new SnowDentEntities6())
        {
            return context.HistorialesClinicos
                .Where(hc => hc.HistorialGeneralId == idHistorialGeneral)
                .ToList();
        }
    }
}