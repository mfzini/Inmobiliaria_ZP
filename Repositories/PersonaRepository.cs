using inmobiliaria.Models;
using MySql.Data.MySqlClient;

namespace inmobiliaria.Repositories;

public class PersonaRepository(IConfiguration configuration) : RepositorioBase(configuration)
{
    public int Create(Persona persona)
    {
        var query = @"insert into Personas (dni, nombre, apellido, email, telefono)
            values (@dni, @nombre, @apellido, @email, @telefono)";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("dni", persona.Dni);
        command.Parameters.AddWithValue("nombre", persona.Nombre);
        command.Parameters.AddWithValue("apellido", persona.Apellido);
        command.Parameters.AddWithValue("email", persona.Email);
        command.Parameters.AddWithValue("telefono", persona.Telefono);
        connection.Open();
        return command.ExecuteNonQuery();
    }
}