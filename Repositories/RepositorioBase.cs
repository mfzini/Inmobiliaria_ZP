using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;


namespace inmobiliaria.Repositories
{
	public abstract class RepositorioBase(IConfiguration configuration)
	{
		protected readonly IConfiguration configuration = configuration;
		protected readonly string? connectionString = configuration["ConnectionStrings:DefaultConnection"];

		protected void Log(HttpContext ctx, MySqlTransaction tx, string action)
		{
			var user_id = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
			var query = @"insert into Logs (user_id, action) values (@dni, @action)";
			using MySqlCommand command = new(query, tx.Connection, tx);
			command.Parameters.AddWithValue("@dni", user_id);
			command.Parameters.AddWithValue("@action", action);
			command.ExecuteNonQuery();
		}
	}
}