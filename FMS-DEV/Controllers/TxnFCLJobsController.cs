using Microsoft.AspNetCore.Mvc;
using System.Linq;
using FMS_DEV.Models; 
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
    public class TxnFCLJobsController : Controller
    {
        private readonly FtlcolombOperationContext _context;

        public TxnFCLJobsController(FtlcolombOperationContext context)
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
            var fclJobs = await _context.TxnFCLJobs.OrderByDescending(p => p.JobNo).ToListAsync();

            // Pass the data to the view
            return View(fclJobs);
        }


        public IActionResult Create(string searchString)
        {

            var tables = new TxnFCLJobsViewModel
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
            ViewData["ContainerSize"] = new SelectList(_context.RefContainerSizes.OrderBy(p => p.Description), "ContainerId", "Description", "ContainerId");
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



        // GET: TxnFCLJobs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            //if (string.IsNullOrEmpty(id))
            //{
            //    return NotFound();
            //}

            //var txnFCLJob = await _context.TxnFCLJobs
            //    .Include(t => t.FCLJobContainers) // Include related containers if needed
            //    .FirstOrDefaultAsync(m => m.JobNo == id); // Use id instead of id

            //if (txnFCLJob == null)
            //{
            //    return NotFound();
            //}

            if (id == null)
            {
                return NotFound();
            }

            var txnFCLJob = await _context.TxnFCLJobs.FindAsync(id);
            if (txnFCLJob == null)
            {
                return NotFound();
            }

            var viewModel = new TxnFCLJobsViewModel
            {
                FCLJobs = new List<TxnFCLJob> { txnFCLJob }, // Populate with the fetched job
                FCLJobContainers = await _context.TxnFCLJobContainers.Where(c => c.JobNo == id).ToListAsync()
            };

            // Load ViewData for dropdowns
            ViewBag.ContainerSize = new SelectList(_context.RefContainerSizes.OrderBy(p => p.Description), "ContainerId", "Description");

            ViewData["IntendedVessel"] = new SelectList(_context.RefVessels.Where(v => v.IsActive.Equals(true)).OrderBy(s => s.VesselName), "VesselId", "VesselName", txnFCLJob.VesselId);
            ViewData["POD"] = new SelectList(_context.RefPorts.OrderBy(p => p.PortName), "PortCode", "PortName", txnFCLJob.POD);
            ViewData["Customers"] = new SelectList(_context.RefCustomers.OrderBy(c => c.Name), "CustomerId", "Name", txnFCLJob.ShipperID);
            ViewData["ShippingLines"] = new SelectList(_context.RefShippingLines.OrderBy(c => c.Name), "ShippingLineId", "Name", txnFCLJob.ShippingLineID);
            ViewData["Agents"] = new SelectList(_context.RefAgents.OrderBy(c => c.AgentName), "AgentId", "AgentName", txnFCLJob.AgentId);
            ViewData["HandleBy"] = new SelectList(_context.RefStaffs.Where(h => h.IsSalesPerson.Equals(false) && h.IsActive.Equals(true)).OrderBy(st => st.StaffName), "SatffId", "StaffName", txnFCLJob.HandleBy);
            ViewData["SalesPerson"] = new SelectList(_context.RefStaffs.Where(s => s.IsSalesPerson.Equals(true) && s.IsActive.Equals(true)).OrderBy(sl => sl.StaffName), "SatffId", "StaffName", txnFCLJob.SalesPersonID);

            //return View(txnFCLJob);
            return View(viewModel);
        }


        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Edit(string id, string dtlItemsList,
        //        [Bind("JobNo,Date,JobType,ExchangeRate,HandleBy,SalesPersonID,ShipperID,ShippingLineID,MasterBL,VesselId,Voyage,ETD,POD,ETA,BLType,AgentId,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,CanceledBy,CanceledDateTime,CanceledReason")]
        //TxnFCLJob txnFCLJob)
        //    {
        //        if (id != txnFCLJob.JobNo)
        //        {
        //            return NotFound();
        //        }

        //        // Load the txnFCLJob including FCLJobContainers
        //        //txnFCLJob = await _context.TxnFCLJobs
        //        //    .Include(j => j.FCLJobContainers) 
        //        //    .FirstOrDefaultAsync(j => j.JobNo == id);

        //        // If txnFCLJob is null after attempting to load, return NotFound
        //        //if (txnFCLJob == null)
        //        //{
        //        //    return NotFound();
        //        //}

        //        txnFCLJob.LastUpdatedBy = "Admin";
        //        txnFCLJob.LastUpdatedDateTime = DateTime.Now;
        //        ModelState.Remove("FCLJobContainers");



        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                if (!string.IsNullOrWhiteSpace(dtlItemsList))
        //                {
        //                    var existingContainers = _context.TxnFCLJobContainers.Where(c => c.JobNo == txnFCLJob.JobNo);
        //                    if (existingContainers.Any())
        //                    {
        //                        _context.TxnFCLJobContainers.RemoveRange(existingContainers);
        //                    }

        //                    var containerData = JsonConvert.DeserializeObject<List<TxnFCLJobContainers>>(dtlItemsList);
        //                    if (containerData != null)
        //                    {
        //                        foreach (var item in containerData)
        //                        {
        //                            item.JobNo = txnFCLJob.JobNo;
        //                            item.CreatedDateTime = DateTime.Now;
        //                            // Check if item already exists
        //                            var existingContainer = existingContainers.FirstOrDefault(c => c.SerialNo == item.SerialNo);
        //                            if (existingContainer != null)
        //                            {
        //                                // Update the existing container
        //                                _context.Entry(existingContainer).CurrentValues.SetValues(item);
        //                            }
        //                            else
        //                            {
        //                                // Add a new container
        //                                _context.TxnFCLJobContainers.Add(item);
        //                            }
        //                            //_context.TxnFCLJobContainers.Add(item);
        //                        }
        //                    }
        //                }

        //                _context.Update(txnFCLJob);
        //                await _context.SaveChangesAsync();
        //                return RedirectToAction(nameof(Index));
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!TxnFCLJobExists(txnFCLJob.JobNo))
        //                {
        //                    return NotFound();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }
        //        }

        //        // Initialize ViewModel for the return view in case of an error
        //        var viewModel = new TxnFCLJobsViewModel
        //        {
        //            FCLJobs = new List<TxnFCLJob> { txnFCLJob },
        //            FCLJobContainers = await _context.TxnFCLJobContainers.Where(c => c.JobNo == txnFCLJob.JobNo).ToListAsync(),
        //            //ContainerSizes = new SelectList(await _context.RefContainerSizes.OrderBy(p => p.Description).ToListAsync(), "ContainerId", "Description"),
        //        };

        //        // Load ViewData for dropdowns
        //        //ViewBag.ContainerSize = new SelectList(_context.RefContainerSizes.OrderBy(p => p.Description), "ContainerId", "Description");

        //        ViewData["ContainerSize"] = new SelectList(await _context.RefContainerSizes.OrderBy(p => p.Description).ToListAsync(), "ContainerId", "Description");

        //        //ViewBag["ContainerSize"] = new SelectList(await _context.RefContainerSizes.OrderBy(p => p.Description).ToListAsync(), "ContainerId", "Description");
        //        ViewData["IntendedVessel"] = new SelectList(await _context.RefVessels.Where(v => v.IsActive).OrderBy(s => s.VesselName).ToListAsync(), "VesselId", "VesselName", txnFCLJob.VesselId);
        //        ViewData["POD"] = new SelectList(await _context.RefPorts.OrderBy(p => p.PortName).ToListAsync(), "PortCode", "PortName", txnFCLJob.POD);
        //        ViewData["Customers"] = new SelectList(await _context.RefCustomers.OrderBy(c => c.Name).ToListAsync(), "CustomerId", "Name", txnFCLJob.ShipperID);
        //        ViewData["ShippingLines"] = new SelectList(await _context.RefShippingLines.OrderBy(c => c.Name).ToListAsync(), "ShippingLineId", "Name", txnFCLJob.ShippingLineID);
        //        ViewData["Agents"] = new SelectList(await _context.RefAgents.OrderBy(c => c.AgentName).ToListAsync(), "AgentId", "AgentName", txnFCLJob.AgentId);
        //        ViewData["HandleBy"] = new SelectList(await _context.RefStaffs.Where(h => !h.IsSalesPerson && h.IsActive).OrderBy(st => st.StaffName).ToListAsync(), "StaffId", "StaffName", txnFCLJob.HandleBy);
        //        ViewData["SalesPerson"] = new SelectList(await _context.RefStaffs.Where(s => s.IsSalesPerson && s.IsActive).OrderBy(sl => sl.StaffName).ToListAsync(), "StaffId", "StaffName", txnFCLJob.SalesPersonID);

        //        return View(viewModel);
        //    }













        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public async Task<IActionResult> Edit(string id, string dtlItemsList,
        //    [Bind("JobNo,Date,JobType,ExchangeRate,HandleBy,SalesPersonID,ShipperID,ShippingLineID,MasterBL,VesselId,Voyage,ETD,POD,ETA,BLType,AgentId,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,CanceledBy,CanceledDateTime,CanceledReason")]
        //TxnFCLJob txnFCLJob)
        //        {
        //            if (id != txnFCLJob.JobNo)
        //            {
        //                return NotFound();
        //            }

        //            txnFCLJob.LastUpdatedBy = "Admin";
        //            txnFCLJob.LastUpdatedDateTime = DateTime.Now;
        //            ModelState.Remove("FCLJobContainers");

        //            if (ModelState.IsValid)
        //            {
        //                try
        //                {
        //                    // Load existing containers for the current job
        //                    var existingContainers = await _context.TxnFCLJobContainers
        //                        .Where(c => c.JobNo == txnFCLJob.JobNo)
        //                        .ToListAsync();

        //                    if (!string.IsNullOrWhiteSpace(dtlItemsList))
        //                    {
        //                        // Deserialize the incoming container data
        //                        var containerData = JsonConvert.DeserializeObject<List<TxnFCLJobContainers>>(dtlItemsList);
        //                        if (containerData != null)
        //                        {
        //                            foreach (var item in containerData)
        //                            {
        //                                item.JobNo = txnFCLJob.JobNo;

        //                                var existingContainer = existingContainers.FirstOrDefault(c => c.ContainerNo == item.ContainerNo);
        //                                if (existingContainer != null)
        //                                {
        //                                    // Update existing container
        //                                    existingContainer.ContainerSize = item.ContainerSize;
        //                                    existingContainer.SealNo = item.SealNo;
        //                                    existingContainer.GrossWeight = item.GrossWeight;
        //                                    existingContainer.LastUpdatedBy = "Admin";
        //                                    existingContainer.LastUpdatedDateTime = DateTime.Now;
        //                                }
        //                                else
        //                                {
        //                                    // Add new container
        //                                    item.CreatedBy = "Admin";
        //                                    item.CreatedDateTime = DateTime.Now;
        //                                    _context.TxnFCLJobContainers.Add(item);
        //                                }
        //                            }

        //                            // Remove containers that are not in the updated list
        //                            var containersToRemove = existingContainers
        //                                .Where(ec => !containerData.Any(cd => cd.ContainerNo == ec.ContainerNo))
        //                                .ToList();
        //                            _context.TxnFCLJobContainers.RemoveRange(containersToRemove);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        // If dtlItemsList is empty, remove all existing containers
        //                        _context.TxnFCLJobContainers.RemoveRange(existingContainers);
        //                    }

        //                    // Update the main job record
        //                    _context.Update(txnFCLJob);
        //                    await _context.SaveChangesAsync();
        //                    return RedirectToAction(nameof(Index));
        //                }
        //                catch (DbUpdateConcurrencyException)
        //                {
        //                    if (!TxnFCLJobExists(txnFCLJob.JobNo))
        //                    {
        //                        return NotFound();
        //                    }
        //                    else
        //                    {
        //                        throw;
        //                    }
        //                }
        //            }

        //            // Initialize ViewModel for the return view in case of an error
        //            var viewModel = new TxnFCLJobsViewModel
        //            {
        //                FCLJobs = new List<TxnFCLJob> { txnFCLJob },
        //                FCLJobContainers = await _context.TxnFCLJobContainers.Where(c => c.JobNo == txnFCLJob.JobNo).ToListAsync(),
        //            };

        //            // Load ViewData for dropdowns
        //            ViewData["ContainerSize"] = new SelectList(await _context.RefContainerSizes.OrderBy(p => p.Description).ToListAsync(), "ContainerId", "Description");
        //            ViewData["IntendedVessel"] = new SelectList(await _context.RefVessels.Where(v => v.IsActive).OrderBy(s => s.VesselName).ToListAsync(), "VesselId", "VesselName", txnFCLJob.VesselId);
        //            ViewData["POD"] = new SelectList(await _context.RefPorts.OrderBy(p => p.PortName).ToListAsync(), "PortCode", "PortName", txnFCLJob.POD);
        //            ViewData["Customers"] = new SelectList(await _context.RefCustomers.OrderBy(c => c.Name).ToListAsync(), "CustomerId", "Name", txnFCLJob.ShipperID);
        //            ViewData["ShippingLines"] = new SelectList(await _context.RefShippingLines.OrderBy(c => c.Name).ToListAsync(), "ShippingLineId", "Name", txnFCLJob.ShippingLineID);
        //            ViewData["Agents"] = new SelectList(await _context.RefAgents.OrderBy(c => c.AgentName).ToListAsync(), "AgentId", "AgentName", txnFCLJob.AgentId);
        //            ViewData["HandleBy"] = new SelectList(await _context.RefStaffs.Where(h => !h.IsSalesPerson && h.IsActive).OrderBy(st => st.StaffName).ToListAsync(), "StaffId", "StaffName", txnFCLJob.HandleBy);
        //            ViewData["SalesPerson"] = new SelectList(await _context.RefStaffs.Where(s => s.IsSalesPerson && s.IsActive).OrderBy(sl => sl.StaffName).ToListAsync(), "StaffId", "StaffName", txnFCLJob.SalesPersonID);

        //            return View(viewModel);
        //        }





        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public async Task<IActionResult> Edit(string id, string dtlItemsList,
        //            [Bind("JobNo,Date,JobType,ExchangeRate,HandleBy,SalesPersonID,ShipperID,ShippingLineID,MasterBL,VesselId,Voyage,ETD,POD,ETA,BLType,AgentId,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,CanceledBy,CanceledDateTime,CanceledReason")]
        //TxnFCLJob txnFCLJob)
        //        {
        //            if (id != txnFCLJob.JobNo)
        //            {
        //                return NotFound();
        //            }

        //            txnFCLJob.LastUpdatedBy = "Admin";
        //            txnFCLJob.LastUpdatedDateTime = DateTime.Now;
        //            ModelState.Remove("FCLJobContainers");

        //            if (ModelState.IsValid)
        //            {
        //                try
        //                {
        //                    _context.Entry(txnFCLJob).State = EntityState.Modified;

        //                    // Load existing containers for the current job
        //                    var existingContainers = await _context.TxnFCLJobContainers
        //                        .Where(c => c.JobNo == txnFCLJob.JobNo)
        //                        .ToListAsync();

        //                    if (!string.IsNullOrWhiteSpace(dtlItemsList))
        //                    {
        //                        // Deserialize the incoming container data
        //                        var containerData = JsonConvert.DeserializeObject<List<TxnFCLJobContainers>>(dtlItemsList);
        //                        if (containerData != null)
        //                        {
        //                            // Track existing container IDs for deletion
        //                            var existingContainerIds = existingContainers.Select(c => c.SerialNo).ToHashSet();

        //                            foreach (var item in containerData)
        //                            {
        //                                item.JobNo = txnFCLJob.JobNo;

        //                                // Check if the container already exists
        //                                var existingContainer = existingContainers.FirstOrDefault(c => c.SerialNo == item.SerialNo);
        //                                if (existingContainer != null)
        //                                {
        //                                    // Update existing container
        //                                    _context.Entry(existingContainer).CurrentValues.SetValues(item);
        //                                    existingContainer.LastUpdatedDateTime = DateTime.Now;

        //                                    // Remove the ID from the set since it has been updated
        //                                    existingContainerIds.Remove(item.SerialNo);
        //                                }
        //                                else
        //                                {
        //                                    // Add new container if it doesn't exist
        //                                    item.CreatedDateTime = DateTime.Now;
        //                                    _context.TxnFCLJobContainers.Add(item);
        //                                }
        //                            }

        //                            // Remove containers that were not part of the new data
        //                            var containersToRemove = existingContainers.Where(c => existingContainerIds.Contains(c.SerialNo)).ToList();
        //                            _context.TxnFCLJobContainers.RemoveRange(containersToRemove);
        //                        }
        //                    }

        //                    await _context.SaveChangesAsync();
        //                    return RedirectToAction(nameof(Index));
        //                }
        //                catch (DbUpdateConcurrencyException)
        //                {
        //                    if (!TxnFCLJobExists(txnFCLJob.JobNo))
        //                    {
        //                        return NotFound();
        //                    }
        //                    else
        //                    {
        //                        throw;
        //                    }
        //                }
        //            }

        //            // Initialize ViewModel for the return view in case of an error
        //            var viewModel = new TxnFCLJobsViewModel
        //            {
        //                FCLJobs = new List<TxnFCLJob> { txnFCLJob },
        //                FCLJobContainers = await _context.TxnFCLJobContainers.Where(c => c.JobNo == txnFCLJob.JobNo).ToListAsync(),
        //            };

        //            // Load ViewData for dropdowns
        //            ViewData["ContainerSize"] = new SelectList(await _context.RefContainerSizes.OrderBy(p => p.Description).ToListAsync(), "ContainerId", "Description");
        //            ViewData["IntendedVessel"] = new SelectList(await _context.RefVessels.Where(v => v.IsActive).OrderBy(s => s.VesselName).ToListAsync(), "VesselId", "VesselName", txnFCLJob.VesselId);
        //            ViewData["POD"] = new SelectList(await _context.RefPorts.OrderBy(p => p.PortName).ToListAsync(), "PortCode", "PortName", txnFCLJob.POD);
        //            ViewData["Customers"] = new SelectList(await _context.RefCustomers.OrderBy(c => c.Name).ToListAsync(), "CustomerId", "Name", txnFCLJob.ShipperID);
        //            ViewData["ShippingLines"] = new SelectList(await _context.RefShippingLines.OrderBy(c => c.Name).ToListAsync(), "ShippingLineId", "Name", txnFCLJob.ShippingLineID);
        //            ViewData["Agents"] = new SelectList(await _context.RefAgents.OrderBy(c => c.AgentName).ToListAsync(), "AgentId", "AgentName", txnFCLJob.AgentId);
        //            ViewData["HandleBy"] = new SelectList(await _context.RefStaffs.Where(h => !h.IsSalesPerson && h.IsActive).OrderBy(st => st.StaffName).ToListAsync(), "StaffId", "StaffName", txnFCLJob.HandleBy);
        //            ViewData["SalesPerson"] = new SelectList(await _context.RefStaffs.Where(s => s.IsSalesPerson && s.IsActive).OrderBy(sl => sl.StaffName).ToListAsync(), "StaffId", "StaffName");

        //            return View(viewModel);
        //        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string dtlItemsList,
            string[] ContainerNo, string[] Size, string[] Seal,
            [Bind("JobNo,Date,JobType,ExchangeRate,HandleBy,SalesPersonID,ShipperID,ShippingLineID,MasterBL,VesselId,Voyage,ETD,POD,ETA,BLType,AgentId,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,CanceledBy,CanceledDateTime,CanceledReason")]
    TxnFCLJob txnFCLJob)
        {
            if (id != txnFCLJob.JobNo)
            {
                return NotFound();
            }

            txnFCLJob.LastUpdatedBy = "Admin";
            txnFCLJob.LastUpdatedDateTime = DateTime.Now;
            ModelState.Remove("FCLJobContainers");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(txnFCLJob).State = EntityState.Modified;

                    // Load existing containers for the current job
                    var existingContainers = await _context.TxnFCLJobContainers
                        .Where(c => c.JobNo == txnFCLJob.JobNo)
                        .ToListAsync();

                    // Track existing container IDs for deletion
                    var existingContainerIds = existingContainers.Select(c => c.SerialNo).ToHashSet();

                    // Process new container data
                    if (ContainerNo != null && Size != null && Seal != null)
                    {
                        for (int i = 0; i < ContainerNo.Length; i++)
                        {
                            var item = new TxnFCLJobContainers
                            {
                                JobNo = txnFCLJob.JobNo,
                                ContainerNo = ContainerNo[i],
                                Seal = Seal[i],
                                CreatedDateTime = DateTime.Now
                            };

                            // Convert Size to decimal?
                            if (decimal.TryParse(Size[i], out decimal sizeValue))
                            {
                                item.Size = sizeValue; // Set the size if parsing is successful
                            }
                            else
                            {
                                item.Size = null; // Set to null or handle as needed
                            }

                            // Check if the container already exists
                            var existingContainer = existingContainers.FirstOrDefault(c => c.ContainerNo == item.ContainerNo);
                            if (existingContainer != null)
                            {
                                // Update existing container
                                _context.Entry(existingContainer).CurrentValues.SetValues(item);
                                existingContainer.LastUpdatedDateTime = DateTime.Now;

                                // Remove the ID from the set since it has been updated
                                existingContainerIds.Remove(existingContainer.SerialNo);
                            }
                            else
                            {
                                // Add new container if it doesn't exist
                                _context.TxnFCLJobContainers.Add(item);
                            }
                        }
                    }

                    // Remove containers that were not part of the new data
                    var containersToRemove = existingContainers.Where(c => existingContainerIds.Contains(c.SerialNo)).ToList();
                    _context.TxnFCLJobContainers.RemoveRange(containersToRemove);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TxnFCLJobExists(txnFCLJob.JobNo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Initialize ViewModel for the return view in case of an error
            var viewModel = new TxnFCLJobsViewModel
            {
                FCLJobs = new List<TxnFCLJob> { txnFCLJob },
                FCLJobContainers = await _context.TxnFCLJobContainers.Where(c => c.JobNo == txnFCLJob.JobNo).ToListAsync(),
            };

            // Load ViewData for dropdowns
            ViewData["ContainerSize"] = new SelectList(await _context.RefContainerSizes.OrderBy(p => p.Description).ToListAsync(), "ContainerId", "Description");
            ViewData["IntendedVessel"] = new SelectList(await _context.RefVessels.Where(v => v.IsActive).OrderBy(s => s.VesselName).ToListAsync(), "VesselId", "VesselName", txnFCLJob.VesselId);
            ViewData["POD"] = new SelectList(await _context.RefPorts.OrderBy(p => p.PortName).ToListAsync(), "PortCode", "PortName", txnFCLJob.POD);
            ViewData["Customers"] = new SelectList(await _context.RefCustomers.OrderBy(c => c.Name).ToListAsync(), "CustomerId", "Name", txnFCLJob.ShipperID);
            ViewData["ShippingLines"] = new SelectList(await _context.RefShippingLines.OrderBy(c => c.Name).ToListAsync(), "ShippingLineId", "Name", txnFCLJob.ShippingLineID);
            ViewData["Agents"] = new SelectList(await _context.RefAgents.OrderBy(c => c.AgentName).ToListAsync(), "AgentId", "AgentName", txnFCLJob.AgentId);
            ViewData["HandleBy"] = new SelectList(await _context.RefStaffs.Where(h => !h.IsSalesPerson && h.IsActive).OrderBy(st => st.StaffName).ToListAsync(), "StaffId", "StaffName", txnFCLJob.HandleBy);
            ViewData["SalesPerson"] = new SelectList(await _context.RefStaffs.Where(s => s.IsSalesPerson && s.IsActive).OrderBy(sl => sl.StaffName).ToListAsync(), "StaffId", "StaffName");

            return View(viewModel);
        }




        private bool TxnFCLJobExists(string id)
        {
            return _context.TxnFCLJobs.Any(e => e.JobNo == id);
        }






        // GET: TxnImportJobDtls/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var tables = new TxnFCLJobsViewModel
            {

                FCLJobs = _context.TxnFCLJobs.Where(p => p.JobNo == id),
                FCLJobContainers = _context.TxnFCLJobContainers.Where(p => p.JobNo == id),
            };


            ViewData["IntendedVessel"] = new SelectList(_context.RefVessels.Where(v => v.IsActive.Equals(true)).OrderBy(s => s.VesselName), "VesselId", "VesselName", "VesselId");
            ViewData["POD"] = new SelectList(_context.RefPorts.OrderBy(p => p.PortName), "PortCode", "PortName", "PortCode");
            ViewData["Customers"] = new SelectList(_context.RefCustomers.OrderBy(c => c.Name), "CustomerId", "Name", "CustomerId");
            ViewData["ShippingLines"] = new SelectList(_context.RefShippingLines.OrderBy(c => c.Name), "ShippingLineId", "Name", "ShippingLineId");
            ViewData["Agents"] = new SelectList(_context.RefAgents.OrderBy(c => c.AgentName), "AgentId", "AgentName", "AgentId");
            ViewData["HandleBy"] = new SelectList(_context.RefStaffs.Where(h => h.IsSalesPerson.Equals(false) && h.IsActive.Equals(true)).OrderBy(st => st.StaffName), "SatffId", "StaffName", "SatffId");
            ViewData["SalesPerson"] = new SelectList(_context.RefStaffs.Where(s => s.IsSalesPerson.Equals(true) && s.IsActive.Equals(true)).OrderBy(sl => sl.StaffName), "SatffId", "StaffName", "SatffId");
            ViewData["ContainerSize"] = new SelectList(_context.RefContainerSizes.OrderBy(p => p.Description), "ContainerId", "Description", "ContainerId");
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





    }
}

