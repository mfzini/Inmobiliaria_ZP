using inmobiliaria.Models;
using MySql.Data.MySqlClient;

namespace inmobiliaria.Repositories;

public class UsuariosRepo(IConfiguration config, IWebHostEnvironment environment) : RepositorioBase(config)
{

    public List<Usuario> ListAll(int page = 1, int limit = 7)
    {
        List<Usuario> usuarios = [];
        var query = $@"select u.dni, p.nombre, p.apellido, p.telefono, p.email, u.password, u.role, u.avatar 
                    from Usuarios u
                    inner join Personas p on u.dni = p.dni
                    order by p.apellido, p.nombre
                    limit {(page - 1) * limit}, {limit}";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        connection.Open();
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            usuarios.Add(ParseUsuario(reader));
        }
        return usuarios;
    }

    public Usuario? FindByEmail(string email)
    {
        var query = @"select u.dni, p.nombre, p.apellido, p.telefono, p.email, u.password, u.role, u.avatar 
                    from Usuarios u
                    join Personas p on u.dni = p.dni
                    where p.email = @email";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@email", email);
        connection.Open();
        var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;      
        } 
        
        return ParseUsuario(reader);
    }

    public Usuario? FindByDni(string dni)
    {
        var query = @"select u.dni, p.nombre, p.apellido, p.telefono, p.email, u.password, u.role, u.avatar 
                    from Usuarios u
                    inner join Personas p on u.dni = p.dni
                    where u.dni = @dni";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@dni", dni);
        connection.Open();
        var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        return ParseUsuario(reader);
    }

    public int Create(Usuario usuario)
    {
        var query = @"insert into Usuarios (dni, password, role, avatar) 
                    values (@dni, @password, @role, @avatar)";
        
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@dni", usuario.Dni);
        command.Parameters.AddWithValue("@nombre", usuario.Nombre);
        command.Parameters.AddWithValue("@apellido", usuario.Apellido);
        command.Parameters.AddWithValue("@telefono", usuario.Telefono ?? "");
        command.Parameters.AddWithValue("@email", usuario.Email);
        command.Parameters.AddWithValue("@password", usuario.Password);
        command.Parameters.AddWithValue("@role", usuario.Role);
        command.Parameters.AddWithValue("@avatar", usuario.Avatar?.Url ?? "");
        
        connection.Open();
        return command.ExecuteNonQuery();
    }

    public int Update(Usuario usuario)
    {
        var query = @"update Personas 
                    set nombre = @nombre, apellido = @apellido, telefono = @telefono, email = @email 
                    where dni = @dni;
                    
                    update Usuarios 
                    set role = @role, avatar = @avatar, password = @password 
                    where dni = @dni;";

        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@dni", usuario.Dni);
        command.Parameters.AddWithValue("@nombre", usuario.Nombre);
        command.Parameters.AddWithValue("@apellido", usuario.Apellido);
        command.Parameters.AddWithValue("@telefono", usuario.Telefono ?? "");
        command.Parameters.AddWithValue("@email", usuario.Email);
        command.Parameters.AddWithValue("@role", usuario.Role);
        command.Parameters.AddWithValue("@avatar", usuario.Avatar?.Url ?? "");
        command.Parameters.AddWithValue("@password", usuario.Password);

        connection.Open();
        return command.ExecuteNonQuery();
    }

    public int UploadAvatar(Imagen img, Usuario usuario)
    {

        if (usuario.Avatar != null)
        {
            DeleteAvatar(usuario.Avatar.Url);
        }

        img.Id = Guid.NewGuid().ToString();
        img.Url = $"/Uploads/Usuarios/{usuario.Dni}/{img.Id}{Path.GetExtension(img.File!.FileName)}";
        
        string uploadPath = Path.Combine(environment.WebRootPath, "Uploads", "Usuarios", usuario.Dni!);
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }
        
        var filePath = Path.Combine(uploadPath, $"{img.Id}{Path.GetExtension(img.File!.FileName)}");
        using var stream = new FileStream(filePath, FileMode.Create);
        img.File.CopyTo(stream);
        
        usuario.Avatar = img;
        return UpdateAvatar(img, usuario);
    }

    private int UpdateAvatar(Imagen img, Usuario usuario)
    {
        var query = "update Usuarios set avatar = @avatar where dni = @dni";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@avatar", img.Url);
        command.Parameters.AddWithValue("@dni", usuario.Dni);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    private void DeleteAvatar(string urlAvatar)
    {
        if(string.IsNullOrEmpty(urlAvatar)) return;
        var fullPath = Path.Combine(environment.WebRootPath, urlAvatar.TrimStart('/'));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        };    
    }   

    public int Delete(string dni)
    {
        var usuario = FindByDni(dni);
        if (usuario?.Avatar?.Url != null)
        {
            DeleteAvatar(usuario.Avatar.Url);
        }
        var query = "delete from Usuarios where dni = @dni";
        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@dni", dni);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    private static Usuario ParseUsuario(MySqlDataReader reader)
    {

        var usuario =  new Usuario
        {
            Dni = reader.GetString("dni"),
            Nombre = reader.GetString("nombre"),
            Apellido = reader.GetString("apellido"),
            Telefono = reader["telefono"] as string,
            Email = reader.GetString("email"),
            Password = reader.GetString("password"),
            Role = reader.GetString("role"),
        };

        var avatar = reader["avatar"]?.ToString();
        if (!string.IsNullOrEmpty(avatar))
        {
            usuario.Avatar = new Imagen { Url = avatar };
        }
        return usuario;
    }

}