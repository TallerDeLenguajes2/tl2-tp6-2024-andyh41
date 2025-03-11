using Microsoft.AspNetCore.Mvc;
using Models;
using Repositorios;

namespace Controllers;
public class ClienteController : Controller
{
    private readonly ILogger<ClienteController> _logger;
    private readonly ClienteRepository _repositorioCliente;

    public ClienteController(ILogger<ClienteController> logger)
    {
        _logger = logger;
        _repositorioCliente = new ClienteRepository();
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(_repositorioCliente.ListarClientes());
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Crear(Cliente cliente)
    {
        if (ModelState.IsValid)
        {
            _repositorioCliente.CrearCliente(cliente);
            return RedirectToAction(nameof(Index));
        }
        return View(cliente);
    }



    [HttpGet]
    public IActionResult Modificar(int id)
    {
        var cliente = _repositorioCliente.DetallarCliente(id);
        if (cliente == null)
        {
            return NotFound();
        }
        return View(cliente);
    }

    [HttpPost]
    public IActionResult Modificar(Cliente Cliente)
    {
        if (ModelState.IsValid)
        {
            _repositorioCliente.ModificarCliente(Cliente);
            return RedirectToAction(nameof(Index));
        }
        return View(Cliente);
    }

    [HttpGet]
    public IActionResult Eliminar(int id)
    {
        var cliente = _repositorioCliente.DetallarCliente(id);
        if (cliente == null)
        {
            return NotFound();
        }
        return View(cliente);
    }

    [HttpGet]
    public IActionResult ConfirmarEliminacion(int id)
    {
        _repositorioCliente.EliminarCliente(id);
        return RedirectToAction(nameof(Index));
    }

}