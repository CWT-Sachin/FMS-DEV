using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FMS_DEV.ViewModel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FMS_DEV.Models;
using FMS_DEV.ModelsAccounts;
using FMS_DEV.DataAccounts;
using FMS_DEV.Data;
using FMS_DEV.CommonMethods;

namespace FMS_DEV.Controllers
{
    public class TxnDebitNoteExportHdsController : Controller
    {
        private readonly FtlcolombOperationContext _context;
        private readonly FtlcolomboAccountsContext _accountsContext;

        public string jobNo { get; private set; }

        public TxnDebitNoteExportHdsController(FtlcolombOperationContext context, FtlcolomboAccountsContext accountsContext)
        {
            _context = context;
            _accountsContext = accountsContext;
        }

        // GET: TxnDebitNoteHds
        public async Task<IActionResult> Index(string searchString, string searchType, int pg = 1)
        {
            var txnDebitNoteHds = _context.TxnDebitNoteExportHds.OrderByDescending(p => p.DebitNoteNo).Include(t => t.AgentDebitNoteExportNavigation).Include(t => t.ShippingLineDbitNoteExpNavigation);

            if (searchType == "Approve")
            {
                // Filter only not approved invoices
                txnDebitNoteHds = txnDebitNoteHds.Where(txnDebitNoteHds => txnDebitNoteHds.Approved != true).OrderByDescending(p => p.DebitNoteNo).Include(t => t.AgentDebitNoteExportNavigation).Include(t => t.ShippingLineDbitNoteExpNavigation);
            }
            else if (!String.IsNullOrEmpty(searchString))
            {
                if (searchType == "CreditNote")
                {
                    txnDebitNoteHds = txnDebitNoteHds.Where(txnDebitNoteHds => txnDebitNoteHds.DebitNoteNo.Contains(searchString)).OrderByDescending(p => p.DebitNoteNo).Include(t => t.AgentDebitNoteExportNavigation).Include(t => t.ShippingLineDbitNoteExpNavigation);
                }
                else if (searchType == "Job")
                {
                    txnDebitNoteHds = txnDebitNoteHds.Where(txnDebitNoteHds => txnDebitNoteHds.JobNo.Contains(searchString)).OrderByDescending(p => p.DebitNoteNo).Include(t => t.AgentDebitNoteExportNavigation).Include(t => t.ShippingLineDbitNoteExpNavigation);
                }

                // Check if results are found
                if (txnDebitNoteHds.Any())
                {
                    ViewData["DebitNoteFound"] = "";
                }
                else
                {
                    ViewData["DebitNoteFound"] = $"{searchType} Number: {searchString} not found";
                }
            }

            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int recsCount = txnDebitNoteHds.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = txnDebitNoteHds.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;
            return View(data);
        }


