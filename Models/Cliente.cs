using System.ComponentModel.DataAnnotations;

namespace Models;

public class Cliente
{
    private int clienteId;
    private string nombre;
    private string email;
    private string telefono;

    public int ClienteId { get => clienteId; set => clienteId = value; }


    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(150, ErrorMessage = "El nombre debe tener menos de 150 caracteres")]
    public String Nombre { get => nombre; set => nombre = value; }

    [EmailAddress(ErrorMessage = "Debe tener formato de correo electronico")]
    [Required(ErrorMessage = "El email es obligatorio")]
    public String Email { get => email; set => email = value; }

    [Phone(ErrorMessage = "Debe tener formato de número de telefono")]
    [Required(ErrorMessage = "El telefono es obligatorio")]
    public String Telefono { get => telefono; set => telefono = value; }

    public Cliente(int id, string name, string mail, string tel){
        clienteId=id;
        nombre=name;
        email=mail;
        telefono=tel;
    }  
    public Cliente() { }
}