using System.ComponentModel.DataAnnotations;
using inmobiliaria.Models;

namespace inmobiliaria.DTO;

public class InmuebleDTO
{
    [Required(ErrorMessage = "El dni es obligatorio")]
    [StringLength(20, ErrorMessage = "El DNI no puede superar los 20 caracteres")]
    public required string? Propietario {get; set;}
    
    [Required(ErrorMessage = "El tipo de inmueble es obligatorio")]
    public required int Tipo {get; set;}
    
    [Required(ErrorMessage = "La direccion es obligatoria")]
    [StringLength(100, ErrorMessage = "La direccion no puede superar los 100 caracteres")]
    public required string? Direccion {get; set;}
    
    [Required(ErrorMessage = "La capacidad es obligatoria")]
    [Range(1, 100, ErrorMessage = "La capacidad debe ser de al menos 1 persona")]
    public required int Capacidad {get; set;}
    
    [Required(ErrorMessage = "El precio es obligatorio")]
    public decimal Precio {get; set;}

    [Required(ErrorMessage = "El porcentaje de reserva es obligatorio")]
    [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
    public decimal PorcentajeReserva {get; set;}
    
    public bool Listado {get; set;}

    public decimal Latitud {get; set;}
    public decimal Longitud {get; set;}

}