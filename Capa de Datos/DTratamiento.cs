using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace Capa_de_Datos
{
    public class DTratamiento
    {
        private SnowDentEntities7 context;
        public DTratamiento()
        {
            context = new SnowDentEntities7();
            context.Configuration.LazyLoadingEnabled = false;
        }
        public string Registrar(Tratamiento obj, string pacienteDNI)
        {
            using (var context = new SnowDentEntities7())
            {
                try
                {
                    // Asegúrate de que las propiedades de navegación sean null para evitar inserciones no deseadas
                    obj.TipoTratamiento = null;
                    obj.Cita = null;
                    obj.HistorialClinico = null;

                    context.Tratamientos.Add(obj);
                    context.SaveChanges();
                    return "Tratamiento registrado correctamente.";
                }
                catch (DbEntityValidationException ex)
                {
                    var errores = ex.EntityValidationErrors
                        .SelectMany(e => e.ValidationErrors)
                        .Select(e => $"Propiedad: {e.PropertyName}, Error: {e.ErrorMessage}");
                    return string.Join("\n", errores);
                }
            }
        }

        public List<Tratamiento> ListarActivos()
        {
            
            using (var context = new SnowDentEntities7())
            {
                return context.Tratamientos
                .Include(t => t.Cita)
                .Include(t => t.Cita.Odontologo)
                .Include(t => t.Cita.Paciente)
                .Include(t => t.TipoTratamiento)
                .Include(t => t.HistorialClinico.HistorialGeneral)
                .Where(t => t.Estado == "Inicio" || t.Estado == "En Progreso")
                .ToList();
            }
        }
        public List<Tratamiento> ListarActivosReporte()
        {

            using (var context = new SnowDentEntities7())
            {
                return context.Tratamientos
                .Include(t => t.Cita)
                .Include(t => t.Cita.Odontologo)
                .Include(t => t.Cita.Paciente)
                .Include(t => t.TipoTratamiento)
                .Include(t => t.HistorialClinico.HistorialGeneral)
                .Where(t => t.Estado == "Terminado")
                .ToList();
            }
        }


        public List<TipoTratamiento> ListarTipos()
        {
            using (var context = new SnowDentEntities7())
            {
                return context.TiposTratamientos.ToList();
            }
        }

        // Modificar tratamiento
        public string Modificar(Tratamiento obj)
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    var temp = context.Tratamientos.Find(obj.IdTratamiento);
                    if (temp == null)
                        return "Tratamiento no encontrado.";

                    // Actualizar los campos necesarios
                    temp.TipoId = obj.TipoId;
                    temp.Estado = obj.Estado;
                    temp.FechaProximaCita = obj.FechaProximaCita;
                    temp.HoraProximaCita = obj.HoraProximaCita;
                    temp.InicioTratamiento = obj.InicioTratamiento;
                    temp.FinTratamiento = obj.FinTratamiento;
                    temp.CitaId = obj.CitaId;
                    temp.IdHistorialClinico = obj.IdHistorialClinico;
                    temp.Diagnostico = obj.Diagnostico;

                    context.SaveChanges();
                }
                return "Tratamiento modificado correctamente.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Eliminar tratamiento (físico)
        public string Eliminar(int id)
        {
            try
            {
                using (var context = new SnowDentEntities7())
                {
                    var temp = context.Tratamientos.Find(id);
                    if (temp == null)
                        return "Tratamiento no encontrado.";

                    context.Tratamientos.Remove(temp);
                    context.SaveChanges();
                }
                return "Tratamiento eliminado correctamente.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

