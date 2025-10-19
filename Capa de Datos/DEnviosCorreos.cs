using System;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Capa_de_Datos
{
    public class DEnvioCorreos
    {
        // Envía un correo de recordatorio de cita al paciente si faltan 3 días exactos
        public string EnviarRecordatorioCita(string pacienteDNI, int idCita)
        {
            try
            {
                using (var context = new SnowDentEntities2())
                {
                    // Validar paciente y obtener correo
                    var paciente = context.Pacientes.FirstOrDefault(p => p.DNI == pacienteDNI);
                    if (paciente == null)
                        return "Paciente no registrado.";

                    if (string.IsNullOrWhiteSpace(paciente.Email))
                        return "El paciente no tiene un correo registrado.";

                    // Validar cita
                    var cita = context.Citas.FirstOrDefault(c => c.IdCita == idCita && c.PacienteDNI == pacienteDNI);
                    if (cita == null)
                        return "Cita no encontrada para el paciente.";

                    if (cita.Fecha == null || cita.Hora < 0 || cita.Hora > 23)
                        return "La fecha u hora de la cita no es válida.";

                    // Validar que falten exactamente 3 días para la cita
                    var diasFaltantes = (cita.Fecha - DateTime.Now.Date).TotalDays;
                    if (diasFaltantes != 3)
                        return "El recordatorio solo se envía exactamente 3 días antes de la cita.";

                    // Configuración SMTP para Gmail
                    string remitente = "clinicaodontologicasnowdent@gmail.com";
                    string contrasenaApp = "hrzr sfqs qfnd husp"; // Contraseña de aplicación

                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential(remitente, contrasenaApp),
                        EnableSsl = true
                    };

                    string asunto = "Recordatorio de cita odontológica";
                    string cuerpo = $"Hola {paciente.Nombre},\n\n" +
                                    $"Te recordamos que tienes una cita programada para el día {cita.Fecha:dd/MM/yyyy} a las {cita.Hora}:00 horas.\n\n" +
                                    "Por favor, no olvides asistir.\n\n" +
                                    "Saludos,\nClínica Dental SnowDent";

                    MailMessage mensaje = new MailMessage(remitente, paciente.Email, asunto, cuerpo);

                    smtp.Send(mensaje);
                    return "Correo de recordatorio enviado correctamente.";
                }
            }
            catch (Exception ex)
            {
                return $"Error al enviar correo: {ex.Message}";
            }
        }
    }
}

