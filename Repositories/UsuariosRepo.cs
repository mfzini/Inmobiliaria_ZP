using inmobiliaria.Models;
using MySql.Data.MySqlClient;

namespace inmobiliaria.Repositories;

public class UsuariosRepo(IConfiguration config, IWebHostEnvironment environment) : RepositorioBase(config)
{

    public List<Usuario> ListAll()
    {
        List<Usuario> usuarios = [];
        var query = @"select dni, nombre, apellido, telefono, email, password, role, avatar 
                    from Usuarios
                    order by apellido, nombre";
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

    public Usuario? FindByDni(string dni)
    {
        var query = @"select dni, nombre, apellido, telefono, email, password, role, avatar 
                    from Usuarios
                    where dni = @dni";

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
        var query = @"insert into Usuarios (dni, nombre, apellido, telefono, email, password, role, avatar) 
                    values (@dni, @nombre, @apellido, @telefono, @email, @password, @role, @avatar)";
        
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
        var query = @"update Usuarios 
                    set nombre = @nombre, apellido = @apellido, telefono = @telefono, 
                    email = @email, role = @role, avatar = @avatar 
                    where dni = @dni";

        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@dni", usuario.Dni);
        command.Parameters.AddWithValue("@nombre", usuario.Nombre);
        command.Parameters.AddWithValue("@apellido", usuario.Apellido);
        command.Parameters.AddWithValue("@telefono", usuario.Telefono ?? "");
        command.Parameters.AddWithValue("@email", usuario.Email);
        command.Parameters.AddWithValue("@role", usuario.Role);
        command.Parameters.AddWithValue("@avatar", usuario.Avatar?.Url ?? "");

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