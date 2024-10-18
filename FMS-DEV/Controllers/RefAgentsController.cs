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
    public class RefAgentsController : Controller
    {
        private readonly FtlcolombOperationContext _context;

        public RefAgentsController(FtlcolombOperationContext context)
        {
            _context = context;
        }

        // GET: RefAgents
        public async Task<IActionResult> Index(string searchString, int pg = 1)
        {
            //var agents = _context.RefAgents.Include(r => r.Port);
            
            var agents = from ag in _context.RefAgents.Include(r => r.Port).OrderBy(s => s.AgentName) select ag;
            if (!String.IsNullOrEmpty(searchString))
            {
                agents = agents.Where(agents => agents.AgentName.Contains(searchString));
            }
            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int recsCount = agents.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = agents.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;
            return View(data);

            //return View(await agents.ToListAsync());
        }

        // GET: RefAgents/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.RefAgents == null)
            {
                return NotFound();
            }

            var refAgent = await _context.RefAgents
                .Include(r => r.Port)
                .FirstOrDefaultAsync(m => m.AgentId == id);
            if (refAgent == null)
            {
                return NotFound();
            }

            return View(refAgent);
        }

        // GET: RefAgents/Create
        public IActionResult Create()
        {
            ViewData["PortId"] = new SelectList(_context.RefPorts.OrderBy(p => p.PortName), "PortCode", "PortName", "PortCode");
            return View();
        }

        // POST: RefAgents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PortId,AgentName,Address1,Address2,Address3,City,County,TelNo,Email,Remarks, IsActive")] RefAgent refAgent)
        {
            var TableID = "Ref_Agent";  // Table ID in the Ref_Last
            var refLastNumber = await _context.RefLastNumbers.FindAsync(TableID);
            if (refLastNumber != null)
            {
                var nextNumber = refLastNumber.LastNumber + 1;
                refLastNumber.LastNumber = nextNumber;
                var IDNumber = "A" + nextNumber.ToString().PadLeft(5, '0');
                refAgent.AgentId = IDNumber;

                // _context.RefLastNumbers.Remove(refLastNumber);
            }
            else
            {
                return View(refAgent);
            }


            ModelState.Remove("Port");  // Port is the vrtual property in Model - RefAgent.cs
            ModelState.Remove("AgentId");
            refAgent.IsActive= true;
            refAgent.CreatedBy = "Admin";
            refAgent.CreatedDateTime = DateTime.Now;


            if (ModelState.IsValid)
            {
                _context.Add(refAgent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PortId"] = new SelectList(_context.RefPorts.OrderBy(p => p.PortName), "PortCode", "PortName", "PortCode");
            return View(refAgent);
        }

        // GET: RefAgents/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.RefAgents == null)
            {
                return NotFound();
            }

            var refAgent = await _context.RefAgents.FindAsync(id);
            if (refAgent == null)
            {
                return NotFound();
            }
            ViewData["PortId"] = new SelectList(_context.RefPorts.OrderBy(p => p.PortName), "PortCode", "PortName", "PortCode");
            return View(refAgent);
        }

        // POST: RefAgents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AgentId,PortId,AgentName,Address1,Address2,Address3,City,County,TelNo,Email,Remarks,IsActive")] RefAgent refAgent)
        {
            if (id != refAgent.AgentId)
            {
                return NotFound();
            }

            ModelState.Remove("Port");  // Port is the vrtual property in Model - RefAgent.cs

            refAgent.LastUpdatedDateTime = DateTime.Now;
            refAgent.LastUpdatedBy = "Admin";

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(refAgent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RefAgentExists(refAgent.AgentId))
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
            ViewData["PortId"] = new SelectList(_context.RefPorts.OrderBy(p => p.PortName), "PortCode", "PortName", "PortCode");
            return View(refAgent);
        }

        // GET: RefAgents/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.RefAgents == null)
            {
                return NotFound();
            }

            var refAgent = await _context.RefAgents
                .Include(r => r.Port)
                .FirstOrDefaultAsync(m => m.AgentId == id);
            if (refAgent == null)
            {
                return NotFound();
            }

            return View(refAgent);
        }

        // POST: RefAgents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.RefAgents == null)
            {
                return Problem("Entity set 'FtlcolombOperationContext.RefAgents'  is null.");
            }
            var refAgent = await _context.RefAgents.FindAsync(id);
            if (refAgent != null)
            {
                refAgent.IsActive= false; // Instead delete 
                _context.Update(refAgent);
                //_context.RefAgents.Remove(refAgent);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RefAgentExists(string id)
        {
          return _context.RefAgents.Any(e => e.AgentId == id);
        }
    }
}
