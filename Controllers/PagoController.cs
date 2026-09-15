using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria.Controllers;

public class PagoController(RepoPagos pagosRepo) : Controller
{
    [Authorize]
    [HttpGet]
    public IActionResult Registrar(string reservaId)
    {       
        ViewBag.ReservaId = reservaId;
        ViewBag.Conceptos = pagosRepo.ListAllConceptos();
        return View();
    }

    [Authorize]
    [HttpPost]
    public IActionResult Registrar(Pago pago, string reservaId)
    {       
        pago.Reserva = new Reserva { Id = reservaId };
        pagosRepo.Create(HttpContext, pago);
        return Redirect($"/Reserva/Detalles/{reservaId}");
    }

    [Authorize]
    [HttpGet]
    public IActionResult Editar(string id)
    {       
        var pago = pagosRepo.FindById(id);
        return View(pago);
    }


    [Authorize]    
    [HttpPost]
    public IActionResult Editar(Pago pago, [FromForm] int concepto)
    {       
        pago.Concepto = new ConceptoPago { Id = concepto};
        pagosRepo.Update(HttpContext, pago);
        return Redirect($"/Reserva/Detalles/{pago.Reserva.Id}");
    }

    [Authorize]
    [HttpGet]
    public IActionResult Anular(string id)
    {
        var pago = pagosRepo.FindById(id);
        pago.Anulado = !pago.Anulado;
        pagosRepo.Update(HttpContext, pago);
        return Redirect($"/Reserva/Detalles/{pago.Reserva.Id}");
    }


}
