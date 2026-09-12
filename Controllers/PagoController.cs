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
    public IActionResult Editar(string reservaId)
    {       
        ViewBag.ReservaId = reservaId;
        return View();
    }

    
    [HttpPost]
    public IActionResult Editar(Pago pago, string reservaId, [FromForm] int concepto)
    {       
        pago.Reserva = new Reserva { Id = reservaId };
        pago.Concepto = new ConceptoPago { Id = pago.Concepto.Id};
        pagosRepo.Update(pago);
        return Redirect($"/Reserva/Detalles/{reservaId}");
    }



}
