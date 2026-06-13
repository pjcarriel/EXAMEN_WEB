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
    public class CatalogoMateriasController : Controller
    {
        private readonly CatalogoMateriaBdCoreContext _context;

        public CatalogoMateriasController(CatalogoMateriaBdCoreContext context)
        {
            _context = context;
        }

        // GET: CatalogoMaterias
        public async Task<IActionResult> Index()
        {
            var catalogoMateriaBdCoreContext = _context.CatalogoMaterias.Include(c => c.Carrera).Include(c => c.Departamento).Include(c => c.Nivel);
            return View(await catalogoMateriaBdCoreContext.ToListAsync());
        }

        // GET: CatalogoMaterias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CatalogoMaterias == null)
            {
                return NotFound();
            }

            var catalogoMateria = await _context.CatalogoMaterias
                .Include(c => c.Carrera)
                .Include(c => c.Departamento)
                .Include(c => c.Nivel)
                .FirstOrDefaultAsync(m => m.MateriaId == id);
            if (catalogoMateria == null)
            {
                return NotFound();
            }

            return View(catalogoMateria);
        }

        // GET: CatalogoMaterias/Create
        public IActionResult Create()
        {
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "CarreraId");
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "DepartamentoId", "DepartamentoId");
            ViewData["NivelId"] = new SelectList(_context.Nivels, "NivelId", "NivelId");
            return View();
        }

        // POST: CatalogoMaterias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MateriaId,NivelId,DepartamentoId,CarreraId,NombreMateria")] CatalogoMateria catalogoMateria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(catalogoMateria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "CarreraId", catalogoMateria.CarreraId);
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "DepartamentoId", "DepartamentoId", catalogoMateria.DepartamentoId);
            ViewData["NivelId"] = new SelectList(_context.Nivels, "NivelId", "NivelId", catalogoMateria.NivelId);
            return View(catalogoMateria);
        }

        // GET: CatalogoMaterias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CatalogoMaterias == null)
            {
                return NotFound();
            }

            var catalogoMateria = await _context.CatalogoMaterias.FindAsync(id);
            if (catalogoMateria == null)
            {
                return NotFound();
            }
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "CarreraId", catalogoMateria.CarreraId);
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "DepartamentoId", "DepartamentoId", catalogoMateria.DepartamentoId);
            ViewData["NivelId"] = new SelectList(_context.Nivels, "NivelId", "NivelId", catalogoMateria.NivelId);
            return View(catalogoMateria);
        }

        // POST: CatalogoMaterias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MateriaId,NivelId,DepartamentoId,CarreraId,NombreMateria")] CatalogoMateria catalogoMateria)
        {
            if (id != catalogoMateria.MateriaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(catalogoMateria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatalogoMateriaExists(catalogoMateria.MateriaId))
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
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "CarreraId", "CarreraId", catalogoMateria.CarreraId);
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "DepartamentoId", "DepartamentoId", catalogoMateria.DepartamentoId);
            ViewData["NivelId"] = new SelectList(_context.Nivels, "NivelId", "NivelId", catalogoMateria.NivelId);
            return View(catalogoMateria);
        }

        // GET: CatalogoMaterias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CatalogoMaterias == null)
            {
                return NotFound();
            }

            var catalogoMateria = await _context.CatalogoMaterias
                .Include(c => c.Carrera)
                .Include(c => c.Departamento)
                .Include(c => c.Nivel)
                .FirstOrDefaultAsync(m => m.MateriaId == id);
            if (catalogoMateria == null)
            {
                return NotFound();
            }

            return View(catalogoMateria);
        }

        // POST: CatalogoMaterias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CatalogoMaterias == null)
            {
                return Problem("Entity set 'CatalogoMateriaBdCoreContext.CatalogoMaterias'  is null.");
            }
            var catalogoMateria = await _context.CatalogoMaterias.FindAsync(id);
            if (catalogoMateria != null)
            {
                _context.CatalogoMaterias.Remove(catalogoMateria);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CatalogoMateriaExists(int id)
        {
          return (_context.CatalogoMaterias?.Any(e => e.MateriaId == id)).GetValueOrDefault();
        }
    }
}
