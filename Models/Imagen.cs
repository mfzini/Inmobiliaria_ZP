namespace inmobiliaria.Models;
using System.ComponentModel.DataAnnotations;

public class Imagen
{
    [Key]
    public string? Id { get; set; }
    public string? OriginalName {get; set;}
    
    [Required]
    public string? Url { get; set; }
    
    public IFormFile? File {get; set;}
}