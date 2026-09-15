using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Claim = System.Security.Claims.Claim;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace inmobiliaria.Controllers;

public class UsuarioController(UsuariosRepo repoUsuarios, PersonaRepository personasRepo) : Controller
{
    

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        
        var usuario = repoUsuarios.FindByEmail(email);

        if (usuario == null || usuario.Password != password)
        {
            ViewBag.Error = "Email o contraseña incorrectos.";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Dni),
            new Claim(ClaimTypes.Name, usuario.Email),
            new Claim("FullName", $"{usuario.Nombre} {usuario.Apellido}"),
            new Claim(ClaimTypes.Role, usuario.Role)
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, 
            CookieAuthenticationDefaults.AuthenticationScheme,
            ClaimTypes.NameIdentifier, ClaimTypes.Role);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity)
        );

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Usuario");
    }


    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registrar(Usuario usuario, IFormFile? avatarFile)
    {

        var usuarioExistente = repoUsuarios.FindByDni(usuario.Dni!);
        if (usuarioExistente != null)
        {
            ModelState.AddModelError("Dni", "Ya existe un usuario con este DNI.");
            return View(usuario);
        }


        var personaExistente = personasRepo.FindByDni(usuario.Dni!);
        if (personaExistente == null)
        {
            var nuevaPersona = new Persona
            {
                Dni = usuario.Dni!,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Telefono = usuario.Telefono ?? "",
                Email = usuario.Email
            };
            personasRepo.Create(nuevaPersona);
            TempData["Mensaje"] = "Se creo la persona y el usuario con exito";
        }
        else
        {
            TempData["Mensaje"] = "Esa persona ya existia, se le asigno un usuario y rol";
        }

        repoUsuarios.Create(usuario);

        if (avatarFile != null && avatarFile.Length > 0)
        {
            var img = new Imagen { File = avatarFile };
            repoUsuarios.UploadAvatar(img, usuario);
        }

        return RedirectToAction(nameof(Listar)); 
    }

    [HttpGet]
    public IActionResult BuscarPersona(string dni)
    {
        var persona = personasRepo.FindByDni(dni);
        if (persona == null) return NotFound();

        return Json(new {
            nombre = persona.Nombre,
            apellido = persona.Apellido,
            telefono = persona.Telefono,
            email = persona.Email
        });
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
        TempData["Mensaje"] = "Ese USUARIO se elimino del sistema, la persona sigue estando cargada";
        return RedirectToAction(nameof(Listar));
    }


}
