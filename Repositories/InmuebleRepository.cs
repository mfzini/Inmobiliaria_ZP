using inmobiliaria.Models;
using MySql.Data.MySqlClient;

namespace inmobiliaria.Repositories;

public class InmuebleRepository(IConfiguration configuration) : RepositorioBase(configuration)
{
    public int Create(Inmueble inmueble)
    {
        var id = Guid.NewGuid().ToString();
        var query = @"insert into Inmuebles (id, propietario, direccion, latitud, longitud, tipo, capacidad, precio, listado)
        values (@id, @propietario, @direccion, @latitud, @longitud, @tipo, @capacidad, @precio, @listado)";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@propietario", inmueble.Propietario.Dni);
        command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
        command.Parameters.AddWithValue("@latitud", inmueble.Latitud);
        command.Parameters.AddWithValue("@longitud", inmueble.Longitud);
        command.Parameters.AddWithValue("@tipo", inmueble.Tipo.Id);
        command.Parameters.AddWithValue("@capacidad", inmueble.Capacidad);
        command.Parameters.AddWithValue("@precio", inmueble.Precio);
        command.Parameters.AddWithValue("@listado", inmueble.Listado);
        connection.Open();
        var r = command.ExecuteNonQuery();
        inmueble.Id = id;
        return r;
    }

    public int Delete(Inmueble inmbueble)
    {
        var query = @"delete from Inmuebles where id=@id";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", inmbueble.Id);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    public int Update(Inmueble inmueble)
    {
        var query = @"update Inmuebles set
            propietario=@propietario,
            direccion=@direccion,
            latitud=@latitud,
            longitud=@longitud,
            tipo=@tipo,
            capacidad=@capacidad,
            precio=@precio,
            listado=@listado
            where id=@id";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@propietario", inmueble.Propietario.Dni);
        command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
        command.Parameters.AddWithValue("@latitud", inmueble.Latitud);
        command.Parameters.AddWithValue("@longitud", inmueble.Longitud);
        command.Parameters.AddWithValue("@tipo", inmueble.Tipo.Id);
        command.Parameters.AddWithValue("@capacidad", inmueble.Capacidad);
        command.Parameters.AddWithValue("@precio", inmueble.Precio);
        command.Parameters.AddWithValue("@listado", inmueble.Listado);
        command.Parameters.AddWithValue("@id", inmueble.Id);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    public List<Inmueble> FindByPropietarioDni(string dni)
    {
        List<Inmueble> inmuebles = [];
        var query = @"select *, i.id as i_id, p.nombre as p_nombre, t.id as t_id, t.nombre as t_nombre from Inmuebles i
            join Personas p on i.propietario = p.dni
            join TipoInmueble t on i.tipo = t.id
            where i.propietario = @dni";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@dni", dni);
        connection.Open();
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            inmuebles.Add(ParseInmueble(reader));
        }
        return inmuebles;
    }

    public List<Inmueble> FindByListingStatus(bool listado, int page = 1, int limit = 10)
    {
        List<Inmueble> inmuebles = [];
        var query = $@"select *, i.id as i_id, p.nombre as p_nombre, t.id as t_id, t.nombre as t_nombre from Inmuebles i
            join Personas p on i.propietario = p.dni
            join TipoInmueble t on i.tipo = t.id
            where i.listado = @listado
            order by i.precio
            limit {(page - 1) * limit}, {limit}";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@listado", listado);
        connection.Open();
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            inmuebles.Add(ParseInmueble(reader));
        }
        return inmuebles;
    }
    public List<Inmueble> FindByPriceRange(decimal start, decimal end, int page = 1, int limit = 10)
    {
        List<Inmueble> inmuebles = [];
        var query = $@"select *, i.id as i_id, p.nombre as p_nombre, t.id as t_id, t.nombre as t_nombre  from Inmuebles i
            join Personas p on p.dni = i.propietario
            join TipoInmueble t on i.tipo = t.id
            where i.precio between @start and @end
            order by i.precio
            limit {(page - 1) * limit}, {limit}";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@start", start);
        command.Parameters.AddWithValue("@end", end);
        connection.Open();
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            inmuebles.Add(ParseInmueble(reader));
        }
        return inmuebles;
    }

    public List<Inmueble> GetPage(int page = 1, int limit = 10)
    {
        List<Inmueble> inmuebles = [];
        var query = $@"select *, i.id as i_id, p.nombre as p_nombre, t.id as t_id, t.nombre as t_nombre from Inmuebles i
            join Personas p on i.propietario = p.dni
            join TipoInmueble t on i.tipo = t.id
            order by i.precio
            limit {(page - 1) * limit}, {limit}";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        connection.Open();
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            inmuebles.Add(ParseInmueble(reader));
        }
        return inmuebles;
    }

    public List<Inmueble> FindByCapacity(int cap = 1, int page = 1, int limit = 10)
    {
        List<Inmueble> inmuebles = [];

        var query = $@"select *, i.id as i_id, p.nombre as p_nombre, t.id as t_id, t.nombre as t_nombre from Inmuebles i
            join Personas p on i.propietario = p.dni
            join TipoInmueble t on i.tipo = t.id
            where i.capacidad >= @cap
            order by i.capacidad
            limit {(page - 1) * limit}, {limit}";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@cap", cap);
        connection.Open();
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            inmuebles.Add(ParseInmueble(reader));
        }
        return inmuebles;
    }

    public List<Inmueble> FindByType(string tipo, int page = 1, int limit = 10)
    {
        List<Inmueble> inmuebles = [];

        var query = $@"select *, i.id as i_id, p.nombre as p_nombre, t.id as t_id, t.nombre as t_nombre
            from Inmuebles i
            join Personas p on p.dni = i.propietario
            join TipoInmueble t on t.id = i.tipo
            where t.nombre = @tipo
            order by i.capacidad
            limit {(page - 1) * limit}, {limit}";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@tipo", tipo);

        connection.Open();
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            inmuebles.Add(ParseInmueble(reader));
        }
        return inmuebles;
    }


    private static Inmueble ParseInmueble(MySqlDataReader reader)
    {
        return new Inmueble
        {
            Id = reader.GetString("i_id"),
            Propietario = new Propietario
            {
                Dni = reader.GetString("dni"),
                Nombre = reader.GetString("p_nombre"),
                Apellido = reader.GetString("apellido"),
                Telefono = reader["telefono"] as string,
                Email = reader.GetString("email")
            },
            Direccion = reader.GetString("direccion"),
            Latitud = reader.GetDecimal("latitud"),
            Longitud = reader.GetDecimal("longitud"),
            Tipo = new TipoInmueble
            {
                Id = reader.GetInt32("t_id"),
                Nombre = reader.GetString("t_nombre")
            },
            Capacidad = reader.GetInt32("capacidad"),
            Precio = reader.GetDecimal("precio"),
            Listado = reader.GetBoolean("listado")
        };
    }

    public Inmueble? GetById(string id)
    {
        var query = @"select *, i.id as i_id, p.nombre as p_nombre, t.id as t_id, t.nombre as t_nombre from Inmuebles i
        join Personas p on p.dni = propietario
        join TipoInmueble t on t.id = i.tipo
        where i.id = @id";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", id);
        connection.Open();
        using MySqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return ParseInmueble(reader);
        }

        return null;
    }
}
