using System;
using System.Collections.Generic;

namespace MoldeMVC_Core.Models;

public partial class Profesor
{
    public int ProfesorId { get; set; }

    public int PersonalId { get; set; }

    public int DepartamentoId { get; set; }

    public double? SueldoProfesor { get; set; }

    public virtual Departamento Departamento { get; set; } = null!;

    public virtual ICollection<NrcMaterium> NrcMateria { get; set; } = new List<NrcMaterium>();

    public virtual Persona Personal { get; set; } = null!;
}