        public async Task<IActionResult> Approve(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var txnDebitNoteHds = await _context.TxnDebitNoteExportHds
                .FirstOrDefaultAsync(m => m.DebitNoteNo == id);

            if (txnDebitNoteHds == null)
            {
                return NotFound();
            }

            jobNo = txnDebitNoteHds.JobNo; // Set the jobNo property

            var tables = new DebitNoteExportViewModel
            {
                DebitNoteHdMulti = _context.TxnDebitNoteExportHds.Where(t => t.DebitNoteNo == id),
                DebitNoteDtlMulti = _context.TxnDebitNoteExportDtls.Where(t => t.DebitNoteNo == id),

            };

            return View(tables);
        }
        [HttpPost]
        public async Task<IActionResult> Approve(string id, bool approved)
        {
            if (id == null)
            {
                return NotFound();
            }

            var txnDebitNoteHds = await _context.TxnDebitNoteExportHds
                .FirstOrDefaultAsync(m => m.DebitNoteNo == id);

            if (txnDebitNoteHds == null)
            {
                return NotFound();
            }

            // Inserting Transaction Data to Acccounts 
            var txnDebitNoteExportDtls = await _context.TxnDebitNoteExportDtls
                .Include(t => t.ChargeItemNavigation)
                .Where(m => m.DebitNoteNo == id)
                .ToListAsync();

            // Next RefNumber for Txn_Transactions
            var nextAccTxnNo = "";
            var TableIDAccTxn = "Txn_Transactions";
            var refLastNumberAccTxn = await _accountsContext.RefLastNumberAcc.FindAsync(TableIDAccTxn);
            if (refLastNumberAccTxn != null)
            {
                var nextNumberAccTxn = refLastNumberAccTxn.LastNumber + 1;
                refLastNumberAccTxn.LastNumber = nextNumberAccTxn;
                nextAccTxnNo = "TXN" + DateTime.Now.Year.ToString() + nextNumberAccTxn.ToString().PadLeft(5, '0');

                // _context.RefLastNumbers.Remove(refLastNumber);
            }
            else
            {
                return NotFound();
                //return View(tables);
            }
            if (txnDebitNoteExportDtls != null)
            {
                var SerialNo_AccTxn = 1;
                var AccTxnDescription = "Debtors EDB No: " +txnDebitNoteHds.DebitNoteNo;
                // First Acc transaction 
                TxnTransactions NewRowAccTxnFirst = new TxnTransactions();
                NewRowAccTxnFirst.TxnNo = nextAccTxnNo;
                NewRowAccTxnFirst.TxnSNo = SerialNo_AccTxn;
                NewRowAccTxnFirst.Date = txnDebitNoteHds.Date; //  Jurnal Date 
                NewRowAccTxnFirst.Description = AccTxnDescription;
                NewRowAccTxnFirst.TxnAccCode = txnDebitNoteHds.DebitAcc;
                NewRowAccTxnFirst.Dr = (decimal)txnDebitNoteHds.TotalDebitAmountLkr;
                NewRowAccTxnFirst.Cr = (decimal)0;
                NewRowAccTxnFirst.RefNo = txnDebitNoteHds.DebitNoteNo; // Debit Note No
                NewRowAccTxnFirst.Note = "";
                NewRowAccTxnFirst.Reconciled = false;
                NewRowAccTxnFirst.DocType = "DebitNTExport";
                NewRowAccTxnFirst.IsMonthEndDone = false;
                NewRowAccTxnFirst.CreatedBy = "Admin";
                NewRowAccTxnFirst.CreatedDateTime = DateTime.Now;
                NewRowAccTxnFirst.Canceled = false;

                _accountsContext.TxnTransactions.Add(NewRowAccTxnFirst);

                foreach (var item in txnDebitNoteExportDtls)
                {
                    // Transaction table Insert 
                    TxnTransactions NewRowAccTxn = new TxnTransactions();

                    SerialNo_AccTxn = SerialNo_AccTxn + 1;
                    NewRowAccTxn.TxnNo = nextAccTxnNo;
                    NewRowAccTxn.TxnSNo = SerialNo_AccTxn;
                    NewRowAccTxn.Date = txnDebitNoteHds.Date; //  Jurnal Date 
                    NewRowAccTxn.Description = item.ChargeItemNavigation.Description + "EDN: " + item.DebitNoteNo;
                    NewRowAccTxn.TxnAccCode = item.AccNo;
                    NewRowAccTxn.Dr = (decimal)0;
                    if (item.Currency == "USD")
                    {
                        var AmtLkr = item.Amount * txnDebitNoteHds.ExchangeRate; // convert to LKR
                        NewRowAccTxn.Cr = (decimal)AmtLkr;
                    }
                    else
                    {
                        NewRowAccTxn.Cr = (decimal)item.Amount;
                    }
                    NewRowAccTxn.RefNo = item.DebitNoteNo; // Invoice No
                    NewRowAccTxn.Note = "";
                    NewRowAccTxn.Reconciled = false;
                    NewRowAccTxn.DocType = "DebitNTExport";
                    NewRowAccTxn.IsMonthEndDone = false;
                    NewRowAccTxn.CreatedBy = "Admin";
                    NewRowAccTxn.CreatedDateTime = DateTime.Now;
                    NewRowAccTxn.Canceled = false;

                    _accountsContext.TxnTransactions.Add(NewRowAccTxn);
                }
            }
            // END Inserting Transaction Data to Acccounts 

            /// Update the Approved property based on the form submission
            txnDebitNoteHds.Approved = approved;
            txnDebitNoteHds.ApprovedBy = "CurrentUserName"; // Replace with the actual user name
            txnDebitNoteHds.ApprovedDateTime = DateTime.Now;

            _context.Update(txnDebitNoteHds);
            await _context.SaveChangesAsync();
            await _accountsContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirect to the appropriate action
        }



