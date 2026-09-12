using inmobiliaria.Models;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;

namespace inmobiliaria.Repositories;

public class RepoPagos(IConfiguration config) : RepositorioBase(config)
{
    public int Create(Pago pago)
    {
        var id = Guid.NewGuid().ToString();
        var query = "insert into Pagos (id, reserva, concepto, monto, fecha) values (@id, @reserva, @concepto, @monto, @fecha)";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@reserva", pago.Reserva.Id);
        command.Parameters.AddWithValue("@concepto", pago.Concepto.Id);
        command.Parameters.AddWithValue("@monto", pago.Monto);
        command.Parameters.AddWithValue("@fecha", pago.Fecha);
        connection.Open();
        return command.ExecuteNonQuery();
    }
    public int Update(Pago pago)
    {
        var query = "update Pagos set concepto = @concepto, anulado = @anulado where id = @id";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", pago.Id);
        command.Parameters.AddWithValue("@concepto", pago.Concepto.Id);
        command.Parameters.AddWithValue("@anulado", pago.Anulado);
        connection.Open();
        return command.ExecuteNonQuery();
    }
    public Pago FindById(string id)
    {
        var query = @"select *, p.id as p_id, p.reserva as p_reserva, c.id as c_id, c.nombre as c_nombre
            from Pagos p
            join ConceptoPago c on c.id = p.concepto
            where p.id = @id";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", id);
        connection.Open();
        using var reader = command.ExecuteReader();

        if (!reader.Read()) return null;
        return new Pago
        {
            Id = reader.GetString("p_id"),
            Reserva = new Reserva
            {
                Id = reader.GetString("p_reserva")
            },
            Concepto = new ConceptoPago
            {
                Id = reader.GetInt32("c_id"),
                Nombre = reader.GetString("nombre")
            },
            Monto = reader.GetDecimal("monto"),
            Fecha = reader.GetDateTime("fecha"),
            Anulado = reader.GetBoolean("anulado")
        };

    }

    public List<Pago> FindByReserva(Reserva reserva)
    {
        List<Pago> pagos = [];
        var query = @"select p.*, p.id as p_id, c.id as c_id, c.nombre as c_nombre
            from Pagos p
            join Reservas r on r.id = p.reserva
            join ConceptoPago c on c.id = p.concepto
            where reserva = @reserva";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@reserva", reserva.Id);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            pagos.Add(new Pago
            {
                Id = reader.GetString("p_id"),
                Concepto = new ConceptoPago
                {
                    Id = reader.GetInt32("c_id"),
                    Nombre = reader.GetString("c_nombre")
                },
                Monto = reader.GetDecimal("monto"),
                Fecha = reader.GetDateTime("fecha"),
                Anulado = reader.GetBoolean("anulado")
            });
        }
        return pagos;
    }
}