using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using System.Runtime.InteropServices;

namespace inmobiliaria.Controllers;

    public class MockImagen // modelo de prueba
    {
        public int Id { get; set; }
		public int InmuebleId { get; set; }
		public string Url { get; set; } = "";
    }   

public class ImagenesController : Controller
{
    
    private static List<MockImagen> _imagenesBD = new List<MockImagen>(); // static para que los datos se mantengan
    private static int _contadorId =1;  // lo puse para simular el autoincremental

    [HttpPost]
    public async Task<IActionResult> CambiarPortada(int id, IFormFile portadaFile, [FromServices] IWebHostEnvironment environment)
    {
        if(portadaFile == null || portadaFile.Length == 0)
        {
            return BadRequest("tenes que seleccionar un archivo de portada");
        }
        string wwwPath = environment.WebRootPath;
        string rutaCarpeta = Path.Combine(wwwPath, "Uploads", "Inmuebles");
        if (!Directory.Exists(rutaCarpeta))
        {
            Directory.CreateDirectory(rutaCarpeta);
        }
        
        var extension = Path.GetExtension(portadaFile.FileName);
        var nombreArchivo = $"portada_{id}_{Guid.NewGuid()}{extension}";
        var rutaFisica = Path.Combine(rutaCarpeta, nombreArchivo);
        
        using( var stream = new FileStream(rutaFisica, FileMode.Create))
        {
            await portadaFile.CopyToAsync(stream);
        }

        string urlRelativa = $"/Uploads/Inmuebles/{nombreArchivo}";

        return Ok(new { url = urlRelativa});
    }

    [HttpPost]
    public async Task<IActionResult> Alta(int id, List<IFormFile> imagenes, [FromServices] IWebHostEnvironment environment)
    {
        if(imagenes == null || imagenes.Count == 0)
        {
            return BadRequest("no se recibieron los archivos para la galeria");
        }
        string rutaCarpeta = Path.Combine(environment.WebRootPath, "Uploads", "Inmuebles", id.ToString());
        if (!Directory.Exists(rutaCarpeta))
        {
            Directory.CreateDirectory(rutaCarpeta);
        }
        foreach( var file in imagenes)
        {
            if (file.Length > 0)
            {
                var extension = Path.GetExtension(file.FileName);
                var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                var rutaFisica = Path.Combine(rutaCarpeta, nombreArchivo);

                using (var stream = new FileStream(rutaFisica, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _imagenesBD.Add(new MockImagen
                {
                    Id = _contadorId++,
                    InmuebleId = id,
                    Url = $"/Uploads/Inmuebles/{id}/{nombreArchivo}"    
                });
            }
        }

        List<MockImagen> galeria = new List<MockImagen>();

        foreach( var img in _imagenesBD)
        {
            if(img.InmuebleId == id)
            {
                galeria.Add(img);
            }
        }

        return Ok(galeria);
    }


    [HttpPost]
    public IActionResult Eliminar(int id, [FromServices] IWebHostEnvironment environment)
    {
        MockImagen fotoAEliminar = null;
        foreach(var img in _imagenesBD)
        {
            if(img.Id == id)
            {
                fotoAEliminar = img;
                break;
            }
        }

        if(fotoAEliminar == null)
        {
            return NotFound("esa foto no existe.");
        }
        
        var rutaRelativaLimpia = fotoAEliminar.Url.TrimStart('/');
        var rutaFisica = Path.Combine(environment.WebRootPath, rutaRelativaLimpia);

        if (System.IO.File.Exists(rutaFisica))
        {
            System.IO.File.Delete(rutaFisica);
        }

        int inmuebleId = fotoAEliminar.InmuebleId;
        _imagenesBD.Remove(fotoAEliminar);

        List<MockImagen> galeriaRestante = new List<MockImagen>();
        foreach(var img in _imagenesBD)
        {
            if(img.InmuebleId == inmuebleId)
            {
                galeriaRestante.Add(img);
            }
        }

        return Ok(galeriaRestante);
    }

}
