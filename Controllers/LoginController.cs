using Microsoft.AspNetCore.Mvc;
using Models;

namespace Controllers;
public class LoginController : Controller
{
    private readonly IUsuarioRepository _userRepository;
    private readonly ILogger<LoginController> _logger;

    public LoginController(IUsuarioRepository userRepository, ILogger<LoginController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        try
        {
            var model = new LoginViewModel
            {
                IsAuthenticated = HttpContext.Session.GetString("IsAuthenticated") == "true"
            };
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la página de inicio de sesión.");
            ViewBag.ErrorMessage = "Ocurrió un error inesperado. Por favor, intente nuevamente más tarde.";
            return View("Error");
        }
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        try
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                model.ErrorMessage = "Por favor ingrese su nombre de usuario y contraseña.";
                return View("Index", model);
            }

            var user = _userRepository.DetallarUsuario(model.Username, model.Password);
            if (user == null)
            {
                _logger.LogWarning("Intento de acceso inválido - Usuario: {Username}", model.Username);
                model.ErrorMessage = "Credenciales inválidas";
                model.IsAuthenticated = false;
                return View("Index", model);
            }

            // Establecer variables de sesión y cookies
            HttpContext.Session.SetString("IsAuthenticated", "true");
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Nivel);

            Response.Cookies.Append("AuthCookie", user.Username, new CookieOptions { HttpOnly = true, Secure = true });

            _logger.LogInformation("El usuario {Username} ingresó correctamente", model.Username);
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en el proceso de autenticación para el usuario {Username}", model.Username);
            ViewBag.ErrorMessage = "Ocurrió un error inesperado. Por favor, intente nuevamente más tarde.";
            return View("Index", model);
        }
    }

    [HttpGet]
    public IActionResult Logout()
    {
        try
        {
            Response.Cookies.Delete("AuthCookie");
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el proceso de cierre de sesión.");
            ViewBag.ErrorMessage = "Ocurrió un error inesperado. Por favor, intente nuevamente más tarde.";
            return View("Error");
        }
    }

     public IActionResult CrearUsuario()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "No se pudo ejecutar la accion requerida";
            return View("Index");
        }
    }

    [HttpPost]

    public IActionResult AltaUsuario(LoginViewModel usuarioVM)
    {
        try
        {            
            if(!ModelState.IsValid) return RedirectToAction ("CrearUsuario");
            var usuario = new Usuario(usuarioVM);
            _userRepository.CrearUsuario(usuario);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "No se puso autenticar el usuario";
            return View("Index");
        }
    }
}
