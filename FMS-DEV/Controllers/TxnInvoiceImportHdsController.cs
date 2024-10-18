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
    public class TxnInvoiceImportHdsController : Controller
    {
        private readonly FtlcolombOperationContext _context;
        private readonly FtlcolomboAccountsContext _accountsContext;


        public string jobNo { get; private set; }

        public TxnInvoiceImportHdsController(FtlcolombOperationContext context, FtlcolomboAccountsContext accountsContext)
        {
            _context = context;
            _accountsContext = accountsContext;
        }


        public async Task<IActionResult> Index(string searchString, string searchType, int pg = 1)
        {

            var ftlcolombOperationContext = _context.TxnImportJobHds.OrderByDescending(b => b.JobNo)
                    .Include(t => t.LastPortNavigation)
                    .Include(t => t.AgentNavigation)
                    .Include(t => t.PolNavigation)
                    .Include(t => t.ShippingLineNavigation)
                    .Include(t => t.TerminalNavigation)
                    .Include(t => t.VesselImportJobDtlNavigation) ;

            if (!String.IsNullOrEmpty(searchString))
            {
                ftlcolombOperationContext = _context.TxnImportJobHds.Where(p => p.JobNo.Contains(searchString)).OrderByDescending(b => b.JobNo)
                    .Include(t => t.LastPortNavigation)
                    .Include(t => t.AgentNavigation)
                    .Include(t => t.PolNavigation)
                    .Include(t => t.ShippingLineNavigation)
                    .Include(t => t.TerminalNavigation)
                    .Include(t => t.VesselImportJobDtlNavigation);
            }
            var txnInvoiceImportHds = _context.TxnInvoiceImportHds.OrderByDescending(p => p.InvoiceNo).Include(t => t.CustomerInvoiceImportHd);

            if (searchType == "Approve")
            {
                // Filter only not approved invoices
                txnInvoiceImportHds = txnInvoiceImportHds.Where(txnInvoiceImportHds => txnInvoiceImportHds.Approved != true).OrderByDescending(p => p.InvoiceNo).Include(t => t.CustomerInvoiceImportHd);
            }
            else if (!String.IsNullOrEmpty(searchString))
            {
                if (searchType == "Invoice")
                {
                    txnInvoiceImportHds = txnInvoiceImportHds.Where(txnInvoiceImportHds => txnInvoiceImportHds.InvoiceNo.Contains(searchString)).OrderByDescending(p => p.InvoiceNo).Include(t => t.CustomerInvoiceImportHd);
                }
                else if (searchType == "Job")
                {
                    txnInvoiceImportHds = txnInvoiceImportHds.Where(txnInvoiceImportHds => txnInvoiceImportHds.JobNo.Contains(searchString)).OrderByDescending(p => p.InvoiceNo).Include(t => t.CustomerInvoiceImportHd);
                }

                // Check if results are found
                if (txnInvoiceImportHds.Any())
                {
                    ViewData["InvoiceFound"] = "";
                }
                else
                {
                    ViewData["InvoiceFound"] = $"{searchType} Number: {searchString} not found";
                }
            }

            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int recsCount = txnInvoiceImportHds.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = txnInvoiceImportHds.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;
            return View(data);
        }



        // GET: Print Invoice 
        public async Task<IActionResult> RepPrintInvoice(string InvoiceNo)
        {
            if (InvoiceNo == null || _context.TxnInvoiceImportHds == null)
            {
                return NotFound();
            }

            var txnInvoiceImportHd = await _context.TxnInvoiceImportHds
                .FirstOrDefaultAsync(m => m.InvoiceNo == InvoiceNo);

            var RefNoBlDtl = txnInvoiceImportHd.Blno; // BL no contain the refNo

            if (txnInvoiceImportHd == null)
            {
                return NotFound();
            }

            var strjobNo = txnInvoiceImportHd.JobNo;

            var containerNoList = _context.TxnImportJobCnts
                .Where(s => s.JobNo == strjobNo && s.RefNo == RefNoBlDtl)
                .ToList();

            var tables = new InvoiceImportViewModel
            {
                InvoiceHdMulti = _context.TxnInvoiceImportHds.Where(t => t.InvoiceNo == InvoiceNo)
                    .Include(t => t.CustomerInvoiceImportHd),
                InvoiceDtMulti = _context.TxnInvoiceImportDtls.Where(t => t.InvoiceNo == InvoiceNo)
                    .Include(t => t.ChargeItemNavigation),
                ImportJobDtlMulti = _context.TxnImportJobDtls.Where(t => t.JobNo == strjobNo),
                ImportJobHdMulti = _context.TxnImportJobHds
                    .Include(t => t.HandlebyImportJobNavigation)
                    .Include(t => t.LastPortNavigation)
                    .Include(t => t.AgentNavigation)
                    .Include(t => t.PolNavigation)
                    .Include(t => t.ShippingLineNavigation)
                    .Include(t => t.TerminalNavigation)
                    .Include(t => t.VesselImportJobDtlNavigation)
                    .Where(t => t.JobNo == strjobNo),
                    //ContainerNo = containerNo // I Just Set the Container Number 
            };


            var txnImportJobDtls = _context.TxnImportJobDtls
            .Where(t => t.RefNo == RefNoBlDtl).ToList(); //t.JobNo == jobNo &&

            ViewData["HBL"] = txnImportJobDtls.FirstOrDefault().HouseBl;
            ViewData["ContainerNo"] = containerNoList.FirstOrDefault().ContainerNo;

            DateTime etaPolDate = (DateTime)txnImportJobDtls.FirstOrDefault().ETAPod;
            string formattedDate = etaPolDate.ToString("dd/MM/yyyy");
            ViewData["ETA"] = formattedDate;
          
            



            //List<SelectListItem> Currency = new List<SelectListItem>
            //    {
            //        new SelectListItem { Value = "USD", Text = "USD" },
            //        new SelectListItem { Value = "LKR", Text = "LKR" }
            //    };
            //ViewData["CurrencyList"] = new SelectList(Currency, "Value", "Text", "CurrencyList");
            //List<SelectListItem> Unit = new List<SelectListItem>
            //    {
            //        new SelectListItem { Value = "CBM", Text = "CBM" },
            //        new SelectListItem { Value = "BL", Text = "BL" },
            //        new SelectListItem { Value = "CNT", Text = "CNT" }
            //    };
            //ViewData["UnitList"] = new SelectList(Unit, "Value", "Text", "UnitList");
            //ViewData["ChargeItemsList"] = new SelectList(_context.Set<RefChargeItemAcc>().Where(a => a.IsActive.Equals(true)).OrderBy(p => p.Description), "ChargeId", "Description", "ChargeId");
            //ViewData["Customer"] = new SelectList(_context.RefCustomers.OrderBy(c => c.Name), "CustomerId", "Name", "CustomerId");


            return View(tables);

        }


        // GET: txnInvoiceImportHds/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TxnInvoiceImportHds == null)
            {
                return NotFound();
            }

            var txnInvoiceImportHd = await _context.TxnInvoiceImportHds
                .FirstOrDefaultAsync(m => m.InvoiceNo == id);

            if (txnInvoiceImportHd == null)
            {
                return NotFound();
            }

            var jobNo = txnInvoiceImportHd.JobNo;
         

            var tables = new InvoiceImportViewModel
            {
                InvoiceHdMulti = _context.TxnInvoiceImportHds.Where(t => t.InvoiceNo == id),
                InvoiceDtMulti = _context.TxnInvoiceImportDtls.Where(t => t.InvoiceNo == id),
                ImportJobDtlMulti = _context.TxnImportJobDtls.Where(t => t.JobNo == jobNo),
                ImportJobHdMulti = _context.TxnImportJobHds
                    .Include(t => t.HandlebyImportJobNavigation)
                    .Include(t => t.LastPortNavigation)
                    .Include(t => t.AgentNavigation)
                    .Include(t => t.PolNavigation)
                    .Include(t => t.ShippingLineNavigation)
                    .Include(t => t.TerminalNavigation)
                    .Include(t => t.VesselImportJobDtlNavigation)
                    .Where(t => t.JobNo == jobNo),
            };

           // var houseBLList = _context.TxnImportJobDtls
           //.Where(t => t.JobNo == jobNo)
           //.Select(t => new SelectListItem
           //{
           //    Text = t.HouseBl,
           //    Value = t.RefNo // Map RefNo to Value
           //})
           //.Distinct()
           //.OrderBy(bl => bl.Text)
           //.ToList();

           // ViewData["HouseBL"] = new SelectList(houseBLList, "Value", "Text", "HouseBL");

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

        public async Task<IActionResult> Approve(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var txnInvoiceImportHd = await _context.TxnInvoiceImportHds
                .FirstOrDefaultAsync(m => m.InvoiceNo == id);

            if (txnInvoiceImportHd == null)
            {
                return NotFound();
            }

            jobNo = txnInvoiceImportHd.JobNo; // Set the jobNo property

            var tables = new InvoiceImportViewModel
            {
                InvoiceHdMulti = _context.TxnInvoiceImportHds.Where(t => t.InvoiceNo == id),
                InvoiceDtMulti = _context.TxnInvoiceImportDtls.Where(t => t.InvoiceNo == id),
                ImportJobDtlMulti = _context.TxnImportJobDtls.Where(t => t.JobNo == jobNo),
                ImportJobHdMulti = _context.TxnImportJobHds
                    .Include(t => t.HandlebyImportJobNavigation)
                    .Include(t => t.LastPortNavigation)
                    .Include(t => t.AgentNavigation)
                    .Include(t => t.PolNavigation)
                    .Include(t => t.ShippingLineNavigation)
                    .Include(t => t.TerminalNavigation)
                    .Include(t => t.VesselImportJobDtlNavigation)
                    .Where(t => t.JobNo == jobNo),
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

            var txnInvoiceImportHd = await _context.TxnInvoiceImportHds
                .FirstOrDefaultAsync(m => m.InvoiceNo == id);

            if (txnInvoiceImportHd == null)
            {
                return NotFound();
            }

            // Inserting Transaction Data to Acccounts 

            var txnInvoiceImportDtls = await _context.TxnInvoiceImportDtls
                .Include(t => t.ChargeItemNavigation)
                .Where(m => m.InvoiceNo == id)
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
            if (txnInvoiceImportDtls != null)
            {
                var SerialNo_AccTxn = 1;
                var AccTxnDescription = " Local Debtors INV: " + txnInvoiceImportHd.InvoiceNo;
                // First Acc transaction 
                TxnTransactions NewRowAccTxnFirst = new TxnTransactions();
                NewRowAccTxnFirst.TxnNo = nextAccTxnNo;
                NewRowAccTxnFirst.TxnSNo = SerialNo_AccTxn;
                NewRowAccTxnFirst.Date = txnInvoiceImportHd.Date; //  Jurnal Date 
                NewRowAccTxnFirst.Description = AccTxnDescription;
                NewRowAccTxnFirst.TxnAccCode = txnInvoiceImportHd.DebitAcc;
                NewRowAccTxnFirst.Dr = (decimal)txnInvoiceImportHd.TotalInvoiceAmountLkr;
                NewRowAccTxnFirst.Cr = (decimal)0;
                NewRowAccTxnFirst.RefNo = txnInvoiceImportHd.InvoiceNo; // Invoice No
                NewRowAccTxnFirst.Note = "";
                NewRowAccTxnFirst.Reconciled = false;
                NewRowAccTxnFirst.DocType = "Invoice";
                NewRowAccTxnFirst.IsMonthEndDone = false;
                NewRowAccTxnFirst.CreatedBy = "Admin";
                NewRowAccTxnFirst.CreatedDateTime = DateTime.Now;
                NewRowAccTxnFirst.Canceled = false;

                _accountsContext.TxnTransactions.Add(NewRowAccTxnFirst);

                foreach (var item in txnInvoiceImportDtls)
                {
                    // Transaction table Insert 
                    TxnTransactions NewRowAccTxn = new TxnTransactions();

                    SerialNo_AccTxn = SerialNo_AccTxn + 1;
                    NewRowAccTxn.TxnNo = nextAccTxnNo;
                    NewRowAccTxn.TxnSNo = SerialNo_AccTxn;
                    NewRowAccTxn.Date = txnInvoiceImportHd.Date; //  Jurnal Date 
                    NewRowAccTxn.Description = item.ChargeItemNavigation.Description + "INV: " + item.InvoiceNo;
                    NewRowAccTxn.TxnAccCode = item.AccNo;
                    NewRowAccTxn.Dr = (decimal)0;
                    if (item.Currency == "USD")
                    {
                        var AmtLkr = item.Amount * txnInvoiceImportHd.ExchangeRate; // convert to LKR
                        NewRowAccTxn.Cr = (decimal)AmtLkr;
                    }
                    else
                    {
                        NewRowAccTxn.Cr = (decimal)item.Amount;
                    }
                    NewRowAccTxn.RefNo = item.InvoiceNo; // Invoice No
                    NewRowAccTxn.Note = "";
                    NewRowAccTxn.Reconciled = false;
                    NewRowAccTxn.DocType = "InvoiceImport";
                    NewRowAccTxn.IsMonthEndDone = false;
                    NewRowAccTxn.CreatedBy = "Admin";
                    NewRowAccTxn.CreatedDateTime = DateTime.Now;
                    NewRowAccTxn.Canceled = false;

                    _accountsContext.TxnTransactions.Add(NewRowAccTxn);
                }
            }
            // END Inserting Transaction Data to Acccounts 

            /// Update the Approved property based on the form submission
            txnInvoiceImportHd.Approved = approved;
            txnInvoiceImportHd.ApprovedBy = "CurrentUserName"; // Replace with the actual user name
            txnInvoiceImportHd.ApprovedDateTime = DateTime.Now;

            _context.Update(txnInvoiceImportHd);
            await _context.SaveChangesAsync();
            await _accountsContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirect to the appropriate action
        }




        // GET: txnInvoiceImportHds/Create
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
            var tables = new InvoiceImportViewModel
            {
                InvoiceHdMulti = _context.TxnInvoiceImportHds.Where(t => t.InvoiceNo == "xxx"),
                InvoiceDtMulti = _context.TxnInvoiceImportDtls.Where(t => t.InvoiceNo == "xxx"),
                ImportJobDtlMulti = _context.TxnImportJobDtls.Where(t => t.JobNo == jobNo), // Temp
                ImportJobHdMulti = _context.TxnImportJobHds
                .Include(t => t.HandlebyImportJobNavigation)
                    .Include(t => t.LastPortNavigation)
                    .Include(t => t.AgentNavigation)
                    .Include(t => t.PolNavigation)
                    .Include(t => t.ShippingLineNavigation)
                    .Include(t => t.TerminalNavigation)
                    .Include(t => t.VesselImportJobDtlNavigation)
                    .Where(t => t.JobNo == jobNo),

            };
            // Populate the HouseBL dropdown based on the search results from Txn_ImportJob_Dtl
            var houseBLList = _context.TxnImportJobDtls
                .Where(t => t.JobNo == jobNo)
                .Select(t => new SelectListItem
                {
                    Text = t.HouseBl,
                    Value = t.RefNo // Map RefNo to Value
                })
                .Distinct()
                .OrderBy(bl => bl.Text)
                .ToList();

            ViewData["HouseBL"] = new SelectList(houseBLList, "Value", "Text", "HouseBL");

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




        // POST: txnInvoiceImportHds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string dtlItemsList, [Bind("InvoiceNo,Date,JobNo,Blno,Customer,ExchangeRate,TotalInvoiceAmountLkr,TotalInvoiceAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,Approved,CreditAcc,DebitAcc,AmountPaid,AmountToBePaid")] TxnInvoiceImportHd txnInvoiceImportHd)
        {

            // Next RefNumber for txnImportJobDtl
            var nextRefNo = "";
            var TableID = "Txn_InvoiceHD_Import";
            var refLastNumber = await _context.RefLastNumbers.FindAsync(TableID);
            if (refLastNumber != null)
            {
                var nextNumber = refLastNumber.LastNumber + 1;
                refLastNumber.LastNumber = nextNumber;
                nextRefNo = "INV" + DateTime.Now.Year.ToString() + nextNumber.ToString().PadLeft(5, '0');
                      txnInvoiceImportHd.InvoiceNo = nextRefNo;

                // _context.RefLastNumbers.Remove(refLastNumber);
            }
            else
            {
                return NotFound();
                //return View(tables);
            }
            txnInvoiceImportHd.Canceled = false;
            txnInvoiceImportHd.Approved = false;
            txnInvoiceImportHd.CreatedBy = "Admin";
            txnInvoiceImportHd.CreatedDateTime = DateTime.Now;

            txnInvoiceImportHd.DebitAcc = "ACC0023"; // LOCAL DEBTORS  2200-01
            txnInvoiceImportHd.CreditAcc = null;

            txnInvoiceImportHd.AmountPaid = 0;
            txnInvoiceImportHd.AmountToBePaid = txnInvoiceImportHd.TotalInvoiceAmountLkr;

            var TransDocMode = "Import";
            var TransDocType = "Revenue";
            var TransDocVender = "";

            ModelState.Remove("InvoiceNo");  // Auto genrated
            if (ModelState.IsValid)
            {
                // Adding CargoBreakDown records
                if (!string.IsNullOrWhiteSpace(dtlItemsList))
                {
                    var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnInvoiceImportDtl>>(dtlItemsList);
                    if (DtailItemdataTable != null)
                    {
                        var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the helper class

                        foreach (var item in DtailItemdataTable)
                        {
                            TxnInvoiceImportDtl DetailItem = new TxnInvoiceImportDtl();
                            DetailItem.InvoiceNo = nextRefNo; // New Invoice Number
                            DetailItem.SerialNo = item.SerialNo;
                            DetailItem.ChargeItem = item.ChargeItem;
                            DetailItem.Unit = item.Unit;
                            DetailItem.Rate = item.Rate;
                            DetailItem.Currency = item.Currency;
                            DetailItem.Qty = item.Qty;
                            DetailItem.Amount = item.Amount;
                            DetailItem.CreatedDate = DateTime.Now;
                            DetailItem.AccNo = CommonMethodAccClass.GetChargeItemAccNo(item.ChargeItem, TransDocMode, TransDocType, TransDocVender);
                            _context.TxnInvoiceImportDtls.Add(DetailItem);
                        }
                    }
                }
                _context.Add(txnInvoiceImportHd);
                _context.RefLastNumbers.Update(refLastNumber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var tables = new InvoiceImportViewModel
            {
                InvoiceHdMulti = _context.TxnInvoiceImportHds.Where(t => t.InvoiceNo == "xxx"),
                InvoiceDtMulti = _context.TxnInvoiceImportDtls.Where(t => t.InvoiceNo == "xxx"),

                ImportJobHdMulti = _context.TxnImportJobHds.Where(t => t.JobNo == jobNo),
                ImportJobDtlMulti = _context.TxnImportJobDtls.Where(t => t.JobNo == jobNo),
            };
            // Populate the HouseBL dropdown based on the search results from Txn_ImportJob_Dtl
            var houseBLList = _context.TxnImportJobDtls
                .Where(t => t.JobNo == jobNo)
                .Select(t => new SelectListItem
                {
                    Text = t.HouseBl,
                    Value = t.RefNo // Map RefNo to Value
                })
                .Distinct()
                .OrderBy(bl => bl.Text)
                .ToList();

            ViewData["HouseBL"] = new SelectList(houseBLList, "Value", "Text", "HouseBL");
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

            return View(tables);
        }
        // GET: txnInvoiceImportHds/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var txnInvoiceImportHd = await _context.TxnInvoiceImportHds.FindAsync(id);
            if (txnInvoiceImportHd == null)
            {
                return NotFound();
            }

            var InvoiceNo = id;
            var JobNo = txnInvoiceImportHd.JobNo.ToString(); // Assuming JobNo is an integer; convert to string if needed

            var tables = new InvoiceImportViewModel
            {
                InvoiceHdMulti = _context.TxnInvoiceImportHds.Where(t => t.InvoiceNo == InvoiceNo),
                InvoiceDtMulti = _context.TxnInvoiceImportDtls.Where(t => t.InvoiceNo == InvoiceNo),
                ImportJobHdMulti = _context.TxnImportJobHds
                   .Include(t => t.HandlebyImportJobNavigation)
                    .Include(t => t.LastPortNavigation)
                    .Include(t => t.AgentNavigation)
                    .Include(t => t.PolNavigation)
                    .Include(t => t.ShippingLineNavigation)
                    .Include(t => t.TerminalNavigation)
                    .Include(t => t.VesselImportJobDtlNavigation)
                    .Where(t => t.JobNo == JobNo),
                ImportJobDtlMulti = _context.TxnImportJobDtls.Where(t => t.JobNo == JobNo),
            };

            var houseBLList = _context.TxnImportJobDtls
               .Where(t => t.JobNo == JobNo)
               .Select(t => new SelectListItem
               {
                   Text = t.HouseBl,
                   Value = t.RefNo // Map RefNo to Value
               })
               .Distinct()
               .OrderBy(bl => bl.Text)
               .ToList();

            ViewData["HouseBL"] = new SelectList(houseBLList, "Value", "Text", "HouseBL");

            List<SelectListItem> Currency = new List<SelectListItem>
                {
                    new SelectListItem { Value = "LKR", Text = "LKR" },
                    new SelectListItem { Value = "USD", Text = "USD" }

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
// POST: txnInvoiceImportHds/Edit/5
// To protect from overposting attacks, enable the specific properties you want to bind to.
// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string dtlItemsList, string id, [Bind("InvoiceNo,Date,JobNo,Blno,Customer,ExchangeRate,TotalInvoiceAmountLkr,TotalInvoiceAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,Approved,CreditAcc,DebitAcc,AmountPaid,AmountToBePaid")] TxnInvoiceImportHd txnInvoiceImportHd)
        {
            if (id != txnInvoiceImportHd.InvoiceNo)
            {
                return NotFound();
            }
            txnInvoiceImportHd.LastUpdatedBy = "Admin";
            txnInvoiceImportHd.LastUpdatedDateTime = DateTime.Now;

            txnInvoiceImportHd.DebitAcc = "ACC0023"; // LOCAL DEBTORS  2200-01
            txnInvoiceImportHd.CreditAcc = null;

            txnInvoiceImportHd.AmountPaid = 0;
            txnInvoiceImportHd.AmountToBePaid = txnInvoiceImportHd.TotalInvoiceAmountLkr;

            var TransDocMode = "Export";
            var TransDocType = "Revenue";
            var TransDocVender = "";

            if (ModelState.IsValid)
            {
                try
                {
                    // Adding Invoice Items records
                    if (!string.IsNullOrWhiteSpace(dtlItemsList))
                    {
                        var rowsToDelete = _context.TxnInvoiceImportDtls.Where(t => t.InvoiceNo == id);
                        if (rowsToDelete != null || rowsToDelete.Any())
                        {
                            // Remove the rows from the database context
                            _context.TxnInvoiceImportDtls.RemoveRange(rowsToDelete);
                        }
                        var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnInvoiceImportDtl>>(dtlItemsList);
                        if (DtailItemdataTable != null)
                        {
                            var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the helper class

                            foreach (var item in DtailItemdataTable)
                            {
                                TxnInvoiceImportDtl DetailItem = new TxnInvoiceImportDtl();
                                DetailItem.InvoiceNo = id; // New Invoice Number
                                DetailItem.SerialNo = item.SerialNo;
                                DetailItem.ChargeItem = item.ChargeItem;
                                DetailItem.Unit = item.Unit;
                                DetailItem.Rate = item.Rate;
                                DetailItem.Currency = item.Currency;
                                DetailItem.Qty = item.Qty;
                                DetailItem.Amount = item.Amount;
                                DetailItem.CreatedDate = DateTime.Now;
                                DetailItem.AccNo = CommonMethodAccClass.GetChargeItemAccNo(item.ChargeItem, TransDocMode, TransDocType, TransDocVender);
                                _context.TxnInvoiceImportDtls.Add(DetailItem);
                            }
                        }
                    }

                    _context.Update(txnInvoiceImportHd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!txnInvoiceImportHdExists(txnInvoiceImportHd.InvoiceNo))
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
            return View(txnInvoiceImportHd);
        }

        // GET: txnInvoiceImportHds/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.TxnInvoiceImportHds == null)
            {
                return NotFound();
            }

            var txnInvoiceImportHd = await _context.TxnInvoiceImportHds
                .FirstOrDefaultAsync(m => m.InvoiceNo == id);
            if (txnInvoiceImportHd == null)
            {
                return NotFound();
            }

            return View(txnInvoiceImportHd);
        }

        // POST: txnInvoiceImportHds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.TxnInvoiceImportHds == null)
            {
                return Problem("Entity set 'FtlcolombOperationContext.TxnInvoiceImportHds'  is null.");
            }
            var txnInvoiceImportHd = await _context.TxnInvoiceImportHds.FindAsync(id);
            if (txnInvoiceImportHd != null)
            {
                _context.TxnInvoiceImportHds.Remove(txnInvoiceImportHd);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool txnInvoiceImportHdExists(string id)
        {
          return (_context.TxnInvoiceImportHds?.Any(e => e.InvoiceNo == id)).GetValueOrDefault();
        }
    }
}
