using Microsoft.Data.Sqlite;
using Models;

namespace Repositorios; 

public class UsuarioRepository : IUsuarioRepository
{
    private readonly string connectionString = @"Data Source=db/Tienda.db;Cache=Shared";

    public void CrearUsuario(Usuario cli)
    {
        
        string queryString = @"INSERT INTO Usuario (Nombre,Usuario,Contraseña,Rol) VALUES (@Nombre,@Usuario,@Contr,@Rol)";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                command.Parameters.AddWithValue("@Nombre", cli.Nombre );
                command.Parameters.AddWithValue("@Usuario", cli.Username );
                command.Parameters.AddWithValue("@Contr", cli.Clave );
                command.Parameters.AddWithValue("@Rol", cli.Nivel );

                command.ExecuteNonQuery();
            }
        }
    } 

    public Usuario DetallarUsuario(string username, string password) // obtiene un objeto Usuario a partir de la bd
    {
        const string sqlQuery = @"SELECT * FROM Usuario WHERE Usuario = @us and Contraseña= @pass";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = new SqliteCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@us", username);
        command.Parameters.AddWithValue("@pass", password);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Usuario
            (
                reader.GetInt32(0), // Primer columna: id
                reader.GetString(1), // Segunda columna: nombre
                reader.GetString(2), // Tercera columna: usuario
                reader.GetString(3), // Cuarta columna: clave
                reader.GetString(4) // Cuarta columna: rol
            );
        }
        return null;
    }

    public Usuario ObtenerUsuario(int id) // obtiene un objeto Usuario a partir de la bd
    {
        const string sqlQuery = @"SELECT * FROM Usuario WHERE IdUsuario = @id";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = new SqliteCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Usuario
            (
                reader.GetInt32(0), // Primer columna: id
                reader.GetString(1), // Segunda columna: nombre
                reader.GetString(2), // Tercera columna: usuario
                reader.GetString(3), // Cuarta columna: clave
                reader.GetString(4) // Cuarta columna: rol
            );
        }
        return null;
    }

    
    public List<Usuario> ListarUsuario()
    {
        var clientes = new List<Usuario>();
        const string sqlQuery = @"SELECT * FROM Usuario";

        using (var connection = new SqliteConnection(connectionString)){

            connection.Open();

            SqliteCommand command = new SqliteCommand(sqlQuery, connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var uno = new Usuario
                    (
                        reader.GetInt32(0), // Primer columna: id
                        reader.GetString(1), // Segunda columna: nombre
                        reader.GetString(2), // Tercera columna: usuario
                        reader.GetString(3), // Cuarta columna: clave
                        reader.GetString(4) // Cuarta columna: rol
                    );

                    if (uno.Nivel=="Cliente"){
                        clientes.Add(uno);
                    }
                    
                }
            }
        }
        return clientes;
    }

}   