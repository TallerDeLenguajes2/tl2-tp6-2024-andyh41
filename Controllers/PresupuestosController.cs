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

    [HttpPost]
    public IActionResult Crear(PresupuestoViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            // Reasignar la lista de clientes en caso de que haya un error de validación
            viewModel.Usuario = _repositorioUsuario.ListarUsuario().Select(p => new SelectListItem
            {
                Value = p.IdUsuario.ToString(),
                Text = p.Username
            }).ToList();
            return View(viewModel);
        }

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
        return RedirectToAction(nameof(Index));
    }
    



   [HttpGet]
    public IActionResult Modificar(int id)
    {
        var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
        if (presupuesto == null)
        {
            return NotFound();
        }

        // Crear el ViewModel y llenarlo con los datos del presupuesto
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

    [HttpPost]
    public IActionResult Modificar(PresupuestoViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            // Recargar la lista de clientes si hay un error de validación
            viewModel.Usuario = _repositorioUsuario.ListarUsuario().Select(p => new SelectListItem
            {
                Value = p.IdUsuario.ToString(),
                Text = p.Nombre
            }).ToList();
            return View(viewModel);
        }

        // Obtener el presupuesto existente
        var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(viewModel.PresupuestoId);
        if (presupuesto == null)
        {
            ModelState.AddModelError(string.Empty, "Presupuesto no encontrado.");
            return View(viewModel);
        }

        // Verificar que el usuario seleccionado exista
        var cliente = _repositorioUsuario.ObtenerUsuario(viewModel.IdUsuario);
        if (cliente == null)
        {
            ModelState.AddModelError(string.Empty, "Cliente no encontrado.");
            return View(viewModel);
        }

        // Actualizar los datos del presupuesto
        presupuesto.NombreDestinatario = cliente.Username;
        presupuesto.Fecha = viewModel.Fecha;

        // Validar datos antes de actualizar
        if (string.IsNullOrEmpty(presupuesto.NombreDestinatario))
        {
            ModelState.AddModelError(string.Empty, "Fecha o Cliente no válidos.");
            return View(viewModel);
        }

        // Guardar los cambios en el repositorio
        _repositorioPresupuestos.ModificarPresupuesto(presupuesto);

        return RedirectToAction(nameof(Index));
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
