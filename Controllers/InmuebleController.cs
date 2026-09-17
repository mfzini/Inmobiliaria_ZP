using inmobiliaria.DTO;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria.Controllers;

public class InmuebleController(InmuebleRepository inmuebleRepo, PersonaRepository personaRepo, TipoInmuebleRepo tipoInmuebleRepo) : Controller
{
    [Authorize]
    [HttpGet]
    public IActionResult Registrar()
    {

        return View();


    }

    [Authorize]
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


    [Authorize]
    [HttpGet]
    public IActionResult BuscarPorDireccion(string? direccion, int pagina = 1)
    {
        if (string.IsNullOrWhiteSpace(direccion))
        {
            return Json(inmuebleRepo.GetPage(pagina));
        }

        return Json(inmuebleRepo.ListarByDireccion(direccion, pagina));
    }

    [Authorize]
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

    [Authorize]
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

    [Authorize(Policy = "Administrador")]
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

    [Authorize]
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

    
    [Authorize]
    [HttpGet]
    public IActionResult Listar()
    {
        return View();
    }

    [Authorize]
    [HttpGet]
    public IActionResult ListarTodos(string? direccion, int pagina = 1)
    {
        if (string.IsNullOrWhiteSpace(direccion))
        {
            return Json(inmuebleRepo.GetPage(pagina));
        }

        return Json(inmuebleRepo.ListarByDireccion(direccion, pagina));
    }

    [Authorize]
    [HttpGet]
    public IActionResult ListarPorDisponibilidad(bool disponible, int pagina = 1)
    {
        return Json(inmuebleRepo.FindByListingStatus(disponible, pagina));
    }

    [Authorize]
    [HttpGet]
    public IActionResult ListarMasReservados(int pagina = 1)
    {
        return Json(inmuebleRepo.ListConMasReservas365Dias(pagina));
    }
    
    [Authorize]
    [HttpGet]
    public IActionResult ListarSinReservas(int dias = 30, int pagina = 1)
    {
        return Json(inmuebleRepo.ListSinReservasEnXDias(dias, pagina));
    }

    [Authorize]
    [HttpGet]
    public IActionResult ListarDisponiblesPorFechas(DateTime desde, DateTime hasta, int pagina = 1)
    {   
        return Json(inmuebleRepo.ListarDisponibles(desde, hasta, pagina));
    }




    [Authorize]
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