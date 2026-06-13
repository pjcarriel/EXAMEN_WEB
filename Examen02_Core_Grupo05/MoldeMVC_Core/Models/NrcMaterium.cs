using System;
using System.Collections.Generic;

namespace MoldeMVC_Core.Models;

public partial class NrcMaterium
{
    public string Nrc { get; set; } = null!;

    public int? MateriaId { get; set; }

    public int? ProfesorId { get; set; }

    public virtual CatalogoMateria? Materia { get; set; }

    public virtual ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

    public virtual Profesor? Profesor { get; set; }
}
