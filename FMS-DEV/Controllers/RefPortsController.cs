using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FMS_DEV.Models;
using FMS_DEV.Data;
using Microsoft.AspNetCore.Authorization;

namespace FMS_DEV.Controllers
{
    [Authorize]
    public class RefPortsController : Controller
    {
        private readonly FtlcolombOperationContext _context;

        public RefPortsController(FtlcolombOperationContext context)
        {
            _context = context;
        }

        // GET: RefPorts
        public async Task<IActionResult> Index(string searchString, int pg = 1)
        {
            //List<RefPort> ports = await _context.RefPorts.ToListAsync();

            var ports = from p in _context.RefPorts select p;
            if (!String.IsNullOrEmpty(searchString))
            {
          
                ports = ports.Where(ports => ports.PortName.Contains(searchString));
            }

            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int recsCount = ports.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = ports.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;
            return View(data);
            //return View(await _context.RefPorts.ToListAsync());
        }

        // GET: RefPorts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.RefPorts == null)
            {
                return NotFound();
            }

            var refPort = await _context.RefPorts
                .FirstOrDefaultAsync(m => m.PortCode == id);
            if (refPort == null)
            {
                return NotFound();
            }

            return View(refPort);
        }

        // GET: RefPorts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RefPorts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PortCode,PortName,Country,Custom")] RefPort refPort)
        {
            var TableID = "Ref_Ports";  // Table ID in the Ref_Last
            var refLastNumber = await _context.RefLastNumbers.FindAsync(TableID);
            if (refLastNumber != null)
            {
                var nextNumber = refLastNumber.LastNumber + 1;
                refLastNumber.LastNumber = nextNumber;
                var IDNumber = "P" + nextNumber.ToString().PadLeft(4, '0');
                refPort.PortCode = IDNumber;

                // _context.RefLastNumbers.Remove(refLastNumber);
            }
            else
            {
                return View(refPort);
            }
            refPort.IsActive = true;
            refPort.CreatedBy = "Admin";
            refPort.CreatedDateTime = DateTime.Now;

            ModelState.Remove("PortCode");

            if (ModelState.IsValid)
            {
                _context.Add(refPort);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(refPort);
        }

        // GET: RefPorts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.RefPorts == null)
            {
                return NotFound();
            }

            var refPort = await _context.RefPorts.FindAsync(id);
            if (refPort == null)
            {
                return NotFound();
            }
            return View(refPort);
        }

        // POST: RefPorts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PortCode,PortName,Country,Custom, IsActive")] RefPort refPort)
        {
            if (id != refPort.PortCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(refPort);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RefPortExists(refPort.PortCode))
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
            return View(refPort);
        }

        // GET: RefPorts/Delete/5
       
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.RefPorts == null)
            {
                return NotFound();
            }

            var refPort = await _context.RefPorts
                .FirstOrDefaultAsync(m => m.PortCode == id);
            if (refPort == null)
            {
                return NotFound();
            }

            return View(refPort);
        }

        // POST: RefPorts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.RefPorts == null)
            {
                return Problem("Entity set 'FtlcolombOperationContext.RefPorts'  is null.");
            }
            var refPort = await _context.RefPorts.FindAsync(id);
            if (refPort != null)
            {
                _context.RefPorts.Remove(refPort);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RefPortExists(string id)
        {
          return _context.RefPorts.Any(e => e.PortCode == id);
        }
    }
}
