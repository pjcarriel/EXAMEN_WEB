using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoldeMVC_Core.Models;
using System.Security.Claims;

namespace MoldeMVC_Core.Controllers
{
    [Authorize(Policy = "SoloUsuariosAdmins")]
    public class GestionPermisosController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public GestionPermisosController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: GestionPermisos
        public IActionResult Index()
        {
            var roles = new List<string> { "DECANO", "PLANIFICADOR", "ESTUDIANTE", "PROFESOR", "ABOGADO" };
            return View(roles);
        }

        // GET: GestionPermisos/Configurar?rolNombre=DECANO
        public async Task<IActionResult> Configurar(string rolNombre)
        {
            var rol = await _roleManager.FindByNameAsync(rolNombre);
            if (rol == null) return NotFound();

            var claims = await _roleManager.GetClaimsAsync(rol);

            var modulosPorRol = new Dictionary<string, string[]>
            {
                { "DECANO",       new[] { "Estudiantes", "Profesors", "CatalogoMaterias" } },
                { "PLANIFICADOR", new[] { "NrcMateriums", "Matriculas" } },
                { "ESTUDIANTE",   new[] { "Matriculas" } },
                { "PROFESOR",     new[] { "NrcMateriums" } },
                { "ABOGADO",      new[] { "CatalogoMaterias" } },
            };

            var modulos = modulosPorRol.ContainsKey(rolNombre) ? modulosPorRol[rolNombre] : Array.Empty<string>();

            var permisos = modulos.Select(m => new PermisoRol
            {
                RolNombre = rolNombre,
                Modulo    = m,
                PuedeVer      = claims.Any(c => c.Type == m + ".PuedeVer"      && c.Value == "true"),
                PuedeCrear    = claims.Any(c => c.Type == m + ".PuedeCrear"    && c.Value == "true"),
                PuedeEditar   = claims.Any(c => c.Type == m + ".PuedeEditar"   && c.Value == "true"),
                PuedeEliminar = claims.Any(c => c.Type == m + ".PuedeEliminar" && c.Value == "true"),
            }).ToList();

            ViewData["RolNombre"] = rolNombre;

            if (TempData["Exito"] != null) ViewBag.Exito = TempData["Exito"];

            return View(permisos);
        }

        // POST: GestionPermisos/Configurar
        [HttpPost]
        public async Task<IActionResult> Configurar(List<PermisoRol> permisos)
        {
            if (permisos == null || !permisos.Any())
                return RedirectToAction("Index");

            var rolNombre = permisos.First().RolNombre;
            var rol = await _roleManager.FindByNameAsync(rolNombre);
            if (rol == null) return NotFound();

            // Quitar todos los claims de permisos existentes de este rol
            var claimsExistentes = await _roleManager.GetClaimsAsync(rol);
            foreach (var claim in claimsExistentes)
                await _roleManager.RemoveClaimAsync(rol, claim);

            // Agregar los nuevos claims según los switches del formulario
            foreach (var p in permisos)
            {
                if (p.PuedeVer)      await _roleManager.AddClaimAsync(rol, new Claim(p.Modulo + ".PuedeVer",      "true"));
                if (p.PuedeCrear)    await _roleManager.AddClaimAsync(rol, new Claim(p.Modulo + ".PuedeCrear",    "true"));
                if (p.PuedeEditar)   await _roleManager.AddClaimAsync(rol, new Claim(p.Modulo + ".PuedeEditar",   "true"));
                if (p.PuedeEliminar) await _roleManager.AddClaimAsync(rol, new Claim(p.Modulo + ".PuedeEliminar", "true"));
            }

            TempData["Exito"] = "Permisos guardados correctamente.";
            return RedirectToAction("Configurar", new { rolNombre });
        }
    }
}
