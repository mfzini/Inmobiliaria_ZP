namespace inmobiliaria.Models;
using System.ComponentModel.DataAnnotations;

public class Inmueble
{
    [Key]
    public string? Id {get; set;}
    [Required]
    public Persona? Propietario {get; set;}
    [Required]
    public TipoInmueble? Tipo {get; set;}
    [Required]
    public string? Direccion {get; set;}
    [Required]
    public int Capacidad {get; set;}
    [Required]
    public decimal Precio {get; set;}
    [Required]
    public bool Listado {get; set;}

    public decimal Latitud {get; set;}
    public decimal Longitud {get; set;}
    public override string ToString()
    {
        return $"Inmueble {{Tipo={Tipo}, Direccion={Direccion}, Capacidad={Capacidad}, Precio={Precio}, Listado={Listado}}}";
    }
}