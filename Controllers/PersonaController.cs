using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers;

public class PersonaController(PersonaRepository repo) : Controller
{
    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registrar(Persona persona)
    {
        //return repo.Create(persona);
        return RedirectToAction();
    }

    [HttpGet]
    public IActionResult Listar()
    {
        return View(new List<Persona>());
    }

    [HttpGet]
    public IActionResult Editar()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Eliminar(int id)
    {
        return View();
    }


}