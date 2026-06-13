using System;
using System.Collections.Generic;

namespace MoldeMVC_Core.Models;

public partial class Matricula
{
    public int RegistroId { get; set; }

    public string? Nrc { get; set; }

    public int? EstudianteId { get; set; }

    public virtual Estudiante? Estudiante { get; set; }

    public virtual NrcMaterium? NrcNavigation { get; set; }
}
