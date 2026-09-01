namespace inmobiliaria.Models;
using System.ComponentModel.DataAnnotations;

public class Persona
{

    [Key]
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(20, ErrorMessage = "El DNI no puede superar los 20 caracteres")]
    public string? Dni { get; set; }

    [Required(ErrorMessage = "El Nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El Apellido es obligatorio.")]
    [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres")]
    public string Apellido { get; set; } = "";

    [Display(Name = "Telefono")]
    [StringLength(30, ErrorMessage = "El telefono no puede superar los 30 caracteres")]
    public string? Telefono { get; set; } = "";

    [Required(ErrorMessage = "El email es obligatorio."), 
    EmailAddress(ErrorMessage = "El formato del email no es valido."),
    StringLength(100, ErrorMessage = "El email no puede ser mayor a 100 caracteres")]
    public string Email { get; set; } = "";

    public override string ToString()
    {
        return $"Persona {{Nombre={Nombre}, Apellido={Apellido}, DNI={Dni}, Telefono={Telefono}, Email={Email}}}";
    }

}
