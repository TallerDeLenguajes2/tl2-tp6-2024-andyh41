using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Repositorios;

namespace Controllers;

public class PresupuestosController : Controller
{
    private readonly ILogger<PresupuestosController> _logger;
    private readonly IPresupuestosRepository _repositorioPresupuestos;
    private readonly IProductosRepository _repositorioProductos;
    private readonly IUsuarioRepository _repositorioUsuario;

    // Constructor con inyección de dependencias
    public PresupuestosController(ILogger<PresupuestosController> logger, 
                                   IPresupuestosRepository repositorioPresupuestos,
                                   IProductosRepository repositorioProductos,
                                   IUsuarioRepository repositorioUsuario)
    {
        _logger = logger;
        _repositorioPresupuestos = repositorioPresupuestos;
        _repositorioProductos = repositorioProductos;
        _repositorioUsuario = repositorioUsuario;
    }

 [HttpGet]
    public IActionResult Index()
    {
        try
        {
            var presupuestos = _repositorioPresupuestos.ListarPresupuestos();
            return View(presupuestos);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener los presupuestos: {ex.Message}");
            return View("Error"); // Asegúrate de tener una vista de error
        }
    }

    [HttpGet]
    public IActionResult VerPresupuesto(int id)
    {
        try
        {
            var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
            if (presupuesto == null)
            {
                return NotFound(); // 404 si el presupuesto no se encuentra
            }
            return View(presupuesto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener el presupuesto: {ex.Message}");
            return View("Error");
        }
    }


    [accessLevel("Administrador")]
    [HttpGet]
    public IActionResult Crear()
    {
        
        var viewModel = new PresupuestoViewModel
        {
            Usuario = _repositorioUsuario.ListarUsuario().Select(p => new SelectListItem
            {
                Value = p.IdUsuario.ToString(),
                Text = p.Nombre
            }).ToList()
        };
        
        return View(viewModel);
    }

    [accessLevel("Administrador")]
    [HttpPost]
    public IActionResult Crear(PresupuestoViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            // Reasignar la lista de usuarios en caso de validación incorrecta
            viewModel.Usuario = _repositorioUsuario.ListarUsuario().Select(p => new SelectListItem
            {
                Value = p.IdUsuario.ToString(),
                Text = p.Username
            }).ToList();
            return View(viewModel);
        }

        try
        {
            var cliente = _repositorioUsuario.ObtenerUsuario(viewModel.IdUsuario);
            if (cliente == null)
            {
                ModelState.AddModelError(string.Empty, "Cliente no encontrado.");
                return View(viewModel);
            }

            var presupuesto = new Presupuestos
            {
                NombreDestinatario = cliente.Username,
                Fecha = viewModel.Fecha
            };

            _repositorioPresupuestos.CrearPresupuesto(presupuesto);
            return RedirectToAction(nameof(Index)); // Redirige a la lista de presupuestos
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear el presupuesto: {ex.Message}");
            return View("Error");
        }
    }


    [accessLevel("Administrador")]
    [HttpGet]
    public IActionResult Modificar(int id)
    {
        var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
        if (presupuesto == null)
        {
            return NotFound(); // 404 si el presupuesto no se encuentra
        }

        var viewModel = new PresupuestoViewModel
        {
            PresupuestoId = presupuesto.IdPresupuesto,
            Fecha = presupuesto.Fecha,
            Usuario = _repositorioUsuario.ListarUsuario().Select(p => new SelectListItem
            {
                Value = p.IdUsuario.ToString(),
                Text = p.Nombre
            }).ToList()
        };

        return View(viewModel);
    }


    [accessLevel("Administrador")]
    [HttpPost]
    public IActionResult Modificar(PresupuestoViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.Usuario = _repositorioUsuario.ListarUsuario().Select(p => new SelectListItem
            {
                Value = p.IdUsuario.ToString(),
                Text = p.Nombre
            }).ToList();
            return View(viewModel);
        }

