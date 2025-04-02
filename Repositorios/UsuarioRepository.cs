using Microsoft.Data.Sqlite;
using Models;

namespace Repositorios; 

public class UsuarioRepository : IUsuarioRepository
{
    private readonly string connectionString;

    public UsuarioRepository(string ConnectionString)
    {
        connectionString=ConnectionString;
    }
    public void CrearUsuario(Usuario cli)
    {
        if (cli==null)
        {throw new Exception("Usuario inexistente");}
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
        Usuario resultado = null; // Declarar la variable fuera del bloque if

        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = new SqliteCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@us", username);
        command.Parameters.AddWithValue("@pass", password);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            // Inicializa el objeto resultado solo cuando se ha encontrado un usuario
            resultado = new Usuario
            (
                reader.GetInt32(0), // Primer columna: id
                reader.GetString(1), // Segunda columna: nombre
                reader.GetString(2), // Tercera columna: usuario
                reader.GetString(3), // Cuarta columna: clave
                reader.GetString(4)  // Quinta columna: rol
            );
        }

        // Si no se encontró un usuario, lanza una excepción
        if (resultado == null)
        {
            throw new Exception("Usuario inexistente");
        }

        return resultado;
    }


    public Usuario ObtenerUsuario(int id) // obtiene un objeto Usuario a partir de la bd
    {
        const string sqlQuery = @"SELECT * FROM Usuario WHERE IdUsuario = @id";
        Usuario resultado = null;

        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = new SqliteCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            // Si se encuentra el usuario, se crea el objeto Usuario
            resultado = new Usuario
            (
                reader.GetInt32(0), // Primer columna: id
                reader.GetString(1), // Segunda columna: nombre
                reader.GetString(2), // Tercera columna: usuario
                reader.GetString(3), // Cuarta columna: clave
                reader.GetString(4)  // Quinta columna: rol
            );
        }

        // Si no se encuentra el usuario, lanza una excepción
        if (resultado == null)
        {
            throw new Exception("Usuario no encontrado");
        }

        return resultado;
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
        if (clientes == null)
        {
            throw new Exception("Listado no encontrado");
        }

        return clientes;
    }

}   