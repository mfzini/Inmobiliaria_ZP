using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;

namespace inmobiliaria.Controllers;

public class PagoController(RepoPagos pagosRepo) : Controller
{
    [HttpGet]
    public IActionResult Registrar(string reservaId)
    {       
        ViewBag.ReservaId = reservaId;
        return View();
    }

    [HttpPost]
    public IActionResult Registrar(Pago pago, string reservaId)
    {       
        pago.Reserva = new Reserva { Id = reservaId };
        pagosRepo.Create(pago);
        return Redirect($"/Reserva/Detalles/{reservaId}");
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
