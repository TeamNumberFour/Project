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
    public class UniversitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UniversitiesController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Universities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Universities.ToListAsync());
        }

        // GET: Universities/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var university = await _context.Universities
                .SingleOrDefaultAsync(m => m.Id == id);
            if (university == null)
            {
                return NotFound();
            }

            return View(university);
        }

        // GET: Universities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Universities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] University university)
        {
            if (ModelState.IsValid)
            {
                university.Id = Guid.NewGuid();
                _context.Add(university);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(university);
        }

        // GET: Universities/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var university = await _context.Universities.SingleOrDefaultAsync(m => m.Id == id);
            if (university == null)
            {
                return NotFound();
            }
            return View(university);
        }

        // POST: Universities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title")] University university)
        {
            if (id != university.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(university);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UniversityExists(university.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(university);
        }

        // GET: Universities/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var university = await _context.Universities
                .SingleOrDefaultAsync(m => m.Id == id);
            if (university == null)
            {
                return NotFound();
            }

            return View(university);
        }

        // POST: Universities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var university = await _context.Universities.SingleOrDefaultAsync(m => m.Id == id);
            _context.Universities.Remove(university);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool UniversityExists(Guid id)
        {
            return _context.Universities.Any(e => e.Id == id);
        }
    }
}
