using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Models;

public class Presupuestos
{
    public static float IVA = 0.21f;

    private static int autoincremento = 1;
    private int idPresupuesto;
    
    private string nombreDestinatario;

    private List<PresupuestoDetalle> detalle= new List<PresupuestoDetalle>();

    public Presupuestos(string nombreDestinatario)
    {
        IdPresupuesto = ++autoincremento;
        this.NombreDestinatario = nombreDestinatario;
    }
    public Presupuestos(){
        IdPresupuesto = ++autoincremento;
        nombreDestinatario = string.Empty;
    }

    public int IdPresupuesto { get => idPresupuesto; set => idPresupuesto = value; }
    public string NombreDestinatario { get => nombreDestinatario; set => nombreDestinatario = value; }
    public List<PresupuestoDetalle> Detalle { get => detalle; set => detalle = value; }

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