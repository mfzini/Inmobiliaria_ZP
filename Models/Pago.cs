namespace inmobiliaria.Models;
using System.ComponentModel.DataAnnotations;

public class Pago
{
    [Key]
    public int IdPago {get; set; }
    [Required]
    public int IdReserva{get; set;}
    [Required]
    public decimal monto {get; set;}
    [Required]
    public string concepto {get; set;}
    [Required]
    public DateTime fecha {get; set;}
    public override string ToString()
    {
        return $"Pago {{Monto={monto}, Concepto={concepto}, Fecha={fecha}}}";
    }

}