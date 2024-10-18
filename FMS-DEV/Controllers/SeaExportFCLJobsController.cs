using Microsoft.AspNetCore.Mvc;
using System.Linq;
using FMS_DEV.Models; // Add the appropriate namespace for your model class and DbContext.
using FMS_DEV.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FMS_DEV.Data;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using System.Numerics;
using FMS_DEV.CommonMethods;

namespace FMS_DEV.Controllers
{
    public class SeaExportFCLJobsController : Controller
    {
        private readonly FtlcolombOperationContext _context;

        public SeaExportFCLJobsController(FtlcolombOperationContext context)
        {
            _context = context;
        }



        //public async Task<IActionResult> Index()
        //{

        //    return View();
        //}


        public async Task<IActionResult> Index()
        {
            // Fetch the data from the database
            var fclJobs = await _context.TxnFCLJobs.ToListAsync(); 

            // Pass the data to the view
            return View(fclJobs);
        }


        public IActionResult Create(string searchString)
        {

            var tables = new SeaExportFCLJobsViewModel
            {

                FCLJobs = new List<TxnFCLJob>(),
                FCLJobContainers = new List<TxnFCLJobContainers>(),

            };

            
            ViewData["IntendedVessel"] = new SelectList(_context.RefVessels.Where(v => v.IsActive.Equals(true)).OrderBy(s => s.VesselName), "VesselId", "VesselName", "VesselId");
            ViewData["POD"] = new SelectList(_context.RefPorts.OrderBy(p => p.PortName), "PortCode", "PortName", "PortCode");
            ViewData["Customers"] = new SelectList(_context.RefCustomers.OrderBy(c => c.Name), "CustomerId", "Name", "CustomerId");
            ViewData["ShippingLines"] = new SelectList(_context.RefShippingLines.OrderBy(c => c.Name), "ShippingLineId", "Name", "ShippingLineId");
            ViewData["Agents"] = new SelectList(_context.RefAgents.OrderBy(c => c.AgentName), "AgentId", "AgentName", "AgentId");
            ViewData["HandleBy"] = new SelectList(_context.RefStaffs.Where(h => h.IsSalesPerson.Equals(false) && h.IsActive.Equals(true)).OrderBy(st => st.StaffName), "SatffId", "StaffName", "SatffId");
            ViewData["SalesPerson"] = new SelectList(_context.RefStaffs.Where(s => s.IsSalesPerson.Equals(true) && s.IsActive.Equals(true)).OrderBy(sl => sl.StaffName), "SatffId", "StaffName", "SatffId");
            ViewData["AgentIDNomination"] = new SelectList(_context.RefAgents.Join(_context.RefPorts,

                            a => a.PortId,
                            b => b.PortCode,
                            (a, b) => new
                            {
                                AgentId = a.AgentId,
                                AgentName = a.AgentName + " - " + b.PortName,
                                IsActive = a.IsActive
                            }).Where(a => a.IsActive.Equals(true)).OrderBy(a => a.AgentName), "AgentId", "AgentName", "AgentId");

            return View(tables);
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(string dtlItemsList, [Bind("JobNo,Date,JobType,ExchangeRate,HandleBy,SalesPersonID,ShipperID,ShippingLineID,MasterBL,VesselId,Voyage,ETD,POD,ETA,BLType,AgentId,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,CanceledBy,CanceledDateTime,CanceledReason")] TxnFCLJob txnFCLJob)
        //{
        //    var TableID = "Txn_FCLJob";
        //    var refLastNumber = await _context.RefLastNumbers.FindAsync(TableID);
        //    if (refLastNumber != null)
        //    {
        //        var nextNumber = refLastNumber.LastNumber + 1;
        //        refLastNumber.LastNumber = nextNumber;
        //        var IDNumber = "FCLJB" + nextNumber.ToString().PadLeft(4, '0');
        //        txnFCLJob.JobNo = IDNumber;

        //        _context.Update(refLastNumber);
        //        await _context.SaveChangesAsync();
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }

        //    ModelState.Remove("JobNo");


        //    if (ModelState.IsValid)
        //    {
        //        // Deserialize and add Container records
        //        if (!string.IsNullOrWhiteSpace(dtlItemsList))
        //        {
        //            var containerData = JsonConvert.DeserializeObject<List<TxnFCLJobContainers>>(dtlItemsList);
        //            if (containerData != null)
        //            {
        //                foreach (var item in containerData)
        //                {
        //                    item.JobNo = txnFCLJob.JobNo;
        //                    TxnFCLJobContainers DetailItem = new TxnFCLJobContainers();
        //                    DetailItem.SerialNo = item.SerialNo;
        //                    DetailItem.ContainerNo = item.ContainerNo;
        //                    DetailItem.Size = item.Size;
        //                    DetailItem.Seal = item.Seal;
        //                    item.CreatedDateTime = DateTime.Now;
        //                    item.LastUpdatedDateTime = DateTime.Now;
        //                    _context.TxnFCLJobContainers.Add(item);
        //                }
        //            }
        //        }

