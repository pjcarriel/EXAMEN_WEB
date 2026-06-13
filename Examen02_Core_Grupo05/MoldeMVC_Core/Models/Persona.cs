using System;
using System.Collections.Generic;

namespace MoldeMVC_Core.Models;

public partial class Persona
{
    public int PersonalId { get; set; }

    public string? CedulaPersona { get; set; }

    public int? RolPersonaId { get; set; }

    public string? NombrePersona { get; set; }

    public string? ApellidoPersona { get; set; }

    public string? Foto { get; set; }

    public virtual ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();

    public virtual ICollection<Profesor> Profesors { get; set; } = new List<Profesor>();

    public virtual Rolpersona? RolPersona { get; set; }
}
