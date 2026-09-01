using System.ComponentModel.DataAnnotations;

namespace inmobiliaria.Models;
public class TipoInmueble
{
    [Key]
    public int Id {get; set;}

    [Required(ErrorMessage = "El nombre del tipo es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
    public string? Nombre {get; set;}
}