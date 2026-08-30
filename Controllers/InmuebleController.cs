using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers;

public class InmuebleController(InmuebleRepository repo) : Controller
{
    [HttpGet]
    public IActionResult Registrar()
    {
        
        return View();

    
    }

    [HttpPost]
    public IActionResult Registrar(Inmueble inmueble)
    {
        
        /*if (!ModelState.IsValid)
        {
            return View(inmueble);
        }*/
        repo.Create(inmueble);
        return RedirectToAction(nameof(Listar));

    }


    [HttpGet]
    public IActionResult Listar()
    {
        return View();
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

    [HttpGet]
    public IActionResult Detalle()
    {
        return View();
    }

}