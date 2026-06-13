using System;
using System.Collections.Generic;

namespace MoldeMVC_Core.Models;

public partial class Nivel
{
    public int NivelId { get; set; }

    public string? NivelDescrip { get; set; }

    public virtual ICollection<CatalogoMateria> CatalogoMateria { get; set; } = new List<CatalogoMateria>();

    public virtual ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
}
