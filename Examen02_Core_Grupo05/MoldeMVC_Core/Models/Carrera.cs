using System;
using System.Collections.Generic;

namespace MoldeMVC_Core.Models;

public partial class Carrera
{
    public int CarreraId { get; set; }

    public string? NombreCarrera { get; set; }

    public int? CantSemestres { get; set; }

    public virtual ICollection<CatalogoMateria> CatalogoMateria { get; set; } = new List<CatalogoMateria>();

    public virtual ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
}
