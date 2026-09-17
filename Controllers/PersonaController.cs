using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria.Controllers;

public class PersonaController(PersonaRepository repo, InmuebleRepository inmuebleRepo) : Controller
{
    [Authorize]
    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public IActionResult Registrar(Persona persona)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        
        if(repo.FindByDni(persona.Dni) != null)
        {
            ModelState.AddModelError("Dni", "Ya existe una persona con ese DNI");
            return View(persona);
        }

        repo.Create(persona);
        return RedirectToAction(nameof(Listar));

    }

    [Authorize]
    [HttpGet]
    public IActionResult Listar(int pagina = 1)
    {
        int tamano = 7;
        var lista = repo.ListAll(pagina, tamano);

        ViewBag.Pagina = pagina;
        ViewBag.HaySiguiente = lista.Count == tamano;

        return View(lista);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Editar(string id)
    {
        if (id == null)
        {
            return RedirectToAction(nameof(Listar));
        }
        try
        {
            var persona = repo.FindByDni(id);
            if (persona == null)
            {
                return NotFound();
            }
            return View(persona);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }
    }

    [Authorize]
    [HttpPost]
    public IActionResult Editar(Persona persona, string oldDni)
    {
        if (!ModelState.IsValid)
        {
            return View(persona);
        }

        if(persona.Dni != oldDni && repo.FindByDni(persona.Dni) != null)
        {
            ModelState.AddModelError("Dni", "Ese Dni que ingresaste ya lo tiene otra persona");
            return View(persona);
        }

        repo.Update(persona, oldDni);
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

        var persona = repo.FindByDni(id);
        if (persona == null)
        {
            return RedirectToAction(nameof(Listar));
        }
        return View(persona);
    }

    [Authorize(Policy = "Administrador")]
    [HttpPost]
    public IActionResult Eliminar(Persona persona)
    {

        if(string.IsNullOrEmpty(persona.Dni))
        {
            return RedirectToAction(nameof(Listar));
        }

        try
        {
            repo.Delete(persona);
            TempData["Mensaje"] = "Persona eliminada correctamente"; 
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            TempData["Error"] = "No podes eliminar esta persona porque tiene otros registros asociados";
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
            var persona = repo.FindByDni(id);
            if(persona == null)
            {
                return NotFound();
            }
            persona.Inmuebles = inmuebleRepo.FindByPropietarioDni(persona.Dni); 
            return View(persona);
        } catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }

    }

    [Authorize]
    [HttpGet]
    public IActionResult BuscarNombre(string nombreBuscado)
    {
        try
        {
            return Json(repo.FindByNombre(nombreBuscado));
        } catch(Exception e)
        {
            return Json(new {Error = e.Message});
        }
    }




}