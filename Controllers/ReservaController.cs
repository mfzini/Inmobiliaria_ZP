using inmobiliaria.DTO;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria.Controllers;

public class ReservaController(ReservaRepo reservaRepo, PersonaRepository personaRepo, InmuebleRepository inmuebleRepo, RepoPagos pagosRepo) : Controller
{

    [Authorize]
    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public IActionResult Registrar(ReservaDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        if (dto.FechaFin <= dto.FechaInicio)
        {
            ModelState.AddModelError("FechaFin", "La fecha fin debe ser mayor a la fecha inicio.");
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
        } else if (!inmueble.Listado)
        {
            ModelState.AddModelError("Inmueble", "El inmueble seleccionado no esta disponible para alquilar");
        }

        if (inmueble != null && reservaRepo.InmuebleOcupado(dto.Inmueble, dto.FechaInicio, dto.FechaFin))
        {
            ModelState.AddModelError(string.Empty, "Ese inmueble esta ocupado en otras fechas");
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        decimal montoTotal = (dto.FechaFin - dto.FechaInicio).Days * inmueble!.Precio;
        
        
        Reserva reserva = new Reserva
        {
            Inmueble = inmueble,
            Inquilino = inquilino,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            Monto = montoTotal
        };

        reservaRepo.Create(reserva, HttpContext);
        decimal montoPrimerPago = montoTotal * (inmueble.PorcentajeReserva / 100m);

        Pago pago = new Pago
        {
            Reserva = reserva,
            Monto = Math.Round(montoPrimerPago, 2),
            Concepto = new ConceptoPago { Id = 4 }, 
            Fecha = DateTime.Now
        };

        pagosRepo.Create(HttpContext, pago);
        return RedirectToAction(nameof(Listar));

    }

    [Authorize]
    [HttpGet]
    public IActionResult CalcularPorcentajeInicial(string idInmueble, DateTime FechaInicio, DateTime FechaFin)
    {
        if (string.IsNullOrEmpty(idInmueble) || FechaFin <= FechaInicio)
        {
            return BadRequest();
        }

        var inmueble = inmuebleRepo.GetById(idInmueble);
        if (inmueble == null)
        {
            return NotFound();
        }

        var monto = (FechaFin - FechaInicio).Days * inmueble.Precio;
        return Json(monto * inmueble.PorcentajeReserva / 100m);

    }

    [Authorize]
    [HttpGet]
    public IActionResult Listar()
    {
        return View();
    }

    [Authorize]
    [HttpGet]
    public IActionResult ListarTodas(int pagina = 1)
    {
        return Json(reservaRepo.GetPage(pagina, 7));
    }

    [Authorize]
    [HttpGet]
    public IActionResult ListarVigentes(DateTime desde, DateTime hasta, int pagina = 1)
    {
        return Json(reservaRepo.ListarVigentes(desde, hasta, pagina, 7));
    }

    [Authorize]
    [HttpGet]
    public IActionResult ListarPorTerminar(int dias, int pagina = 1)
    {
        return Json(reservaRepo.ListarFinalizanEnXDias(dias, pagina, 7));
    }

    [Authorize]
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
            if (reserva == null)
            {
                return NotFound();
            }
            var dto = new ReservaDTO
            {
                Inquilino = reserva.Inquilino.Dni,
                Inmueble = reserva.Inmueble.Id,
                FechaInicio = reserva.FechaInicio,
                FechaFin = reserva.FechaFin,
                Monto = reserva.Monto
            };

            ViewBag.ReservaId = id;
            ViewBag.TextoInmueble = reserva.Inmueble.Direccion;
            ViewBag.TextoInquilino = $"{reserva.Inquilino.Nombre} {reserva.Inquilino.Apellido} ({reserva.Inquilino.Dni})";
            return View(dto);

        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }

    }

    [Authorize]
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

        var monto = ((decimal)(dto.FechaFin - dto.FechaInicio).TotalDays) * inmueble.Precio;
        Reserva reserva = new Reserva
        {
            Id = id,
            Inmueble = inmueble,
            Inquilino = inquilino,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            Monto = monto
        };

        reservaRepo.Update(HttpContext, reserva);
        return RedirectToAction(nameof(Listar));
    }

    [Authorize(Policy = "Administrador")]
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
            if (reserva == null)
            {
                return NotFound();
            }
            return View(reserva);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }
    }

    [Authorize(Policy = "Administrador")]
    [HttpPost]
    public IActionResult Eliminar(Reserva reserva)
    {

        if (string.IsNullOrEmpty(reserva.Id))
        {
            return RedirectToAction(nameof(Listar));
        }

        try
        {
            reservaRepo.Delete(reserva);
            TempData["Mensaje"] = "Reserva eliminada correctamente.";
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            TempData["Error"] = "No se puede eliminar la reserva porque tiene pagos asociados";
        }
        return RedirectToAction(nameof(Listar));
    }

    [Authorize]
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
            if (reserva == null)
            {
                return NotFound();
            }
            var pagosReserva = pagosRepo.FindByReserva(reserva);
            reserva.Pagos = pagosReserva;
            return View(reserva);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }
    }

    [Authorize]
    [HttpGet]
    public IActionResult BuscarDireccion(string direccionBuscada)
    {
        try
        {
            var res = inmuebleRepo.ListarByDireccion(direccionBuscada);
            return Json(res);
        }
        catch (Exception e)
        {
            return Json(new { Error = e.Message });
        }
    }

    [Authorize]
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


    [Authorize]
    [HttpGet]
    public IActionResult Extender(string id)
    {
        var reservaAnterior = reservaRepo.FindByID(id);
        if (reservaAnterior == null) return NotFound();

        if (reservaAnterior.FechaCancelacion != null)
        {
            return RedirectToAction("Detalles", new { id });
        }

        var nueva = new Reserva
        {
            Id = reservaAnterior.Id,
            Inmueble = reservaAnterior.Inmueble,
            Inquilino = reservaAnterior.Inquilino,
            FechaInicio = reservaAnterior.FechaFin,
        };

        return View(nueva);
    }

    [Authorize]
    [HttpPost]
    public IActionResult Extender(string Inmueble, string Inquilino, DateTime FechaInicio, DateTime FechaFin, decimal montoExtension, string idAnterior)
    {
        if (FechaFin <= FechaInicio)
        {
            return RedirectToAction("Extender", new { id = idAnterior });
        }
        var inmueble = inmuebleRepo.GetById(Inmueble);
        if (inmueble == null) return NotFound();
        
        decimal nuevoMontoTotal = (FechaFin - FechaInicio).Days * inmueble.Precio;

        var nuevaReservaExtendida = new Reserva
        {
            Inmueble = inmueble,
            Inquilino = new Persona { Dni = Inquilino },
            FechaInicio = FechaInicio,
            FechaFin = FechaFin,
            Monto = nuevoMontoTotal
        };

        reservaRepo.Create(nuevaReservaExtendida, HttpContext);

        if (montoExtension > 0)
        {
            var pagoNuevo = new Pago
            {
                Reserva = nuevaReservaExtendida,
                Concepto = new ConceptoPago { Id = 1 },
                Monto = montoExtension,
                Fecha = DateTime.Today
            };

            pagosRepo.Create(HttpContext, pagoNuevo);
        }

        return RedirectToAction("Detalles", new { id = nuevaReservaExtendida.Id });
    }

    [Authorize]
    [HttpGet]
    public IActionResult FinalizarTemprano(string id)
    {
        var reserva = reservaRepo.FindByID(id);
        if (reserva == null)
        {
            return NotFound();
        }
        

        DateTime FechaFinalizacion = DateTime.Today;
        ViewBag.FechaFinalizacion = FechaFinalizacion.ToString("dd/MM/yyyy");

        int diasTotales = (reserva.FechaFin - reserva.FechaInicio).Days;
        int diasPasados = (FechaFinalizacion - reserva.FechaInicio).Days;
        int diasRestantes = (reserva.FechaFin - FechaFinalizacion).Days;

        decimal precioAlquilerDia = reserva.Inmueble.Precio;
        decimal montoAlquilerRestante = diasRestantes * precioAlquilerDia;
        decimal multaCalculada;

        if (diasPasados < (diasTotales / 2.0))
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

    [Authorize]
    [HttpPost]
    public IActionResult HacerFinalizacion(string idReserva, decimal montoMulta)
    {
        var reserva = reservaRepo.FindByID(idReserva);
        if (reserva == null)
        {
            return NotFound();
        }

        reserva.FechaCancelacion = DateTime.Today;
        reservaRepo.Update(HttpContext, reserva, "finalizo");



        var pagoMulta = new Pago
        {
            Reserva = reserva,
            Concepto = new ConceptoPago { Id = 2 },
            Monto = montoMulta,
            Fecha = DateTime.Now,
        };

        pagosRepo.Create(HttpContext, pagoMulta);

        return RedirectToAction("Detalles", new { id = idReserva });

    }


}
