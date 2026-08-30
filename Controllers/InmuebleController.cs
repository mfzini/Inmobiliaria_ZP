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

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        Persona? propietario = personaRepo.FindByDni(dto.Propietario);
        TipoInmueble? tipo = inmuebleRepo.FindTipoByID(dto.Tipo);

        Inmueble inmueble = new Inmueble
        {
            Propietario = propietario,
            Tipo = tipo,
            Direccion = dto.Direccion,
            Capacidad = dto.Capacidad,
            Precio = dto.Precio,
            Listado = dto.Listado,
            Latitud = dto.Latitud,
            Longitud = dto.Longitud
        };

        inmuebleRepo.Create(inmueble);

        return RedirectToAction(nameof(Listar));

    }


    [HttpGet]
    public IActionResult Listar()
    {
        return View(inmuebleRepo.GetPage());
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

    [HttpPost]
    public IActionResult Eliminar(Inmueble inmueble)
    {
        inmuebleRepo.Delete(inmueble);
        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public IActionResult Detalle()
    {
        return View();
    }

}