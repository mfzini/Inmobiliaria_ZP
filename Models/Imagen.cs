namespace inmobiliaria.Models;
using System.ComponentModel.DataAnnotations;

public class Imagen
{
    [Key]
    public string? Id { get; set; }

    [Required]
    public string? Url { get; set; }
    public Inmueble? Inmueble { get; set; }
    
}