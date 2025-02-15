using Microsoft.AspNetCore.Mvc;
using Models;
using Repositorios;

namespace Controllers;
public class ProductosController : Controller
{
    private readonly ILogger<ProductosController> _logger;
    private readonly ProductosRepository _repositorioProductos;

    public ProductosController(ILogger<ProductosController> logger)
    {
        _logger = logger;
        _repositorioProductos = new ProductosRepository();
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(_repositorioProductos.ListarProductos());
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Crear(Productos producto)
    {
        if (ModelState.IsValid)
        {
            _repositorioProductos.CrearProducto(producto);
            return RedirectToAction(nameof(Index));
        }
        return View(producto);
    }

    [HttpGet]
    public IActionResult Modificar(int id)
    {
        var producto = _repositorioProductos.DetallarProducto(id);
        if (producto == null)
        {
            return NotFound();
        }
        return View(producto);
    }

    [HttpPost]
    public IActionResult Modificar(Productos producto)
    {
        if (ModelState.IsValid)
        {
            _repositorioProductos.ModificarProducto(producto);
            return RedirectToAction(nameof(Index));
        }
        return View(producto);
    }

    [HttpGet]
    public IActionResult Eliminar(int id)
    {
        var producto = _repositorioProductos.DetallarProducto(id);
        if (producto == null)
        {
            return NotFound();
        }
        return View(producto);
    }

    [HttpGet]
    public IActionResult ConfirmarEliminacion(int id)
    {
        _repositorioProductos.EliminarProducto(id);
        return RedirectToAction(nameof(Index));
    }


}
