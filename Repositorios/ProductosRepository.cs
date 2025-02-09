using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;
using Models;
using System.Text.Json.Serialization;
using System.Collections.Generic;


namespace Repositorios; 
public class ProductosRepository()
{
    private readonly string connectionString = "Data Source=Db/Tienda.db;Cache=Shared";
    
    public void CrearProducto(Productos producto)
    {
        string queryString = "INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio);";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                command.Parameters.AddWithValue("@Precio", producto.Precio);
                
                command.ExecuteNonQuery();
            }
        }
    }

    public void ModificarProducto(Productos producto){ 

        string queryString = "UPDATE Productos SET Descripcion = @Descripcion, Precio = @Precio WHERE idProducto = @Id;";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                command.Parameters.AddWithValue("@Id", producto.IdProducto);
                command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                command.Parameters.AddWithValue("@Precio", producto.Precio);
                
                command.ExecuteNonQuery();
            }
        }
    }

    public List<Productos> ListarProductos()
    {

        var productos = new List<Productos>();
        string queryString = "SELECT idProducto, Descripcion, Precio FROM Productos;";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                using(var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var producto = new Productos
                        (
                            reader.GetInt32(0), // Primer columna: Id
                            reader.GetString(1), // Segunda columna: Descripcion
                            reader.GetInt32(2) // Tercera columna: Precio
                        );
                        productos.Add(producto);
                    }
                }
            }
        }
        return productos;
    }

    public Productos DetallarProducto(int id)
    {
        string queryString = "SELECT idProducto, Descripcion, Precio FROM Productos WHERE idProducto = @Id;";

         using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using(var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var producto = new Productos
                        (
                            reader.GetInt32(0), // Primer columna: Id
                            reader.GetString(1), // Segunda columna: Descripcion
                            reader.GetInt32(2) // Tercera columna: Precio
                        );
                        return producto;
                    }
                }
            }
        }
        return null;
    }

    public void EliminarProducto(int id)
    {
        string query = "DELETE FROM Productos WHERE idProducto = @Id;";
        string query2 = @"DELETE FROM PresupuestosDetalle WHERE idProducto = @id;";


        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand(query2,connection);
            SqliteCommand command2 = new SqliteCommand(query,connection);
            command.Parameters.AddWithValue("@Id", id);
            command2.Parameters.AddWithValue("@id", id);
            command2.ExecuteNonQuery();
            command.ExecuteNonQuery();
            connection.Close();            

        }
    }
}
