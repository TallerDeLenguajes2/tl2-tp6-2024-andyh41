using Models;
public interface IProductosRepository
{
    void CrearProducto(Productos producto);
    void ModificarProducto(Productos producto);
    List<Productos> ListarProductos();
    Productos DetallarProducto(int id);
    void EliminarProducto(int id);
}
