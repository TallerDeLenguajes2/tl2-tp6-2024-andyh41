using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

// Define dónde se puede usar un atributo.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

// Filtro de nivel de acceso para controladores 
public class accessLevelAttribute : Attribute, IAuthorizationFilter
{
    private string[] arrayAccess;

    // Constructor con argumento del tipo params string[] que permite declarar un arreglo concatenado por ','
    public accessLevelAttribute(params string[] nivelesDeAcceso){
        arrayAccess=nivelesDeAcceso;
    }

    
    //AuthorizationFilterContext proporciona información sobre la solicitud actual en un filtro de autorización.
    //Se usa para verificar permisos, bloquear acceso, o redirigir usuarios.
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var nivelUsuario = context.HttpContext.Session.GetString("Role");

        // Verificamos si el usuario está autenticado
        if (context.HttpContext.Session.GetString("IsAuthenticated") == "true")
        {
            // Si no está autenticado, redirigir a la página de login
            context.Result = new RedirectToActionResult("Index", "Login",null);
            return;
        }

        // Si el usuario está autenticado, verificamos si tiene el nivel de acceso adecuado
        if (arrayAccess == null || !arrayAccess.Contains(nivelUsuario))
        {
            // Redirigir
            context.Result = new RedirectToActionResult("Index", "Presupuestos", null);
            return;
        }

        // Si está autenticado y tiene el acceso adecuado, la acción continuará.
    }
}
