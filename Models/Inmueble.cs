namespace inmobiliaria.Models;
using System.ComponentModel.DataAnnotations;

public class Inmueble
{
    [Key]
    public int idInmueble {get; set;}
    [Required]
    public int idPropietario {get; set;}
    [Required]
    public string tipo {get; set;}
    [Required]
    public string direccion {get; set;}
    [Required]
    public string capacidad {get; set;}
    [Required]
    public decimal precio {get; set;}
    [Required]
    public bool listado {get; set;}
    public override string ToString()
    {
        return $"Inmueble {{Tipo={tipo}, Direccion={direccion}, Capacidad={capacidad}, Precio={precio}, Listado={listado}}}";
    }
}