using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers;

public class ReservaController(PersonaRepository repo) : Controller // todo: cambiar por reserva
{
    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Listar()
    {
        return View(new List<Reserva>());
    }

    [HttpGet]
    public IActionResult Editar()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Eliminar()
    {
        return View();
    }

}