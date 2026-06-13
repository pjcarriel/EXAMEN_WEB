using System;
using System.Collections.Generic;

namespace MoldeMVC_Core.Models;

public partial class Estudiante
{
    public int EstudianteId { get; set; }

    public int PersonalId { get; set; }

    public int? CarreraId { get; set; }

    public int? NivelActualEst { get; set; }

    public virtual Carrera? Carrera { get; set; }

    public virtual ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

    public virtual Nivel? NivelActualEstNavigation { get; set; }

    public virtual Persona Personal { get; set; } = null!;
}
