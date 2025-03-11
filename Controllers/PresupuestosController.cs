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
    private readonly ClienteRepository _repositorioCliente; 



    public PresupuestosController(ILogger<PresupuestosController> logger)
    {
        _logger = logger;
        _repositorioPresupuestos = new PresupuestosRepository();
        _repositorioProductos = new  ProductosRepository();
        _repositorioCliente = new ClienteRepository();
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

    public IActionResult Crear()
    {
        var viewModel = new PresupuestoViewModel
        {
            Clientes = _repositorioCliente.ListarClientes().Select(p => new SelectListItem
            {
                Value = p.ClienteId.ToString(),
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
            viewModel.Clientes = _repositorioCliente.ListarClientes().Select(p => new SelectListItem
            {
                Value = p.ClienteId.ToString(),
                Text = p.Nombre
            }).ToList();
            return View(viewModel);
        }

        var cliente = _repositorioCliente.DetallarCliente(viewModel.ClienteId);
        if (cliente == null)
        {
            ModelState.AddModelError(string.Empty, "Cliente no encontrado.");
            return View(viewModel);
        }

        var presupuesto = new Presupuestos
        {
            Cliente = cliente,
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
        //Crear el ViewModel y llenarlo con los datos del presupuesto
        var viewModel = new PresupuestoViewModel
        {
            PresupuestoId=presupuesto.IdPresupuesto,
            Fecha = presupuesto.Fecha,
            Clientes = _repositorioCliente.ListarClientes().Select(p => new SelectListItem
            {
                Value = p.ClienteId.ToString(),
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
            // Recargar los datos necesarios
            viewModel.Clientes = _repositorioCliente.ListarClientes().Select(p => new SelectListItem
            {
                Value = p.ClienteId.ToString(),
                Text = p.Nombre
            }).ToList();
            return View(viewModel);
        }

        // Obtener el presupuesto por su ID
        var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(viewModel.PresupuestoId);

        if (presupuesto == null)
        {
            ModelState.AddModelError(string.Empty, "Presupuesto no encontrado.");
            return View(viewModel);
        }

        // Asegurarse de que ClienteId no sea null
        var cliente = _repositorioCliente.DetallarCliente(viewModel.ClienteId);
        if (cliente == null)
        {
            ModelState.AddModelError(string.Empty, "Cliente no encontrado.");
            return View(viewModel);
        }

        // Modificar el presupuesto
        presupuesto.Cliente = cliente;
        presupuesto.Fecha = viewModel.Fecha;

        // Asegúrate de que la fecha y el cliente estén correctamente asignados y no sean nulos.
        if (string.IsNullOrEmpty(presupuesto.Fecha) || presupuesto.Cliente == null)
        {
            ModelState.AddModelError(string.Empty, "Fecha o Cliente no válidos.");
            return View(viewModel);
        }

        // Llamar al método del repositorio para actualizar el presupuesto
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
