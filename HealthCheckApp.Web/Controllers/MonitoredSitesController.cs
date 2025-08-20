using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthCheckApp.Web.Models;

namespace HealthCheckApp.Web.Controllers
{
    public class MonitoredSitesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MonitoredSitesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MonitoredSites
        public async Task<IActionResult> Index()
        {
            return View(await _context.MonitoredSites.ToListAsync());
        }

        // GET: MonitoredSites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monitoredSite = await _context.MonitoredSites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (monitoredSite == null)
            {
                return NotFound();
            }

            return View(monitoredSite);
        }

        // GET: MonitoredSites/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MonitoredSites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Url,Name,CheckIntervalSeconds,IsUp,LastChecked,LastDownTime,UserEmail")] MonitoredSite monitoredSite)
        {
            if (ModelState.IsValid)
            {
                _context.Add(monitoredSite);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(monitoredSite);
        }

        // GET: MonitoredSites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monitoredSite = await _context.MonitoredSites.FindAsync(id);
            if (monitoredSite == null)
            {
                return NotFound();
            }
            return View(monitoredSite);
        }

        // POST: MonitoredSites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Url,Name,CheckIntervalSeconds,IsUp,LastChecked,LastDownTime,UserEmail")] MonitoredSite monitoredSite)
        {
            if (id != monitoredSite.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(monitoredSite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonitoredSiteExists(monitoredSite.Id))
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
            return View(monitoredSite);
        }

        // GET: MonitoredSites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monitoredSite = await _context.MonitoredSites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (monitoredSite == null)
            {
                return NotFound();
            }

            return View(monitoredSite);
        }

        // POST: MonitoredSites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monitoredSite = await _context.MonitoredSites.FindAsync(id);
            if (monitoredSite != null)
            {
                _context.MonitoredSites.Remove(monitoredSite);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MonitoredSiteExists(int id)
        {
            return _context.MonitoredSites.Any(e => e.Id == id);
        }
    }
}
