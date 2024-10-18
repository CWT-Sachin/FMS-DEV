using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FMS_DEV.ViewModel;
using Newtonsoft.Json;
using FMS_DEV.Models;
using FMS_DEV.ModelsAccounts;

using FMS_DEV.Data;
using FMS_DEV.DataAccounts;
using FMS_DEV.CommonMethods;



namespace FMS_DEV.Controllers
{
    public class TxnDebitNoteFCLHdsController : Controller
    {
        private readonly FtlcolombOperationContext _context;
        private readonly FtlcolomboAccountsContext _accountsContext;

        public string jobNo { get; private set; }

        public TxnDebitNoteFCLHdsController(FtlcolombOperationContext context, FtlcolomboAccountsContext accountsContext)
        {
            _context = context; 
            _accountsContext = accountsContext;
        }

        // GET: TxnDebitNoteHds
        public async Task<IActionResult> Index(string searchString, string searchType, int pg = 1)
        {
            var fclDebitNotes = await _context.TxnDebitNoteFCLHds.ToListAsync();

            // Pass the data to the view
            return View(fclDebitNotes);
        }


        //// GET: TxnDebitNoteHds/Create
        public IActionResult Create(string searchString)
        {
            var fCLjobNo = "xxx";

            if (!string.IsNullOrEmpty(searchString))
            {


                fCLjobNo = searchString;

                // Check if results are found
                if (_context.TxnFCLJobs.Any(t => t.JobNo == fCLjobNo))
                {
                    ViewData["FCLJobFound"] = "";
                }
                else
                {
                    ViewData["FCLJobFound"] = "FCLJob Number: " + searchString + " not found";
                }

            }
            var tables = new DebitNoteFCLViewModel
            {
                DebitNoteFCLHdMulti = _context.TxnDebitNoteFCLHds.Where(t => t.DebitNoteNo == "xxx"),
                DebitNoteFCLDtlMulti = _context.TxnDebitNoteFCLDtls.Where(t => t.DebitNoteNo == "xxx"),
                FCLJobMulti = _context.TxnFCLJobs
                .Include(t => t.ShippingLineFCLJobExportNavigation)
               .Where(t => t.JobNo == fCLjobNo),

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
        public async Task<IActionResult> Create(string dtlItemsList, [Bind("DebitNoteNo,Date,JobNo,Customer,ExchangeRate,TotalDebitAmountLkr,TotalDebitAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,Approved,Type, AmountPaid,AmountToBePaid, AgentID")] TxnDebitNoteFCLHd txnDebitNoteFCLHd)
        {

            // Next RefNumber for txnImportJobDtl
            var nextRefNo = "";
            var TableID = "Txn_DebitNoteHD_FCL";
            var refLastNumber = await _context.RefLastNumbers.FindAsync(TableID);

            if (refLastNumber != null)
            {
                var nextNumber = refLastNumber.LastNumber + 1;
                refLastNumber.LastNumber = nextNumber;
                nextRefNo = "FCN" + DateTime.Now.Year.ToString() + nextNumber.ToString().PadLeft(5, '0');
                txnDebitNoteFCLHd.DebitNoteNo = nextRefNo;

                // _context.RefLastNumbers.Remove(refLastNumber);
            }
            else
            {
                return NotFound();
                //return View(tables);
            }

            txnDebitNoteFCLHd.Canceled = false;
            txnDebitNoteFCLHd.Approved = false;
            txnDebitNoteFCLHd.CreatedBy = "Admin";
            txnDebitNoteFCLHd.CreatedDateTime = DateTime.Now;

            txnDebitNoteFCLHd.AmountPaid = 0;
            txnDebitNoteFCLHd.AmountToBePaid = txnDebitNoteFCLHd.TotalDebitAmountLkr;

            var LocalOverseas = txnDebitNoteFCLHd.Type;
            var TransDocMode = "Export";
            var TransDocType = "Expenses";
            var TransDocVender = "";

            if (LocalOverseas == "Local")
            {
                txnDebitNoteFCLHd.DebitAcc = "ACC0067"; // LOCAL DebitORS  1200-02	
                TransDocVender = "Liner";

            }
            else // Oversease
            {
                txnDebitNoteFCLHd.DebitAcc = "ACC0068"; // OVERSEAS DebitORS	  1200-03	
                TransDocVender = "Agent";
                txnDebitNoteFCLHd.Type = "Overseas"; // There is an error in the create if the Selection is "Overseae" Type not pass. This is  a temporary solution. 

            }

            txnDebitNoteFCLHd.DebitAcc = null;


            ModelState.Remove("DebitNoteNo");  // Auto genrated
            if (ModelState.IsValid)
            {
                // Adding CargoBreakDown records
                if (!string.IsNullOrWhiteSpace(dtlItemsList))
                {
                    var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnDebitNoteFCLDtl>>(dtlItemsList);
                    if (DtailItemdataTable != null)
                    {
                        var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the common method  class
                        foreach (var item in DtailItemdataTable)
                        {
                            TxnDebitNoteFCLDtl DetailItem = new TxnDebitNoteFCLDtl();
                            DetailItem.DebitNoteNo = nextRefNo; // New DebitNote Number
                            DetailItem.SerialNo = item.SerialNo;
                            DetailItem.ChargeItem = item.ChargeItem;
                            DetailItem.Unit = item.Unit ?? "DefaultUnit"; // Set a default value if 'Unit' is null
                            DetailItem.Rate = item.Rate;
                            DetailItem.Currency = item.Currency;
                            DetailItem.Qty = item.Qty;
                            DetailItem.Amount = item.Amount;
                            DetailItem.BlContainerNo = item.BlContainerNo;
                            DetailItem.AccNo = DetailItem.AccNo = CommonMethodAccClass.GetChargeItemAccNo(item.ChargeItem, TransDocMode, TransDocType, TransDocVender);

                            _context.TxnDebitNoteFCLDtls.Add(DetailItem);
                        }
                    }
                }
                _context.Add(txnDebitNoteFCLHd);
                _context.RefLastNumbers.Update(refLastNumber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var tables = new DebitNoteFCLViewModel
            {
                DebitNoteFCLHdMulti = _context.TxnDebitNoteFCLHds.Where(t => t.DebitNoteNo == "xxx"),
                DebitNoteFCLDtlMulti = _context.TxnDebitNoteFCLDtls.Where(t => t.DebitNoteNo == "xxx"),
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
            ViewData["RefAgentList"] = new SelectList(_context.Set<RefAgent>().Where(a => a.IsActive.Equals(true)).OrderBy(p => p.AgentName), "AgentID", "AgentName", "AgentID");
            ViewData["Customer"] = new SelectList(_context.RefCustomers.OrderBy(c => c.Name), "CustomerId", "Name", "CustomerId");

            return View(tables);
        }


        // GET: TxnDebitNoteHds/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.TxnDebitNoteFCLHds == null)
            {
                return NotFound();
            }

            var txnDebitNoteHd = await _context.TxnDebitNoteFCLHds.FindAsync(id);
            if (txnDebitNoteHd == null)
            {
                return NotFound();
            }
            var DebitNoteNo = id;
            var JobNo = txnDebitNoteHd.JobNo;

            var tables = new DebitNoteFCLViewModel
            {
                DebitNoteFCLHdMulti = _context.TxnDebitNoteFCLHds.Where(t => t.DebitNoteNo == id),
                DebitNoteFCLDtlMulti = _context.TxnDebitNoteFCLDtls.Where(t => t.DebitNoteNo == id),
                FCLJobMulti = _context.TxnFCLJobs
                .Include(t => t.ShippingLineFCLJobExportNavigation)
               .Where(t => t.JobNo == jobNo),

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
        public async Task<IActionResult> Edit(string id, string dtlItemsList, [Bind("DebitNoteNo,Date,JobNo,Customer,ExchangeRate,TotalDebitAmountLkr,TotalDebitAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,Approved ,Type, AmountPaid,AmountToBePaid, AgentID")] TxnDebitNoteFCLHd txnDebitNoteHd)
        {
            if (id != txnDebitNoteHd.DebitNoteNo)
            {
                return NotFound();
            }
            txnDebitNoteHd.LastUpdatedBy = "Admin";
            txnDebitNoteHd.LastUpdatedDateTime = DateTime.Now;

            txnDebitNoteHd.AmountPaid = 0;
            txnDebitNoteHd.AmountToBePaid = txnDebitNoteHd.TotalDebitAmountLkr;

            var LocalOverseas = txnDebitNoteHd.Type;
            var TransDocMode = "Import";
            var TransDocType = "Expenses";
            var TransDocVender = "";

            if (LocalOverseas == "Local")
            {
                txnDebitNoteHd.DebitAcc = "ACC0067"; // LOCAL DebitORS  1200-02	
                TransDocVender = "Liner";

            }
            else // Oversease
            {
                txnDebitNoteHd.DebitAcc = "ACC0068"; // OVERSEAS DebitORS	  1200-03	
                TransDocVender = "Agent";
                txnDebitNoteHd.Type = "Overseas"; // There is an error in the create if the Selection is "Overseae" Type not pass. This is  a temporary solution. 

            }

            txnDebitNoteHd.DebitAcc = null;



            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(dtlItemsList))
                    {
                        var rowsToDelete = _context.TxnDebitNoteFCLDtls.Where(t => t.DebitNoteNo == id);
                        if (rowsToDelete != null || rowsToDelete.Any())
                        {
                            // Remove the rows from the database context
                            _context.TxnDebitNoteFCLDtls.RemoveRange(rowsToDelete);
                        }
                        var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnDebitNoteFCLDtl>>(dtlItemsList);
                        if (DtailItemdataTable != null)
                        {
                            var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the common method  class

                            foreach (var item in DtailItemdataTable)
                            {
                                TxnDebitNoteFCLDtl DetailItem = new TxnDebitNoteFCLDtl();
                                DetailItem.DebitNoteNo = id; // New DebitNote Number
                                DetailItem.SerialNo = item.SerialNo;
                                DetailItem.ChargeItem = item.ChargeItem;
                                DetailItem.Unit = item.Unit;
                                DetailItem.Rate = item.Rate;
                                DetailItem.Currency = item.Currency;
                                DetailItem.Qty = item.Qty;
                                DetailItem.Amount = item.Amount;
                                DetailItem.BlContainerNo = item.BlContainerNo;
                                DetailItem.AccNo = DetailItem.AccNo = CommonMethodAccClass.GetChargeItemAccNo(item.ChargeItem, TransDocMode, TransDocType, TransDocVender);
                                _context.TxnDebitNoteFCLDtls.Add(DetailItem);
                            }
                        }
                    }
                    _context.Update(txnDebitNoteHd);
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!TxnDebitNoteHdExists(txnDebitNoteHd.DebitNoteNo))
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
            return View(txnDebitNoteHd);
        }


        private bool TxnDebitNoteHdExists(string id)
        {
            return (_context.TxnDebitNoteFCLHds?.Any(e => e.DebitNoteNo == id)).GetValueOrDefault();
        }





        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TxnDebitNoteFCLHds == null)
            {
                return NotFound();
            }

            var txnDebitNoteFCLHd = await _context.TxnDebitNoteFCLHds
                .FirstOrDefaultAsync(m => m.DebitNoteNo == id);
            if (txnDebitNoteFCLHd == null)
            {
                return NotFound();
            }
            var jobNo = txnDebitNoteFCLHd.JobNo;

            //var tables = new DebitNoteFCLViewModel
            //{
            //    DebitNoteFCLHdMulti = await _context.TxnDebitNoteFCLHds.Where(t => t.DebitNoteNo == "xxx").ToListAsync(),
            //    DebitNoteFCLDtlMulti = await _context.TxnDebitNoteFCLDtls.Where(t => t.DebitNoteNo == "xxx").ToListAsync(),
            //    FCLJobMulti = await  _context.TxnFCLJobs
            //    .Include(t => t.ShippingLineFCLJobExportNavigation)
            //   .Where(t => t.JobNo == jobNo).ToListAsync(),

            //};

            var tables = new DebitNoteFCLViewModel
            {
                DebitNoteFCLHdMulti = await _context.TxnDebitNoteFCLHds
        .Where(t => t.DebitNoteNo == id) // Change "xxx" to "id"
        .ToListAsync(),
                DebitNoteFCLDtlMulti = await _context.TxnDebitNoteFCLDtls
        .Where(t => t.DebitNoteNo == id) // Change "xxx" to "id"
        .ToListAsync(),
                FCLJobMulti = await _context.TxnFCLJobs
        .Include(t => t.ShippingLineFCLJobExportNavigation)
        .Where(t => t.JobNo == jobNo).ToListAsync(),
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


        public async Task<IActionResult> Approve(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var txnDebitNoteHds = await _context.TxnDebitNoteFCLHds
                .FirstOrDefaultAsync(m => m.DebitNoteNo == id);

            if (txnDebitNoteHds == null)
            {
                return NotFound();
            }

            jobNo = txnDebitNoteHds.JobNo; // Set the jobNo property

            var tables = new DebitNoteFCLViewModel
            {
                DebitNoteFCLHdMulti = _context.TxnDebitNoteFCLHds.Where(t => t.DebitNoteNo == id),
                DebitNoteFCLDtlMulti = _context.TxnDebitNoteFCLDtls.Where(t => t.DebitNoteNo == id),


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

            var txnDebitNoteHds = await _context.TxnDebitNoteFCLHds
                .FirstOrDefaultAsync(m => m.DebitNoteNo == id);

            if (txnDebitNoteHds == null)
            {
                return NotFound();
            }

            // Inserting Transaction Data to Acccounts 
            var txnDebitNoteFCLDtls = await _context.TxnDebitNoteFCLDtls
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
            if (txnDebitNoteFCLDtls != null)
            {
                var SerialNo_AccTxn = 1;
                var AccTxnDescription = " Local Debitor CREDT: " + txnDebitNoteHds.DebitNoteNo;
                // First Acc transaction 
                TxnTransactions NewRowAccTxnFirst = new TxnTransactions();
                NewRowAccTxnFirst.TxnNo = nextAccTxnNo;
                NewRowAccTxnFirst.TxnSNo = SerialNo_AccTxn;
                NewRowAccTxnFirst.Date = txnDebitNoteHds.Date; //  Jurnal Date 
                NewRowAccTxnFirst.Description = AccTxnDescription;
                NewRowAccTxnFirst.TxnAccCode = txnDebitNoteHds.DebitAcc;
                NewRowAccTxnFirst.Dr = (decimal)0;
                NewRowAccTxnFirst.Cr = (decimal)txnDebitNoteHds.TotalDebitAmountLkr;
                NewRowAccTxnFirst.RefNo = txnDebitNoteHds.DebitNoteNo; // Debit Note No
                NewRowAccTxnFirst.Note = "";
                NewRowAccTxnFirst.Reconciled = false;
                NewRowAccTxnFirst.DocType = "DebitNTFCL";
                NewRowAccTxnFirst.IsMonthEndDone = false;
                NewRowAccTxnFirst.CreatedBy = "Admin";
                NewRowAccTxnFirst.CreatedDateTime = DateTime.Now;
                NewRowAccTxnFirst.Canceled = false;

                _accountsContext.TxnTransactions.Add(NewRowAccTxnFirst);

                foreach (var item in txnDebitNoteFCLDtls)
                {
                    // Transaction table Insert 
                    TxnTransactions NewRowAccTxn = new TxnTransactions();

                    SerialNo_AccTxn = SerialNo_AccTxn + 1;
                    NewRowAccTxn.TxnNo = nextAccTxnNo;
                    NewRowAccTxn.TxnSNo = SerialNo_AccTxn;
                    NewRowAccTxn.Date = txnDebitNoteHds.Date; //  Jurnal Date 
                    NewRowAccTxn.Description = item.ChargeItemNavigation.Description + "CRD: " + item.DebitNoteNo;
                    NewRowAccTxn.TxnAccCode = item.AccNo;
                    NewRowAccTxn.Cr = (decimal)0;
                    if (item.Currency == "USD")
                    {
                        var AmtLkr = item.Amount * txnDebitNoteHds.ExchangeRate; // convert to LKR
                        NewRowAccTxn.Dr = (decimal)AmtLkr;
                    }
                    else
                    {
                        NewRowAccTxn.Dr = (decimal)item.Amount;
                    }
                    NewRowAccTxn.RefNo = item.DebitNoteNo; // Invoice No
                    NewRowAccTxn.Note = "";
                    NewRowAccTxn.Reconciled = false;
                    NewRowAccTxn.DocType = "DebitNTFCL";
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


        // GET: Print DebitNote 
        public async Task<IActionResult> RepPrintDebitNote(string DebitNoteNo)
        {
            if (DebitNoteNo == null)
            {
                return NotFound();
            }

            var txnDebitNoteHd = await _context.TxnDebitNoteFCLHds
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

            var containerNo = _context.TxnStuffingPlanHds
                .Where(s => s.JobNumber == strjobNo)
                .Select(s => s.ContainerNo)
                .FirstOrDefault();


            var tables = new DebitNoteFCLViewModel
            {
                DebitNoteFCLHdMulti = _context.TxnDebitNoteFCLHds.Include(c => c.ShippingLineDebitNoteFCLNavigation)
                .Include(a => a.AgentDebitNoteFCLNavigation).Where(t => t.DebitNoteNo == DebitNoteNo),
                DebitNoteFCLDtlMulti = _context.TxnDebitNoteFCLDtls.Include(c => c.ChargeItemNavigation).Where(t => t.DebitNoteNo == DebitNoteNo),
                FCLJobMulti = _context.TxnFCLJobs
                .Include(t => t.ShippingLineFCLJobExportNavigation)
               .Where(t => t.JobNo == jobNo),
            };



            ViewData["ContainerNo"] = containerNo;

            //DateTime etaDate = (DateTime)tables.FCLJobMulti.FirstOrDefault().ETA;
            //string formattedDate = etaDate.ToString("dd/MM/yyyy");
            //ViewData["ETA"] = formattedDate;
            //ViewData["POD"] = "COLOMBO";
            //ViewData["POL"] = tables.FCLJobMulti.FirstOrDefault().POD;

            return View(tables);

        }






    }
}