        // GET: TxnDebitNoteHds/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TxnDebitNoteExportHds == null)
            {
                return NotFound();
            }

            var txnDebitNoteHd = await _context.TxnDebitNoteExportHds
                .FirstOrDefaultAsync(m => m.DebitNoteNo == id);
            if (txnDebitNoteHd == null)
            {
                return NotFound();
            }
            var jobNo = txnDebitNoteHd.JobNo;
            var tables = new DebitNoteExportViewModel
            {
                DebitNoteHdMulti = _context.TxnDebitNoteExportHds.Where(t => t.DebitNoteNo == id),
                DebitNoteDtlMulti = _context.TxnDebitNoteExportDtls.Where(t => t.DebitNoteNo == id),
                ExportJobDtlMulti = _context.TxnExportJobDtls.Where(t => t.JobNo == jobNo),
                ExportJobHdMulti = _context.TxnExportJobHds
                    .Include(t => t.AgentExportNavigation)
                    .Include(t => t.HandlebyExportJobNavigation)
                    .Include(t => t.CreatedByExportJobNavigation)
                    .Include(t => t.ShippingLineExportNavigation)
                    .Include(t => t.ColoaderExportNavigation)
                    .Include(t => t.VesselExportJobDtlNavigation)
                    .Include(t => t.PODExportJobNavigation)
                    .Include(t => t.FDNExportJobNavigation)
                    .Where(t => t.JobNo == jobNo),
                 
            };
            ViewData["AccountsCodes"] = new SelectList(
                               _accountsContext.Set<RefChartOfAcc>()
                                   .Where(a => a.IsInactive.Equals(false))
                                   .OrderBy(p => p.AccCode)
                                   .Select(a => new { AccNo = a.AccNo, DisplayValue = $"{a.AccCode} - {a.Description}" }),
                               "AccNo",
                               "DisplayValue",
                               "AccNo"
);
            ViewData["AgentIDNomination"] = new SelectList(_context.RefAgents.Join(_context.RefPorts,
                 a => a.PortId,
                 b => b.PortCode,
                 (a, b) => new
                 {
                     AgentId = a.AgentId,
                     AgentName = a.AgentName + " - " + b.PortName,
                     IsActive = a.IsActive
                 }).Where(a => a.IsActive.Equals(true)).OrderBy(a => a.AgentName), "AgentId", "AgentName", "AgentId");
            List<SelectListItem> Currency = new List<SelectListItem>
                {
                    new SelectListItem { Value = "USD", Text = "USD" },
                    new SelectListItem { Value = "LKR", Text = "LKR" }
                };
            ViewData["CurrencyList"] = new SelectList(Currency, "Value", "Text", "CurrencyList");
            List<SelectListItem> Unit = new List<SelectListItem>
                {
                    new SelectListItem { Value = "CBM", Text = "CBM" },
                    new SelectListItem { Value = "BL", Text = "BL" },
                    new SelectListItem { Value = "CNT", Text = "CNT" }
                };
            ViewData["UnitList"] = new SelectList(Unit, "Value", "Text", "UnitList");
            ViewData["ChargeItemsList"] = new SelectList(_context.Set<RefChargeItemAcc>().Where(a => a.IsActive.Equals(true)).OrderBy(p => p.Description), "ChargeId", "Description", "ChargeId");
            ViewData["ShippingLine"] = new SelectList(_context.RefShippingLines.OrderBy(c => c.Name), "ShippingLineId", "Name", "ShippingLineId");
            ViewData["Customer"] = new SelectList(_context.RefCustomers.OrderBy(c => c.Name), "CustomerId", "Name", "CustomerId");
            return View(tables);
        }

