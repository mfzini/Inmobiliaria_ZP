using inmobiliaria.DTO;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers;

public class ReservaController(ReservaRepo reservaRepo, PersonaRepository personaRepo, InmuebleRepository inmuebleRepo) : Controller 
{
    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    
    [HttpPost]
    public IActionResult Registrar(ReservaDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        Persona? inquilino = personaRepo.FindByDni(dto.Inquilino);
        Inmueble? inmueble = inmuebleRepo.GetById(dto.Inmueble);
        
        Reserva reserva = new Reserva
        {
            Inmueble = inmueble,
            Inquilino = inquilino,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin
        };

        reservaRepo.Create(reserva);
        return RedirectToAction(nameof(Listar));

    }
    

    [HttpGet]
    public IActionResult Listar()
    {
        return View(reservaRepo.GetPage());
    }

    [HttpGet]
    public IActionResult Editar(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToAction(nameof(Listar));
        }
        try
        {
            var reserva = reservaRepo.FindByID(id);
            if(reserva == null)
            {
                return NotFound();
            }
            var dto = new ReservaDTO
            {
                Inquilino = reserva.Inquilino.Dni,
                Inmueble = reserva.Inmueble.Id,
                FechaInicio = reserva.FechaInicio,
                FechaFin = reserva.FechaFin
            };

            ViewBag.ReservaId = id;
            return View(dto);

        } catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }

    }

    [HttpPost]
    public IActionResult Editar(string id, ReservaDTO dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReservaId = id;
            return View(dto);
        }

        Persona? inquilino = personaRepo.FindByDni(dto.Inquilino);
        Inmueble? inmueble = inmuebleRepo.GetById(dto.Inmueble);

        Reserva reserva = new Reserva
        {
            Id = id,
            Inmueble = inmueble,
            Inquilino = inquilino,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin
        };

        reservaRepo.Update(reserva);
        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public IActionResult Eliminar(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToAction(nameof(Listar));
        }

        try
        {
            var reserva = reservaRepo.FindByID(id);
            if(reserva == null)
            {
                return NotFound();
            }
            return View(reserva);
        } catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }
    }

    [HttpPost]
    public IActionResult Eliminar(Reserva reserva)
    {
        try
        {
            reservaRepo.Delete(reserva);
            return RedirectToAction(nameof(Listar));    
        } catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }
        
    }

    [HttpGet]
    public IActionResult Detalles(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToAction(nameof(Listar));
        }

        try
        {
            var reserva = reservaRepo.FindByID(id);
            if(reserva == null)
            {
                return NotFound();
            }
            return View(reserva);
        } catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }
    }




}