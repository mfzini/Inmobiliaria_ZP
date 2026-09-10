using inmobiliaria.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.CRUD;

namespace inmobiliaria.Repositories;

public class RepoConceptoPago(IConfiguration config) : RepositorioBase(config)
{
    public int Create(ConceptoPago concepto)
    {
        var query = "insert into ConceptoPago (nombre) values (@nombre); select last_insert_id()";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@nombre", concepto.Nombre);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public int Delete(ConceptoPago concepto)
    {
        var query = "delete from ConceptoPago where id = @id";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id", concepto.Id);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    public ConceptoPago FindByNombre(string nombre)
    {
        var query = "select * from ConceptoPago where nombre = @nombre";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@nobre", nombre);
        connection.Open();
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return null;
        return new ConceptoPago
        {
            Id = reader.GetInt32("id"),
            Nombre = reader.GetString("nombre")
        };
    }

    public List<ConceptoPago> FindAll()
    {
        List<ConceptoPago> conceptos = [];
        var query = "select * from ConceptoPago";
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
}