        try
        {
            var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(viewModel.PresupuestoId);
            if (presupuesto == null)
            {
                ModelState.AddModelError(string.Empty, "Presupuesto no encontrado.");
                return View(viewModel);
            }

            var cliente = _repositorioUsuario.ObtenerUsuario(viewModel.IdUsuario);
            if (cliente == null)
            {
                ModelState.AddModelError(string.Empty, "Cliente no encontrado.");
                return View(viewModel);
            }

            presupuesto.NombreDestinatario = cliente.Username;
            presupuesto.Fecha = viewModel.Fecha;

            if (string.IsNullOrEmpty(presupuesto.NombreDestinatario))
            {
                ModelState.AddModelError(string.Empty, "Fecha o Cliente no válidos.");
                return View(viewModel);
            }

            _repositorioPresupuestos.ModificarPresupuesto(presupuesto);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al modificar el presupuesto: {ex.Message}");
            return View("Error");
        }
    }


    [accessLevel("Administrador")]
    [HttpGet]
    public IActionResult Eliminar(int id)
    {
        var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
        if (presupuesto == null)
        {
            return NotFound(); // 404 si el presupuesto no se encuentra
        }
        return View(presupuesto);
    }


    [accessLevel("Administrador")]
    [HttpGet]
    public IActionResult ConfirmarEliminacion(int id)
    {
        try
        {
            _repositorioPresupuestos.EliminarPresupuesto(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar el presupuesto: {ex.Message}");
            return View("Error");
        }
    }


    [accessLevel("Administrador")]
    [HttpGet]
    public IActionResult AgregarProductoDetalle(int id)
    {
        try
        {
            ViewData["Productos"] = _repositorioProductos.ListarProductos()
                                                        .Select(p => new SelectListItem()
                                                        {
                                                            Value = p.IdProducto.ToString(),
                                                            Text = p.Descripcion
                                                        });

            return View(id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al cargar productos para agregar al presupuesto: {ex.Message}");
            return View("Error"); // Asegúrate de tener una vista de error
        }
    }


    [accessLevel("Administrador")]
    [HttpPost]
    public IActionResult AgregarProductoDetalle(int idPresupuesto, int cantidad, int producto)
    {
        try
        {
            // Validar los parámetros
            if (cantidad <= 0 || producto <= 0)
            {
                ModelState.AddModelError(string.Empty, "Cantidad o Producto inválidos.");
                return RedirectToAction(nameof(AgregarProductoDetalle), new { id = idPresupuesto });
            }

            // Llamada al repositorio para agregar el detalle al presupuesto
            _repositorioPresupuestos.AgregarDetalle(idPresupuesto, producto, cantidad);
            return RedirectToAction(nameof(Index)); // Redirige a la lista de presupuestos
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al agregar producto al presupuesto: {ex.Message}");
            return View("Error");
        }
    }


    [accessLevel("Administrador")]
    [HttpGet]
    public IActionResult EliminarDetalle(int id)
    {
        try
        {
            var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
            if (presupuesto == null)
            {
                return NotFound(); // 404 si el presupuesto no se encuentra
            }

            ViewData["Productos"] = presupuesto.Detalle.Select(p => new SelectListItem
            {
                Value = p.Producto.IdProducto.ToString(),
                Text = p.Producto.Descripcion
            });

            return View(id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al cargar los detalles para eliminar: {ex.Message}");
            return View("Error");
        }
    }


    [accessLevel("Administrador")]
    [HttpPost]
    public IActionResult EliminarElDetalle(int idPresupuesto, int idProducto)
    {
        try
        {
            if (idProducto <= 0)
            {
                ModelState.AddModelError(string.Empty, "Producto no válido.");
                return RedirectToAction(nameof(EliminarDetalle), new { id = idPresupuesto });
            }

            // Llamada al repositorio para eliminar el detalle del presupuesto
            _repositorioPresupuestos.EliminarDetalle(idPresupuesto, idProducto);
            return RedirectToAction(nameof(Index)); // Redirige a la lista de presupuestos
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar el detalle del presupuesto: {ex.Message}");
            return View("Error");
        }
    }
}