using Microsoft.AspNetCore.Mvc.Rendering;

public class PresupuestoViewModel
{
    public int PresupuestoId { get; set; }  // ID del presupuesto
    public int ClienteId { get; set; }      // ID del cliente
    public string Fecha { get; set; }
    public List<SelectListItem> Clientes { get; set; }  // Lista de clientes para el dropdown
}
