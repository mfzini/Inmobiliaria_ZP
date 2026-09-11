using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using System.Runtime.InteropServices;
using inmobiliaria.Repositories;

namespace inmobiliaria.Controllers;

public class ImagenesController(ImagesRepo repoImages, [FromServices] IWebHostEnvironment environment, InmuebleRepository repoInmueble) : Controller
{

    [HttpGet]
    public IActionResult TraerFotos(string id)
    {
        var inmueble = repoInmueble.GetById(id);
        repoImages.Load(inmueble);
        return Ok(new { inmueble.Imagenes, inmueble.Portada});
    }

    [HttpPost]
    public async Task<IActionResult> CambiarPortada(string id, IFormFile portadaFile)
    {
        var inmueble = repoInmueble.GetById(id);

        if (portadaFile == null || portadaFile.Length == 0)
        {
            return BadRequest("tenes que seleccionar un archivo de portada");
        }
        var img = new Imagen
        {
            OriginalName = portadaFile.FileName,
            File = portadaFile,
            IsPortada = true
        };

        repoImages.Upload(img, inmueble);

        return Ok(new { url = img.Url });
    }

    [HttpPost]
    public async Task<IActionResult> Alta(string id, [FromForm] List<IFormFile> imagenes)
    {
        if (imagenes == null || imagenes.Count == 0)
        {
            return BadRequest("no se recibieron los archivos para la galeria");
        }
        var inmueble = repoInmueble.GetById(id);
        foreach (var file in imagenes)
        {
            if (file.Length > 0)
            {
                var img = new Imagen
                {
                    OriginalName = file.FileName,
                    File = file
                };
                repoImages.Upload(img, inmueble);
            }
        }

        repoImages.Load(inmueble);
        return Ok(inmueble.Imagenes);
    }

    [HttpPost]
    public IActionResult Eliminar(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("el id de la imagen es obligatorio");
        }
        var img = new Imagen { Id = id };
        repoImages.Delete(img);
        return Ok();
    }
}