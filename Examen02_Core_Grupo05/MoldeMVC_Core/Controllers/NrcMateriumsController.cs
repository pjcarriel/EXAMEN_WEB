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
    public class NrcMateriumsController : Controller
    {
        private readonly CatalogoMateriaBdCoreContext _context;

        public NrcMateriumsController(CatalogoMateriaBdCoreContext context)
        {
            _context = context;
        }

        // GET: NrcMateriums
        public async Task<IActionResult> Index()
        {
            var catalogoMateriaBdCoreContext = _context.NrcMateria.Include(n => n.Materia).Include(n => n.Profesor);
            return View(await catalogoMateriaBdCoreContext.ToListAsync());
        }

        // GET: NrcMateriums/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.NrcMateria == null)
            {
                return NotFound();
            }

            var nrcMaterium = await _context.NrcMateria
                .Include(n => n.Materia)
                .Include(n => n.Profesor)
                .FirstOrDefaultAsync(m => m.Nrc == id);
            if (nrcMaterium == null)
            {
                return NotFound();
            }

            return View(nrcMaterium);
        }

        // GET: NrcMateriums/Create
        public IActionResult Create()
        {
            ViewData["MateriaId"] = new SelectList(_context.CatalogoMaterias, "MateriaId", "MateriaId");
            ViewData["ProfesorId"] = new SelectList(_context.Profesors, "ProfesorId", "ProfesorId");
            return View();
        }

        // POST: NrcMateriums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nrc,MateriaId,ProfesorId")] NrcMaterium nrcMaterium)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nrcMaterium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MateriaId"] = new SelectList(_context.CatalogoMaterias, "MateriaId", "MateriaId", nrcMaterium.MateriaId);
            ViewData["ProfesorId"] = new SelectList(_context.Profesors, "ProfesorId", "ProfesorId", nrcMaterium.ProfesorId);
            return View(nrcMaterium);
        }

        // GET: NrcMateriums/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.NrcMateria == null)
            {
                return NotFound();
            }

            var nrcMaterium = await _context.NrcMateria.FindAsync(id);
            if (nrcMaterium == null)
            {
                return NotFound();
            }
            ViewData["MateriaId"] = new SelectList(_context.CatalogoMaterias, "MateriaId", "MateriaId", nrcMaterium.MateriaId);
            ViewData["ProfesorId"] = new SelectList(_context.Profesors, "ProfesorId", "ProfesorId", nrcMaterium.ProfesorId);
            return View(nrcMaterium);
        }

        // POST: NrcMateriums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Nrc,MateriaId,ProfesorId")] NrcMaterium nrcMaterium)
        {
            if (id != nrcMaterium.Nrc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nrcMaterium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NrcMateriumExists(nrcMaterium.Nrc))
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
            ViewData["MateriaId"] = new SelectList(_context.CatalogoMaterias, "MateriaId", "MateriaId", nrcMaterium.MateriaId);
            ViewData["ProfesorId"] = new SelectList(_context.Profesors, "ProfesorId", "ProfesorId", nrcMaterium.ProfesorId);
            return View(nrcMaterium);
        }

        // GET: NrcMateriums/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.NrcMateria == null)
            {
                return NotFound();
            }

            var nrcMaterium = await _context.NrcMateria
                .Include(n => n.Materia)
                .Include(n => n.Profesor)
                .FirstOrDefaultAsync(m => m.Nrc == id);
            if (nrcMaterium == null)
            {
                return NotFound();
            }

            return View(nrcMaterium);
        }

        // POST: NrcMateriums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.NrcMateria == null)
            {
                return Problem("Entity set 'CatalogoMateriaBdCoreContext.NrcMateria'  is null.");
            }
            var nrcMaterium = await _context.NrcMateria.FindAsync(id);
            if (nrcMaterium != null)
            {
                _context.NrcMateria.Remove(nrcMaterium);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NrcMateriumExists(string id)
        {
          return (_context.NrcMateria?.Any(e => e.Nrc == id)).GetValueOrDefault();
        }
    }
}
