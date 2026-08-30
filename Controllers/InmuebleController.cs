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
    public IActionResult Editar(string id)
    {
        if(string.IsNullOrEmpty(id))
        {
            return RedirectToAction(nameof(Listar));
        }
        try
        {
            var inmueble = inmuebleRepo.GetById(id);
            if(inmueble == null)
            {
                Response.StatusCode = 404;
                //todo.
            }

            var dto = new InmuebleDTO
            {
                Propietario = inmueble.Propietario.Dni,
                Tipo = inmueble.Tipo.Id,
                Direccion = inmueble.Direccion,
                Capacidad = inmueble.Capacidad,
                Precio = inmueble.Precio,
                Listado = inmueble.Listado,
                Latitud = inmueble.Latitud,
                Longitud = inmueble.Longitud
            };

            ViewBag.InmuebleId = id;
            return View(dto);

        }catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }
    }

    [HttpPost]
    public IActionResult Editar(string id, InmuebleDTO dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.InmuebleId = id;
            return View(dto);
        }

        Persona? propietario = personaRepo.FindByDni(dto.Propietario);
        TipoInmueble? tipo = inmuebleRepo.FindTipoByID(dto.Tipo);

        Inmueble inmueble = new Inmueble
        {
            Id = id,
            Propietario = propietario,
            Tipo = tipo,
            Direccion = dto.Direccion,
            Capacidad = dto.Capacidad,
            Precio = dto.Precio,
            Listado = dto.Listado,
            Latitud = dto.Latitud,
            Longitud = dto.Longitud
        };

        inmuebleRepo.Update(inmueble);
        return RedirectToAction(nameof(Listar));
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