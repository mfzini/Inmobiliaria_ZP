namespace inmobiliaria.Models;
using System.ComponentModel.DataAnnotations;

public class Pago
{
    [Key]
    public string? Id {get; set; }
    [Required]
    public Reserva Reserva{get; set;}
    [Required]
    public decimal Monto {get; set;}
    [Required]
    public ConceptoPago Concepto {get; set;}
    [Required]
    public DateTime Fecha {get; set;}
    public override string ToString()
    {
        return $"Pago {{Monto={Monto}, Concepto={Concepto}, Fecha={Fecha}}}";
    }

}