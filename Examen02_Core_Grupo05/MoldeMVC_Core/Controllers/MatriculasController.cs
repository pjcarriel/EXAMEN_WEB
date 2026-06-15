using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoldeMVC_Core.Models;

namespace MoldeMVC_Core.Controllers
{
    public class MatriculasController : Controller
    {
        private readonly CatalogoMateriaBdCoreContext _context;

        public MatriculasController(CatalogoMateriaBdCoreContext context)
        {
            _context = context;
        }

        private bool TienePermiso(string modulo, string accion)
            => HttpContext.Session.GetString(modulo + "." + accion) == "true";

        [Authorize(Roles = "PLANIFICADOR,ESTUDIANTE")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("ESTUDIANTE"))
            {
                var estudianteId = HttpContext.Session.GetInt32("EstudianteId");

                if (estudianteId == null)
                {
                    var est = _context.Estudiantes.Include(e => e.Personal)
                        .FirstOrDefault(e => e.Personal.NombrePersona == User.Identity!.Name);
                    if (est != null)
                    {
                        estudianteId = est.EstudianteId;
                        HttpContext.Session.SetInt32("EstudianteId", est.EstudianteId);
                    }
                }

                if (estudianteId == null)
                {
                    ViewBag.Mensaje = "No se encontró un perfil de estudiante vinculado a este usuario.";
                    return View(new List<Matricula>());
                }

                ViewBag.PuedeCrear    = TienePermiso("Matriculas", "PuedeCrear");
                ViewBag.PuedeEditar   = TienePermiso("Matriculas", "PuedeEditar");
                ViewBag.PuedeEliminar = TienePermiso("Matriculas", "PuedeEliminar");

                var misMatriculas = await _context.Matriculas
                    .Include(m => m.NrcNavigation).ThenInclude(n => n!.Materia)
                    .Include(m => m.Estudiante).ThenInclude(e => e!.Personal)
                    .Where(m => m.EstudianteId == estudianteId)
                    .ToListAsync();
                return View(misMatriculas);
            }

            // PLANIFICADOR
            if (!TienePermiso("Matriculas", "PuedeVer"))
                return RedirectToAction("AccesoDenegado", "Home");

            ViewBag.PuedeCrear    = TienePermiso("Matriculas", "PuedeCrear");
            ViewBag.PuedeEditar   = TienePermiso("Matriculas", "PuedeEditar");
            ViewBag.PuedeEliminar = TienePermiso("Matriculas", "PuedeEliminar");

            var todas = await _context.Matriculas
                .Include(m => m.Estudiante).ThenInclude(e => e!.Personal)
                .Include(m => m.NrcNavigation).ThenInclude(n => n!.Materia)
                .ToListAsync();
            return View(todas);
        }

