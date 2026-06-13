using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoldeMVC_Core.Models;

namespace MoldeMVC_Core.Controllers
{
    public class EstudiantesController : Controller
    {
        private readonly CatalogoMateriaBdCoreContext _context;

        public EstudiantesController(CatalogoMateriaBdCoreContext context)
        {
            _context = context;
        }

        // GET: Estudiantes
        public async Task<IActionResult> Index()
        {
            var catalogoMateriaBdCoreContext = _context.Estudiantes.Include(e => e.Carrera).Include(e => e.NivelActualEstNavigation).Include(e => e.Personal);
            return View(await catalogoMateriaBdCoreContext.ToListAsync());
        }

        // GET: Estudiantes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Estudiantes == null)
            {
                return NotFound();
            }

            var estudiante = await _context.Estudiantes
                .Include(e => e.Carrera)
                .Include(e => e.NivelActualEstNavigation)
                .Include(e => e.Personal)
                .FirstOrDefaultAsync(m => m.EstudianteId == id);
            if (estudiante == null)
            {
                return NotFound();
            }

            return View(estudiante);
        }

        // GET: Estudiantes/Create
        public IActionResult Create()
        {
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "CarreraId");
            ViewData["NivelActualEst"] = new SelectList(_context.Nivels, "NivelId", "NivelId");
            ViewData["PersonalId"] = new SelectList(_context.Personas, "PersonalId", "PersonalId");
            return View();
        }

        // POST: Estudiantes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EstudianteId,PersonalId,CarreraId,NivelActualEst")] Estudiante estudiante)
        {
            if (ModelState.IsValid)
            {
                _context.Add(estudiante);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "CarreraId", estudiante.CarreraId);
            ViewData["NivelActualEst"] = new SelectList(_context.Nivels, "NivelId", "NivelId", estudiante.NivelActualEst);
            ViewData["PersonalId"] = new SelectList(_context.Personas, "PersonalId", "PersonalId", estudiante.PersonalId);
            return View(estudiante);
        }

        // GET: Estudiantes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Estudiantes == null)
            {
                return NotFound();
            }

            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante == null)
            {
                return NotFound();
            }
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "CarreraId", estudiante.CarreraId);
            ViewData["NivelActualEst"] = new SelectList(_context.Nivels, "NivelId", "NivelId", estudiante.NivelActualEst);
            ViewData["PersonalId"] = new SelectList(_context.Personas, "PersonalId", "PersonalId", estudiante.PersonalId);
            return View(estudiante);
        }

        // POST: Estudiantes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EstudianteId,PersonalId,CarreraId,NivelActualEst")] Estudiante estudiante)
        {
            if (id != estudiante.EstudianteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estudiante);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstudianteExists(estudiante.EstudianteId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "CarreraId", estudiante.CarreraId);
            ViewData["NivelActualEst"] = new SelectList(_context.Nivels, "NivelId", "NivelId", estudiante.NivelActualEst);
            ViewData["PersonalId"] = new SelectList(_context.Personas, "PersonalId", "PersonalId", estudiante.PersonalId);
            return View(estudiante);
        }

        // GET: Estudiantes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Estudiantes == null)
            {
                return NotFound();
            }

            var estudiante = await _context.Estudiantes
                .Include(e => e.Carrera)
                .Include(e => e.NivelActualEstNavigation)
                .Include(e => e.Personal)
                .FirstOrDefaultAsync(m => m.EstudianteId == id);
            if (estudiante == null)
            {
                return NotFound();
            }

            return View(estudiante);
        }

        // POST: Estudiantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Estudiantes == null)
            {
                return Problem("Entity set 'CatalogoMateriaBdCoreContext.Estudiantes'  is null.");
            }
            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante != null)
            {
                _context.Estudiantes.Remove(estudiante);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstudianteExists(int id)
        {
          return (_context.Estudiantes?.Any(e => e.EstudianteId == id)).GetValueOrDefault();
        }
    }
}
