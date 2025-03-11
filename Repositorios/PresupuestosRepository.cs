using Microsoft.Data.Sqlite;
using Models;

namespace Repositorios; 

public class PresupuestosRepository
{
    private readonly string connectionString = @"Data Source=db/Tienda.db;Cache=Shared";

    public void CrearPresupuesto(Presupuestos pres)
    {
        if (pres == null || pres.Cliente == null || pres.Cliente.ClienteId == 0 || string.IsNullOrEmpty(pres.Fecha))
        {
            throw new ArgumentException("El presupuesto o algunos de sus parámetros no son válidos.");
        }

        string queryString = @"INSERT INTO Presupuestos (ClienteId, FechaCreacion) VALUES (@ClienteId, @Fecha)";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                // Usar ClienteId y Fecha
                command.Parameters.AddWithValue("@ClienteId", pres.Cliente.ClienteId);
                command.Parameters.AddWithValue("@Fecha", pres.Fecha);

                command.ExecuteNonQuery();
            }
        }
    }


  

    public List<Presupuestos> ListarPresupuestos()
    {
        var presupuestos = new List<Presupuestos>();

        // Abre la conexión 
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            // Ajustada la consulta para hacer LEFT JOIN con ON y seleccionar columnas específicas
            string queryString = @"
                SELECT 
                    p.idPresupuesto, p.FechaCreacion, 
                    c.ClienteId, c.Nombre, c.Email, c.Telefono
                FROM 
                    Presupuestos p
                LEFT JOIN 
                    Clientes c ON p.ClienteId = c.ClienteId";

            using (var command = new SqliteCommand(queryString, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Crear cliente
                        var cliente = new Cliente(
                            reader.GetInt32(2),  // ClienteId (3ra columna en el SELECT)
                            reader.GetString(3), // Nombre (4ta columna en el SELECT)
                            reader.GetString(4), // Mail (5ta columna en el SELECT)
                            reader.GetString(5)  // Telefono (6ta columna en el SELECT)
                        );

                        // Crear un nuevo presupuesto
                        var presupuesto = new Presupuestos
                        (
                            reader.GetInt32(0), // idPresupuesto (1ra columna en el SELECT)
                            cliente,
                            reader.GetString(1) // FechaCreacion (2da columna en el SELECT)
                        );

                        // Agregar a la lista
                        presupuestos.Add(presupuesto);
                    }
                }
            }
        }

        return presupuestos;
    }





    public Presupuestos ObtenerPresupuesto(int id)
    {
        string queryString = @"SELECT 
                                p.idPresupuesto,
                                p.FechaCreacion,
                                p.ClienteId,
                                c.Nombre, 
                                c.Email, 
                                c.Telefono,
                                pd.idProducto, 
                                pr.Descripcion AS ProductoNombre,
                                pr.Precio,
                                pd.Cantidad
                            FROM Presupuestos p 
                            LEFT JOIN Clientes c ON p.ClienteId = c.ClienteId
                            LEFT JOIN PresupuestosDetalle pd ON p.idPresupuesto = pd.idPresupuesto
                            LEFT JOIN Productos pr ON pd.idProducto = pr.idProducto
                            WHERE p.idPresupuesto=@id";

        var presupuesto = new Presupuestos();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // Crear un cliente
                            var cliente = new Cliente(
                                reader.GetInt32(2), // id cliente
                                reader.GetString(3), // nombre
                                reader.GetString(4), // mail
                                reader.GetString(5)  // telefono
                            );

                            if (presupuesto.IdPresupuesto == 0)
                            {
                                presupuesto.IdPresupuesto = reader.GetInt32(0); // id presupuesto
                                presupuesto.Cliente = cliente;
                                presupuesto.Fecha = reader.GetString(1); // fecha
                            }

                            if (!reader.IsDBNull(6)) // Verifica si idProducto no es nulo
                            {
                                var producto = new Productos(
                                    reader.GetInt32(6), // idProducto
                                    reader.GetString(7), // ProductoNombre
                                    reader.GetInt32(8)  // Precio
                                );
                                var detalle = new PresupuestoDetalle(producto, reader.GetInt32(9)); // Crear el detalle

                                presupuesto.Detalle.Add(detalle);
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        return presupuesto;
    }





    public void AgregarDetalle(int id, int producto, int cantidad){
        string queryString = @"INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@Idpres, @Idprod, @Cantidad)";
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                command.Parameters.AddWithValue("@Idpres", id);
                command.Parameters.AddWithValue("@Idprod", producto);
                command.Parameters.AddWithValue("@Cantidad", cantidad);
                    
                command.ExecuteNonQuery();
            }
        }
    }

    public void EliminarDetalle(int id, int producto){
        string queryString = @"DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @Idpres AND idProducto = @Idprod";
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                command.Parameters.AddWithValue("@Idpres", id);
                command.Parameters.AddWithValue("@Idprod", producto);
                    
                command.ExecuteNonQuery();
            }
        }
    }


public void EliminarPresupuesto(int id)
{
    string queryDetalle = @"DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @Id";
    string queryPresupuesto = @"DELETE FROM Presupuestos WHERE idPresupuesto = @Id";

    using (SqliteConnection connection = new SqliteConnection(connectionString))
    {
        connection.Open();

        using (SqliteCommand commandDetalle = new SqliteCommand(queryDetalle, connection))
        {
            commandDetalle.Parameters.AddWithValue("@Id", id);
            commandDetalle.ExecuteNonQuery(); // 🔹 Primero eliminamos los detalles
        }

        using (SqliteCommand commandPresupuesto = new SqliteCommand(queryPresupuesto, connection))
        {
            commandPresupuesto.Parameters.AddWithValue("@Id", id);
            commandPresupuesto.ExecuteNonQuery(); // 🔹 Luego eliminamos el presupuesto
        }
    }
}


    public void ModificarPresupuesto(Presupuestos presupuesto)
    {
        if (presupuesto == null)
        {
            throw new ArgumentNullException(nameof(presupuesto), "El presupuesto no puede ser nulo.");
        }

        if (presupuesto.Cliente == null)
        {
            throw new ArgumentNullException(nameof(presupuesto.Cliente), "El cliente no puede ser nulo.");
        }

        if (string.IsNullOrEmpty(presupuesto.Fecha))
        {
            throw new ArgumentException("La fecha del presupuesto no puede estar vacía.");
        }

        string query = @"UPDATE Presupuestos SET ClienteId = @destinatario, FechaCreacion = @date WHERE idPresupuesto = @Id";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand(query,connection);
            command.Parameters.AddWithValue("@destinatario", presupuesto.Cliente.ClienteId);
            command.Parameters.AddWithValue("@date", presupuesto.Fecha);
            command.Parameters.AddWithValue("@Id", presupuesto.IdPresupuesto);
            command.ExecuteNonQuery();
            connection.Close();            
        }
    }

}