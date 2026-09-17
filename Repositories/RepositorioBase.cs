using System.Security.Claims;
using inmobiliaria.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;


namespace inmobiliaria.Repositories
{
	public abstract class RepositorioBase(IConfiguration configuration)
	{
		protected readonly IConfiguration configuration = configuration;
		protected readonly string? connectionString = configuration["ConnectionStrings:DefaultConnection"];

		protected void WriteLog(HttpContext ctx, MySqlTransaction tx, string entry)
		{
			var user_id = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
			var query = @"insert into Logs (dni, entry) values (@dni, @entry)";
			using MySqlCommand command = new(query, tx.Connection, tx);
			command.Parameters.AddWithValue("@dni", user_id);
			command.Parameters.AddWithValue("@entry", entry);
			command.ExecuteNonQuery();
		}

		public List<Log> GetLogs(int page = 1, int limit = 7)
		{
			List<Log> logs = [];
			var query = $@"select l.dni as dni, l.createdAt as l_created_at, l.entry, p.nombre, p.apellido
				from Logs l
				join Personas p on p.dni = l.dni
				order by l.createdAt DESC
				limit {(page - 1) * limit}, {limit}";

			using MySqlConnection connection = new(connectionString);
			using MySqlCommand command = new(query, connection);
			connection.Open();
			using MySqlDataReader reader = command.ExecuteReader();

			while (reader.Read())
			{
				logs.Add(new Log
				{
					Usuario = new Usuario
					{
						Dni = reader.GetString("dni"),
						Nombre = reader.GetString("nombre"),
						Apellido = reader.GetString("apellido")
					},
					Entry = reader.GetString("entry"),
					CreatedAt = reader.GetDateTime("l_created_at")
				});
			}
			return logs;
		}

		public List<Log> GetLogsByDni(string dni, int page = 1, int limit = 10)
		{
			List<Log> logs = [];
			var query = $@"select l.dni as dni, l.createdAt as l_created_at, l.entry, p.nombre, p.apellido
				from Logs l
				join Personas p on p.dni = l.dni
				where l.dni = @dni
				limit {(page - 1) * limit}, {limit}";

			using MySqlConnection connection = new(connectionString);
			using MySqlCommand command = new(query, connection);
			command.Parameters.AddWithValue("@dni", dni);
			connection.Open();
			using MySqlDataReader reader = command.ExecuteReader();

			while (reader.Read())
			{
				logs.Add(new Log
				{
					Usuario = new Usuario
					{
						Dni = reader.GetString("dni"),
						Nombre = reader.GetString("nombre"),
						Apellido = reader.GetString("apellido")
					},
					Entry = reader.GetString("entry"),
					CreatedAt = reader.GetDateTime("l_created_at")
				});
			}
			return logs;
		}
	}
}