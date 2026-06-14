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
    public class PersonasController : Controller
    {
        private readonly CatalogoMateriaBdCoreContext _context;

        public PersonasController(CatalogoMateriaBdCoreContext context)
        {
            _context = context;
        }

        // GET: Personas
        public async Task<IActionResult> Index()
        {
            var catalogoMateriaBdCoreContext = _context.Personas.Include(p => p.RolPersona);
            return View(await catalogoMateriaBdCoreContext.ToListAsync());
        }

        // GET: Personas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Personas == null)
            {
                return NotFound();
            }

            var persona = await _context.Personas
                .Include(p => p.RolPersona)
                .FirstOrDefaultAsync(m => m.PersonalId == id);
            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        // GET: Personas/Create
        public IActionResult Create()
        {
            ViewData["RolPersonaId"] = new SelectList(_context.Rolpersonas, "RolPersonaId", "RolNombre");
            return View();
        }

        // POST: Personas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonalId,CedulaPersona,RolPersonaId,NombrePersona,ApellidoPersona,Foto")] Persona persona)
        {
            ModelState.Remove("RolPersona");
            ModelState.Remove("Estudiantes");
            ModelState.Remove("Profesors");

            if (ModelState.IsValid)
            {
                _context.Add(persona);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RolPersonaId"] = new SelectList(_context.Rolpersonas, "RolPersonaId", "RolNombre", persona.RolPersonaId);
            return View(persona);
        }

        // GET: Personas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Personas == null)
            {
                return NotFound();
            }

            var persona = await _context.Personas.FindAsync(id);
            if (persona == null)
            {
                return NotFound();
            }
            ViewData["RolPersonaId"] = new SelectList(_context.Rolpersonas, "RolPersonaId", "RolNombre", persona.RolPersonaId);
            return View(persona);
        }

        // POST: Personas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonalId,CedulaPersona,RolPersonaId,NombrePersona,ApellidoPersona,Foto")] Persona persona)
        {
            if (id != persona.PersonalId)
            {
                return NotFound();
            }

            ModelState.Remove("RolPersona");
            ModelState.Remove("Estudiantes");
            ModelState.Remove("Profesors");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(persona);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                }
            }
            ViewData["RolPersonaId"] = new SelectList(_context.Rolpersonas, "RolPersonaId", "RolNombre", persona.RolPersonaId);
            return View(persona);
        }

        // GET: Personas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Personas == null)
            {
                return NotFound();
            }

            var persona = await _context.Personas
                .Include(p => p.RolPersona)
                .FirstOrDefaultAsync(m => m.PersonalId == id);
            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Personas == null)
            {
                return Problem("Entity set 'CatalogoMateriaBdCoreContext.Personas'  is null.");
            }
            var persona = await _context.Personas.FindAsync(id);
            if (persona != null)
            {
                _context.Personas.Remove(persona);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonaExists(int id)
        {
          return (_context.Personas?.Any(e => e.PersonalId == id)).GetValueOrDefault();
        }
    }
}
