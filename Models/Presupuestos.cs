
namespace Models;

public class Presupuestos
{
    public static float IVA = 0.21f;

    private static int autoincremento = 1;
    private int idPresupuesto;
    
    private string nombreDestinatario;

    private List<PresupuestoDetalle> detalle= new List<PresupuestoDetalle>();

    string fecha;

    public Presupuestos(string nombreDestinatario, string fech)
    {
        IdPresupuesto = ++autoincremento;
        this.NombreDestinatario = nombreDestinatario;
        fecha = fech;
    }
    public Presupuestos(){
        IdPresupuesto = ++autoincremento;
        nombreDestinatario = string.Empty;
        fecha = DateTime.Now.ToString("yyyy-MM-dd");
    }

    public int IdPresupuesto { get => idPresupuesto; set => idPresupuesto = value; }
    public string NombreDestinatario { get => nombreDestinatario; set => nombreDestinatario = value; }
    public List<PresupuestoDetalle> Detalle { get => detalle; set => detalle = value; }
    public string Fecha { get => fecha; set => fecha = value; }

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