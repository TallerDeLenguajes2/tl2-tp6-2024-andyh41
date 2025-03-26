using Microsoft.AspNetCore.Mvc.Rendering;

public class PresupuestoViewModel
{
    public int PresupuestoId { get; set; }  // ID del presupuesto
    public int IdUsuario { get; set; }      // ID del cliente
    public string Fecha { get; set; }
    public List<SelectListItem> Usuario { get; set; }  // Lista de clientes para el dropdown
}