using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers;

public class PersonaController(PersonaRepository repo) : Controller
{
    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

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

    [HttpGet]
    public IActionResult Listar()
    {
        return View(repo.ListAll());
    }

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

    [HttpGet]
    public IActionResult Eliminar(string id)
    {
        var persona = repo.FindByDni(id);
        if (persona == null)
        {
            return RedirectToAction(nameof(Listar));
        }
        return View(persona);
    }

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
            return RedirectToAction(nameof(Listar));    
        }
        catch (Exception e)
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
            var persona = repo.FindByDni(id);
            if(persona == null)
            {
                return NotFound();
            }
            return View(persona);
        } catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }

    }



}