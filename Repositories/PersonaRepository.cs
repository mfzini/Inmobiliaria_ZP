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
    public Persona? FindByDni(int dni)
    {
        var query = @"select * from Personas where dni = @dni";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("dni", dni);
        connection.Open();
        var reader = command.ExecuteReader();
        if (!reader.HasRows) return null;
        reader.Read();
        var persona = new Persona
        {
            Dni = reader.GetInt32("dni"),
            Nombre = reader.GetString("nombre"),
            Apellido = reader.GetString("apellido"),
            Telefono = reader.GetString("telefono"),
            Email = reader.GetString("email")
        };
        return persona;
    }

    public int Update(Persona persona)
    {
        var query = @"update Personas set nombre=@nombre, apellido=@apellido, email=@email, telefono=@telefono where dni = @dni";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@dni", persona.Dni);
        command.Parameters.AddWithValue("@nombre", persona.Nombre);
        command.Parameters.AddWithValue("@apellido", persona.Apellido);
        command.Parameters.AddWithValue("@email", persona.Email);
        command.Parameters.AddWithValue("@telefono", persona.Telefono);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    public int Delete(Persona persona)
    {
        var query = @"delete from Personas where dni = @dni";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("dni", persona.Dni);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    public List<Persona> ListAll()
    {
        var query = @"select * from Personas";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        connection.Open();
        var reader = command.ExecuteReader();
        List<Persona> personas = [];
        while (reader.Read())
        {
            personas.Add(new Persona
            {
                Dni = reader.GetInt32("dni"),
                Nombre = reader.GetString("nombre"),
                Apellido = reader.GetString("apellido"),
                Telefono = reader.GetString("telefono"),
                Email = reader.GetString("email")
            });
        }
        return personas;
    }

    public List<Persona> ListPropietarios()
    {
        List<Persona> propietarios = [];
        var query = @"select distinct dni, nombre, apellido, email, telefono from Personas join Inmuebles on dni = propietario";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        connection.Open();
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            propietarios.Add(new Inquilino
            {
                Dni = reader.GetInt32(nameof(Inquilino.Dni)),
                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                Email = reader.GetString(nameof(Inquilino.Email))
            });
        }
        return propietarios;
    }
    public List<Persona> ListInquilinos()
    {
        List<Persona> inquilinos = [];
        var query = @"select distinct dni, nombre, apellido, email, telefono from Personas join Reservas on dni = inquilino";
        MySqlConnection connection = new(connectionString);
        MySqlCommand command = new(query, connection);
        connection.Open();
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            inquilinos.Add(new Inquilino
            {
                Dni = reader.GetInt32(nameof(Inquilino.Dni)),
                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                Email = reader.GetString(nameof(Inquilino.Email))
            });
        }
        return inquilinos;
    }
}