namespace inmobiliaria.Models;
using System.ComponentModel.DataAnnotations;

public class Propietario : Persona
{
    public override string ToString()
    {
        return $"Propietario {{IdPropietario={Id}, Nombre={Nombre}, Apellido={Apellido}, DNI={Dni}, Telefono={Telefono}, Email={Email}}}";
    }
}




