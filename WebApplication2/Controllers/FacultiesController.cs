using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = ApplicationRoles.Administrators)]
    public class FacultiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacultiesController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Faculties
        public async Task<IActionResult> Index(Guid? UniversityId)
        {
            if (UniversityId == null)
            {
                return this.NotFound();
            }

            var University = await this._context.Universities
                .SingleOrDefaultAsync(x => x.Id == UniversityId);

            if (University == null)
            {
                return this.NotFound();
            }

            this.ViewBag.University = University;
            var faculties = await this._context.Faculties
                .Include(w => w.University)
                .Where(x => x.UniversityId == UniversityId)
                .ToListAsync();

            return this.View(faculties);
        }

        // GET: Faculties/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties
                .Include(f => f.University)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (faculty == null)
            {
                return NotFound();
            }

            return View(faculty);
        }

        // GET: Faculties/Create
        public async Task<IActionResult> Create(Guid? universityId)
        {
            if (universityId == null)
            {
                return this.NotFound();
            }

            var university = await this._context.Universities
                .SingleOrDefaultAsync(x => x.Id == universityId);

            if (university == null)
            {
                return this.NotFound();
            }

            this.ViewBag.University = university;
            
            return View();
        }

        // POST: Faculties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UniversityId,Title")] Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                faculty.Id = Guid.NewGuid();
                _context.Add(faculty);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { UniversityId = faculty.UniversityId });
            }
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "Title", faculty.UniversityId);
            return View(faculty);
        }

        // GET: Faculties/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties.SingleOrDefaultAsync(m => m.Id == id);
            if (faculty == null)
            {
                return NotFound();
            }
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "Title", faculty.UniversityId);
            return View(faculty);
        }

        // POST: Faculties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UniversityId,Title")] Faculty faculty)
        {
            if (id != faculty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faculty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyExists(faculty.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { UniversityId = faculty.UniversityId });
            }
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "Title", faculty.UniversityId);
            return View(faculty);
        }

        // GET: Faculties/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties
                .Include(f => f.University)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (faculty == null)
            {
                return NotFound();
            }

            return View(faculty);
        }

        // POST: Faculties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var faculty = await _context.Faculties.SingleOrDefaultAsync(m => m.Id == id);
            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { UniversityId = faculty.UniversityId });
        }

        private bool FacultyExists(Guid id)
        {
            return _context.Faculties.Any(e => e.Id == id);
        }
    }
}
