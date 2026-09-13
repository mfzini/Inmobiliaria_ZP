using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;

namespace inmobiliaria.Controllers;

public class UsuarioController(UsuariosRepo repoUsuarios) : Controller
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

    [HttpPost]
    public IActionResult Registrar(Usuario usuario, IFormFile? avatarFile)
    {
        repoUsuarios.Create(usuario);

        if (avatarFile != null && avatarFile.Length > 0)
        {
            var img = new Imagen { File = avatarFile };
            repoUsuarios.UploadAvatar(img, usuario);
        }

        return RedirectToAction(nameof(Listar)); 
    }

    [HttpGet]
    public IActionResult Editar(string id)
    {

        var usuario = repoUsuarios.FindByDni(id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }

    [HttpPost]
    public IActionResult Editar(Usuario usuario, IFormFile? avatarFile)
    {
        var actual = repoUsuarios.FindByDni(usuario.Dni!);
        usuario.Avatar = actual?.Avatar;

        if (avatarFile != null)
        {
            repoUsuarios.UploadAvatar(new Imagen { File = avatarFile }, usuario);
        }

        repoUsuarios.Update(usuario);
        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public IActionResult Listar()
    {
        var usuarios = repoUsuarios.ListAll();
        return View(usuarios);
    }

    [HttpGet]
    public IActionResult Eliminar(string id)
    {
        var usuario = repoUsuarios.FindByDni(id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }

    [HttpPost]
    public IActionResult Eliminar(Usuario usuario)
    {
        repoUsuarios.Delete(usuario.Dni);
        return RedirectToAction(nameof(Listar));
    }


}
