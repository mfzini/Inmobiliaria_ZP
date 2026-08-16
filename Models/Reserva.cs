namespace inmobiliaria.Models;
using System.ComponentModel.DataAnnotations;

public class Reserva
{
    [Key]
    public int idReserva {get; set;}
    [Required]
    public int idInmueble {get; set;}
    [Required]
    public int idInquilino {get; set;}
    [Required]
    public DateTime FechaInicio { get; set;}
    [Required]
    public DateTime FechaFin {get; set;}
    public override string ToString()
    {
        return $"Reserva {{FechaInicio={FechaInicio}, FechaFin={FechaFin}}}";
    }
}

