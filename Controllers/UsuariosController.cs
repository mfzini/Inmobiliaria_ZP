using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;

namespace inmobiliaria.Controllers;

public class UsuarioController : Controller
{
    

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Editar(string id)
    {

        var empleadoPrueba = new
        {
            Id = "emp-1",
            Nombre = "Lucas",
            Apellido = "Rossi"
        };

        return View(empleadoPrueba);
    }

    [HttpGet]
    public IActionResult Listar()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Eliminar(string id)
    {
        var personaPrueba = new Persona
        {
            Dni ="12345678",
            Nombre = "Lucas",
            Apellido = "Rodriguez"
        };

        return View(personaPrueba);
    }


}