        //        _context.Add(txnFCLJob);
        //        _context.RefLastNumbers.Update(refLastNumber);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // Load ViewData for the view
        //    ViewData["IntendedVessel"] = new SelectList(_context.RefVessels.Where(v => v.IsActive.Equals(true)).OrderBy(s => s.VesselName), "VesselId", "VesselName", "VesselId");
        //    ViewData["PortId"] = new SelectList(_context.RefPorts.OrderBy(p => p.PortName), "PortCode", "PortName", "PortCode");
        //    ViewData["Customers"] = new SelectList(_context.RefCustomers.OrderBy(c => c.Name), "CustomerId", "Name", "CustomerId");
        //    ViewData["ShippingLines"] = new SelectList(_context.RefShippingLines.OrderBy(c => c.Name), "ShippingLineId", "Name", "ShippingLineId");
        //    ViewData["Agents"] = new SelectList(_context.RefAgents.OrderBy(c => c.AgentName), "AgentId", "AgentName", "AgentId");
        //    ViewData["HandleBy"] = new SelectList(_context.RefStaffs.Where(h => h.IsSalesPerson.Equals(false) && h.IsActive.Equals(true)).OrderBy(st => st.StaffName), "SatffId", "StaffName", "SatffId");
        //    ViewData["SalesPerson"] = new SelectList(_context.RefStaffs.Where(s => s.IsSalesPerson.Equals(true) && s.IsActive.Equals(true)).OrderBy(sl => sl.StaffName), "SatffId", "StaffName", "SatffId");
        //    ViewData["AgentIDNomination"] = new SelectList(_context.RefAgents.Join(_context.RefPorts,
        //                        a => a.PortId,
        //                        b => b.PortCode,
        //                        (a, b) => new
        //                        {
        //                            AgentId = a.AgentId,
        //                            AgentName = a.AgentName + " - " + b.PortName,
        //                            IsActive = a.IsActive
        //                        }).Where(a => a.IsActive.Equals(true)).OrderBy(a => a.AgentName), "AgentId", "AgentName", "AgentId");

        //    return View();
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string dtlItemsList, [Bind("JobNo,Date,JobType,ExchangeRate,HandleBy,SalesPersonID,ShipperID,ShippingLineID,MasterBL,VesselId,Voyage,ETD,POD,ETA,BLType,AgentId,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,CanceledBy,CanceledDateTime,CanceledReason")] TxnFCLJob txnFCLJob)
        {
            var TableID = "Txn_FCLJob";
            var refLastNumber = await _context.RefLastNumbers.FindAsync(TableID);
            if (refLastNumber != null)
            {
                var nextNumber = refLastNumber.LastNumber + 1;
                refLastNumber.LastNumber = nextNumber;
                var IDNumber = "FCLJB" + nextNumber.ToString().PadLeft(4, '0');
                txnFCLJob.JobNo = IDNumber;

                _context.Update(refLastNumber);
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            ModelState.Remove("JobNo");

            if (ModelState.IsValid)
            {
                // Deserialize and add Container records
                if (!string.IsNullOrWhiteSpace(dtlItemsList))
                {
                    var containerData = JsonConvert.DeserializeObject<List<TxnFCLJobContainers>>(dtlItemsList);
                    if (containerData != null)
                    {
                        foreach (var item in containerData)
                        {
                            item.JobNo = txnFCLJob.JobNo;
                            var existingEntity = _context.TxnFCLJobContainers
                                .Local
                                .FirstOrDefault(e => e.JobNo == item.JobNo && e.SerialNo == item.SerialNo);

                            if (existingEntity != null)
                            {
                                // Update existing entity
                                existingEntity.ContainerNo = item.ContainerNo;
                                existingEntity.Size = item.Size;
                                existingEntity.Seal = item.Seal;
                                existingEntity.LastUpdatedDateTime = DateTime.Now;
                            }
                            else
                            {
                                // Add new entity
                                item.CreatedDateTime = DateTime.Now;
                                item.LastUpdatedDateTime = DateTime.Now;
                                _context.TxnFCLJobContainers.Add(item);
                            }
                        }
                    }
                }

                _context.Add(txnFCLJob);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Load ViewData for the view
            ViewData["IntendedVessel"] = new SelectList(_context.RefVessels.Where(v => v.IsActive.Equals(true)).OrderBy(s => s.VesselName), "VesselId", "VesselName", "VesselId");
            ViewData["PortId"] = new SelectList(_context.RefPorts.OrderBy(p => p.PortName), "PortCode", "PortName", "PortCode");
            ViewData["Customers"] = new SelectList(_context.RefCustomers.OrderBy(c => c.Name), "CustomerId", "Name", "CustomerId");
            ViewData["ShippingLines"] = new SelectList(_context.RefShippingLines.OrderBy(c => c.Name), "ShippingLineId", "Name", "ShippingLineId");
            ViewData["Agents"] = new SelectList(_context.RefAgents.OrderBy(c => c.AgentName), "AgentId", "AgentName", "AgentId");
            ViewData["HandleBy"] = new SelectList(_context.RefStaffs.Where(h => h.IsSalesPerson.Equals(false) && h.IsActive.Equals(true)).OrderBy(st => st.StaffName), "SatffId", "StaffName", "SatffId");
            ViewData["SalesPerson"] = new SelectList(_context.RefStaffs.Where(s => s.IsSalesPerson.Equals(true) && s.IsActive.Equals(true)).OrderBy(sl => sl.StaffName), "SatffId", "StaffName", "SatffId");
            ViewData["AgentIDNomination"] = new SelectList(_context.RefAgents.Join(_context.RefPorts,
                                    a => a.PortId,
                                    b => b.PortCode,
                                    (a, b) => new
                                    {
                                        AgentId = a.AgentId,
                                        AgentName = a.AgentName + " - " + b.PortName,
                                        IsActive = a.IsActive
                                    }).Where(a => a.IsActive.Equals(true)).OrderBy(a => a.AgentName), "AgentId", "AgentName", "AgentId");

            return View();
        }






    }
}
