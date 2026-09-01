using System.ComponentModel.DataAnnotations;
using inmobiliaria.Models;

namespace inmobiliaria.DTO;

public class ReservaDTO
{
    [Key]
    [Required(ErrorMessage = "El dni de inquilino es obligatorio")]
    [StringLength(20, ErrorMessage = "El DNI no puede superar los 20 caracteres")]
    public string? Inquilino {get; set;}
    [Required(ErrorMessage = "El Inmueble es obligatorio")]
    public string? Inmueble {get; set;}
    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    public DateTime FechaInicio { get; set;}
    [Required(ErrorMessage = "La fecha de fin es obligatoria")]
    public DateTime FechaFin {get; set;}
    
}