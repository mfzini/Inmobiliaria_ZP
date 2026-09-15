namespace inmobiliaria.Models;

public class Log
{
    public Usuario Usuario { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Entry { get; set; }
}