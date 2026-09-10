using System.ComponentModel.DataAnnotations;

namespace inmobiliaria.Models;
public class TipoInmueble
{
    [Key]
    public int Id {get; set;}

    [Required(ErrorMessage = "El nombre del tipo es obligatorio")]
    [StringLength(20, ErrorMessage = "El nombre no puede superar los 20 caracteres")]
    public string? Nombre {get; set;}
}