# Inmobiliaria

> Proyecto de inmobiliaria en c# para la materia Laboratorio de Programacion 2

---

## 👥 Integrantes del Grupo

* **Peralta Alejandro** - *galejandroperalta475@gmail.com* - Discord: `rickert478`
* **Miguel Francisco Zini** - *mfzini.dev@gmail.com* - Discord: `c15c0`

---

## 📐 Modelado de Datos

El diagrama de entidad-relacion se encuentra en la carpeta `docs`, consideramos
**Persona** tanto como inquilinos y propietarios.

![Diagrama](docs/diagrama.png)

## 🔑 Usuarios de prueba

| Rol | Correo Electrónico | Contraseña | DNI |
| :--- | :--- | :--- | :--- |
| 👑 **Administrador** | `lionel_hutz@example.com` | `password` | `0` |
| 💼 **Empleado** | `lucia.fernandez@example.com` | `password` | `3` |


### Para levantar la base de datos: 

1. **Ejecutar el script.sql** en su gestor de base de datos de preferencia (BDs compatibles MariaDB y MySQL):

2. **Cadena de conexión:** Modificar la propiedad `DefaultConnection` (o el nombre que uses) en el archivo `appsettings.json` para que coincida con sus credenciales locales.

