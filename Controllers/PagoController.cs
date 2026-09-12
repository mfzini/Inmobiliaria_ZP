using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;

namespace inmobiliaria.Controllers;

public class PagoController : Controller
{
    [HttpGet]
    public IActionResult Registrar(string reservaId)
    {       
        ViewBag.ReservaId = "reserva123";

        var pagoPrueba = new Pago
        {
            Reserva = new Reserva { Id = ViewBag.ReservaId },
            Monto = 45000,
            Fecha = DateTime.Now,
            Concepto = new ConceptoPago { Id = 1, Nombre = "Alquiler" }
        };

        return View(pagoPrueba);
    }

    [HttpGet]
    public IActionResult Editar(string id)
    {
        var pago = new Pago
        {
            Id = "pago-1",
            Reserva = new Reserva { Id = "reserva-1" },
            Monto = 5000,
            Fecha = DateTime.Now,
            Concepto = new ConceptoPago { Id = 1, Nombre = "Alquiler" }
        };

        return View(pago);
    }





}
