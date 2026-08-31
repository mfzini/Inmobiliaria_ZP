namespace inmobiliaria.Models;
using System.ComponentModel.DataAnnotations;

public class Reserva
{
    [Key]
    public string? Id {get; set;}
    [Required]
    public Inmueble? Inmueble {get; set;}
    [Required]
    public Persona? Inquilino {get; set;}
    [Required]
    public DateTime FechaInicio { get; set;}
    [Required]
    public DateTime FechaFin {get; set;}
    public override string ToString()
    {
        return $"Reserva {{FechaInicio={FechaInicio}, FechaFin={FechaFin}}}";
    }
}

