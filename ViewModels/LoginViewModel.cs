public class LoginViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }

    public string AccessLevel { get; set; }

    public string Nombre { get; set; }
    public string ErrorMessage { get; set; }
    public bool IsAuthenticated { get; set; } 
}