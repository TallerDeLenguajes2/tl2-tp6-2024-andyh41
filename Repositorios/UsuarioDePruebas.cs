using Microsoft.Data.Sqlite;
using Models;

namespace Repositorios;

public class UsuarioDePruebas : IUsuarioRepository
{
    public void CrearUsuario(Usuario usuario)
    {
        throw new NotImplementedException();
    }

    public Usuario DetallarUsuario(string username, string password)
    {
        return new Usuario();
    }

    public List<Usuario> ListarUsuario()
    {
        throw new NotImplementedException();
    }

    public Usuario ObtenerUsuario(int id)
    {
        throw new NotImplementedException();
    }
}