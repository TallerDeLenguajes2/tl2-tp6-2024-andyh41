using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Repositorios;

namespace Controllers;

public class ProductosController : Controller
{
    private readonly IProductosRepository _repositorioProductos;
    private readonly ILogger<ProductosController> _logger;

    public ProductosController(IProductosRepository repositorioProductos, ILogger<ProductosController> logger)
    {
        _repositorioProductos = repositorioProductos;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        try
        {
            return View(_repositorioProductos.ListarProductos());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar productos");
            return View("Error");
        }
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Crear(Productos producto)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _repositorioProductos.CrearProducto(producto);
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto");
            return View("Error");
        }
    }

    [HttpGet]
    public IActionResult Modificar(int id)
    {
        try
        {
            var producto = _repositorioProductos.DetallarProducto(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el producto para modificar");
            return View("Error");
        }
    }

    [HttpPost]
    public IActionResult Modificar(Productos producto)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _repositorioProductos.ModificarProducto(producto);
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al modificar producto");
            return View("Error");
        }
    }

    [HttpGet]
    public IActionResult Eliminar(int id)
    {
        try
        {
            var producto = _repositorioProductos.DetallarProducto(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto para eliminar");
            return View("Error");
        }
    }

    [HttpGet]
    public IActionResult ConfirmarEliminacion(int id)
    {
        try
        {
            _repositorioProductos.EliminarProducto(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto");
            return View("Error");
        }
    }
}
