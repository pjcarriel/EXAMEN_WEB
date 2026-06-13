using System;
using System.Collections.Generic;

namespace MoldeMVC_Core.Models;

public partial class CatalogoMateria
{
    public int MateriaId { get; set; }

    public int? NivelId { get; set; }

    public int? DepartamentoId { get; set; }

    public int? CarreraId { get; set; }

    public string? NombreMateria { get; set; }

    public virtual Carrera? Carrera { get; set; }

    public virtual Departamento? Departamento { get; set; }

    public virtual Nivel? Nivel { get; set; }

    public virtual ICollection<NrcMaterium> NrcMateria { get; set; } = new List<NrcMaterium>();
}