        //// GET: TxnDebitNoteHds/Create
        public IActionResult Create(string searchString)
        {
            var JobNo = "xxx";

            if (!string.IsNullOrEmpty(searchString))
            {


                JobNo = searchString;

                // Check if results are found
                if (_context.TxnExportJobHds.Any(t => t.JobNo == JobNo))
                {
                    ViewData["JobFound"] = "";
                }
                else
                {
                    ViewData["JobFound"] = "Job Number: " + searchString + " not found";
                }

            }
            var tables = new DebitNoteExportViewModel
            {
                DebitNoteHdMulti = _context.TxnDebitNoteExportHds.Where(t => t.DebitNoteNo == "xxx"),
                DebitNoteDtlMulti = _context.TxnDebitNoteExportDtls.Where(t => t.DebitNoteNo == "xxx"),
                ExportJobDtlMulti = _context.TxnExportJobDtls.Where(t => t.JobNo == JobNo),
                ExportJobHdMulti = _context.TxnExportJobHds
                    .Include(t => t.AgentExportNavigation)
                    .Include(t => t.HandlebyExportJobNavigation)
                    .Include(t => t.CreatedByExportJobNavigation)
                    .Include(t => t.ShippingLineExportNavigation)
                    .Include(t => t.ColoaderExportNavigation)
                    .Include(t => t.VesselExportJobDtlNavigation)
                    .Include(t => t.PODExportJobNavigation)
                    .Include(t => t.FDNExportJobNavigation)
                    .Where(t => t.JobNo == JobNo),

            };

            ViewData["AgentIDNomination"] = new SelectList(_context.RefAgents.Join(_context.RefPorts,
                 a => a.PortId,
                 b => b.PortCode,
                 (a, b) => new
                 {
                     AgentId = a.AgentId,
                     AgentName = a.AgentName + " - " + b.PortName,
                     IsActive = a.IsActive
                 }).Where(a => a.IsActive.Equals(true)).OrderBy(a => a.AgentName), "AgentId", "AgentName", "AgentId");
            List<SelectListItem> Currency = new List<SelectListItem>
                {
                    new SelectListItem { Value = "USD", Text = "USD" },
                    new SelectListItem { Value = "LKR", Text = "LKR" }
                };
            ViewData["CurrencyList"] = new SelectList(Currency, "Value", "Text", "CurrencyList");
            List<SelectListItem> Unit = new List<SelectListItem>
                {
                    new SelectListItem { Value = "CBM", Text = "CBM" },
                    new SelectListItem { Value = "BL", Text = "BL" },
                    new SelectListItem { Value = "CNT", Text = "CNT" }
                };
            ViewData["UnitList"] = new SelectList(Unit, "Value", "Text", "UnitList");
            ViewData["ChargeItemsList"] = new SelectList(_context.Set<RefChargeItemAcc>().Where(a => a.IsActive.Equals(true)).OrderBy(p => p.Description), "ChargeId", "Description", "ChargeId");
            ViewData["ShippingLine"] = new SelectList(_context.RefShippingLines.OrderBy(c => c.Name), "ShippingLineId", "Name", "ShippingLineId");
            ViewData["Customer"] = new SelectList(_context.RefCustomers.OrderBy(c => c.Name), "CustomerId", "Name", "CustomerId");
            ViewData["AccountsCodes"] = new SelectList(
                                            _accountsContext.Set<RefChartOfAcc>()
                                                .Where(a => a.IsInactive.Equals(false))
                                                .OrderBy(p => p.AccCode)
                                                .Select(a => new { AccNo = a.AccNo, DisplayValue = $"{a.AccCode} - {a.Description}" }),
                                            "AccNo",
                                            "DisplayValue",
                                            "AccNo"
         );
            return View(tables);
        }




