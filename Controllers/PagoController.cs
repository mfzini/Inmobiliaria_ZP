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
        var pago = pagosRepo.FindById(id);
        return View(pago);
    }

    
    [HttpPost]
    public IActionResult Editar(Pago pago, [FromForm] int concepto)
    {       
        pago.Concepto = new ConceptoPago { Id = concepto};
        pagosRepo.Update(pago);
        return Redirect($"/Reserva/Detalles/{pago.Reserva.Id}");
    }

    [HttpGet]
    public IActionResult Anular(string id)
    {
        var pago = pagosRepo.FindById(id);
        pago.Anulado = !pago.Anulado;
        pagosRepo.Update(pago);
        return Redirect($"/Reserva/Detalles/{pago.Reserva.Id}");
    }


}
