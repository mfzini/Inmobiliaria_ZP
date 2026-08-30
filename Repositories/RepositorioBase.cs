using Microsoft.Extensions.Configuration;


namespace inmobiliaria.Repositories
{
	public abstract class RepositorioBase(IConfiguration configuration)
    {
		protected readonly IConfiguration configuration = configuration;
		protected readonly string? connectionString = configuration["ConnectionStrings:DefaultConnection"];
    }
}