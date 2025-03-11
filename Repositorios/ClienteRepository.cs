using Microsoft.Data.Sqlite;
using Models;

namespace Repositorios; 

public class ClienteRepository
{
    private readonly string connectionString = @"Data Source=db/Tienda.db;Cache=Shared";

    public void CrearCliente(Cliente cli)
    {
        
        string queryString = @"INSERT INTO Clientes (ClienteId,Nombre,Email,Telefono) VALUES (@Id,@Nombre,@Mail,@Telefono)";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {

                command.Parameters.AddWithValue("@Id", cli.ClienteId );
                command.Parameters.AddWithValue("@Nombre", cli.Nombre );
                command.Parameters.AddWithValue("@Mail", cli.Email );
                command.Parameters.AddWithValue("@Telefono", cli.Telefono );

                command.ExecuteNonQuery();
            }
        }
    } 

    public void ModificarCliente(Cliente cli)
    {
        const string sqlQuery = @"UPDATE Clientes SET Nombre = @name, Email = @mail, Telefono=@tel WHERE ClienteId = @Id";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = new SqliteCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@name", cli.Nombre);
        command.Parameters.AddWithValue("@mail", cli.Email);
        command.Parameters.AddWithValue("@tel", cli.Telefono);
        command.Parameters.AddWithValue("@Id", cli.ClienteId);
        command.ExecuteNonQuery();
    }

    public Cliente DetallarCliente(int id) // obtiene un objeto cliente a partir de la bd
    {
        const string sqlQuery = @"SELECT * FROM Clientes WHERE ClienteId = @Id";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = new SqliteCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@Id", id);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Cliente
            (
                reader.GetInt32(0), // Primer columna: id
                reader.GetString(1), // Segunda columna: nombre
                reader.GetString(2), // Tercera columna: mail
                reader.GetString(3) // Cuarta columna: telefono
            );
        }
        return null;
    }


    public void EliminarCliente(int id)
    {
        const string deletePresQuery = @"DELETE FROM Presupuestos WHERE ClienteId = @Id";
        const string deleteClieQuery = @"DELETE FROM Clientes WHERE ClienteId = @Id";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using (var command = new SqliteCommand(deletePresQuery, connection))
        {
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }

        using (var command = new SqliteCommand(deleteClieQuery, connection))
        {
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }

        connection.Close();
    }

    public List<Cliente> ListarClientes()
    {
        var clientes = new List<Cliente>();
        const string sqlQuery = @"SELECT * FROM Clientes";

        using (var connection = new SqliteConnection(connectionString)){

            connection.Open();

            SqliteCommand command = new SqliteCommand(sqlQuery, connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var uno = new Cliente
                    (
                        reader.GetInt32(0), // Primer columna: id
                        reader.GetString(1), // Segunda columna: nombre
                        reader.GetString(2), // Tercera columna: mail
                        reader.GetString(3) // Cuarta columna: telefono
                    );
                    clientes.Add(uno);
                }
            }
        }
        return clientes;
    }
}