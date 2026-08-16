namespace inmobiliaria.Models;
using System.ComponentModel.DataAnnotations;

public class Inquilino : Persona
{
    public override string ToString()
    {
        return $"Inquilino {{IdInquilino={Id}, Nombre={Nombre}, Apellido={Apellido}, DNI={Dni}, Telefono={Telefono}, Email={Email}}}";
    }
}




