using inmobiliaria.DTO;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers;

public class InmuebleController(InmuebleRepository inmuebleRepo, PersonaRepository personaRepo) : Controller
{
    [HttpGet]
    public IActionResult Registrar()
    {

        return View();


    }

    [HttpPost]
    public IActionResult Registrar(InmuebleDTO dto)
    {

        /*if (!ModelState.IsValid)
        {
            return View(inmueble);
        }*/
        Persona propietario = personaRepo.FindByDni(dto.Propietario);
        TipoInmueble tipo = inmuebleRepo.FindTipoByID(dto.Tipo);

        /* Inmueble inmueble = new Inmueble
        {
            Propietario = propietario,
            Tipo = tipo,
            }
        inmuebleRepo.Create(inmueble);
        */
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