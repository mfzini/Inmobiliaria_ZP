using inmobiliaria.Models;
using MySql.Data.MySqlClient;

namespace inmobiliaria.Repositories;

public class TipoInmuebleRepo(IConfiguration config) : RepositorioBase(config)
{
    public List<TipoInmueble> ListAll()
    {
        List<TipoInmueble> tipos = [];
        var query = "select * from TipoInmueble order by nombre";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        connection.Open();
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            tipos.Add(new TipoInmueble
            {
                Id = reader.GetInt32("id"),
                Nombre = reader.GetString("nombre")
            });
        }
        return tipos;
    }

    public void CreateTipoInmueble(TipoInmueble tipo)
    {
        var query = "insert into TipoInmueble (nombre) values (@nombre)";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@nombre", tipo.Nombre);
        connection.Open();
        command.ExecuteNonQuery();
        tipo.Id = Convert.ToInt32(command.LastInsertedId);
    }
    public int DeleteTipoInmueble(TipoInmueble tipo)
    {
        var query = "delete from TipoInmueble where id = @id";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", tipo.Id);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    public TipoInmueble? FindTipoByID(int id)
    {
        var query = @"select * from TipoInmueble where id = @id";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", id);
        connection.Open();
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return null;
        return new TipoInmueble
        {
            Id = reader.GetInt16("id"),
            Nombre = reader.GetString("nombre")
        };

    }
}