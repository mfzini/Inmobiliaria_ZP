using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers;

public class TipoInmuebleController(InmuebleRepository repo) : Controller // todo: cambiar por tipoinmueble
{
    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Listar()
    {
        var listaTipos = new List<TipoInmueble>
        {
            new TipoInmueble { Id = 1, Nombre = "Casa" },
            new TipoInmueble { Id = 2, Nombre = "Departamento" },
            new TipoInmueble { Id = 3, Nombre = "PH" },
        };

        return View(listaTipos);
    }

    [HttpGet]
    public IActionResult Editar()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Eliminar()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Detalles()
    {   
        var tipo = new TipoInmueble
    {
        Id = 1,
        Nombre = "Casa"
    };

    return View(tipo);
}


}