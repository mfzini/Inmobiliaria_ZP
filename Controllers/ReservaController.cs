using inmobiliaria.DTO;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers;

public class ReservaController(ReservaRepo reservaRepo, PersonaRepository personaRepo, InmuebleRepository inmuebleRepo, RepoPagos pagosRepo) : Controller 
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
            var pagosReserva = pagosRepo.FindByReserva(reserva);
            reserva.Pagos = pagosReserva;
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
        var reservaAnterior = reservaRepo.FindByID(id);
        if (reservaAnterior == null) return NotFound();

        var nueva = new Reserva
        {
            Id = reservaAnterior.Id,
            Inmueble = reservaAnterior.Inmueble,
            Inquilino = reservaAnterior.Inquilino,
            FechaInicio = reservaAnterior.FechaFin,
        };

        return View(nueva);
    }

    [HttpPost]
    public IActionResult Extender(string Inmueble, string Inquilino, DateTime FechaInicio, DateTime FechaFin, string idAnterior)
    {
        if (FechaFin <= FechaInicio)
        {
            return RedirectToAction("Extender", new { id = idAnterior });
        }

        var nuevaReservaExtendida = new Reserva
        {
            Inmueble = new Inmueble {Id= Inmueble},
            Inquilino = new Persona{Dni= Inquilino},
            FechaInicio = FechaInicio,
            FechaFin = FechaFin
        };

        reservaRepo.Create(nuevaReservaExtendida);
        return RedirectToAction("Listar");
    }

    [HttpGet]
    public IActionResult FinalizarTemprano(string id)
    {
        var reserva = reservaRepo.FindByID(id);
        if(reserva == null){
            return NotFound();
        };

        DateTime FechaFinalizacion = DateTime.Today; 
        ViewBag.FechaFinalizacion = FechaFinalizacion.ToString("dd/MM/yyyy"); 

        int diasTotales = (reserva.FechaFin - reserva.FechaInicio).Days;
        int diasPasados = (FechaFinalizacion - reserva.FechaInicio).Days;
        int diasRestantes = (reserva.FechaFin - FechaFinalizacion).Days;

        decimal precioAlquilerDia = reserva.Inmueble.Precio / 30m; 
        decimal montoAlquilerRestante = diasRestantes * precioAlquilerDia;
        decimal multaCalculada = 0;

        if(diasPasados < (diasTotales / 2))
        {
            multaCalculada = montoAlquilerRestante * 0.50m;
        }
        else
        {
            multaCalculada = montoAlquilerRestante * 0.25m;
        }

        ViewBag.MontoMulta = Math.Round(multaCalculada, 2);
        return View(reserva);

    }

    [HttpPost]
    public IActionResult HacerFinalizacion(string idReserva, decimal montoMulta)
    {
        var pagoMulta = new Pago
        {
            Reserva = new Reserva { Id = idReserva }, 
            Concepto = new ConceptoPago { Id = 2 }, // para multa
            Monto = montoMulta,
            Fecha = DateTime.Today,
        };

        pagosRepo.Create(pagoMulta);
        return RedirectToAction("Detalles", new {id = idReserva});
    }


}
