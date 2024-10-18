using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FMS_DEV.ViewModel;
using Newtonsoft.Json;
using FMS_DEV.CommonMethods;
using FMS_DEV.Models;
using FMS_DEV.ModelsAccounts;

using FMS_DEV.Data;
using FMS_DEV.DataAccounts;


namespace FMS_DEV.Controllers
{
    public class TxnCreditNoteImportHdsController : Controller
    {
        private readonly FtlcolombOperationContext _context;
        private readonly FtlcolomboAccountsContext _accountsContext;

       

        public TxnCreditNoteImportHdsController(FtlcolombOperationContext context, FtlcolomboAccountsContext accountsContext)
        {
            _context = context;
            _accountsContext = accountsContext;
        }

        public string? jobNo { get; private set; }

        // GET: TxnCreditNoteHds
        public async Task<IActionResult> Index(string searchString, string searchType, int pg = 1)
        {
            var txnCreditNoteHds = _context.TxnCreditNoteImportHds.OrderByDescending(p => p.CreditNoteNo).Include(t => t.ShippingLineCreditNoteImpNavigation).Include(t => t.AgentCreditNoteImportNavigation);

            if (searchType == "Approve")
            {
                // Filter only not approved invoices
                txnCreditNoteHds = txnCreditNoteHds.Where(txnCreditNoteHds => txnCreditNoteHds.Approved != true).OrderByDescending(p => p.CreditNoteNo).Include(t => t.ShippingLineCreditNoteImpNavigation).Include(t => t.AgentCreditNoteImportNavigation);
            }
            else if (!String.IsNullOrEmpty(searchString))
            {
                if (searchType == "CreditNote")
                {
                    txnCreditNoteHds = txnCreditNoteHds.Where(txnCreditNoteHds => txnCreditNoteHds.CreditNoteNo.Contains(searchString)).OrderByDescending(p => p.CreditNoteNo).Include(t => t.ShippingLineCreditNoteImpNavigation).Include(t => t.AgentCreditNoteImportNavigation);
                }
                else if (searchType == "Job")
                {
                    txnCreditNoteHds = txnCreditNoteHds.Where(txnCreditNoteHds => txnCreditNoteHds.JobNo.Contains(searchString)).OrderByDescending(p => p.CreditNoteNo).Include(t => t.ShippingLineCreditNoteImpNavigation).Include(t => t.AgentCreditNoteImportNavigation);
                }

                // Check if results are found
                if (txnCreditNoteHds.Any())
                {
                    ViewData["CreditNoteFound"] = "";
                }
                else
                {
                    ViewData["CreditNoteFound"] = $"{searchType} Number: {searchString} not found";
                }
            }

            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int recsCount = txnCreditNoteHds.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = txnCreditNoteHds.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;
            return View(data);
        }


