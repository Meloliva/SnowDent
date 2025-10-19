using Capa_de_Datos;

public class NLogueo
{
    private DLogueo dLogueo = new DLogueo();
    private DLogger_Auditoria logger = new DLogger_Auditoria();

    public Usuario Login(string username, string password, string rolSeleccionado)
    {
        // Validar credenciales y obtener usuario de la base de datos
        Usuario usuario = dLogueo.ValidarCredenciales(username, password);

        // Si el usuario existe y el rol coincide, retorna el usuario
        if (usuario != null &&
            usuario.Role != null &&
            usuario.Role.Trim().ToLower() == rolSeleccionado.Trim().ToLower())
        {
            return usuario;
        }

        // Si no coincide, retorna null
        return null;
    }
}