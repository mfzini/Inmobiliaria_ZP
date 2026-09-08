using inmobiliaria.Controllers;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace inmobiliaria.Repositories;

public class ImagesRepo(IConfiguration config, [FromServices] IWebHostEnvironment environment) : RepositorioBase(config)
{
    public int Upload(Imagen img, string inmuebleId)
    {
        img.Id = Guid.NewGuid().ToString();
        img.Url = $"/Uploads/Inmuebles/{inmuebleId}/{img.Id}";
        string uploadPath = Path.Combine(environment.WebRootPath, "Uploads", "Inmuebles", inmuebleId);
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }
        var filePath = Path.Combine(uploadPath, $"{img.Id}{Path.GetExtension(img.File!.FileName)}");
        using var stream = new FileStream(filePath, FileMode.Create);
        img.File.CopyTo(stream);
        return Create(img, inmuebleId);
    }

    private int Create(Imagen img, string inmuebleId)
    {
        var query = @"insert into ImagenesInmuebles (id, inmueble, original_name, location) values (
            @id, @inmueble, @original_name, @location)";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", img.Id);
        command.Parameters.AddWithValue("@inmueble", inmuebleId);
        command.Parameters.AddWithValue("@original_name", img.OriginalName);
        command.Parameters.AddWithValue("@location", img.Url);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    public void Load(Inmueble inmueble)
    {
        List<Imagen> images = [];
        var query = @"select * from ImagenesInmuebles where inmueble = @inmueble";
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
        inmueble!.Imagenes = images;
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
}