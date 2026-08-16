namespace inmobiliaria.Models;
using System.ComponentModel.DataAnnotations;

public class Persona
{

    [Key]
    [Required]
    public int Dni { get; set; }

    [Required]
    public string Nombre { get; set; } = "";

    [Required]
    public string Apellido { get; set; } = "";

    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    public override string ToString()
    {
        return $"Persona {{Nombre={Nombre}, Apellido={Apellido}, DNI={Dni}, Telefono={Telefono}, Email={Email}}}";
    }

}
