using inmobiliaria.Models;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;

namespace inmobiliaria.Repositories;

public class RepoPagos(IConfiguration config) : RepositorioBase(config)
{
    public void Create(HttpContext ctx, Pago pago)
    {
        var id = Guid.NewGuid().ToString();
        var query = "insert into Pagos (id, reserva, concepto, monto, fecha) values (@id, @reserva, @concepto, @monto, @fecha)";
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using var tx = connection.BeginTransaction();
        using MySqlCommand command = new(query, connection, tx);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@reserva", pago.Reserva.Id);
        command.Parameters.AddWithValue("@concepto", pago.Concepto.Id);
        command.Parameters.AddWithValue("@monto", pago.Monto);
        command.Parameters.AddWithValue("@fecha", pago.Fecha);
        try
        {
            command.ExecuteNonQuery();
            WriteLog(ctx, tx, $"creó {pago}");
            tx.Commit();
        } catch (Exception)
        {
            tx.Rollback();
            throw;
        }
    }
    public void Update(HttpContext ctx, Pago pago)
    {
        Pago original = FindById(pago.Id);
        var query = "update Pagos set concepto = @concepto, anulado = @anulado where id = @id";
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        using var tx = connection.BeginTransaction();
        using MySqlCommand command = new(query, connection, tx);
        command.Parameters.AddWithValue("@id", pago.Id);
        command.Parameters.AddWithValue("@concepto", pago.Concepto.Id);
        command.Parameters.AddWithValue("@anulado", pago.Anulado);
        try
        {
            command.ExecuteNonQuery();
            WriteLog(ctx, tx, $"actualizó {original} >> {pago}");
            tx.Commit();
        } catch (Exception)
        {
            tx.Rollback();
            throw;
        }
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

    public List<ConceptoPago> ListAllConceptos()
    {
        var conceptos = new List<ConceptoPago>();
        var query = @"select id, nombre from ConceptoPago order by nombre";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            conceptos.Add(new ConceptoPago
            {
                Id = reader.GetInt32("id"),
                Nombre = reader.GetString("nombre")
            });
        }
        return conceptos;
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