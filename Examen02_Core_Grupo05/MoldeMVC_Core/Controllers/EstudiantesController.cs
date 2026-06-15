using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoldeMVC_Core.Models;

namespace MoldeMVC_Core.Controllers
{
    [Authorize]
    public class EstudiantesController : Controller
    {
        private readonly CatalogoMateriaBdCoreContext _context;

        public EstudiantesController(CatalogoMateriaBdCoreContext context)
        {
            _context = context;
        }

        private bool TienePermiso(string modulo, string accion)
            => HttpContext.Session.GetString(modulo + "." + accion) == "true";

        public async Task<IActionResult> Index()
        {
            if (!TienePermiso("Estudiantes", "PuedeVer"))
                return RedirectToAction("AccesoDenegado", "Home");

            var lista = _context.Estudiantes
                .Include(e => e.Carrera)
                .Include(e => e.NivelActualEstNavigation)
                .Include(e => e.Personal);
            return View(await lista.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!TienePermiso("Estudiantes", "PuedeVer"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null) return NotFound();

            var estudiante = await _context.Estudiantes
                .Include(e => e.Carrera)
                .Include(e => e.NivelActualEstNavigation)
                .Include(e => e.Personal)
                .FirstOrDefaultAsync(m => m.EstudianteId == id);

            if (estudiante == null) return NotFound();
            return View(estudiante);
        }

        public IActionResult Create()
        {
            if (!TienePermiso("Estudiantes", "PuedeCrear"))
                return RedirectToAction("AccesoDenegado", "Home");

            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "NombreCarrera");
            ViewData["NivelActualEst"] = new SelectList(_context.Nivels, "NivelId", "NivelDescrip");
            ViewData["PersonalId"] = new SelectList(
                _context.Personas.Include(p => p.RolPersona)
                    .Where(p => p.RolPersona.RolNombre == "ESTUDIANTE")
                    .Select(p => new { p.PersonalId, NombreCompleto = p.NombrePersona + " " + p.ApellidoPersona })
                    .ToList(),
                "PersonalId", "NombreCompleto");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EstudianteId,PersonalId,CarreraId,NivelActualEst")] Estudiante estudiante)
        {
            if (!TienePermiso("Estudiantes", "PuedeCrear"))
                return RedirectToAction("AccesoDenegado", "Home");

            ModelState.Remove("Personal");
            ModelState.Remove("Carrera");
            ModelState.Remove("NivelActualEstNavigation");
            ModelState.Remove("Matriculas");

            if (ModelState.IsValid)
            {
                _context.Add(estudiante);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "NombreCarrera", estudiante.CarreraId);
            ViewData["NivelActualEst"] = new SelectList(_context.Nivels, "NivelId", "NivelDescrip", estudiante.NivelActualEst);
            ViewData["PersonalId"] = new SelectList(
                _context.Personas.Include(p => p.RolPersona)
                    .Where(p => p.RolPersona.RolNombre == "ESTUDIANTE")
                    .Select(p => new { p.PersonalId, NombreCompleto = p.NombrePersona + " " + p.ApellidoPersona })
                    .ToList(),
                "PersonalId", "NombreCompleto", estudiante.PersonalId);
            return View(estudiante);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!TienePermiso("Estudiantes", "PuedeEditar"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null) return NotFound();
            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante == null) return NotFound();

            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "NombreCarrera", estudiante.CarreraId);
            ViewData["NivelActualEst"] = new SelectList(_context.Nivels, "NivelId", "NivelDescrip", estudiante.NivelActualEst);
            ViewData["PersonalId"] = new SelectList(
                _context.Personas.Include(p => p.RolPersona)
                    .Where(p => p.RolPersona.RolNombre == "ESTUDIANTE")
                    .Select(p => new { p.PersonalId, NombreCompleto = p.NombrePersona + " " + p.ApellidoPersona })
                    .ToList(),
                "PersonalId", "NombreCompleto", estudiante.PersonalId);
            return View(estudiante);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EstudianteId,PersonalId,CarreraId,NivelActualEst")] Estudiante estudiante)
        {
            if (!TienePermiso("Estudiantes", "PuedeEditar"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id != estudiante.EstudianteId) return NotFound();

            ModelState.Remove("Personal");
            ModelState.Remove("Carrera");
            ModelState.Remove("NivelActualEstNavigation");
            ModelState.Remove("Matriculas");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estudiante);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                }
            }
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "NombreCarrera", estudiante.CarreraId);
            ViewData["NivelActualEst"] = new SelectList(_context.Nivels, "NivelId", "NivelDescrip", estudiante.NivelActualEst);
            ViewData["PersonalId"] = new SelectList(
                _context.Personas.Include(p => p.RolPersona)
                    .Where(p => p.RolPersona.RolNombre == "ESTUDIANTE")
                    .Select(p => new { p.PersonalId, NombreCompleto = p.NombrePersona + " " + p.ApellidoPersona })
                    .ToList(),
                "PersonalId", "NombreCompleto", estudiante.PersonalId);
            return View(estudiante);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!TienePermiso("Estudiantes", "PuedeEliminar"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null) return NotFound();
            var estudiante = await _context.Estudiantes
                .Include(e => e.Carrera).Include(e => e.NivelActualEstNavigation).Include(e => e.Personal)
                .FirstOrDefaultAsync(m => m.EstudianteId == id);
            if (estudiante == null) return NotFound();
            return View(estudiante);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!TienePermiso("Estudiantes", "PuedeEliminar"))
                return RedirectToAction("AccesoDenegado", "Home");

            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante != null) _context.Estudiantes.Remove(estudiante);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
