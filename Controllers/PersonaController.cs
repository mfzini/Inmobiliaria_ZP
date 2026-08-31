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
                Response.StatusCode = 404;
                // todo.
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
        repo.Delete(persona);
        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public IActionResult Detalles(string dni)
    {
        if (string.IsNullOrEmpty(dni))
        {
            return RedirectToAction(nameof(Listar));
        }

        try
        {
            var persona = repo.FindByDni(dni);
            if(persona == null)
            {
                Response.StatusCode = 404;
                //todo
            }
            return View(persona);
        } catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return RedirectToAction(nameof(Listar));
        }

    }



}