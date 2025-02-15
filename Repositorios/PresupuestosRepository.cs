using Microsoft.Data.Sqlite;
using Models;

namespace Repositorios; 

public class PresupuestosRepository
{
    private readonly string connectionString = @"Data Source=db/Tienda.db;Cache=Shared";

    public void CrearPresupuesto(Presupuestos pres)
    {
        // primero modifica la tabla presupuestos, agregando nombre y fecha
        string queryString = @"INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES (@Nombre, @Fecha)";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {

                command.Parameters.AddWithValue("@Nombre", pres.NombreDestinatario);
                command.Parameters.AddWithValue("@Fecha",  pres.Fecha);
                
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

            string queryString = @"SELECT * FROM Presupuestos ";

            using (var command = new SqliteCommand(queryString, connection))
            {
                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                            // Crear un nuevo presupuesto
                            var presupuesto = new Presupuestos
                            {
                                IdPresupuesto = reader.GetInt32(0),
                                NombreDestinatario = reader.GetString(1),
                                Fecha = reader.GetString(2)
                            };
                        presupuestos.Add(presupuesto);
                    }
                    
                }
            }
        }

        return presupuestos;
    }




    public Presupuestos ObtenerPresupuesto(int id)
    {
        string queryString = @"SELECT p.idPresupuesto, p.NombreDestinatario, p.FechaCreacion, pr.idProducto, pr.Descripcion, pr.Precio, pd.Cantidad
                             FROM Presupuestos p 
                             LEFT JOIN PresupuestosDetalle pd USING (idPresupuesto)
                             LEFT JOIN Productos pr USING (idProducto)
                             WHERE idPresupuesto=@id";
        Presupuestos presupuesto = new Presupuestos();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                // Corregir el nombre del parámetro para que sea consistente con la consulta
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                   
                    // Lee los registros
                    while (reader.Read())
                    {
                        presupuesto.IdPresupuesto = reader.GetInt32(0);  // Primer columna: IdPresupuesto
                        presupuesto.NombreDestinatario = reader.GetString(1); // Segunda columna: NombreDestinatario
                        presupuesto.Fecha = reader.GetString(2);
                            
                        // Verifica si el idProducto es nulo (es decir, si hay detalles de presupuesto)
                        if (!reader.IsDBNull(3)) // Si idProducto no es null
                        {
                            var producto = new Productos(reader.GetInt32(3), reader.GetString(4), reader.GetInt32(5));
                            var detalle = new PresupuestoDetalle(producto, reader.GetInt32(6)); // Crear el detalle

                            // Agregar el detalle al presupuesto actual
                            presupuesto.Detalle.Add(detalle);
                        }
                    }
                }
            }
        }
        // Devuelve el presupuesto si existe, de lo contrario, devuelve null
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

        string query = @"UPDATE Presupuestos SET NombreDestinatario = @destinatario, FechaCreacion = @date WHERE idPresupuesto = @Id";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand(query,connection);
            command.Parameters.AddWithValue("@destinatario", presupuesto.NombreDestinatario);
            command.Parameters.AddWithValue("@date", presupuesto.Fecha);
            command.Parameters.AddWithValue("@Id", presupuesto.IdPresupuesto);
            command.ExecuteNonQuery();
            connection.Close();            
        }
    }

}