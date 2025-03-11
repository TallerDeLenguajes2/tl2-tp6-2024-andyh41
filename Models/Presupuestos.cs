
using Microsoft.AspNetCore.Mvc;

namespace Models;

public class Presupuestos
{
    public static float IVA = 0.21f;
    
    private int idPresupuesto;
    
    private Cliente cliente;

    private List<PresupuestoDetalle> detalle= new List<PresupuestoDetalle>();

    string fecha;

    public Presupuestos(int id, Cliente client, string fech)
    {

        idPresupuesto = id;
        cliente=client;
        fecha = fech;
    }
    
    public Presupuestos(){
        idPresupuesto=0;
    }

    public int IdPresupuesto { get => idPresupuesto; set => idPresupuesto = value; }
    public List<PresupuestoDetalle> Detalle { get => detalle; set => detalle = value; }
    public string Fecha { get => fecha; set => fecha = value; }
    public Cliente Cliente { get => cliente; set => cliente = value; }

    public int MontoPresupuesto() {
        return this.Detalle.Sum(d=> d.Producto.Precio * d.Cantidad);
    }

    public double MontoPresupuestoConIva() {
        return  this.MontoPresupuesto()* (1 + IVA);
    }

    public int CantidadProductos() {
        return this.Detalle.Sum(d=> d.Cantidad);
    }
    
}