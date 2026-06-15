using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoldeMVC_Core.Models;

namespace MoldeMVC_Core.Controllers
{
    public class CatalogoMateriasController : Controller
    {
        private readonly CatalogoMateriaBdCoreContext _context;

        public CatalogoMateriasController(CatalogoMateriaBdCoreContext context)
        {
            _context = context;
        }

        private bool TienePermiso(string modulo, string accion)
            => HttpContext.Session.GetString(modulo + "." + accion) == "true";

        [Authorize(Roles = "DECANO,ABOGADO")]
        public async Task<IActionResult> Index(int? carreraId)
        {
            if (User.IsInRole("ABOGADO"))
            {
                ViewData["Departamentos"] = new SelectList(_context.Departamentos, "DepartamentoId", "NombreDepartamento");

                if (carreraId.HasValue)
                {
                    var materias = _context.CatalogoMaterias
                        .Include(m => m.Nivel).Include(m => m.Carrera)
                        .Where(m => m.CarreraId == carreraId).ToList();

                    var opciones = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles, WriteIndented = true };
                    var json = JsonSerializer.Serialize(materias, opciones);
                    HttpContext.Session.SetString("MallaCurricular", json);

                    var mallaDeserializada = JsonSerializer.Deserialize<List<CatalogoMateria>>(json, opciones);
                    ViewData["JsonOriginal"] = json;
                    return View(mallaDeserializada ?? new List<CatalogoMateria>());
                }

                var jsonSession = HttpContext.Session.GetString("MallaCurricular");
                if (jsonSession != null)
                {
                    var opciones = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles, WriteIndented = true };
                    ViewData["JsonOriginal"] = jsonSession;
                    return View(JsonSerializer.Deserialize<List<CatalogoMateria>>(jsonSession, opciones) ?? new List<CatalogoMateria>());
                }

                return View(new List<CatalogoMateria>());
            }

            // DECANO
            if (!TienePermiso("CatalogoMaterias", "PuedeVer"))
                return RedirectToAction("AccesoDenegado", "Home");

            var todos = await _context.CatalogoMaterias
                .Include(c => c.Carrera).Include(c => c.Departamento).Include(c => c.Nivel)
                .ToListAsync();
            return View(todos);
        }

        [Authorize(Roles = "ABOGADO")]
        public JsonResult GetCarrerasByDepto(int deptoId)
        {
            var carreras = _context.CatalogoMaterias
                .Where(c => c.DepartamentoId == deptoId && c.Carrera != null)
                .Select(c => c.Carrera!).Distinct()
                .Select(c => new { c.CarreraId, c.NombreCarrera })
                .ToList();
            return Json(carreras);
        }

        [Authorize(Roles = "DECANO")]
        public async Task<IActionResult> Details(int? id)
        {
            if (!TienePermiso("CatalogoMaterias", "PuedeVer"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null || _context.CatalogoMaterias == null) return NotFound();

            var catalogoMateria = await _context.CatalogoMaterias
                .Include(c => c.Carrera).Include(c => c.Departamento).Include(c => c.Nivel)
                .FirstOrDefaultAsync(m => m.MateriaId == id);
            if (catalogoMateria == null) return NotFound();
            return View(catalogoMateria);
        }

        [Authorize(Roles = "DECANO")]
        public IActionResult Create()
        {
            if (!TienePermiso("CatalogoMaterias", "PuedeCrear"))
                return RedirectToAction("AccesoDenegado", "Home");

            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "NombreCarrera");
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "DepartamentoId", "NombreDepartamento");
            ViewData["NivelId"] = new SelectList(_context.Nivels, "NivelId", "NivelDescrip");
            return View();
        }

        [Authorize(Roles = "DECANO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MateriaId,NivelId,DepartamentoId,CarreraId,NombreMateria")] CatalogoMateria catalogoMateria)
        {
            if (!TienePermiso("CatalogoMaterias", "PuedeCrear"))
                return RedirectToAction("AccesoDenegado", "Home");

            ModelState.Remove("Carrera");
            ModelState.Remove("Departamento");
            ModelState.Remove("Nivel");
            ModelState.Remove("NrcMateria");

            if (ModelState.IsValid)
            {
                _context.Add(catalogoMateria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "NombreCarrera", catalogoMateria.CarreraId);
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "DepartamentoId", "NombreDepartamento", catalogoMateria.DepartamentoId);
            ViewData["NivelId"] = new SelectList(_context.Nivels, "NivelId", "NivelDescrip", catalogoMateria.NivelId);
            return View(catalogoMateria);
        }

        [Authorize(Roles = "DECANO")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!TienePermiso("CatalogoMaterias", "PuedeEditar"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null || _context.CatalogoMaterias == null) return NotFound();
            var catalogoMateria = await _context.CatalogoMaterias.FindAsync(id);
            if (catalogoMateria == null) return NotFound();

            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "NombreCarrera", catalogoMateria.CarreraId);
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "DepartamentoId", "NombreDepartamento", catalogoMateria.DepartamentoId);
            ViewData["NivelId"] = new SelectList(_context.Nivels, "NivelId", "NivelDescrip", catalogoMateria.NivelId);
            return View(catalogoMateria);
        }

        [Authorize(Roles = "DECANO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MateriaId,NivelId,DepartamentoId,CarreraId,NombreMateria")] CatalogoMateria catalogoMateria)
        {
            if (!TienePermiso("CatalogoMaterias", "PuedeEditar"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id != catalogoMateria.MateriaId) return NotFound();

            ModelState.Remove("Carrera");
            ModelState.Remove("Departamento");
            ModelState.Remove("Nivel");
            ModelState.Remove("NrcMateria");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(catalogoMateria);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                }
            }
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "NombreCarrera", catalogoMateria.CarreraId);
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "DepartamentoId", "NombreDepartamento", catalogoMateria.DepartamentoId);
            ViewData["NivelId"] = new SelectList(_context.Nivels, "NivelId", "NivelDescrip", catalogoMateria.NivelId);
            return View(catalogoMateria);
        }

        [Authorize(Roles = "DECANO")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!TienePermiso("CatalogoMaterias", "PuedeEliminar"))
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null || _context.CatalogoMaterias == null) return NotFound();
            var catalogoMateria = await _context.CatalogoMaterias
                .Include(c => c.Carrera).Include(c => c.Departamento).Include(c => c.Nivel)
                .FirstOrDefaultAsync(m => m.MateriaId == id);
            if (catalogoMateria == null) return NotFound();
            return View(catalogoMateria);
        }

        [Authorize(Roles = "DECANO")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!TienePermiso("CatalogoMaterias", "PuedeEliminar"))
                return RedirectToAction("AccesoDenegado", "Home");

            var catalogoMateria = await _context.CatalogoMaterias.FindAsync(id);
            if (catalogoMateria != null) _context.CatalogoMaterias.Remove(catalogoMateria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
