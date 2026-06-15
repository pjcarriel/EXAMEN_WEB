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
    public class ProfesorsController : Controller
    {
        private readonly CatalogoMateriaBdCoreContext _context;

        public ProfesorsController(CatalogoMateriaBdCoreContext context)
        {
            _context = context;
        }

        private bool TienePermiso(string modulo, string accion)
            => HttpContext.Session.GetString(modulo + "." + accion) == "true";

        public async Task<IActionResult> Index()
        {
            if (!TienePermiso("Profesors", "PuedeVer"))
                return RedirectToAction("AccesoDenegado", "Home");

            var lista = _context.Profesors.Include(p => p.Departamento).Include(p => p.Personal);
            return View(await lista.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!TienePermiso("Profesors", "PuedeVer"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null || _context.Profesors == null) return NotFound();

            var profesor = await _context.Profesors
                .Include(p => p.Departamento).Include(p => p.Personal)
                .FirstOrDefaultAsync(m => m.ProfesorId == id);
            if (profesor == null) return NotFound();
            return View(profesor);
        }

        public IActionResult Create()
        {
            if (!TienePermiso("Profesors", "PuedeCrear"))
                return RedirectToAction("AccesoDenegado", "Home");

            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "DepartamentoId", "NombreDepartamento");
            ViewData["PersonalId"] = new SelectList(
                _context.Personas.Include(p => p.RolPersona)
                    .Where(p => p.RolPersona.RolNombre == "PROFESOR")
                    .Select(p => new { p.PersonalId, NombreCompleto = p.NombrePersona + " " + p.ApellidoPersona })
                    .ToList(),
                "PersonalId", "NombreCompleto");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProfesorId,PersonalId,DepartamentoId,SueldoProfesor")] Profesor profesor)
        {
            if (!TienePermiso("Profesors", "PuedeCrear"))
                return RedirectToAction("AccesoDenegado", "Home");

            ModelState.Remove("Personal");
            ModelState.Remove("Departamento");
            ModelState.Remove("NrcMateria");

            if (ModelState.IsValid)
            {
                _context.Add(profesor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "DepartamentoId", "NombreDepartamento", profesor.DepartamentoId);
            ViewData["PersonalId"] = new SelectList(
                _context.Personas.Include(p => p.RolPersona)
                    .Where(p => p.RolPersona.RolNombre == "PROFESOR")
                    .Select(p => new { p.PersonalId, NombreCompleto = p.NombrePersona + " " + p.ApellidoPersona })
                    .ToList(),
                "PersonalId", "NombreCompleto", profesor.PersonalId);
            return View(profesor);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!TienePermiso("Profesors", "PuedeEditar"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null || _context.Profesors == null) return NotFound();
            var profesor = await _context.Profesors.FindAsync(id);
            if (profesor == null) return NotFound();

            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "DepartamentoId", "NombreDepartamento", profesor.DepartamentoId);
            ViewData["PersonalId"] = new SelectList(
                _context.Personas.Include(p => p.RolPersona)
                    .Where(p => p.RolPersona.RolNombre == "PROFESOR")
                    .Select(p => new { p.PersonalId, NombreCompleto = p.NombrePersona + " " + p.ApellidoPersona })
                    .ToList(),
                "PersonalId", "NombreCompleto", profesor.PersonalId);
            return View(profesor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProfesorId,PersonalId,DepartamentoId,SueldoProfesor")] Profesor profesor)
        {
            if (!TienePermiso("Profesors", "PuedeEditar"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id != profesor.ProfesorId) return NotFound();

            ModelState.Remove("Personal");
            ModelState.Remove("Departamento");
            ModelState.Remove("NrcMateria");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profesor);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                }
            }
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "DepartamentoId", "NombreDepartamento", profesor.DepartamentoId);
            ViewData["PersonalId"] = new SelectList(
                _context.Personas.Include(p => p.RolPersona)
                    .Where(p => p.RolPersona.RolNombre == "PROFESOR")
                    .Select(p => new { p.PersonalId, NombreCompleto = p.NombrePersona + " " + p.ApellidoPersona })
                    .ToList(),
                "PersonalId", "NombreCompleto", profesor.PersonalId);
            return View(profesor);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!TienePermiso("Profesors", "PuedeEliminar"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null || _context.Profesors == null) return NotFound();
            var profesor = await _context.Profesors
                .Include(p => p.Departamento).Include(p => p.Personal)
                .FirstOrDefaultAsync(m => m.ProfesorId == id);
            if (profesor == null) return NotFound();
            return View(profesor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!TienePermiso("Profesors", "PuedeEliminar"))
                return RedirectToAction("AccesoDenegado", "Home");

            var profesor = await _context.Profesors.FindAsync(id);
            if (profesor != null) _context.Profesors.Remove(profesor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
