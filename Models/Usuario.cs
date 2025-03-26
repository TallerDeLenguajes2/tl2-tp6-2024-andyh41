
using SQLitePCL;

namespace Models;
public class Usuario
{
    private int idUsuario;
    private string nombre;
    private string username;
    private string clave;
    private string nivel;

    public Usuario(int idUsuario, string nombre, string usuario, string clave, string nivel)
    {
        this.idUsuario = idUsuario;
        this.nombre = nombre;
        this.username = usuario;
        this.clave = clave;
        this.nivel = nivel;
    }

    public Usuario(){
        idUsuario = 0;
        nombre = string.Empty;
        username = string.Empty;
        clave = string.Empty;
        nivel = "Ninguno";
    }

    public int IdUsuario { get => idUsuario; set => idUsuario = value; }
    public string Nombre { get => nombre; set => nombre = value; }
    public string Username { get => username; set => username = value; }
    public string Clave { get => clave; set => clave = value; }
    public string Nivel { get => nivel; set => nivel = value; }
   
}