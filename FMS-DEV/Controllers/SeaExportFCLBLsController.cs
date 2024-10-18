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

namespace FMS_DEV.Controllers
{
    public class SeaExportFCLBLsController : Controller
    {
        private readonly FtlcolombOperationContext _context;

        public SeaExportFCLBLsController(FtlcolombOperationContext context)
        {
            _context = context;
        }


        // GET: PnL
        public async Task<IActionResult> Index()
        {
            var fclBLs = await _context.TxnFCLBLs.ToListAsync();

            // Pass the data to the view
            return View(fclBLs);
        }


        public IActionResult Create(string searchString)
        {
            var FcljobNo = "xxx";

            if (!string.IsNullOrEmpty(searchString))
            {
                FcljobNo = searchString;
                if (_context.TxnFCLJobs.Any(t => t.JobNo == FcljobNo))
                {
                    ViewData["FcljobFound"] = "";
                }
                else
                {
                    ViewData["FcljobFound"] = "Fcljob Number: " + searchString + " not found";
                }
            }

            var tables = new SeaExportFCLBLViewModel
            {
                FCLBLsMulti = new List<TxnFCLBL>(),
                FCLJobsMulti = _context.TxnFCLJobs
                .Where(t => t.JobNo == FcljobNo)
                .ToList()
            };

            ViewData["FCLContainer"] = new SelectList(_context.TxnFCLJobContainers.OrderBy(p => p.ContainerNo), "SerialNo", "ContainerNo", "SerialNo");
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



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string dtlItemsList, [Bind("JobNo,BLNo,BLTypeID,BLStatus,ShipmentTerm,GrossWeight,CBM,NetWeight,NoofPackages,PackageType,Commodity,HSCode,ShipperID,ShippreName,ShippreAddress1,ShippreAddress2,ShippreAddress3,ShipperCity,ShipperCountry,ShipperTel,ShipperEmail,ConsigneeName,ConsigneeAddress1,ConsigneeAddress2,ConsigneeAddress3,ConsigneeCity,ConsigneeCountry,ConsigneeTel,ConsigneeEmail,NotifyPartyName,NotifyPartyAddress1,NotifyPartyAddress2,NotifyPartyAddress3,NotifyPartyCity,NotifyPartyCountry,NotifyPartyTel,NotifyPartyEmail,Freight,Forwading,Forwader,LocalCharges,MarksAndNos,Description,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,CanceledBy,CanceledDateTime,CanceledReson")] TxnFCLBL txnFCLBL)
        {
            // Check if JobNo is null or empty
            if (string.IsNullOrEmpty(txnFCLBL.JobNo))
            {
                ModelState.AddModelError("JobNo", "JobNo cannot be null or empty.");
                return View(txnFCLBL); // Return view with the current model if JobNo is missing
            }

            ModelState.Remove("dtlItemsList"); // To ignore validation on dtlItemsList

            if (ModelState.IsValid)
            {
                // Deserialize and save container records
                if (!string.IsNullOrWhiteSpace(dtlItemsList))
                {
                    var containerData = JsonConvert.DeserializeObject<List<TxnFCLJobContainers>>(dtlItemsList);
                    if (containerData != null)
                    {
                        foreach (var item in containerData)
                        {
                            item.JobNo = txnFCLBL.JobNo; // Assign JobNo to each container
                            var existingEntity = _context.TxnFCLJobContainers
                                .Local
                                .FirstOrDefault(e => e.JobNo == item.JobNo && e.SerialNo == item.SerialNo);

                            if (existingEntity != null)
                            {
                                // Update existing container
                                existingEntity.ContainerNo = item.ContainerNo;
                                existingEntity.Size = item.Size;
                                existingEntity.Seal = item.Seal;
                                existingEntity.LastUpdatedDateTime = DateTime.Now;
                            }
                            else
                            {
                                // Add new container
                                item.CreatedDateTime = DateTime.Now;
                                item.LastUpdatedDateTime = DateTime.Now;
                                _context.TxnFCLJobContainers.Add(item);
                            }
                        }
                    }
                }

                // Save main BL record
                _context.Add(txnFCLBL); // This saves to the table mapped to TxnFCLBL
                await _context.SaveChangesAsync(); // Save changes to the database
                return RedirectToAction(nameof(Index)); // Redirect to index after saving
            }

            // Reload ViewData for the view in case of model errors
            var viewModel = new SeaExportFCLBLViewModel
            {
                FCLJobsMulti = _context.TxnFCLJobs.ToList(),
                FCLJobContainersMulti = _context.TxnFCLJobContainers.ToList(),
                FCLBLsMulti = _context.TxnFCLBLs.ToList()
            };

            // Load necessary ViewData for dropdown lists, etc.
            ViewData["IntendedVessel"] = new SelectList(_context.RefVessels.Where(v => v.IsActive).OrderBy(s => s.VesselName), "VesselId", "VesselName");
            ViewData["PortId"] = new SelectList(_context.RefPorts.OrderBy(p => p.PortName), "PortCode", "PortName");
            ViewData["Customers"] = new SelectList(_context.RefCustomers.OrderBy(c => c.Name), "CustomerId", "Name");
            ViewData["ShippingLines"] = new SelectList(_context.RefShippingLines.OrderBy(c => c.Name), "ShippingLineId", "Name");
            ViewData["Agents"] = new SelectList(_context.RefAgents.OrderBy(c => c.AgentName), "AgentId", "AgentName");
            ViewData["HandleBy"] = new SelectList(_context.RefStaffs.Where(h => h.IsSalesPerson == false && h.IsActive).OrderBy(st => st.StaffName), "SatffId", "StaffName");
            ViewData["SalesPerson"] = new SelectList(_context.RefStaffs.Where(s => s.IsSalesPerson && s.IsActive).OrderBy(sl => sl.StaffName), "SatffId", "StaffName");
            ViewData["AgentIDNomination"] = new SelectList(_context.RefAgents.Join(_context.RefPorts,
                                                a => a.PortId,
                                                b => b.PortCode,
                                                (a, b) => new
                                                {
                                                    AgentId = a.AgentId,
                                                    AgentName = a.AgentName + " - " + b.PortName,
                                                    IsActive = a.IsActive
                                                }).Where(a => a.IsActive).OrderBy(a => a.AgentName), "AgentId", "AgentName");

            return View(viewModel); // Return the view with errors and necessary data
        }














    }
}
