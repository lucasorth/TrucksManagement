using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Trucks.Data;
using Trucks.Models;

namespace Trucks.Controllers
{
    public class TrucksController : Controller
    {
        private readonly ILogger<TrucksController> _logger;
        private readonly TrucksDbContext _context;

        public TrucksController(TrucksDbContext context)
        {
            _context = context;
        }

        // GET: Trucks
        public async Task<IActionResult> Index()
        {
            return View("Index", await _context.Truck.ToListAsync());
        }

        // GET: Trucks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var truck = await _context.Truck.FirstOrDefaultAsync(m => m.Id == id);
            
            if (truck == null)
            {
                return NotFound();
            }

            return View(truck);
        }

        // GET: Trucks/Create
        public IActionResult Create()
        {
            return View("Create");
        }

        // POST: Trucks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Model,ManufacturingYear,ModelYear")] Truck truck)
        {
            if (ModelState.IsValid)
            {
                truck.CreationDate = DateTime.Now;
                truck.CreatedBy = string.IsNullOrEmpty(User?.Identity?.Name) ? "Guest" : User.Identity.Name;

                _context.Add(truck);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(truck);
        }

        // GET: Trucks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var truck = await _context.Truck.FindAsync(id);
            if (truck == null)
            {
                return NotFound();
            }
            return View(truck);
        }

        // POST: Trucks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Model,ManufacturingYear,ModelYear")] Truck truck)
        {
            if (id != truck.Id)
            {
                return NotFound();
            }

            var truckToUpdate = await _context.Truck.FirstOrDefaultAsync(s => s.Id == id);
            if (truckToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            { 
                truckToUpdate.Title = truck.Title;
                truckToUpdate.Model = truck.Model;
                truckToUpdate.ManufacturingYear = truck.ManufacturingYear;
                truckToUpdate.ModelYear = truck.ModelYear;
                    
                try
                {
                    _context.Update(truckToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TruckExists(truck.Id))
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

            return View(truckToUpdate);
        }

        // GET: Trucks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var truck = await _context.Truck
                .FirstOrDefaultAsync(m => m.Id == id);
            if (truck == null)
            {
                return NotFound();
            }

            return View(truck);
        }

        // POST: Trucks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var truck = await _context.Truck.FindAsync(id);
            _context.Truck.Remove(truck);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TruckExists(int id)
        {
            return _context.Truck.Any(e => e.Id == id);
        }
    }
}
