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
    public class TxnCreditNoteExportHdsController : Controller
    {
        private readonly FtlcolombOperationContext _context;
        private readonly FtlcolomboAccountsContext _accountsContext;

        public string jobNo { get; private set; }

        public TxnCreditNoteExportHdsController(FtlcolombOperationContext context, FtlcolomboAccountsContext accountsContext)
        {
            _context = context; 
            _accountsContext = accountsContext;
        }

        // GET: TxnCreditNoteHds
        public async Task<IActionResult> Index(string searchString, string searchType, int pg = 1)
        {
            var fclCreditNotes = await _context.TxnCreditNoteFCLHds.ToListAsync();

            // Pass the data to the view
            return View(fclCreditNotes);
        }


        public async Task<IActionResult> Approve(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var txnCreditNoteHds = await _context.TxnCreditNoteExportHds
                .FirstOrDefaultAsync(m => m.CreditNoteNo == id);

            if (txnCreditNoteHds == null)
            {
                return NotFound();
            }

            jobNo = txnCreditNoteHds.JobNo; // Set the jobNo property

            var tables = new CreditNoteExportViewModel
            {
                CreditNoteHdMulti = _context.TxnCreditNoteExportHds.Where(t => t.CreditNoteNo == id),
                CreditNoteDtlMulti = _context.TxnCreditNoteExportDtls.Where(t => t.CreditNoteNo == id),
               

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

            var txnCreditNoteHds = await _context.TxnCreditNoteExportHds
                .FirstOrDefaultAsync(m => m.CreditNoteNo == id);

            if (txnCreditNoteHds == null)
            {
                return NotFound();
            }

            // Inserting Transaction Data to Acccounts 
            var txnCreditNoteExportDtls = await _context.TxnCreditNoteExportDtls
                .Include(t => t.ChargeItemNavigation)
                .Where(m => m.CreditNoteNo == id)
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
            if (txnCreditNoteExportDtls != null)
            {
                var SerialNo_AccTxn = 1;
                var AccTxnDescription = " Local Creditor CREDT: " + txnCreditNoteHds.CreditNoteNo;
                // First Acc transaction 
                TxnTransactions NewRowAccTxnFirst = new TxnTransactions();
                NewRowAccTxnFirst.TxnNo = nextAccTxnNo;
                NewRowAccTxnFirst.TxnSNo = SerialNo_AccTxn;
                NewRowAccTxnFirst.Date = txnCreditNoteHds.Date; //  Jurnal Date 
                NewRowAccTxnFirst.Description = AccTxnDescription;
                NewRowAccTxnFirst.TxnAccCode = txnCreditNoteHds.CreditAcc;
                NewRowAccTxnFirst.Dr = (decimal)0;
                NewRowAccTxnFirst.Cr = (decimal)txnCreditNoteHds.TotalCreditAmountLkr;
                NewRowAccTxnFirst.RefNo = txnCreditNoteHds.CreditNoteNo; // Debit Note No
                NewRowAccTxnFirst.Note = "";
                NewRowAccTxnFirst.Reconciled = false;
                NewRowAccTxnFirst.DocType = "CreditNTExport";
                NewRowAccTxnFirst.IsMonthEndDone = false;
                NewRowAccTxnFirst.CreatedBy = "Admin";
                NewRowAccTxnFirst.CreatedDateTime = DateTime.Now;
                NewRowAccTxnFirst.Canceled = false;

                _accountsContext.TxnTransactions.Add(NewRowAccTxnFirst);

                foreach (var item in txnCreditNoteExportDtls)
                {
                    // Transaction table Insert 
                    TxnTransactions NewRowAccTxn = new TxnTransactions();

                    SerialNo_AccTxn = SerialNo_AccTxn + 1;
                    NewRowAccTxn.TxnNo = nextAccTxnNo;
                    NewRowAccTxn.TxnSNo = SerialNo_AccTxn;
                    NewRowAccTxn.Date = txnCreditNoteHds.Date; //  Jurnal Date 
                    NewRowAccTxn.Description = item.ChargeItemNavigation.Description + "CRD: " + item.CreditNoteNo;
                    NewRowAccTxn.TxnAccCode = item.AccNo;
                    NewRowAccTxn.Cr = (decimal)0;
                    if (item.Currency == "USD")
                    {
                        var AmtLkr = item.Amount * txnCreditNoteHds.ExchangeRate; // convert to LKR
                        NewRowAccTxn.Dr = (decimal)AmtLkr;
                    }
                    else
                    {
                        NewRowAccTxn.Dr = (decimal)item.Amount;
                    }
                    NewRowAccTxn.RefNo = item.CreditNoteNo; // Invoice No
                    NewRowAccTxn.Note = "";
                    NewRowAccTxn.Reconciled = false;
                    NewRowAccTxn.DocType = "CreditNTExport";
                    NewRowAccTxn.IsMonthEndDone = false;
                    NewRowAccTxn.CreatedBy = "Admin";
                    NewRowAccTxn.CreatedDateTime = DateTime.Now;
                    NewRowAccTxn.Canceled = false;

                    _accountsContext.TxnTransactions.Add(NewRowAccTxn);
                }
            }
            // END Inserting Transaction Data to Acccounts 


            /// Update the Approved property based on the form submission
            txnCreditNoteHds.Approved = approved;
            txnCreditNoteHds.ApprovedBy = "CurrentUserName"; // Replace with the actual user name
            txnCreditNoteHds.ApprovedDateTime = DateTime.Now;

            _context.Update(txnCreditNoteHds);
            await _context.SaveChangesAsync();
            await _accountsContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirect to the appropriate action
        }

        // GET: TxnCreditNoteHds/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TxnCreditNoteExportHds == null)
            {
                return NotFound();
            }

            var txnCreditNoteHd = await _context.TxnCreditNoteExportHds
                .FirstOrDefaultAsync(m => m.CreditNoteNo == id);
            if (txnCreditNoteHd == null)
            {
                return NotFound();
            }
            var jobNo = txnCreditNoteHd.JobNo;

            var tables = new CreditNoteExportViewModel
            {
                CreditNoteHdMulti = _context.TxnCreditNoteExportHds.Where(t => t.CreditNoteNo == id),
                CreditNoteDtlMulti = _context.TxnCreditNoteExportDtls.Where(t => t.CreditNoteNo == id),
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

        //// GET: TxnCreditNoteHds/Create
        public IActionResult Create(string searchString)
        {
            var jobNo = "xxx";

            if (!string.IsNullOrEmpty(searchString))
            {


                jobNo = searchString;

                // Check if results are found
                if (_context.TxnExportJobHds.Any(t => t.JobNo == jobNo))
                {
                    ViewData["JobFound"] = "";
                }
                else
                {
                    ViewData["JobFound"] = "Job Number: " + searchString + " not found";
                }

            }
            var tables = new CreditNoteExportViewModel
            {
                CreditNoteHdMulti = _context.TxnCreditNoteExportHds.Where(t => t.CreditNoteNo == "xxx"),
                CreditNoteDtlMulti = _context.TxnCreditNoteExportDtls.Where(t => t.CreditNoteNo == "xxx"),
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

     

      

        // POST: TxnCreditNoteHds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string dtlItemsList, [Bind("CreditNoteNo,Date,JobNo,Customer,ExchangeRate,TotalCreditAmountLkr,TotalCreditAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,Approved,Type, AmountPaid,AmountToBePaid, AgentID")] TxnCreditNoteFCLHd txnCreditNoteHd)
        {

            // Next RefNumber for txnImportJobDtl
            var nextRefNo = "";
            var TableID = "Txn_CreditNoteHd_Export";
            var refLastNumber = await _context.RefLastNumbers.FindAsync(TableID);

            if (refLastNumber != null)
            {
                var nextNumber = refLastNumber.LastNumber + 1;
                refLastNumber.LastNumber = nextNumber;
                nextRefNo = "ECN" + DateTime.Now.Year.ToString() + nextNumber.ToString().PadLeft(5, '0');
                txnCreditNoteHd.CreditNoteNo = nextRefNo;

                // _context.RefLastNumbers.Remove(refLastNumber);
            }
            else
            {
                return NotFound();
                //return View(tables);
            }

            txnCreditNoteHd.Canceled = false;
            txnCreditNoteHd.Approved = false;
            txnCreditNoteHd.CreatedBy = "Admin";
            txnCreditNoteHd.CreatedDateTime = DateTime.Now;

            txnCreditNoteHd.AmountPaid = 0;
            txnCreditNoteHd.AmountToBePaid = txnCreditNoteHd.TotalCreditAmountLkr;

            var LocalOverseas = txnCreditNoteHd.Type;
            var TransDocMode = "Export";
            var TransDocType = "Expenses";
            var TransDocVender = "";

            if (LocalOverseas == "Local")
            {
                txnCreditNoteHd.CreditAcc = "ACC0067"; // LOCAL CREDITORS  1200-02	
                TransDocVender = "Liner";

            }
            else // Oversease
            {
                txnCreditNoteHd.CreditAcc = "ACC0068"; // OVERSEAS CREDITORS	  1200-03	
                TransDocVender = "Agent";
                txnCreditNoteHd.Type = "Overseas"; // There is an error in the create if the Selection is "Overseae" Type not pass. This is  a temporary solution. 

            }

            txnCreditNoteHd.DebitAcc = null;


            ModelState.Remove("CreditNoteNo");  // Auto genrated
            if (ModelState.IsValid)
            {
                // Adding CargoBreakDown records
                if (!string.IsNullOrWhiteSpace(dtlItemsList))
                {
                    var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnCreditNoteFCLDtl>>(dtlItemsList);
                    if (DtailItemdataTable != null)
                    {
                        var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the common method  class
                        foreach (var item in DtailItemdataTable)
                        {
                            TxnCreditNoteFCLDtl DetailItem = new TxnCreditNoteFCLDtl();
                            DetailItem.CreditNoteNo = nextRefNo; // New CreditNote Number
                            DetailItem.SerialNo = item.SerialNo;
                            DetailItem.ChargeItem = item.ChargeItem;
                            DetailItem.Unit = item.Unit ?? "DefaultUnit"; // Set a default value if 'Unit' is null
                            DetailItem.Rate = item.Rate;
                            DetailItem.Currency = item.Currency;
                            DetailItem.Qty = item.Qty;
                            DetailItem.Amount = item.Amount;
                            DetailItem.BlContainerNo = item.BlContainerNo;
                            DetailItem.AccNo = DetailItem.AccNo = CommonMethodAccClass.GetChargeItemAccNo(item.ChargeItem, TransDocMode, TransDocType, TransDocVender);

                            _context.TxnCreditNoteExportDtls.Add(DetailItem);
                        }
                    }
                }
                _context.Add(txnCreditNoteHd);
                _context.RefLastNumbers.Update(refLastNumber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var tables = new CreditNoteExportViewModel
            {
                CreditNoteHdMulti = _context.TxnCreditNoteExportHds.Where(t => t.CreditNoteNo == "xxx"),
                CreditNoteDtlMulti = _context.TxnCreditNoteExportDtls.Where(t => t.CreditNoteNo == "xxx"),

                ExportJobHdMulti = _context.TxnExportJobHds.Where(t => t.JobNo == jobNo),
                ExportJobDtlMulti = _context.TxnExportJobDtls.Where(t => t.JobNo == jobNo),
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


        private bool TxnCreditNoteHdExists(string id)
        {
          return (_context.TxnCreditNoteExportHds?.Any(e => e.CreditNoteNo == id)).GetValueOrDefault();
        }
    }
}
