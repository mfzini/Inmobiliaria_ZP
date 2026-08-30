using System.ComponentModel.DataAnnotations;
using inmobiliaria.Models;

namespace inmobiliaria.DTO;

public class InmuebleDTO
{
    public required string? Propietario {get; set;}
    [Required(ErrorMessage = "El dni es obligatorio")]
    public required int Tipo {get; set;}
    [Required(ErrorMessage = "El tipo de inmueble es obligatorio")]
    public required string? Direccion {get; set;}
    [Required(ErrorMessage = "La direccion es obligatoria")]
    public required int Capacidad {get; set;}
    [Required(ErrorMessage = "La capacidad es obligatoria")]
    public decimal Precio {get; set;}
    [Required(ErrorMessage = "El Precio es obligatorio")]
    public bool Listado {get; set;}

    public decimal Latitud {get; set;}
    public decimal Longitud {get; set;}

}