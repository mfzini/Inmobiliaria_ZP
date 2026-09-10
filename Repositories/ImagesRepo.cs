using inmobiliaria.Controllers;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace inmobiliaria.Repositories;

public class ImagesRepo(IConfiguration config, [FromServices] IWebHostEnvironment environment, InmuebleRepository repoInmueble) : RepositorioBase(config)
{

    public int Upload(Imagen img, Inmueble inmueble)
    {
        img.Id = Guid.NewGuid().ToString();
        img.Url = $"/Uploads/Inmuebles/{inmueble.Id}/{img.Id}{Path.GetExtension(img.File!.FileName)}";
        string uploadPath = Path.Combine(environment.WebRootPath, "Uploads", "Inmuebles", inmueble.Id);
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }
        var filePath = Path.Combine(uploadPath, $"{img.Id}{Path.GetExtension(img.File!.FileName)}");
        using var stream = new FileStream(filePath, FileMode.Create);
        img.File.CopyTo(stream);
        return Create(img, inmueble);
    }

    private int Create(Imagen img, Inmueble inmueble)
    {
        var query = @"insert into ImagenesInmuebles (id, inmueble, original_name, location, is_portada) values (
            @id, @inmueble, @original_name, @location, @is_portada)";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", img.Id);
        command.Parameters.AddWithValue("@inmueble", inmueble.Id);
        command.Parameters.AddWithValue("@original_name", img.OriginalName);
        command.Parameters.AddWithValue("@location", img.Url);
        command.Parameters.AddWithValue("@is_portada", img.IsPortada);
        connection.Open();
        if (command.ExecuteNonQuery() == 0)
        {
            return 0;
        }
        
        if (img.IsPortada)
        {
            inmueble.Portada = img;
            repoInmueble.Update(inmueble);
        }
        return 1;
    }

    public void Load(Inmueble inmueble)
    {
        FetchGaleria(inmueble);
        FetchPortada(inmueble);
    }

    public void Delete(Imagen img)
    {
        var query = @"delete from ImagenesInmuebles where id = @id";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", img.Id);
        connection.Open();
        command.ExecuteNonQuery();
    }

    private void FetchGaleria(Inmueble inmueble)
    {
        List<Imagen> images = [];

        var query = @"select * from ImagenesInmuebles where inmueble = @inmueble and is_portada = 0";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@inmueble", inmueble?.Id);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            images.Add(new Imagen
            {
                Id = reader.GetString("id"),
                OriginalName = reader.GetString("original_name"),
                Url = reader.GetString("location")
            });
        }
        inmueble.Imagenes = images;
    }
    private void FetchPortada(Inmueble inmueble)
    {
        var query = @"select * from ImagenesInmuebles where inmueble = @inmueble and is_portada = 1";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@inmueble", inmueble?.Id);
        connection.Open();
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            var portada = new Imagen
            {
                Id = reader.GetString("id"),
                OriginalName = reader.GetString("original_name"),
                Url = reader.GetString("location"),
                IsPortada = true
            };
            inmueble.Portada = portada;
        }
    }
}