using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using System.Runtime.InteropServices;
using inmobiliaria.Repositories;

namespace inmobiliaria.Controllers;

public class ImagenesController(ImagesRepo repoImages, [FromServices] IWebHostEnvironment environment) : Controller
{

    [HttpGet]
    public IActionResult TraerFotos(string id)
    {
        var inmueble = new Inmueble{Id=id};
        repoImages.Load(inmueble);
        return Ok(inmueble.Imagenes);
    }

    [HttpPost]
    public async Task<IActionResult> CambiarPortada(string id, IFormFile portadaFile)
    {
        if(portadaFile == null || portadaFile.Length == 0)
        {
            return BadRequest("tenes que seleccionar un archivo de portada");
        }
        var img = new Imagen
        {
            OriginalName = portadaFile.FileName,
            File = portadaFile
        };
        repoImages.UploadPortada(img, id);
        repoImages.GuardarPortada(id, img.Url);

        return Ok(new { url = img.Url});
    }

    [HttpPost]
    public async Task<IActionResult> Alta(string id, [FromForm]List<IFormFile> imagenes)
    {
        if(imagenes == null || imagenes.Count == 0)
        {
            return BadRequest("no se recibieron los archivos para la galeria");
        }
        foreach( var file in imagenes)
        {
            if (file.Length > 0)
            {
                var img = new Imagen
                {
                    OriginalName = file.FileName,
                    File = file
                };
                repoImages.UploadGaleria(img, id);
            }
        }
        var inmueble = new Inmueble {Id= id};
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
        var img = new Imagen {Id = id};
        repoImages.Delete(img);
        return Ok();
    }
}