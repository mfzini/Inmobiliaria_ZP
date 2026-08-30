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
        } else if (reserva.Inquilino == null || reserva.Inquilino.Dni == null)
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
        var res = command.ExecuteNonQuery();
        if (res == 1 )
        {
            reserva.Id = id;
        }
        return res;
    }
}