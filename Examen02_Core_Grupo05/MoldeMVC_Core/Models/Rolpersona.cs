using System;
using System.Collections.Generic;

namespace MoldeMVC_Core.Models;

public partial class Rolpersona
{
    public int RolPersonaId { get; set; }

    public string? RolNombre { get; set; }

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();
}