        public async Task<IActionResult> Approve(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var txnCreditNoteHds = await _context.TxnCreditNoteImportHds
                .FirstOrDefaultAsync(m => m.CreditNoteNo == id);

            if (txnCreditNoteHds == null)
            {
                return NotFound();
            }

            jobNo = txnCreditNoteHds.JobNo; // Set the jobNo property

            var tables = new CreditNoteImportViewModel
            {
                CreditNoteHdMulti = _context.TxnCreditNoteImportHds.Where(t => t.CreditNoteNo == id),
                CreditNoteDtlMulti = _context.TxnCreditNoteImportDtls.Where(t => t.CreditNoteNo == id),
               
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

            var txnCreditNoteHds = await _context.TxnCreditNoteImportHds
                .FirstOrDefaultAsync(m => m.CreditNoteNo == id);

            if (txnCreditNoteHds == null)
            {
                return NotFound();
            }

            // Inserting Transaction Data to Acccounts 
            var txnCreditNoteImportDtls = await _context.TxnCreditNoteImportDtls
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
            if (txnCreditNoteImportDtls != null)
            {
                var SerialNo_AccTxn = 1;
                var AccTxnDescription = "Creditor CREDT: " + txnCreditNoteHds.CreditNoteNo;
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
                NewRowAccTxnFirst.DocType = "CreditNTImport";
                NewRowAccTxnFirst.IsMonthEndDone = false;
                NewRowAccTxnFirst.CreatedBy = "Admin";
                NewRowAccTxnFirst.CreatedDateTime = DateTime.Now;
                NewRowAccTxnFirst.Canceled = false;

                _accountsContext.TxnTransactions.Add(NewRowAccTxnFirst);

                foreach (var item in txnCreditNoteImportDtls)
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
                    NewRowAccTxn.DocType = "CreditNTImport";
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
            if (id == null || _context.TxnCreditNoteImportHds == null)
            {
                return NotFound();
            }

            var txnCreditNoteHd = await _context.TxnCreditNoteImportHds
                .FirstOrDefaultAsync(m => m.CreditNoteNo == id);
            if (txnCreditNoteHd == null)
            {
                return NotFound();
            }
            var jobNo = txnCreditNoteHd.JobNo;

            var tables = new CreditNoteImportViewModel
            {
                CreditNoteHdMulti = _context.TxnCreditNoteImportHds.Where(t => t.CreditNoteNo == id),
                CreditNoteDtlMulti = _context.TxnCreditNoteImportDtls.Where(t => t.CreditNoteNo == id),
                ImportJobDtlMulti = _context.TxnImportJobDtls.Where(t => t.JobNo == jobNo),
                ImportJobHdMulti = _context.TxnImportJobHds
                    .Include(t => t.AgentNavigation)
                    .Include(t => t.HandlebyImportJobNavigation)
                    .Include(t => t.ShippingLineNavigation)
                    .Include(t => t.VesselImportJobDtlNavigation)
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
                if (_context.TxnImportJobHds.Any(t => t.JobNo == jobNo))
                {
                    ViewData["JobFound"] = "";
                }
                else
                {
                    ViewData["JobFound"] = "Job Number: " + searchString + " not found";
                }

            }
            var tables = new CreditNoteImportViewModel
            {
                CreditNoteHdMulti = _context.TxnCreditNoteImportHds.Where(t => t.CreditNoteNo == "xxx"),
                CreditNoteDtlMulti = _context.TxnCreditNoteImportDtls.Where(t => t.CreditNoteNo == "xxx"),
                ImportJobDtlMulti = _context.TxnImportJobDtls.Where(t => t.JobNo == jobNo),
                ImportJobHdMulti = _context.TxnImportJobHds
                    .Include(t => t.AgentNavigation)
                    .Include(t => t.HandlebyImportJobNavigation)
                    .Include(t => t.ShippingLineNavigation)
                    .Include(t => t.VesselImportJobDtlNavigation)
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
        public async Task<IActionResult> Create(string dtlItemsList, [Bind("CreditNoteNo,Date,JobNo,Customer,ExchangeRate,TotalCreditAmountLkr,TotalCreditAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Approved,Type, AmountPaid,AmountToBePaid, AgentID")] TxnCreditNoteImportHd txnCreditNoteHd)
        {

            // Next RefNumber for txnImportJobDtl
            var nextRefNo = "";
            var TableID = "Txn_CreditNoteHd_Import";
            var refLastNumber = await _context.RefLastNumbers.FindAsync(TableID);

            if (refLastNumber != null)
            {
                var nextNumber = refLastNumber.LastNumber + 1;
                refLastNumber.LastNumber = nextNumber;
                nextRefNo = "OCN" + DateTime.Now.Year.ToString() + nextNumber.ToString().PadLeft(5, '0');
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
            var TransDocMode = "Import";
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
                TransDocVender = "Liner";
                txnCreditNoteHd.Type = "Overseas"; // There is an error in the create if the Selection is "Overseae" Type not pass. This is  a temporary solution. 

            }

            txnCreditNoteHd.DebitAcc = null;


            ModelState.Remove("CreditNoteNo");  // Auto genrated
            if (ModelState.IsValid)
            {
                // Adding CargoBreakDown records
                if (!string.IsNullOrWhiteSpace(dtlItemsList))
                {
                    var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnCreditNoteImportDtl>>(dtlItemsList);
                    if (DtailItemdataTable != null)
                    {
                        var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the common method  class

                        foreach (var item in DtailItemdataTable)
                        {
                            TxnCreditNoteImportDtl DetailItem = new TxnCreditNoteImportDtl();
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

                            _context.TxnCreditNoteImportDtls.Add(DetailItem);

                        }
                    }
                }
                _context.Add(txnCreditNoteHd);
                _context.RefLastNumbers.Update(refLastNumber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var tables = new CreditNoteImportViewModel
            {
                CreditNoteHdMulti = _context.TxnCreditNoteImportHds.Where(t => t.CreditNoteNo == "xxx"),
                CreditNoteDtlMulti = _context.TxnCreditNoteImportDtls.Where(t => t.CreditNoteNo == "xxx"),

                //ImportJobHdMulti = _context.TxnImportJobHds.Where(t => t.JobNo == "xxx"), 
                //ImportJobDtlMulti = _context.TxnImportJobDtls.Where(t => t.JobNo == "xxx"), 

                ImportJobHdMulti = _context.TxnImportJobHds.Where(t => t.JobNo == jobNo),
                ImportJobDtlMulti = _context.TxnImportJobDtls.Where(t => t.JobNo == jobNo),
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
        // GET: Print CreditNote 
        public async Task<IActionResult> RepPrintCreditNote(string CreditNoteNo)
        {
            if (CreditNoteNo == null )
            {
                return NotFound();
            }

            var txnCreditNoteHd = await _context.TxnCreditNoteImportHds
                .FirstOrDefaultAsync(m => m.CreditNoteNo == CreditNoteNo);

            if (txnCreditNoteHd == null)
            {
                return NotFound();
            }

            var strjobNo = txnCreditNoteHd.JobNo;

            if (string.IsNullOrEmpty(strjobNo))
            {
                return NotFound(); // Handle the case where JobNo is null or empty
            }

            var containerNo = _context.TxnImportJobCnts
                .Where(s => s.JobNo == strjobNo)
                .Select(s => s.ContainerNo)
                .FirstOrDefault();
        

            var tables = new CreditNoteImportViewModel
            {
                CreditNoteHdMulti = _context.TxnCreditNoteImportHds.Where(t =>t.CreditNoteNo == CreditNoteNo)
                                .Include(t => t.AgentCreditNoteImportNavigation)
                .Include(t => t.ShippingLineCreditNoteImpNavigation),
                CreditNoteDtlMulti = _context.TxnCreditNoteImportDtls.Where(t => t.CreditNoteNo == CreditNoteNo)
                .Include(t => t.ChargeItemNavigation),
                ImportJobDtlMulti = _context.TxnImportJobDtls.Where(t => t.JobNo == strjobNo),
                ImportJobHdMulti = _context.TxnImportJobHds
                    .Include(t => t.AgentNavigation)
                    .Include(t => t.HandlebyImportJobNavigation)
                    .Include(t => t.ShippingLineNavigation)
                    .Include(t => t.VesselImportJobDtlNavigation)
                    .Include(t => t.PolNavigation)
                  .Where(t => t.JobNo == strjobNo),
            };

            ViewData["ContainerNo"] = containerNo;

            DateTime etaDate = (DateTime)tables.ImportJobHdMulti.FirstOrDefault().Eta;
            string formattedDate = etaDate.ToString("dd/MM/yyyy");
            ViewData["ETA"] = formattedDate;
            ViewData["POD"] = "COLOMBO";
            ViewData["POL"] = tables.ImportJobHdMulti.FirstOrDefault().PolNavigation.PortName;
            
            return View(tables);

        }

        // GET: TxnCreditNoteHds/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.TxnCreditNoteImportHds == null)
            {
                return NotFound();
            }

            var txnCreditNoteHd = await _context.TxnCreditNoteImportHds.FindAsync(id);
            if (txnCreditNoteHd == null)
            {
                return NotFound();
            }
            var CreditNoteNo = id;
            var JobNo = txnCreditNoteHd.JobNo;

            var tables = new CreditNoteImportViewModel
            {
                CreditNoteHdMulti = _context.TxnCreditNoteImportHds.Where(t => t.CreditNoteNo == CreditNoteNo),
                CreditNoteDtlMulti = _context.TxnCreditNoteImportDtls.Where(t => t.CreditNoteNo == CreditNoteNo),
                ImportJobDtlMulti = _context.TxnImportJobDtls.Where(t => t.JobNo == JobNo),
                ImportJobHdMulti = _context.TxnImportJobHds
                    .Include(t => t.AgentNavigation)
                    .Include(t => t.HandlebyImportJobNavigation)
                    .Include(t => t.ShippingLineNavigation)
                    .Include(t => t.VesselImportJobDtlNavigation)
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

        // POST: TxnCreditNoteHds/Edit/5 
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string dtlItemsList, [Bind("CreditNoteNo,Date,JobNo,Customer,ExchangeRate,TotalCreditAmountLkr,TotalCreditAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,Approved ,Type, AmountPaid,AmountToBePaid, AgentID")] TxnCreditNoteImportHd txnCreditNoteHd)
        {
            if (id != txnCreditNoteHd.CreditNoteNo)
            {
                return NotFound();
            }
            txnCreditNoteHd.LastUpdatedBy = "Admin";
            txnCreditNoteHd.LastUpdatedDateTime = DateTime.Now;

            txnCreditNoteHd.AmountPaid = 0;
            txnCreditNoteHd.AmountToBePaid = txnCreditNoteHd.TotalCreditAmountLkr;

            var LocalOverseas = txnCreditNoteHd.Type;
            var TransDocMode = "Import";
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



            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(dtlItemsList))
                    {
                        var rowsToDelete = _context.TxnCreditNoteImportDtls.Where(t => t.CreditNoteNo == id);
                        if (rowsToDelete != null || rowsToDelete.Any())
                        {
                            // Remove the rows from the database context
                            _context.TxnCreditNoteImportDtls.RemoveRange(rowsToDelete);
                        }
                        var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnCreditNoteImportDtl>>(dtlItemsList);
                        if (DtailItemdataTable != null)
                        {
                            var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the common method  class

                            foreach (var item in DtailItemdataTable)
                            {
                                TxnCreditNoteImportDtl DetailItem = new TxnCreditNoteImportDtl();
                                DetailItem.CreditNoteNo = id; // New CreditNote Number
                                DetailItem.SerialNo = item.SerialNo;
                                DetailItem.ChargeItem = item.ChargeItem;
                                DetailItem.Unit = item.Unit;
                                DetailItem.Rate = item.Rate;
                                DetailItem.Currency = item.Currency;
                                DetailItem.Qty = item.Qty;
                                DetailItem.Amount = item.Amount;
                                DetailItem.BlContainerNo = item.BlContainerNo;
                                DetailItem.AccNo = DetailItem.AccNo = CommonMethodAccClass.GetChargeItemAccNo(item.ChargeItem, TransDocMode, TransDocType, TransDocVender);
                                _context.TxnCreditNoteImportDtls.Add(DetailItem);
                            }
                        }
                    }
                    _context.Update(txnCreditNoteHd);
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!TxnCreditNoteHdExists(txnCreditNoteHd.CreditNoteNo))
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
            return View(txnCreditNoteHd);
        }

        // GET: TxnCreditNoteHds/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.TxnCreditNoteImportHds == null)
            {
                return NotFound();
            }

            var txnCreditNoteHd = await _context.TxnCreditNoteImportHds
                .FirstOrDefaultAsync(m => m.CreditNoteNo == id);
            if (txnCreditNoteHd == null)
            {
                return NotFound();
            }

            return View(txnCreditNoteHd);
        }

        // POST: TxnCreditNoteHds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.TxnCreditNoteImportHds == null)
            {
                return Problem("Entity set 'FtlcolombOperationContext.TxnCreditNoteHds'  is null.");
            }
            var txnCreditNoteHd = await _context.TxnCreditNoteImportHds.FindAsync(id);
            if (txnCreditNoteHd != null)
            {
                _context.TxnCreditNoteImportHds.Remove(txnCreditNoteHd);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TxnCreditNoteHdExists(string id)
        {
          return (_context.TxnCreditNoteImportHds?.Any(e => e.CreditNoteNo == id)).GetValueOrDefault();
        }
    }
}
