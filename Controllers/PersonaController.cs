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
        repo.Create(persona);
        return RedirectToAction();
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
            return Redirect("/Persona/Listar");
        }
        try
        {
            var dni = int.Parse(id);
            var persona = repo.FindByDni(dni);
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
            return Redirect("/Persona/Listar");
        }
    }
    [HttpPost]
    public IActionResult Editar(Persona persona)
    {
        repo.Update(persona);
        return Redirect("/Persona/Listar");
    }

    [HttpGet]
    public IActionResult Eliminar(int id)
    {
        var persona = repo.FindByDni(id);
        if(persona == null)
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

    

}