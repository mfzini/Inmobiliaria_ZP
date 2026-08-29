using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers;

public class TipoInmuebleController(InmuebleRepository repo) : Controller // todo: cambiar por tipoinmueble
{
    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Listar()
    {
        return View(new List<TipoInmueble>());
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