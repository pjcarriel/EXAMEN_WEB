using System;
using System.Collections.Generic;

namespace MoldeMVC_Core.Models;

public partial class Departamento
{
    public int DepartamentoId { get; set; }

    public string? NombreDepartamento { get; set; }

    public string? SiglasDepartamento { get; set; }

    public virtual ICollection<CatalogoMateria> CatalogoMateria { get; set; } = new List<CatalogoMateria>();

    public virtual ICollection<Profesor> Profesors { get; set; } = new List<Profesor>();
}
