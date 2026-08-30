using System.ComponentModel.DataAnnotations;
using inmobiliaria.Models;

namespace inmobiliaria.DTO;

public class InmuebleDTO
{
    public required string? Propietario {get; set;}
    [Required]
    public required int Tipo {get; set;}
    [Required]
    public required string? Direccion {get; set;}
    [Required]
    public required int Capacidad {get; set;}
    [Required]
    public decimal Precio {get; set;}
    [Required]
    public bool Listado {get; set;}

    public string? Latitud {get; set;}
    public string? Longitud {get; set;}

}