        [Authorize(Roles = "PLANIFICADOR,ESTUDIANTE")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var matricula = await _context.Matriculas
                .Include(m => m.Estudiante).ThenInclude(e => e!.Personal)
                .Include(m => m.NrcNavigation).ThenInclude(n => n!.Materia)
                .FirstOrDefaultAsync(m => m.RegistroId == id);
            if (matricula == null) return NotFound();

            if (User.IsInRole("PLANIFICADOR") && !TienePermiso("Matriculas", "PuedeVer"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (User.IsInRole("ESTUDIANTE"))
            {
                var est = _context.Estudiantes.Include(e => e.Personal)
                    .FirstOrDefault(e => e.Personal.NombrePersona == User.Identity!.Name);
                if (est == null || matricula.EstudianteId != est.EstudianteId)
                    return RedirectToAction("AccesoDenegado", "Home");
            }

            return View(matricula);
        }

        [Authorize]
        public IActionResult Create()
        {
            if (!TienePermiso("Matriculas", "PuedeCrear"))
                return RedirectToAction("AccesoDenegado", "Home");

            ViewData["EstudianteId"] = new SelectList(
                _context.Estudiantes.Include(e => e.Personal)
                    .Select(e => new { e.EstudianteId, Nombre = e.Personal.NombrePersona + " " + e.Personal.ApellidoPersona })
                    .ToList(),
                "EstudianteId", "Nombre");

            ViewData["Nrc"] = new SelectList(
                _context.NrcMateria.Include(n => n.Materia)
                    .Select(n => new { n.Nrc, Descripcion = n.Nrc + " - " + n.Materia.NombreMateria })
                    .ToList(),
                "Nrc", "Descripcion");

            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistroId,Nrc,EstudianteId")] Matricula matricula)
        {
            if (!TienePermiso("Matriculas", "PuedeCrear"))
                return RedirectToAction("AccesoDenegado", "Home");

            ModelState.Remove("Estudiante");
            ModelState.Remove("NrcNavigation");

            if (ModelState.IsValid)
            {
                _context.Add(matricula);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EstudianteId"] = new SelectList(
                _context.Estudiantes.Include(e => e.Personal)
                    .Select(e => new { e.EstudianteId, Nombre = e.Personal.NombrePersona + " " + e.Personal.ApellidoPersona })
                    .ToList(),
                "EstudianteId", "Nombre", matricula.EstudianteId);

            ViewData["Nrc"] = new SelectList(
                _context.NrcMateria.Include(n => n.Materia)
                    .Select(n => new { n.Nrc, Descripcion = n.Nrc + " - " + n.Materia.NombreMateria })
                    .ToList(),
                "Nrc", "Descripcion", matricula.Nrc);

            return View(matricula);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!TienePermiso("Matriculas", "PuedeEditar"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null || _context.Matriculas == null) return NotFound();
            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula == null) return NotFound();

            ViewData["EstudianteId"] = new SelectList(
                _context.Estudiantes.Include(e => e.Personal)
                    .Select(e => new { e.EstudianteId, Nombre = e.Personal.NombrePersona + " " + e.Personal.ApellidoPersona })
                    .ToList(),
                "EstudianteId", "Nombre", matricula.EstudianteId);
            ViewData["Nrc"] = new SelectList(
                _context.NrcMateria.Include(n => n.Materia)
                    .Select(n => new { n.Nrc, Descripcion = n.Nrc + " - " + n.Materia.NombreMateria })
                    .ToList(),
                "Nrc", "Descripcion", matricula.Nrc);
            return View(matricula);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegistroId,Nrc,EstudianteId")] Matricula matricula)
        {
            if (!TienePermiso("Matriculas", "PuedeEditar"))
                return RedirectToAction("AccesoDenegado", "Home");

            ModelState.Remove("Estudiante");
            ModelState.Remove("NrcNavigation");

            if (id != matricula.RegistroId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(matricula);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatriculaExists(matricula.RegistroId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EstudianteId"] = new SelectList(
                _context.Estudiantes.Include(e => e.Personal)
                    .Select(e => new { e.EstudianteId, Nombre = e.Personal.NombrePersona + " " + e.Personal.ApellidoPersona })
                    .ToList(),
                "EstudianteId", "Nombre", matricula.EstudianteId);
            ViewData["Nrc"] = new SelectList(
                _context.NrcMateria.Include(n => n.Materia)
                    .Select(n => new { n.Nrc, Descripcion = n.Nrc + " - " + n.Materia.NombreMateria })
                    .ToList(),
                "Nrc", "Descripcion", matricula.Nrc);
            return View(matricula);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!TienePermiso("Matriculas", "PuedeEliminar"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null || _context.Matriculas == null) return NotFound();
            var matricula = await _context.Matriculas
                .Include(m => m.Estudiante).Include(m => m.NrcNavigation)
                .FirstOrDefaultAsync(m => m.RegistroId == id);
            if (matricula == null) return NotFound();
            return View(matricula);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!TienePermiso("Matriculas", "PuedeEliminar"))
                return RedirectToAction("AccesoDenegado", "Home");

            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula != null) _context.Matriculas.Remove(matricula);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatriculaExists(int id)
            => (_context.Matriculas?.Any(e => e.RegistroId == id)).GetValueOrDefault();
    }
}
