using Microsoft.Data.Sqlite;
using Models;

namespace Repositorios
{
    public class ProductosRepository : IProductosRepository
    {
        private readonly string connectionString;

        public ProductosRepository(string ConnectionString)
        {
            connectionString=ConnectionString;
        }
        public void CrearProducto(Productos producto)
        {
            if (producto==null)
            {throw new Exception("Producto inexistente");}

            const string sqlQuery = @"INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio)";

            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = new SqliteCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
            command.Parameters.AddWithValue("@Precio", producto.Precio);
            command.ExecuteNonQuery();
        }

        public void ModificarProducto(Productos producto)
        {
            if (producto==null)
            {throw new Exception("Producto inexistente");}
            const string sqlQuery = @"UPDATE Productos SET Descripcion = @Descripcion, Precio = @Precio WHERE idProducto = @Id";

            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = new SqliteCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@Id", producto.IdProducto);
            command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
            command.Parameters.AddWithValue("@Precio", producto.Precio);
            command.ExecuteNonQuery();
        }

        public List<Productos> ListarProductos()
        {
            var productos = new List<Productos>();
            const string sqlQuery = @"SELECT idProducto, Descripcion, Precio FROM Productos";

            using (var connection = new SqliteConnection(connectionString)){

                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlQuery, connection);
                using (var reader = command.ExecuteReader())
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
            if (productos==null)
            {throw new Exception("Lista de Productos inexistente");}
            return productos;
        }

    public Productos DetallarProducto(int id)
    {
        const string sqlQuery = @"SELECT idProducto, Descripcion, Precio FROM Productos WHERE idProducto = @Id";
        Productos prod = null;

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = new SqliteCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            prod = new Productos
            (
                reader.GetInt32(0), // Primer columna: Id
                reader.GetString(1), // Segunda columna: Descripcion
                reader.GetInt32(2) // Tercera columna: Precio
            );
        }

        
        if (prod == null)
        {
            throw new Exception("Producto inexistente");
        }

        return prod;
    }


        public void EliminarProducto(int id)
        {
            const string deleteProductQuery = @"DELETE FROM Productos WHERE idProducto = @Id";
            const string deleteDetailsQuery = @"DELETE FROM PresupuestosDetalle WHERE idProducto = @Id";

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using (var command = new SqliteCommand(deleteDetailsQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }

            using (var command = new SqliteCommand(deleteProductQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }
}
