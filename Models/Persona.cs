namespace inmobiliaria.Models;
using System.ComponentModel.DataAnnotations;

public class Persona
{

    [Key]
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    public string? Dni { get; set; }

    [Required(ErrorMessage = "El Nombre es obligatorio.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El Apellido es obligatorio.")]
    public string Apellido { get; set; } = "";

    [Display(Name = "Telefono")]
    public string? Telefono { get; set; } = "";

    [Required(ErrorMessage = "El email es obligatorio."), EmailAddress(ErrorMessage = "El formato del email no es valido.")]
    public string Email { get; set; } = "";

    public override string ToString()
    {
        return $"Persona {{Nombre={Nombre}, Apellido={Apellido}, DNI={Dni}, Telefono={Telefono}, Email={Email}}}";
    }

}
