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
    public class NrcMateriumsController : Controller
    {
        private readonly CatalogoMateriaBdCoreContext _context;

        public NrcMateriumsController(CatalogoMateriaBdCoreContext context)
        {
            _context = context;
        }

        private bool TienePermiso(string modulo, string accion)
            => HttpContext.Session.GetString(modulo + "." + accion) == "true";

        [Authorize(Roles = "PLANIFICADOR,PROFESOR")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("PROFESOR"))
            {
                var profesorId = HttpContext.Session.GetInt32("ProfesorId");

                if (profesorId == null)
                {
                    var prof = _context.Profesors.Include(p => p.Personal)
                        .FirstOrDefault(p => p.Personal.NombrePersona == User.Identity!.Name);
                    if (prof != null)
                    {
                        profesorId = prof.ProfesorId;
                        HttpContext.Session.SetInt32("ProfesorId", prof.ProfesorId);
                    }
                }

                if (profesorId == null)
                {
                    ViewBag.Mensaje = "No se encontró un perfil de profesor vinculado a este usuario.";
                    return View(new List<NrcMaterium>());
                }

                ViewBag.PuedeCrear    = TienePermiso("NrcMateriums", "PuedeCrear");
                ViewBag.PuedeEditar   = TienePermiso("NrcMateriums", "PuedeEditar");
                ViewBag.PuedeEliminar = TienePermiso("NrcMateriums", "PuedeEliminar");

                var materias = await _context.NrcMateria
                    .Include(n => n.Materia)
                    .Include(n => n.Profesor).ThenInclude(p => p!.Personal)
                    .Where(n => n.ProfesorId == profesorId)
                    .ToListAsync();
                return View(materias);
            }

            // PLANIFICADOR
            if (!TienePermiso("NrcMateriums", "PuedeVer"))
                return RedirectToAction("AccesoDenegado", "Home");

            ViewBag.PuedeCrear    = TienePermiso("NrcMateriums", "PuedeCrear");
            ViewBag.PuedeEditar   = TienePermiso("NrcMateriums", "PuedeEditar");
            ViewBag.PuedeEliminar = TienePermiso("NrcMateriums", "PuedeEliminar");

            var todos = await _context.NrcMateria
                .Include(n => n.Materia)
                .Include(n => n.Profesor).ThenInclude(p => p!.Personal)
                .ToListAsync();
            return View(todos);
        }

        [Authorize]
        public async Task<IActionResult> Details(string id)
        {
            if (!TienePermiso("NrcMateriums", "PuedeVer"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null || _context.NrcMateria == null) return NotFound();

            var nrcMaterium = await _context.NrcMateria
                .Include(n => n.Materia).Include(n => n.Profesor).ThenInclude(p => p!.Personal)
                .FirstOrDefaultAsync(m => m.Nrc == id);
            if (nrcMaterium == null) return NotFound();
            return View(nrcMaterium);
        }

        [Authorize]
        public IActionResult Create()
        {
            if (!TienePermiso("NrcMateriums", "PuedeCrear"))
                return RedirectToAction("AccesoDenegado", "Home");

            ViewData["Departamentos"] = new SelectList(_context.Departamentos, "DepartamentoId", "NombreDepartamento");
            ViewData["MateriaId"] = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
            ViewData["ProfesorId"] = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");

            if (TempData["Error"] != null) ViewBag.Error = TempData["Error"];
            if (TempData["Exito"] != null) ViewBag.Exito = TempData["Exito"];
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nrc,MateriaId,ProfesorId")] NrcMaterium nrcMaterium)
        {
            if (!TienePermiso("NrcMateriums", "PuedeCrear"))
                return RedirectToAction("AccesoDenegado", "Home");

            ModelState.Remove("Materia");
            ModelState.Remove("Profesor");
            ModelState.Remove("Matriculas");

            if (await _context.NrcMateria.AnyAsync(n => n.Nrc == nrcMaterium.Nrc))
                ModelState.AddModelError("Nrc", "Este NRC ya está registrado.");

            if (nrcMaterium.ProfesorId.HasValue && nrcMaterium.MateriaId.HasValue)
            {
                var profesor = await _context.Profesors.FindAsync(nrcMaterium.ProfesorId);
                var materia  = await _context.CatalogoMaterias.FindAsync(nrcMaterium.MateriaId);
                if (profesor != null && materia != null && profesor.DepartamentoId != materia.DepartamentoId)
                    ModelState.AddModelError("", "La materia no pertenece al departamento del profesor.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(nrcMaterium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Departamentos"] = new SelectList(_context.Departamentos, "DepartamentoId", "NombreDepartamento");
            ViewData["MateriaId"] = new SelectList(_context.CatalogoMaterias, "MateriaId", "NombreMateria", nrcMaterium.MateriaId);
            ViewData["ProfesorId"] = new SelectList(_context.Profesors.Include(p => p.Personal), "ProfesorId", "Personal.NombrePersona", nrcMaterium.ProfesorId);
            return View(nrcMaterium);
        }

        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            if (!TienePermiso("NrcMateriums", "PuedeEditar"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null || _context.NrcMateria == null) return NotFound();
            var nrcMaterium = await _context.NrcMateria.FindAsync(id);
            if (nrcMaterium == null) return NotFound();

            ViewData["MateriaId"] = new SelectList(_context.CatalogoMaterias, "MateriaId", "NombreMateria", nrcMaterium.MateriaId);
            ViewData["ProfesorId"] = new SelectList(_context.Profesors.Include(p => p.Personal), "ProfesorId", "Personal.NombrePersona", nrcMaterium.ProfesorId);
            return View(nrcMaterium);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Nrc,MateriaId,ProfesorId")] NrcMaterium nrcMaterium)
        {
            if (!TienePermiso("NrcMateriums", "PuedeEditar"))
                return RedirectToAction("AccesoDenegado", "Home");

            ModelState.Remove("Materia");
            ModelState.Remove("Profesor");
            ModelState.Remove("Matriculas");

            if (id != nrcMaterium.Nrc) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nrcMaterium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NrcMateriumExists(nrcMaterium.Nrc)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MateriaId"] = new SelectList(_context.CatalogoMaterias, "MateriaId", "NombreMateria", nrcMaterium.MateriaId);
            ViewData["ProfesorId"] = new SelectList(_context.Profesors.Include(p => p.Personal), "ProfesorId", "Personal.NombrePersona", nrcMaterium.ProfesorId);
            return View(nrcMaterium);
        }

        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            if (!TienePermiso("NrcMateriums", "PuedeEliminar"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null || _context.NrcMateria == null) return NotFound();
            var nrcMaterium = await _context.NrcMateria
                .Include(n => n.Materia).Include(n => n.Profesor)
                .FirstOrDefaultAsync(m => m.Nrc == id);
            if (nrcMaterium == null) return NotFound();
            return View(nrcMaterium);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!TienePermiso("NrcMateriums", "PuedeEliminar"))
                return RedirectToAction("AccesoDenegado", "Home");

            var nrcMaterium = await _context.NrcMateria.FindAsync(id);
            if (nrcMaterium != null) _context.NrcMateria.Remove(nrcMaterium);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public JsonResult GetProfesoresByDepto(int deptoId)
        {
            var profesores = _context.Profesors.Include(p => p.Personal)
                .Where(p => p.DepartamentoId == deptoId)
                .Select(p => new { p.ProfesorId, Nombre = p.Personal.NombrePersona + " " + p.Personal.ApellidoPersona })
                .ToList();
            return Json(profesores);
        }

        [Authorize]
        public JsonResult GetMateriasByDepto(int deptoId)
        {
            var materias = _context.CatalogoMaterias
                .Where(m => m.DepartamentoId == deptoId)
                .Select(m => new { m.MateriaId, m.NombreMateria })
                .ToList();
            return Json(materias);
        }

        private bool NrcMateriumExists(string id)
            => (_context.NrcMateria?.Any(e => e.Nrc == id)).GetValueOrDefault();
    }
}
