namespace MoldeMVC_Core.Models
{
    public class PermisoRol
    {
        public int Id { get; set; }
        public string RolNombre { get; set; } = null!;
        public string Modulo { get; set; } = null!;
        public bool PuedeVer { get; set; }
        public bool PuedeCrear { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
    }
}
