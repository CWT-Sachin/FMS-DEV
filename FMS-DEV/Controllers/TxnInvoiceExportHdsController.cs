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
    public class TxnInvoiceExportHdsController : Controller
    {
        private readonly FtlcolombOperationContext _context;
        private readonly FtlcolomboAccountsContext _accountsContext;

        public string jobNo { get; private set; }
        public TxnInvoiceExportHdsController(FtlcolombOperationContext context, FtlcolomboAccountsContext accountsContext)
        {
            _context = context;
            _accountsContext = accountsContext;
        }


        public async Task<IActionResult> Index(string searchString, string searchType, int pg = 1)
        {


            var txnInvoiceExportHds = _context.TxnInvoiceExportHds
                .OrderByDescending(p => p.InvoiceNo)
                .Include(t => t.CustomerInvoiceExportHd)
                .GroupJoin(
                    _context.TxnBookingExps,
                    invoice => invoice.Blno,
                    booking => booking.BookingNo,
                    (invoice, bookings) => new { Invoice = invoice, Bookings = bookings })
                .SelectMany(
                    x => x.Bookings.DefaultIfEmpty(),
                    (x, booking) => new { x.Invoice, Booking = booking })
                .GroupJoin(
                    _context.TxnImportJobDtls,
                    x => x.Invoice.BLNoType == "TS" ? x.Invoice.BLNoTS : null,
                    importJob => importJob.RefNo,
                    (x, importJobs) => new { x.Invoice, x.Booking, ImportJobs = importJobs })
                .SelectMany(
                    x => x.ImportJobs.DefaultIfEmpty(),
                    (x, importJob) => new { x.Invoice, x.Booking, ImportJob = importJob })
                .Where(x => x.Invoice.BLNoType == "Local" || x.Booking == null || (x.Invoice.BLNoType == "TS" && x.ImportJob != null))
                .Select(x => new CombinedTxnInvoiceExportAndTxnBookingExpModel
                {
                    Invoice = x.Invoice,
                    BKBlNo = x.Booking.Blno,
                    HouseBl = x.ImportJob.HouseBl
                })
                .ToList();






            if (searchType == "Approve")
            {
                // Filter only not approved invoices
                txnInvoiceExportHds = txnInvoiceExportHds.Where(x => x.Invoice.Approved != true).ToList();
            
            }
            else if (!String.IsNullOrEmpty(searchString))
            {
                if (searchType == "Invoice")
                {
                    txnInvoiceExportHds = txnInvoiceExportHds.Where(x => x.Invoice.InvoiceNo.Contains(searchString)).ToList();
                    //txnInvoiceExportHds = txnInvoiceExportHds.Where(txnInvoiceExportHds => txnInvoiceExportHds.InvoiceNo.Contains(searchString)).OrderByDescending(p => p.InvoiceNo).Include(t => t.CustomerInvoiceExportHd);
                }
                else if (searchType == "Job")
                {
                    txnInvoiceExportHds = txnInvoiceExportHds.Where(x => x.Invoice.JobNo.Contains(searchString)).ToList();
                    //txnInvoiceExportHds = txnInvoiceExportHds.Where(txnInvoiceExportHds => txnInvoiceExportHds.JobNo.Contains(searchString)).OrderByDescending(p => p.InvoiceNo).Include(t => t.CustomerInvoiceExportHd);
                }

                // Check if results are found
                if (txnInvoiceExportHds.Any())
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
            int recsCount = txnInvoiceExportHds.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = txnInvoiceExportHds.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;
            return View(data);
        }

        // GET: Print Invoice 
        public async Task<IActionResult> RepPrintInvoice(string InvoiceNo)
        {
            if (InvoiceNo == null || _context.TxnInvoiceExportHds == null)
            {
                return NotFound();
            }

            var txnInvoiceExportHd = await _context.TxnInvoiceExportHds
                .FirstOrDefaultAsync(m => m.InvoiceNo == InvoiceNo);

            if (txnInvoiceExportHd == null)
            {
                return NotFound();
            }

            var strjobNo = txnInvoiceExportHd.JobNo;

            var containerNo = _context.TxnStuffingPlanHds
                .Where(s => s.JobNumber == strjobNo)
                .Select(s => s.ContainerNo)
                .FirstOrDefault();

            var tables = new InvoiceExportViewModel
            {
                InvoiceHdMulti = _context.TxnInvoiceExportHds.Where(t => t.InvoiceNo == InvoiceNo)
                    .Include(t => t.CustomerInvoiceExportHd),
                InvoiceDtMulti = _context.TxnInvoiceExportDtls.Where(t => t.InvoiceNo == InvoiceNo)
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
                    ContainerNo = containerNo // I Just Set the Container Number 
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

            var InvoiceBokkingNo = txnInvoiceExportHd.Blno;  // Contains Booking No. 
            var txnBookingExp = await _context.TxnBookingExps
                .Include(t=>t.FDNNavigation)
                .FirstOrDefaultAsync(m => m.BookingNo == InvoiceBokkingNo);
            ViewData["HBL"] = txnBookingExp.Blno;
            ViewData["CBM"] = txnBookingExp.Cbm;
            ViewData["Weight"] = txnBookingExp.GrossWeight;
            ViewData["NoofPkgs"] = txnBookingExp.NoofPackages;
            ViewData["FinalDest"] = txnBookingExp.FDNNavigation.PortName;

            DateTime etdPolDate = (DateTime)txnBookingExp.EtdPol;
            string formattedDate = etdPolDate.ToString("dd/MM/yyyy");
            ViewData["ETD"] = formattedDate;

            return View(tables);

        }


        // GET: txnInvoiceExportHds/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TxnInvoiceExportHds == null)
            {
                return NotFound();
            }

            var txnInvoiceExportHd = await _context.TxnInvoiceExportHds
                .FirstOrDefaultAsync(m => m.InvoiceNo == id);

            if (txnInvoiceExportHd == null)
            {
                return NotFound();
            }

            var jobNo = txnInvoiceExportHd.JobNo;

            var tables = new InvoiceExportViewModel
            {
                InvoiceHdMulti = _context.TxnInvoiceExportHds.Where(t => t.InvoiceNo == id).Include(t=>t.CustomerInvoiceExportHd),
                InvoiceDtMulti = _context.TxnInvoiceExportDtls.Where(t => t.InvoiceNo == id).Include(t=>t.ChargeItemNavigation),
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
            var BookingNumber = txnInvoiceExportHd.Blno;
            var LocalBookings = from exp in _context.TxnBookingExps
                                join dtl in _context.TxnStuffingPlanDtls on exp.BookingNo equals dtl.BookingRefNo
                                join hd in _context.TxnStuffingPlanHds on dtl.PlanId equals hd.PlanId
                                join job in _context.TxnExportJobDtls on hd.PlanId equals job.StuffingPlnNo
                                where job.JobNo == jobNo && exp.BookingNo== BookingNumber && dtl.CargoType == "Local"
                                select new
                                {
                                    // Select the properties you need or the entire objects as per your requirement
                                    BlNo = exp.Blno

                                };

            var result = LocalBookings.ToList();


            ViewData["BLNumber"] = result.FirstOrDefault().BlNo; // = new SelectList(result, "BlNo", "BlNo", "BlNo");
            return View(tables);
        }
        public async Task<IActionResult> Approve(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var txnInvoiceExportHd = await _context.TxnInvoiceExportHds
                .FirstOrDefaultAsync(m => m.InvoiceNo == id);

            if (txnInvoiceExportHd == null)
            {
                return NotFound();
            }

            jobNo = txnInvoiceExportHd.JobNo; // Set the jobNo property

            var tables = new InvoiceExportViewModel
            {
                InvoiceHdMulti = _context.TxnInvoiceExportHds.Where(t => t.InvoiceNo == id),
                InvoiceDtMulti = _context.TxnInvoiceExportDtls.Where(t => t.InvoiceNo == id),
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

            return View(tables);
        }


        [HttpPost]
        public async Task<IActionResult> Approve(string id, bool approved)
        {
            if (id == null)
            {
                return NotFound();
            }

            var txnInvoiceExportHd = await _context.TxnInvoiceExportHds
                .FirstOrDefaultAsync(m => m.InvoiceNo == id);

            if (txnInvoiceExportHd == null)
            {
                return NotFound();
            }

            // Inserting Transaction Data to Acccounts 
            var txnInvoiceExportDtls = await _context.TxnInvoiceExportDtls
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
            if (txnInvoiceExportDtls != null)
            {
                var SerialNo_AccTxn = 1;
                var AccTxnDescription = " Local Debtors INV: " + txnInvoiceExportHd.InvoiceNo;
                // First Acc transaction 
                TxnTransactions NewRowAccTxnFirst = new TxnTransactions();
                NewRowAccTxnFirst.TxnNo = nextAccTxnNo;
                NewRowAccTxnFirst.TxnSNo = SerialNo_AccTxn;
                NewRowAccTxnFirst.Date = txnInvoiceExportHd.Date; //  Jurnal Date 
                NewRowAccTxnFirst.Description = AccTxnDescription;
                NewRowAccTxnFirst.TxnAccCode = txnInvoiceExportHd.DebitAcc;
                NewRowAccTxnFirst.Dr = (decimal)txnInvoiceExportHd.TotalInvoiceAmountLkr;
                NewRowAccTxnFirst.Cr = (decimal)0;
                NewRowAccTxnFirst.RefNo = txnInvoiceExportHd.InvoiceNo; // Invoice No
                NewRowAccTxnFirst.Note = "";
                NewRowAccTxnFirst.Reconciled = false;
                NewRowAccTxnFirst.DocType = "Invoice";
                NewRowAccTxnFirst.IsMonthEndDone = false;
                NewRowAccTxnFirst.CreatedBy = "Admin";
                NewRowAccTxnFirst.CreatedDateTime = DateTime.Now;
                NewRowAccTxnFirst.Canceled = false;

                _accountsContext.TxnTransactions.Add(NewRowAccTxnFirst);

                foreach (var item in txnInvoiceExportDtls)
                {
                    // Transaction table Insert 
                    TxnTransactions NewRowAccTxn = new TxnTransactions();

                    SerialNo_AccTxn = SerialNo_AccTxn + 1;
                    NewRowAccTxn.TxnNo = nextAccTxnNo;
                    NewRowAccTxn.TxnSNo = SerialNo_AccTxn;
                    NewRowAccTxn.Date = txnInvoiceExportHd.Date; //  Jurnal Date 
                    NewRowAccTxn.Description = item.ChargeItemNavigation.Description + "INV: " + item.InvoiceNo;
                    NewRowAccTxn.TxnAccCode = item.AccNo;
                    NewRowAccTxn.Dr = (decimal)0;
                    if (item.Currency == "USD")
                    {
                        var AmtLkr = item.Amount * txnInvoiceExportHd.ExchangeRate; // convert to LKR
                        NewRowAccTxn.Cr = (decimal)AmtLkr;
                    }
                    else
                    {
                        NewRowAccTxn.Cr = (decimal)item.Amount;
                    }
                    NewRowAccTxn.RefNo = item.InvoiceNo; // Invoice No
                    NewRowAccTxn.Note = "";
                    NewRowAccTxn.Reconciled = false;
                    NewRowAccTxn.DocType = "InvoiceExport";
                    NewRowAccTxn.IsMonthEndDone = false;
                    NewRowAccTxn.CreatedBy = "Admin";
                    NewRowAccTxn.CreatedDateTime = DateTime.Now;
                    NewRowAccTxn.Canceled = false;

                    _accountsContext.TxnTransactions.Add(NewRowAccTxn);
                }
            }
            // END Inserting Transaction Data to Acccounts 

            /// Update the Approved property based on the form submission
            txnInvoiceExportHd.Approved = approved;
            txnInvoiceExportHd.ApprovedBy = "CurrentUserName"; // Replace with the actual user name
            txnInvoiceExportHd.ApprovedDateTime = DateTime.Now;

            _context.Update(txnInvoiceExportHd);
            await _context.SaveChangesAsync();
            await _accountsContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirect to the appropriate action
        }



        // GET: txnInvoiceExportHds/Create
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
            var tables = new InvoiceExportViewModel
            {
                InvoiceHdMulti = _context.TxnInvoiceExportHds.Where(t => t.InvoiceNo == "xxx"),
                InvoiceDtMulti = _context.TxnInvoiceExportDtls.Where(t => t.InvoiceNo == "xxx"),
                ExportJobDtlMulti = _context.TxnExportJobDtls.Where(t => t.JobNo == jobNo), // Temp
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


            var LocalBookings = from exp in _context.TxnBookingExps
                                join dtl in _context.TxnStuffingPlanDtls on exp.BookingNo equals dtl.BookingRefNo
                                join hd in _context.TxnStuffingPlanHds on dtl.PlanId equals hd.PlanId
                                join job in _context.TxnExportJobDtls on hd.PlanId equals job.StuffingPlnNo
                                where job.JobNo == jobNo && dtl.CargoType == "Local"
                                select new
                                {
                                    // Select the properties you need or the entire objects as per your requirement
                                    BlNo = "BL: " + exp.Blno + " RefNo: " + exp.BookingNo,
                                    BookingNo = exp.BookingNo
                                };

            var result = LocalBookings.ToList();

            ViewData["BLNumbers"] = new SelectList(result, "BookingNo", "BlNo", "BookingNo");

            // TS BLs 
            var TSBookings = from imp in _context.TxnImportJobDtls
                             join dtl in _context.TxnStuffingPlanDtls on imp.RefNo equals dtl.BookingRefNo
                             join hd in _context.TxnStuffingPlanHds on dtl.PlanId equals hd.PlanId
                             join job in _context.TxnExportJobDtls on hd.PlanId equals job.StuffingPlnNo
                             where job.JobNo == jobNo && dtl.CargoType == "TS"
                             select new
                             {
                                 // Select the properties you need or the entire objects as per your requirement
                                 BlNo = "BL: " + imp.HouseBl + " RefNo: " + imp.RefNo,
                                 BookingNo = imp.RefNo
                             };
            var resultTS = TSBookings.ToList();

            ViewData["BLNumbersTS"] = new SelectList(resultTS, "BookingNo", "BlNo", "BookingNo");

            return View(tables);
        }




        // POST: txnInvoiceExportHds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string dtlItemsList, [Bind("InvoiceNo,Date,JobNo,Blno,Customer,ExchangeRate,TotalInvoiceAmountLkr,TotalInvoiceAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,Approved,CreditAcc,DebitAcc,AmountPaid,AmountToBePaid, BLNoTS,BLNoType ")] TxnInvoiceExportHd txnInvoiceExportHd)
        {

            // Next RefNumber for txnImportJobDtl
            var nextRefNo = "";
            var TableID = "Txn_InvoiceHD_Export";
            var refLastNumber = await _context.RefLastNumbers.FindAsync(TableID);
            if (refLastNumber != null)
            {
                var nextNumber = refLastNumber.LastNumber + 1;
                refLastNumber.LastNumber = nextNumber;
                nextRefNo = "EINV" + DateTime.Now.Year.ToString() + nextNumber.ToString().PadLeft(5, '0');
                txnInvoiceExportHd.InvoiceNo = nextRefNo;

                // _context.RefLastNumbers.Remove(refLastNumber);
            }
            else
            {
                return NotFound();
                //return View(tables);
            }
            txnInvoiceExportHd.Canceled = false;
            txnInvoiceExportHd.Approved = false;
            txnInvoiceExportHd.CreatedBy = "Admin";
            txnInvoiceExportHd.CreatedDateTime = DateTime.Now;

            txnInvoiceExportHd.DebitAcc = "ACC0023"; // LOCAL DEBTORS  2200-01
            txnInvoiceExportHd.CreditAcc = null;

            txnInvoiceExportHd.AmountPaid = 0;
            txnInvoiceExportHd.AmountToBePaid = txnInvoiceExportHd.TotalInvoiceAmountLkr;

            var TransDocMode = "Export";
            var TransDocType = "Revenue";
            var TransDocVender = "";

            ModelState.Remove("InvoiceNo");  // Auto genrated
            if (ModelState.IsValid)
            {
                // Adding CargoBreakDown records
                if (!string.IsNullOrWhiteSpace(dtlItemsList))
                {
                    var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnInvoiceExportDtl>>(dtlItemsList);
                    if (DtailItemdataTable != null)
                    {
                        var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the common method  class

                        foreach (var item in DtailItemdataTable)
                        {
                            TxnInvoiceExportDtl DetailItem = new TxnInvoiceExportDtl();
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
                            _context.TxnInvoiceExportDtls.Add(DetailItem);
                        }
                    }
                }
                _context.Add(txnInvoiceExportHd);
                _context.RefLastNumbers.Update(refLastNumber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var tables = new InvoiceExportViewModel
            {
                InvoiceHdMulti = _context.TxnInvoiceExportHds.Where(t => t.InvoiceNo == "xxx"),
                InvoiceDtMulti = _context.TxnInvoiceExportDtls.Where(t => t.InvoiceNo == "xxx"),

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
            ViewData["Customer"] = new SelectList(_context.RefCustomers.OrderBy(c => c.Name), "CustomerId", "Name", "CustomerId");

            return View(tables);
        }



        // GET: txnInvoiceExportHds/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var txnInvoiceExportHd = await _context.TxnInvoiceExportHds.FindAsync(id);
            if (txnInvoiceExportHd == null)
            {
                return NotFound();
            }

            var InvoiceNo = id;
            var JobNo = txnInvoiceExportHd.JobNo.ToString(); // Assuming JobNo is an integer; convert to string if needed

            var tables = new InvoiceExportViewModel
            {
                InvoiceHdMulti = _context.TxnInvoiceExportHds.Where(t => t.InvoiceNo == InvoiceNo),
                InvoiceDtMulti = _context.TxnInvoiceExportDtls.Where(t => t.InvoiceNo == InvoiceNo),
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
                ExportJobDtlMulti = _context.TxnExportJobDtls.Where(t => t.JobNo == JobNo),
            };
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
            var bookingNo = txnInvoiceExportHd.Blno;
            var LocalBookings = from exp in _context.TxnBookingExps
                                join dtl in _context.TxnStuffingPlanDtls on exp.BookingNo equals dtl.BookingRefNo
                                join hd in _context.TxnStuffingPlanHds on dtl.PlanId equals hd.PlanId
                                join job in _context.TxnExportJobDtls on hd.PlanId equals job.StuffingPlnNo
                                where job.JobNo == JobNo && exp.BookingNo== bookingNo && dtl.CargoType == "Local"
                                select new
                                {
                                    // Select the properties you need or the entire objects as per your requirement
                                    BlNo = exp.Blno,
                                    BookingNo = exp.BookingNo
                                };

            var result = LocalBookings.ToList();


            ViewData["BLNumbers"] = new SelectList(result, "BookingNo", "BlNo", "BookingNo");

            return View(tables);
        }
// POST: txnInvoiceExportHds/Edit/5
// To protect from overposting attacks, enable the specific properties you want to bind to.
// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string dtlItemsList, string id, [Bind("InvoiceNo,Date,JobNo,Blno,Customer,ExchangeRate,TotalInvoiceAmountLkr,TotalInvoiceAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,Approved,CreditAcc,DebitAcc,AmountPaid,AmountToBePaid")] TxnInvoiceExportHd txnInvoiceExportHd)
        {
            if (id != txnInvoiceExportHd.InvoiceNo)
            {
                return NotFound();
            }
            txnInvoiceExportHd.LastUpdatedBy = "Admin";
            txnInvoiceExportHd.LastUpdatedDateTime = DateTime.Now;
            txnInvoiceExportHd.DebitAcc = "ACC0023"; // LOCAL DEBTORS  2200-01
            txnInvoiceExportHd.CreditAcc = null;

            txnInvoiceExportHd.AmountPaid = 0;
            txnInvoiceExportHd.AmountToBePaid = txnInvoiceExportHd.TotalInvoiceAmountLkr;


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
                        var rowsToDelete = _context.TxnInvoiceExportDtls.Where(t => t.InvoiceNo == id);
                        if (rowsToDelete != null || rowsToDelete.Any())
                        {
                            // Remove the rows from the database context
                            _context.TxnInvoiceExportDtls.RemoveRange(rowsToDelete);
                        }
                        var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnInvoiceExportDtl>>(dtlItemsList);
                        if (DtailItemdataTable != null)
                        {
                            var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the helper class
                            foreach (var item in DtailItemdataTable)
                            {
                                TxnInvoiceExportDtl DetailItem = new TxnInvoiceExportDtl();
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
                                _context.TxnInvoiceExportDtls.Add(DetailItem);
                            }
                        }
                    }

                    _context.Update(txnInvoiceExportHd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!txnInvoiceExportHdExists(txnInvoiceExportHd.InvoiceNo))
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
            return View(txnInvoiceExportHd);
        }

        // GET: txnInvoiceExportHds/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.TxnInvoiceExportHds == null)
            {
                return NotFound();
            }

            var txnInvoiceExportHd = await _context.TxnInvoiceExportHds
                .FirstOrDefaultAsync(m => m.InvoiceNo == id);
            if (txnInvoiceExportHd == null)
            {
                return NotFound();
            }

            return View(txnInvoiceExportHd);
        }

        // POST: txnInvoiceExportHds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.TxnInvoiceExportHds == null)
            {
                return Problem("Entity set 'FtlcolombOperationContext.TxnInvoiceExportHds'  is null.");
            }
            var txnInvoiceExportHd = await _context.TxnInvoiceExportHds.FindAsync(id);
            if (txnInvoiceExportHd != null)
            {
                _context.TxnInvoiceExportHds.Remove(txnInvoiceExportHd);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool txnInvoiceExportHdExists(string id)
        {
          return (_context.TxnInvoiceExportHds?.Any(e => e.InvoiceNo == id)).GetValueOrDefault();
        }
    }
}
