using inmobiliaria.Models;
using MySql.Data.MySqlClient;

namespace inmobiliaria.Repositories;

public class ReservaRepo(IConfiguration configuration) : RepositorioBase(configuration)
{
    public int Create(Reserva reserva)
    {
        if (reserva.Inmueble == null || reserva.Inmueble.Id == null)
        {
            throw new InvalidDataException("Falta setear Inmueble");
        }
        else if (reserva.Inquilino == null || reserva.Inquilino.Dni == null)
        {
            throw new InvalidDataException("Falta setear Inquilino");
        }
        var id = Guid.NewGuid().ToString();
        var query = @"insert into Reservas (id, inmueble, inquilino, fecha_inicio, fecha_fin) values
            (@id, @inmueble, @inquilino, @fecha_inicio, @fecha_fin)";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@inmueble", reserva.Inmueble.Id);
        command.Parameters.AddWithValue("@inquilino", reserva.Inquilino.Dni);
        command.Parameters.AddWithValue("@fecha_inicio", reserva.FechaInicio.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("@fecha_fin", reserva.FechaFin.ToString("yyyy-MM-dd"));
        connection.Open();
        var res = command.ExecuteNonQuery();
        if (res == 1)
        {
            reserva.Id = id;
        }
        return res;
    }

    public int Update(Reserva reserva)
    {
        var query = @"update Reservas set fecha_inicio=@fecha_inicio, fecha_fin=@fecha_fin where id=@id";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@fecha_inicio", reserva.FechaInicio);
        command.Parameters.AddWithValue("@fecha_fin", reserva.FechaFin);
        command.Parameters.AddWithValue("@id", reserva.Id);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    public int Delete(Reserva reserva)
    {
        var query = @"delete from Reservas where id=@id";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    public Reserva? FindByID(string id)
    {
        var query = @"select *, r.id as r_id, i.id as i_id, p.nombre as p_nombre, t.nombre as t_nombre
            from Reservas r
            join Personas p on p.dni = r.inquilino
            join Inmuebles i on i.id = r.inmueble
            join TipoInmueble t on t.id = i.tipo
            where r.id = @id";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", id);
        connection.Open();
        using MySqlDataReader reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        return ParseReserva(reader);
    }

    public List<Reserva> GetPage(int page = 1, int limit = 10)
    {
        List<Reserva> reservas = [];
        var query = $@"select *, r.id as r_id, i.id as i_id, p.nombre as p_nombre, t.nombre as t_nombre
            from Reservas r
            join Personas p on p.dni = r.inquilino
            join Inmuebles i on i.id = r.inmueble
            join TipoInmueble t on t.id = i.tipo
            where fecha_fin < now()
            order by fecha_inicio
            limit {(page - 1) * limit}, {limit}";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        connection.Open();
        using MySqlDataReader reader = command.ExecuteReader();
        while(reader.Read())
        {
            reservas.Add(ParseReserva(reader));
        }
        return reservas;
    }

    private static Reserva ParseReserva(MySqlDataReader reader)
    {
        var nombre = reader.GetString("p_nombre");
        var telefono = reader["telefono"] as string;
        var inquilino = new Inquilino
        {
            Dni = reader.GetString("inquilino"),
            Nombre = nombre,
            Telefono = telefono
        };

        var t_nombre = reader.GetString("t_nombre");
        var tipo = new TipoInmueble { Nombre = t_nombre };
        var direccion = reader.GetString("direccion");
        var inmueble = new Inmueble
        {
            Id = reader.GetString("inmueble"),
            Direccion = direccion,
            Tipo = tipo
        };

        var id = reader.GetString("r_id");
        var fecha_inicio = reader.GetDateTime("fecha_inicio");
        var fecha_fin = reader.GetDateTime("fecha_fin");
        return new Reserva
        {
            Id = id,
            Inmueble = inmueble,
            Inquilino = inquilino,
            FechaInicio = fecha_inicio,
            FechaFin = fecha_fin
        };

    }
}