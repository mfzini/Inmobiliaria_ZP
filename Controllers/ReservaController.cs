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
        
        if (inquilino == null)
        {
            ModelState.AddModelError("Inquilino", "Ese inquilino no existe");
        }

        if (inmueble == null)
        {
            ModelState.AddModelError("Inmueble", "No existe ese inmueble");
        }


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
        return View();
    }

    [HttpGet]
    public IActionResult ListarTodas()
    {
        return Json(reservaRepo.GetPage());
    }

    [HttpGet]
    public IActionResult ListarVigentes(DateTime desde, DateTime hasta)
    {
        return Json(reservaRepo.ListarVigentes(desde, hasta));
    }

    [HttpGet]
    public IActionResult ListarPorTerminar(int dias)
    {
        return Json(reservaRepo.ListarFinalizanEnXDias(dias));
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
            ViewBag.TextoInmueble = reserva.Inmueble.Direccion;
            ViewBag.TextoInquilino = $"{reserva.Inquilino.Nombre} {reserva.Inquilino.Apellido} ({reserva.Inquilino.Dni})";
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

        if (inquilino == null)
        {
            ModelState.AddModelError("Inquilino", "Ese inquilino no existe");
        }

        if (inmueble == null)
        {
            ModelState.AddModelError("Inmueble", "No existe ese inmueble");
        }


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

            reserva.Pagos = new List<Pago>
            {
                new Pago
                {
                    Id = "1",
                    Fecha = DateTime.Now,
                    Concepto = new ConceptoPago { Id = 1, Nombre = "Seña" },
                    Monto = 35000,
                    Anulado = false
                },
                new Pago
                {
                    Id = "2",
                    Fecha = DateTime.Now,
                    Concepto = new ConceptoPago { Id = 2, Nombre = "Alquiler Total" },
                    Monto = 150000,
                    Anulado = true
                }
            };
            return View(reserva);
        } catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }
    }

    [HttpGet]
    public IActionResult BuscarDireccion(string direccionBuscada)
    {
        try
        {
            var res = inmuebleRepo.ListarByDireccion(direccionBuscada);
            return Json(res);
        } catch (Exception e)
        {
            return Json(new { Error = e.Message });
        }
    }

    [HttpGet]
    public IActionResult BuscarInquilino(string nombreBuscado)
    {
        try
        {
            var res = personaRepo.FindByNombre(nombreBuscado);
            return Json(res);
        }
        catch (Exception e)
        {
            return Json(new { Error = e.Message });
        }
    }

    [HttpGet]
    public IActionResult Extender(string id)
    {
        var reservaPrueba = new Reserva
        {
            Id = "reserva-fantasma-1",
            Inmueble = new Inmueble { Direccion = "San Martín 1234" },
            Inquilino = new Persona { Nombre = "Juan", Apellido = "Perez", Dni = "38123456" },
            FechaInicio = DateTime.Now,
            FechaFin = DateTime.Now
        };

        return View(reservaPrueba);
    }




}