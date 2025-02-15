using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Repositorios;

namespace Controllers;

public class PresupuestosController : Controller
{
    private readonly ILogger<PresupuestosController> _logger;
    private readonly PresupuestosRepository _repositorioPresupuestos;
    private readonly ProductosRepository _repositorioProductos;



    public PresupuestosController(ILogger<PresupuestosController> logger)
    {
        _logger = logger;
        _repositorioPresupuestos = new PresupuestosRepository();
        _repositorioProductos = new  ProductosRepository();
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(_repositorioPresupuestos.ListarPresupuestos());
    }


    [HttpGet]
    public IActionResult VerPresupuesto(int id)
    {
        return View(_repositorioPresupuestos.ObtenerPresupuesto(id));
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }


    [HttpPost]
    public IActionResult Crear(Presupuestos presupuestos)
    {
        if (ModelState.IsValid)
        {
            _repositorioPresupuestos.CrearPresupuesto(presupuestos);
            return RedirectToAction(nameof(Index));
        }
        return View(presupuestos);
    }


    [HttpGet]
    public IActionResult Modificar(int id)
    {
        var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
        if (presupuesto == null)
        {
            return NotFound();
        }
        return View(presupuesto);
    }

    [HttpPost] 
    public IActionResult Modificar(Presupuestos presupuesto)
    {
        if (ModelState.IsValid)
        {
            _repositorioPresupuestos.ModificarPresupuesto(presupuesto);
            return RedirectToAction(nameof(Index));
        }
        return View(presupuesto);
    }



    [HttpGet]
    public IActionResult Eliminar(int id)
    {
        var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
        if (presupuesto == null)
        {
            return NotFound();
        }
        return View(presupuesto);
    }

    [HttpGet]
    public IActionResult ConfirmarEliminacion(int id)
    {
        _repositorioPresupuestos.EliminarPresupuesto(id);
        return RedirectToAction(nameof(Index));
    }



    [HttpGet]
    public IActionResult AgregarProductoDetalle(int id) {
        ViewData["Productos"] = _repositorioProductos.ListarProductos()
                                                    .Select(p => new SelectListItem()
                                                                {
                                                                    Value = p.IdProducto.ToString(),
                                                                    Text = p.Descripcion
                                                                });
        return View(id);
    }

    [HttpPost]
    public IActionResult AgregarProductoDetalle(int idPresupuesto, int cantidad, int producto) {
        _repositorioPresupuestos.AgregarDetalle(idPresupuesto, producto, cantidad);
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public IActionResult EliminarDetalle(int id)
    {
        Presupuestos presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
        ViewData["Productos"] = presupuesto.Detalle.Select(p => new SelectListItem
        {
            Value = p.Producto.IdProducto.ToString(), 
            Text = p.Producto.Descripcion 
        });

        return View(id);
    }


    [HttpPost]
    public IActionResult EliminarElDetalle(int idPresupuesto, int idProducto)
    {
        _repositorioPresupuestos.EliminarDetalle(idPresupuesto, idProducto);
        return RedirectToAction (nameof(Index));
    }

}    