        // POST: TxnDebitNoteHds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string dtlItemsList, [Bind("DebitNoteNo,Date,JobNo,Customer,ExchangeRate,TotalDebitAmountLkr,TotalDebitAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,Approved,Type, AmountPaid,AmountToBePaid, AgentID")] TxnDebitNoteExportHd txnDebitNoteExportHd)
        {

            // Next RefNumber for txnImportJobDtl
            var nextRefNo = "";
            var TableID = "Txn_DebitNoteHD_Export";
            var refLastNumber = await _context.RefLastNumbers.FindAsync(TableID);
            if (refLastNumber != null)
            {
                var nextNumber = refLastNumber.LastNumber + 1;
                refLastNumber.LastNumber = nextNumber;
                nextRefNo = "EDN" + DateTime.Now.Year.ToString() + nextNumber.ToString().PadLeft(5, '0');
                txnDebitNoteExportHd.DebitNoteNo = nextRefNo;

                // _context.RefLastNumbers.Remove(refLastNumber);
            }
            else
            {
                return NotFound();
                //return View(tables);
            }

            txnDebitNoteExportHd.Canceled = false;
            txnDebitNoteExportHd.Approved = false;
            txnDebitNoteExportHd.CreatedBy = "Admin";
            txnDebitNoteExportHd.CreatedDateTime = DateTime.Now;

            txnDebitNoteExportHd.AmountPaid = 0;
            txnDebitNoteExportHd.AmountToBePaid = txnDebitNoteExportHd.TotalDebitAmountLkr;

            var LocalOverseas = txnDebitNoteExportHd.Type;

            if (LocalOverseas == "Local")
            {
                txnDebitNoteExportHd.DebitAcc = "ACC0023"; // LOCAL DEBTORS  2200-01
            }
            else // Oversease
            {
                txnDebitNoteExportHd.DebitAcc = "ACC0024"; // OVERSEAS DEBTORS  2200-02
                txnDebitNoteExportHd.Type = "Overseas"; // There is an error in the create if the Selection is "Overseae" Type not pass. This is  a temporary solution. 
            }

            txnDebitNoteExportHd.CreditAcc = null;
            var TransDocMode = "Export";
            var TransDocType = "Revenue";
            var TransDocVender = "";

            ModelState.Remove("DebitNoteNo");  // Auto genrated
            if (ModelState.IsValid)
            {
                // Adding CargoBreakDown records
                if (!string.IsNullOrWhiteSpace(dtlItemsList))
                {
                    var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnDebitNoteExportDtl>>(dtlItemsList);
                    if (DtailItemdataTable != null)
                    {
                        var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the common method  class
                        foreach (var item in DtailItemdataTable)
                        {
                            TxnDebitNoteExportDtl DetailItem = new TxnDebitNoteExportDtl();
                            DetailItem.DebitNoteNo = nextRefNo; // New CreditNote Number
                            DetailItem.SerialNo = item.SerialNo;
                            DetailItem.ChargeItem = item.ChargeItem;
                            DetailItem.Unit = item.Unit ?? "DefaultUnit"; // Set a default value if 'Unit' is null
                            DetailItem.Rate = item.Rate;
                            DetailItem.Currency = item.Currency;
                            DetailItem.Qty = item.Qty;
                            DetailItem.Amount = item.Amount;
                            DetailItem.BlContainerNo = item.BlContainerNo;
                            DetailItem.AccNo = CommonMethodAccClass.GetChargeItemAccNo(item.ChargeItem, TransDocMode, TransDocType, TransDocVender);
                            _context.TxnDebitNoteExportDtls.Add(DetailItem);
                        }
                    }
                }
                _context.Add(txnDebitNoteExportHd);
                _context.RefLastNumbers.Update(refLastNumber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var tables = new DebitNoteExportViewModel
            {
                DebitNoteHdMulti = _context.TxnDebitNoteExportHds.Where(t => t.DebitNoteNo == "xxx"),
                DebitNoteDtlMulti = _context.TxnDebitNoteExportDtls.Where(t => t.DebitNoteNo == "xxx"),

                ExportJobHdMulti = _context.TxnExportJobHds.Where(t => t.JobNo == "xxx"),
                ExportJobDtlMulti = _context.TxnExportJobDtls.Where(t => t.JobNo == "xxx"),
            };

            List<SelectListItem> Currency = new List<SelectListItem>
                {
                    new SelectListItem { Value = "USD", Text = "USD" },
                    new SelectListItem { Value = "LKR", Text = "LKR" }
                };
            ViewData["CurrencyList"] = new SelectList(Currency, "Value", "Text", "CurrencyList");
            List<SelectListItem> Unit = new List<SelectListItem>
                {
                    new SelectListItem { Value = "CBM", Text = "CBM" },
                    new SelectListItem { Value = "BL", Text = "BL" },
                    new SelectListItem { Value = "CNT", Text = "CNT" }
                };
            ViewData["UnitList"] = new SelectList(Unit, "Value", "Text", "UnitList");
            ViewData["ChargeItemsList"] = new SelectList(_context.Set<RefChargeItemAcc>().Where(a => a.IsActive.Equals(true)).OrderBy(p => p.Description), "ChargeId", "Description", "ChargeId");
            //ViewData["Customer"] = new SelectList(_context.RefCustomers.OrderBy(c => c.Name), "CustomerId", "Name", "CustomerId");
            ViewData["AccountsCodes"] = new SelectList(
                                           _accountsContext.Set<RefChartOfAcc>()
                                               .Where(a => a.IsInactive.Equals(false))
                                               .OrderBy(p => p.AccCode)
                                               .Select(a => new { AccNo = a.AccNo, DisplayValue = $"{a.AccCode} - {a.Description}" }),
                                           "AccNo",
                                           "DisplayValue",
                                           "AccNo"
        );
            return View(tables);
        }


        // GET: Print Invoice 
        public async Task<IActionResult> RepPrintDebitNote(string DebitNoteNo)
        {
            if (DebitNoteNo == null)
            {
                return NotFound();
            }

            var txnDebitNoteHd = await _context.TxnDebitNoteExportHds
                .FirstOrDefaultAsync(m => m.DebitNoteNo == DebitNoteNo);

            if (txnDebitNoteHd == null)
            {
                return NotFound();
            }

            var strjobNo = txnDebitNoteHd.JobNo;

            if (string.IsNullOrEmpty(strjobNo))
            {
                return NotFound(); // Handle the case where JobNo is null or empty
            }

            var txnStuffingPlanHD = _context.TxnStuffingPlanHds
                                            .Where(s => s.JobNumber == strjobNo)
                                            .Include(t=> t.PodStuffingNavigation)
                                            .FirstOrDefault();
            var containerNo = txnStuffingPlanHD.ContainerNo;
            //var containerNo = _context.TxnStuffingPlanHds
            //    .Where(s => s.JobNumber == strjobNo)
            //    .Select(s => s.ContainerNo)
            //    .FirstOrDefault();

            var tables = new DebitNoteExportViewModel
            {
                DebitNoteHdMulti = _context.TxnDebitNoteExportHds.Where(t => t.DebitNoteNo == DebitNoteNo)
                .Include(t => t.ShippingLineDbitNoteExpNavigation)
                .Include(t=>t.AgentDebitNoteExportNavigation),
                DebitNoteDtlMulti = _context.TxnDebitNoteExportDtls.Where(t => t.DebitNoteNo == DebitNoteNo)
                    .Include(t => t.ChargeItemNavigation),

                ExportJobDtlMulti = _context.TxnExportJobDtls.Where(t => t.JobNo == strjobNo),
                ExportJobHdMulti = _context.TxnExportJobHds
                    .Include(t => t.AgentExportNavigation)
                    .Include(t => t.HandlebyExportJobNavigation)
                    .Include(t => t.CreatedByExportJobNavigation)
                    .Include(t => t.ShippingLineExportNavigation)
                    .Include(t => t.ColoaderExportNavigation)
                    .Include(t => t.VesselExportJobDtlNavigation)
                    .Include(t => t.PODExportJobNavigation)
                    .Include(t => t.FDNExportJobNavigation)
                    .Where(t => t.JobNo == strjobNo),
            };

            List<SelectListItem> Currency = new List<SelectListItem>
    {
        new SelectListItem { Value = "USD", Text = "USD" },
        new SelectListItem { Value = "LKR", Text = "LKR" }
    };
            ViewData["CurrencyList"] = new SelectList(Currency, "Value", "Text", "CurrencyList");

            List<SelectListItem> Unit = new List<SelectListItem>
    {
        new SelectListItem { Value = "CBM", Text = "CBM" },
        new SelectListItem { Value = "BL", Text = "BL" },
        new SelectListItem { Value = "CNT", Text = "CNT" }
    };
            ViewData["UnitList"] = new SelectList(Unit, "Value", "Text", "UnitList");

            ViewData["ChargeItemsList"] = new SelectList(_context.Set<RefChargeItemAcc>().Where(a => a.IsActive.Equals(true)).OrderBy(p => p.Description), "ChargeId", "Description", "ChargeId");
            ViewData["Customer"] = new SelectList(_context.RefCustomers.OrderBy(c => c.Name), "CustomerId", "Name", "CustomerId");
            

            ViewData["ContainerNo"] = containerNo;

            DateTime etaPolDate = (DateTime)txnStuffingPlanHD.EtaCbm;
            string formattedDate = etaPolDate.ToString("dd/MM/yyyy");
            ViewData["ETA"] = formattedDate;
            ViewData["POD"] = txnStuffingPlanHD.PodStuffingNavigation.PortName;

            return View(tables);
        }





        // GET: TxnDebitNoteHds/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.TxnDebitNoteExportHds == null)
            {
                return NotFound();
            }

            var txnDebitNoteHd = await _context.TxnDebitNoteExportHds.FindAsync(id);
            if (txnDebitNoteHd == null)
            {
                return NotFound();
            }
            var DebitNoteNo = id;
            var JobNo = txnDebitNoteHd.JobNo;
            var tables = new DebitNoteExportViewModel
            {
                DebitNoteHdMulti = _context.TxnDebitNoteExportHds.Where(t => t.DebitNoteNo == DebitNoteNo),
                DebitNoteDtlMulti = _context.TxnDebitNoteExportDtls.Where(t => t.DebitNoteNo == DebitNoteNo),
                ExportJobDtlMulti = _context.TxnExportJobDtls.Where(t => t.JobNo == JobNo),
                ExportJobHdMulti = _context.TxnExportJobHds
                    .Include(t => t.AgentExportNavigation)
                    .Include(t => t.HandlebyExportJobNavigation)
                    .Include(t => t.CreatedByExportJobNavigation)
                    .Include(t => t.ShippingLineExportNavigation)
                    .Include(t => t.ColoaderExportNavigation)
                    .Include(t => t.VesselExportJobDtlNavigation)
                    .Include(t => t.PODExportJobNavigation)
                    .Include(t => t.FDNExportJobNavigation)
                    .Where(t => t.JobNo == JobNo),

            };
            ViewData["AgentIDNomination"] = new SelectList(_context.RefAgents.Join(_context.RefPorts,
                 a => a.PortId,
                 b => b.PortCode,
                 (a, b) => new
                 {
                     AgentId = a.AgentId,
                     AgentName = a.AgentName + " - " + b.PortName,
                     IsActive = a.IsActive
                 }).Where(a => a.IsActive.Equals(true)).OrderBy(a => a.AgentName), "AgentId", "AgentName", "AgentId");
            List<SelectListItem> Currency = new List<SelectListItem>
                {
                    new SelectListItem { Value = "USD", Text = "USD" },
                    new SelectListItem { Value = "LKR", Text = "LKR" }
                };
            ViewData["CurrencyList"] = new SelectList(Currency, "Value", "Text", "CurrencyList");
            List<SelectListItem> Unit = new List<SelectListItem>
                {
                    new SelectListItem { Value = "CBM", Text = "CBM" },
                    new SelectListItem { Value = "BL", Text = "BL" },
                    new SelectListItem { Value = "CNT", Text = "CNT" }
                };
            ViewData["UnitList"] = new SelectList(Unit, "Value", "Text", "UnitList");
            ViewData["ChargeItemsList"] = new SelectList(_context.Set<RefChargeItemAcc>().Where(a => a.IsActive.Equals(true)).OrderBy(p => p.Description), "ChargeId", "Description", "ChargeId");
            ViewData["ShippingLine"] = new SelectList(_context.RefShippingLines.OrderBy(c => c.Name), "ShippingLineId", "Name", "ShippingLineId");

            ViewData["Customer"] = new SelectList(_context.RefCustomers.OrderBy(c => c.Name), "CustomerId", "Name", "CustomerId");
            return View(tables);
        }

        // POST: TxnDebitNoteHds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string dtlItemsList, [Bind("DebitNoteNo,Date,JobNo,Customer,ExchangeRate,TotalDebitAmountLkr,TotalDebitAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,Approved,Type, AmountPaid,AmountToBePaid, AgentID")] TxnDebitNoteExportHd txnDebitNoteExportHd)
        {
            if (id != txnDebitNoteExportHd.DebitNoteNo)
            {
                return NotFound();
            }

            txnDebitNoteExportHd.LastUpdatedBy = "Admin";
            txnDebitNoteExportHd.LastUpdatedDateTime = DateTime.Now;

            txnDebitNoteExportHd.AmountPaid = 0;
            txnDebitNoteExportHd.AmountToBePaid = txnDebitNoteExportHd.TotalDebitAmountLkr;

            var LocalOverseas = txnDebitNoteExportHd.Type;

            if (LocalOverseas == "Local")
            {
                txnDebitNoteExportHd.DebitAcc = "ACC0023"; // LOCAL DEBTORS  2200-01
            }
            else // Oversease
            {
                txnDebitNoteExportHd.DebitAcc = "ACC0024"; // OVERSEAS DEBTORS  2200-02
                txnDebitNoteExportHd.Type = "Overseas"; // There is an error in the create if the Selection is "Overseae" Type not pass. This is  a temporary solution. 

            }

            txnDebitNoteExportHd.CreditAcc = null;
            var TransDocMode = "Export";
            var TransDocType = "Revenue";
            var TransDocVender = "";

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(dtlItemsList))
                    {
                        var rowsToDelete = _context.TxnDebitNoteExportDtls.Where(t => t.DebitNoteNo == id);
                        if (rowsToDelete != null || rowsToDelete.Any())
                        {
                            // Remove the rows from the database context
                            _context.TxnDebitNoteExportDtls.RemoveRange(rowsToDelete);
                        }
                        var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnDebitNoteExportDtl>>(dtlItemsList);
                        if (DtailItemdataTable != null)
                        {
                            var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the common method  class

                            foreach (var item in DtailItemdataTable)
                            {
                                TxnDebitNoteExportDtl DetailItem = new TxnDebitNoteExportDtl();
                                DetailItem.DebitNoteNo = id; // New CreditNote Number
                                DetailItem.SerialNo = item.SerialNo;
                                DetailItem.ChargeItem = item.ChargeItem;
                                DetailItem.Unit = item.Unit;
                                DetailItem.Rate = item.Rate;
                                DetailItem.Currency = item.Currency;
                                DetailItem.Qty = item.Qty;
                                DetailItem.Amount = item.Amount;
                                DetailItem.BlContainerNo = item.BlContainerNo;
                                DetailItem.AccNo = CommonMethodAccClass.GetChargeItemAccNo(item.ChargeItem, TransDocMode, TransDocType, TransDocVender);
                                _context.TxnDebitNoteExportDtls.Add(DetailItem);
                            }
                        }
                    }
                    _context.Update(txnDebitNoteExportHd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TxnDebitNoteHdExists(txnDebitNoteExportHd.DebitNoteNo))
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
            return View(txnDebitNoteExportHd);
        }

        // GET: TxnDebitNoteHds/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.TxnDebitNoteExportHds == null)
            {
                return NotFound();
            }

            var txnDebitNoteHd = await _context.TxnDebitNoteExportHds
                .FirstOrDefaultAsync(m => m.DebitNoteNo == id);
            if (txnDebitNoteHd == null)
            {
                return NotFound();
            }

            return View(txnDebitNoteHd);
        }

        // POST: TxnDebitNoteHds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.TxnDebitNoteExportHds == null)
            {
                return Problem("Entity set 'FtlcolombOperationContext.TxnDebitNoteHds'  is null.");
            }
            var txnDebitNoteHd = await _context.TxnDebitNoteExportHds.FindAsync(id);
            if (txnDebitNoteHd != null)
            {
                _context.TxnDebitNoteExportHds.Remove(txnDebitNoteHd);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TxnDebitNoteHdExists(string id)
        {
            return (_context.TxnDebitNoteExportHds?.Any(e => e.DebitNoteNo == id)).GetValueOrDefault();
        }
    }
}
