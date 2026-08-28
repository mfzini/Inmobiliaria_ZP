using System.ComponentModel.DataAnnotations;

namespace inmobiliaria.Models;
public class TipoInmueble
{
    [Key]
    public int Id {get; set;}

    [Required]
    public required string Nombre {get; set;}
}