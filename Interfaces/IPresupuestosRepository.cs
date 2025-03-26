using Models;
public interface IPresupuestosRepository
{
    void CrearPresupuesto(Presupuestos pres);
    List<Presupuestos> ListarPresupuestos();
    Presupuestos ObtenerPresupuesto(int id);
    void AgregarDetalle(int id, int producto, int cantidad);
    void EliminarDetalle(int id, int producto);
    void EliminarPresupuesto(int id);
    void ModificarPresupuesto(Presupuestos presupuesto);
}