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
    public class MatriculasController : Controller
    {
        private readonly CatalogoMateriaBdCoreContext _context;

        public MatriculasController(CatalogoMateriaBdCoreContext context)
        {
            _context = context;
        }

        // GET: Matriculas
        public async Task<IActionResult> Index()
        {
            var catalogoMateriaBdCoreContext = _context.Matriculas.Include(m => m.Estudiante).Include(m => m.NrcNavigation);
            return View(await catalogoMateriaBdCoreContext.ToListAsync());
        }

        // GET: Matriculas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Matriculas == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matriculas
                .Include(m => m.Estudiante)
                .Include(m => m.NrcNavigation)
                .FirstOrDefaultAsync(m => m.RegistroId == id);
            if (matricula == null)
            {
                return NotFound();
            }

            return View(matricula);
        }

        // GET: Matriculas/Create
        public IActionResult Create()
        {
            ViewData["EstudianteId"] = new SelectList(_context.Estudiantes, "EstudianteId", "EstudianteId");
            ViewData["Nrc"] = new SelectList(_context.NrcMateria, "Nrc", "Nrc");
            return View();
        }

        // POST: Matriculas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistroId,Nrc,EstudianteId")] Matricula matricula)
        {
            if (ModelState.IsValid)
            {
                _context.Add(matricula);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EstudianteId"] = new SelectList(_context.Estudiantes, "EstudianteId", "EstudianteId", matricula.EstudianteId);
            ViewData["Nrc"] = new SelectList(_context.NrcMateria, "Nrc", "Nrc", matricula.Nrc);
            return View(matricula);
        }

        // GET: Matriculas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Matriculas == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula == null)
            {
                return NotFound();
            }
            ViewData["EstudianteId"] = new SelectList(_context.Estudiantes, "EstudianteId", "EstudianteId", matricula.EstudianteId);
            ViewData["Nrc"] = new SelectList(_context.NrcMateria, "Nrc", "Nrc", matricula.Nrc);
            return View(matricula);
        }

        // POST: Matriculas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegistroId,Nrc,EstudianteId")] Matricula matricula)
        {
            if (id != matricula.RegistroId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(matricula);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatriculaExists(matricula.RegistroId))
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
            ViewData["EstudianteId"] = new SelectList(_context.Estudiantes, "EstudianteId", "EstudianteId", matricula.EstudianteId);
            ViewData["Nrc"] = new SelectList(_context.NrcMateria, "Nrc", "Nrc", matricula.Nrc);
            return View(matricula);
        }

        // GET: Matriculas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Matriculas == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matriculas
                .Include(m => m.Estudiante)
                .Include(m => m.NrcNavigation)
                .FirstOrDefaultAsync(m => m.RegistroId == id);
            if (matricula == null)
            {
                return NotFound();
            }

            return View(matricula);
        }

        // POST: Matriculas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Matriculas == null)
            {
                return Problem("Entity set 'CatalogoMateriaBdCoreContext.Matriculas'  is null.");
            }
            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula != null)
            {
                _context.Matriculas.Remove(matricula);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatriculaExists(int id)
        {
          return (_context.Matriculas?.Any(e => e.RegistroId == id)).GetValueOrDefault();
        }
    }
}
