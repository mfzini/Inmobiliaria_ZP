using inmobiliaria.DTO;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers;

public class InmuebleController(InmuebleRepository inmuebleRepo, PersonaRepository personaRepo, TipoInmuebleRepo tipoInmuebleRepo) : Controller
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
        TipoInmueble? tipo = tipoInmuebleRepo.FindTipoByID(dto.Tipo);
        
        if (propietario == null)
        {
            ModelState.AddModelError("Propietario", "Esa persona no existe");
        }

        if (tipo == null)
        {
            ModelState.AddModelError("Tipo", "No existe ese tipo de inmueble");
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        Inmueble inmueble = new Inmueble
        {
            Propietario = propietario,
            Tipo = tipo,
            Direccion = dto.Direccion,
            Capacidad = dto.Capacidad,
            Precio = dto.Precio,
            PorcentajeReserva = dto.PorcentajeReserva,
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
    public IActionResult BuscarPorDireccion(string? direccion)
    {
        if (string.IsNullOrWhiteSpace(direccion))
        {
            return Json(inmuebleRepo.GetPage());
        }

        return Json(inmuebleRepo.ListarByDireccion(direccion));
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
                return NotFound();
            }

            var dto = new InmuebleDTO
            {
                Propietario = inmueble.Propietario.Dni,
                Tipo = inmueble.Tipo.Id,
                Direccion = inmueble.Direccion,
                Capacidad = inmueble.Capacidad,
                Precio = inmueble.Precio,
                PorcentajeReserva = inmueble.PorcentajeReserva,
                Listado = inmueble.Listado,
                Latitud = inmueble.Latitud,
                Longitud = inmueble.Longitud
            };

            ViewBag.InmuebleId = id;
            ViewBag.TextoPropietario = $"{inmueble.Propietario.Nombre} {inmueble.Propietario.Apellido} ({inmueble.Propietario.Dni})";
            ViewBag.TextoTipo = inmueble.Tipo.Nombre;
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
        TipoInmueble? tipo = tipoInmuebleRepo.FindTipoByID(dto.Tipo);

        if (propietario == null)
        {
            ModelState.AddModelError("Propietario", "Esa persona no existe.");
        }

        if (tipo == null)
        {
            ModelState.AddModelError("Tipo", "No existe ese tipo de inmueble");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.InmuebleId = id;
            return View(dto);
        }

        var inmuebleAntes = inmuebleRepo.GetById(id);

        Inmueble inmueble = new Inmueble
        {
            Id = id,
            Propietario = propietario,
            Tipo = tipo,
            Direccion = dto.Direccion,
            Capacidad = dto.Capacidad,
            Precio = dto.Precio,
            PorcentajeReserva = dto.PorcentajeReserva,
            Listado = dto.Listado,
            Latitud = dto.Latitud,
            Longitud = dto.Longitud,
            Portada = inmuebleAntes?.Portada
        };

        inmuebleRepo.Update(inmueble);
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
            var inmueble = inmuebleRepo.GetById(id);
            if(inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        } catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }

    }

    [HttpPost]
    public IActionResult Eliminar(Inmueble inmueble)
    {
        if (string.IsNullOrEmpty(inmueble.Id))
        {
            return RedirectToAction(nameof(Listar));
        }

        try
        {
            inmuebleRepo.Delete(inmueble);
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
            var inmueble = inmuebleRepo.GetById(id);
            if(inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        } catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }
    }

    [HttpGet]
    public IActionResult Fotos(string id)
    {
        var inmueble = inmuebleRepo.GetById(id);

        if(inmueble == null)
        {
            return NotFound();
        }

        return View(inmueble);
    }


    [HttpGet]
    public IActionResult ListarDisponiblesPorFechas(DateTime desde, DateTime hasta)
    {   
        return Json(inmuebleRepo.ListarDisponibles(desde, hasta));
    }

    [HttpGet]
    public IActionResult FiltrarPorEstado(string opcion, int dias)
    {
        if (opcion == "disponibles")
        {
            return Json(inmuebleRepo.FindByListingStatus(true));
        }

        if (opcion == "no_disponibles")
        {
            return Json(inmuebleRepo.FindByListingStatus(false));
        }

        if (opcion == "mas_reservados")
        {
            return Json(inmuebleRepo.ListConMasReservas365Dias());
        }

        if (opcion == "sin_reservas")
        {
            if(dias > 0)
            {
                return Json(inmuebleRepo.ListSinReservasEnXDias(dias));    
            }
            return Json(new List<Inmueble>());
        }

        return Json(inmuebleRepo.GetPage());
    }


    [HttpGet]
    public IActionResult Busqueda(DateTime? fechaInicio, DateTime? fechaFin, int? capacidad, int? tipo)
    {

        ViewBag.Tipos = tipoInmuebleRepo.ListAll();
        List<Inmueble> inmuebles = [];

        if (fechaInicio != null && fechaFin != null && capacidad != null && tipo != null)
        {
            inmuebles = inmuebleRepo.FindByTipoAndCapacidadAndFechas((int)tipo, (int)capacidad, (DateTime)fechaInicio, (DateTime)fechaFin);
        }

        return View(inmuebles);
    }


}