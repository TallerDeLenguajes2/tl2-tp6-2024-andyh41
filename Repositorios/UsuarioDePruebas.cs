using Microsoft.Data.Sqlite;
using Models;

namespace Repositorios;

public class UsuarioDePruebas : IUsuarioRepository
{
    public void CrearUsuario(Usuario usuario)
    {
        
    }

    public Usuario DetallarUsuario(string username, string password)
    {
        return new Usuario();
    }

    public List<Usuario> ListarUsuario()
    {
        return new List<Usuario>();
    }

    public Usuario ObtenerUsuario(int id)
    {
        return new Usuario();
    }
}