using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria.Controllers;

public class TipoInmuebleController(TipoInmuebleRepo tipoRepo) : Controller 
{
    [Authorize]
    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public IActionResult Registrar(TipoInmueble tipo)
    {
        if (!ModelState.IsValid)
        {
            return View(tipo);
        }
        try
        {
            tipoRepo.CreateTipoInmueble(tipo);
            return RedirectToAction(nameof(Listar));    
        } catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }
        
    }

    [Authorize]
    [HttpGet]
    public IActionResult Listar()
    {
        return View(tipoRepo.ListAll());
    }

    [Authorize(Policy = "Administrador")]
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

    [Authorize(Policy = "Administrador")]
    [HttpPost]
    public IActionResult Eliminar(TipoInmueble tipo)
    {
        try
        {
            tipoRepo.DeleteTipoInmueble(tipo);
            return RedirectToAction(nameof(Listar));    
        } catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }
    }

    [Authorize]
    [HttpGet]
    public IActionResult buscarTipo(string tipoBuscado)
    {
        try
        {
            var res = tipoRepo.FindTipoByNombreLike(tipoBuscado);
            return Json(res);
        }catch(Exception e)
        {
            return Json(new {Error = e.Message});
        }
    }


}