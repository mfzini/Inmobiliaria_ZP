using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers;

public class TipoInmuebleController(TipoInmuebleRepo tipoRepo) : Controller 
{
    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registrar(TipoInmueble tipo)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        tipoRepo.CreateTipoInmueble(tipo);
        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public IActionResult Listar()
    {
        return View(tipoRepo.ListAll());
    }

    [HttpGet]
    public IActionResult Eliminar(int id)
    {
        
        var tipo = tipoRepo.FindTipoByID(id);
        if(tipo == null)
        {
            return RedirectToAction(nameof(Listar));
        }
        return View(tipo);
    }

    [HttpPost]
    public IActionResult Eliminar(TipoInmueble tipo)
    {
        tipoRepo.DeleteTipoInmueble(tipo);
        return RedirectToAction(nameof(Listar));
    }

    


}