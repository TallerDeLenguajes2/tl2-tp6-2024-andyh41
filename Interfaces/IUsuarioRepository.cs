using Models;

public interface IUsuarioRepository
{
    void CrearUsuario(Usuario usuario);
    Usuario DetallarUsuario(string username, string password);
    public Usuario ObtenerUsuario(int id);
    public List<Usuario> ListarUsuario();
}