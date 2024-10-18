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
    public class TxnInvoiceFCLHdsController : Controller
    {
        private readonly FtlcolombOperationContext _context;
        private readonly FtlcolomboAccountsContext _accountsContext;

        public string jobNo { get; private set; }
        public TxnInvoiceFCLHdsController(FtlcolombOperationContext context, FtlcolomboAccountsContext accountsContext)
        {
            _context = context;
            _accountsContext = accountsContext;
        }


        public async Task<IActionResult> Index(string searchString, string searchType, int pg = 1)
        {

            var fclInvoices = await _context.TxnInvoiceFCLHds.ToListAsync();

            // Pass the data to the view
            return View(fclInvoices);
        }

        // GET: Print Invoice
        //        // GET: txnInvoiceExportHds/Create
        public IActionResult Create(string searchString)
        {
            var jobNo = "xxx";

            if (!string.IsNullOrEmpty(searchString))
            {

                jobNo = searchString;
                // Check if results are found
                if (_context.TxnFCLJobs.Any(t => t.JobNo == jobNo))
                {
                    ViewData["FCLJobFound"] = "";
                }
                else
                {
                    ViewData["FCLJobFound"] = "FCLJob Number: " + searchString + " not found";
                }
            }
            var tables = new InvoiceFCLViewModel
            {
                InvoiceFCLHdMulti = _context.TxnInvoiceFCLHds.Where(t => t.InvoiceNo == "xxx"),
                InvoiceFCLDtlMulti = _context.TxnInvoiceFCLDtls.Where(t => t.InvoiceNo == "xxx"),
                FCLJobMulti = _context.TxnFCLJobs
                .Include(t => t.ShippingLineFCLJobExportNavigation)
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
        public async Task<IActionResult> Create(string dtlItemsList, [Bind("InvoiceNo,Date,JobNo,Blno,Customer,ExchangeRate,TotalInvoiceAmountLkr,TotalInvoiceAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,Approved,CreditAcc,DebitAcc,AmountPaid,AmountToBePaid, BLNoTS,BLNoType ")] TxnInvoiceFCLHd txnInvoiceFCLHd)
        {

            // Next RefNumber for txnImportJobDtl
            var nextRefNo = "";
            var TableID = "Txn_InvoiceHD_FCL";
            var refLastNumber = await _context.RefLastNumbers.FindAsync(TableID);
            if (refLastNumber != null)
            {
                var nextNumber = refLastNumber.LastNumber + 1;
                refLastNumber.LastNumber = nextNumber;
                nextRefNo = "FINV" + DateTime.Now.Year.ToString() + nextNumber.ToString().PadLeft(5, '0');
                txnInvoiceFCLHd.InvoiceNo = nextRefNo;

                // _context.RefLastNumbers.Remove(refLastNumber);
            }
            else
            {
                return NotFound();
                //return View(tables);
            }
            txnInvoiceFCLHd.Canceled = false;
            txnInvoiceFCLHd.Approved = false;
            txnInvoiceFCLHd.CreatedBy = "Admin";
            txnInvoiceFCLHd.CreatedDateTime = DateTime.Now;

            txnInvoiceFCLHd.DebitAcc = "ACC0023"; // LOCAL DEBTORS  2200-01
            txnInvoiceFCLHd.CreditAcc = null;

            txnInvoiceFCLHd.AmountPaid = 0;
            txnInvoiceFCLHd.AmountToBePaid = txnInvoiceFCLHd.TotalInvoiceAmountLkr;

            var TransDocMode = "Export";
            var TransDocType = "Revenue";
            var TransDocVender = "";

            ModelState.Remove("InvoiceNo");  // Auto genrated
            if (ModelState.IsValid)
            {
                // Adding CargoBreakDown records
                if (!string.IsNullOrWhiteSpace(dtlItemsList))
                {
                    var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnInvoiceFCLDtl>>(dtlItemsList);
                    if (DtailItemdataTable != null)
                    {
                        var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the common method  class

                        foreach (var item in DtailItemdataTable)
                        {
                            TxnInvoiceFCLDtl DetailItem = new TxnInvoiceFCLDtl();
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
                            _context.TxnInvoiceFCLDtls.Add(DetailItem);
                        }
                    }
                }
                _context.Add(txnInvoiceFCLHd);
                _context.RefLastNumbers.Update(refLastNumber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var tables = new InvoiceFCLViewModel
            {
                InvoiceFCLHdMulti = _context.TxnInvoiceFCLHds.Where(t => t.InvoiceNo == "xxx"),
                InvoiceFCLDtlMulti = _context.TxnInvoiceFCLDtls.Where(t => t.InvoiceNo == "xxx"),
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

            var txnInvoiceFCLHd = await _context.TxnInvoiceFCLHds.FindAsync(id);
            if (txnInvoiceFCLHd == null)
            {
                return NotFound();
            }

            var InvoiceNo = id;
            var JobNo = txnInvoiceFCLHd.JobNo.ToString(); // Assuming JobNo is an integer; convert to string if needed

            var tables = new InvoiceFCLViewModel
            {
                InvoiceFCLHdMulti = _context.TxnInvoiceFCLHds.Where(t => t.InvoiceNo == id),
                InvoiceFCLDtlMulti = _context.TxnInvoiceFCLDtls.Where(t => t.InvoiceNo == id),
                FCLJobMulti = _context.TxnFCLJobs
        .Include(t => t.ShippingLineFCLJobExportNavigation)
        .Where(t => t.JobNo == JobNo),
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
            var bookingNo = txnInvoiceFCLHd.Blno;
            var LocalBookings = from exp in _context.TxnBookingExps
                                join dtl in _context.TxnStuffingPlanDtls on exp.BookingNo equals dtl.BookingRefNo
                                join hd in _context.TxnStuffingPlanHds on dtl.PlanId equals hd.PlanId
                                join job in _context.TxnExportJobDtls on hd.PlanId equals job.StuffingPlnNo
                                where job.JobNo == JobNo && exp.BookingNo == bookingNo && dtl.CargoType == "Local"
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
        public async Task<IActionResult> Edit(string dtlItemsList, string id, [Bind("InvoiceNo,Date,JobNo,Blno,Customer,ExchangeRate,TotalInvoiceAmountLkr,TotalInvoiceAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,Approved,CreditAcc,DebitAcc,AmountPaid,AmountToBePaid")] TxnInvoiceFCLHd txnInvoiceFCLHd)
        {
            if (id != txnInvoiceFCLHd.InvoiceNo)
            {
                return NotFound();
            }
            txnInvoiceFCLHd.LastUpdatedBy = "Admin";
            txnInvoiceFCLHd.LastUpdatedDateTime = DateTime.Now;
            txnInvoiceFCLHd.DebitAcc = "ACC0023"; // LOCAL DEBTORS  2200-01
            txnInvoiceFCLHd.CreditAcc = null;

            txnInvoiceFCLHd.AmountPaid = 0;
            txnInvoiceFCLHd.AmountToBePaid = txnInvoiceFCLHd.TotalInvoiceAmountLkr;


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
                        var rowsToDelete = _context.TxnInvoiceFCLDtls.Where(t => t.InvoiceNo == id);
                        if (rowsToDelete != null || rowsToDelete.Any())
                        {
                            // Remove the rows from the database context
                            _context.TxnInvoiceFCLDtls.RemoveRange(rowsToDelete);
                        }
                        var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnInvoiceFCLDtl>>(dtlItemsList);
                        if (DtailItemdataTable != null)
                        {
                            var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the helper class
                            foreach (var item in DtailItemdataTable)
                            {
                                TxnInvoiceFCLDtl DetailItem = new TxnInvoiceFCLDtl();
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
                                _context.TxnInvoiceFCLDtls.Add(DetailItem);
                            }
                        }
                    }

                    _context.Update(txnInvoiceFCLHd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!txnInvoiceFCLHdExists(txnInvoiceFCLHd.InvoiceNo))
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
            return View(txnInvoiceFCLHd);
        }


        private bool txnInvoiceFCLHdExists(string id)
        {
            return (_context.TxnInvoiceFCLHds?.Any(e => e.InvoiceNo == id)).GetValueOrDefault();
        }









        // GET: txnInvoiceExportHds/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TxnInvoiceFCLHds == null)
            {
                return NotFound();
            }

            var txnInvoiceFCLHd = await _context.TxnInvoiceFCLHds
                .FirstOrDefaultAsync(m => m.InvoiceNo == id);

            if (txnInvoiceFCLHd == null)
            {
                return NotFound();
            }

            var jobNo = txnInvoiceFCLHd.JobNo;

            //var tables = new InvoiceFCLViewModel
            //{
            //    InvoiceFCLHdMulti = _context.TxnInvoiceFCLHds.Where(t => t.InvoiceNo == "xxx"),
            //    InvoiceFCLDtlMulti = _context.TxnInvoiceFCLDtls.Where(t => t.InvoiceNo == "xxx"),
            //    FCLJobMulti = _context.TxnFCLJobs
            //    .Include(t => t.ShippingLineFCLJobExportNavigation)
            //   .Where(t => t.JobNo == jobNo),
            //};

            var tables = new InvoiceFCLViewModel
            {
                InvoiceFCLHdMulti = await _context.TxnInvoiceFCLHds.Where(t => t.InvoiceNo == id).ToListAsync(),
                InvoiceFCLDtlMulti = await _context.TxnInvoiceFCLDtls.Where(t => t.InvoiceNo == id).ToListAsync(), // Fix here
                FCLJobMulti = await _context.TxnFCLJobs
        .Include(t => t.ShippingLineFCLJobExportNavigation)
        .Where(t => t.JobNo == jobNo).ToListAsync(), // Ensure this is awaited too
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



        // GET: Print Invoice 
        public async Task<IActionResult> RepPrintInvoice(string InvoiceNo)
        {
            if (InvoiceNo == null || _context.TxnInvoiceFCLHds == null)
            {
                return NotFound();
            }

            var txnInvoiceFCLHd = await _context.TxnInvoiceFCLHds
                .FirstOrDefaultAsync(m => m.InvoiceNo == InvoiceNo);

            if (txnInvoiceFCLHd == null)
            {
                return NotFound();
            }

            var strjobNo = txnInvoiceFCLHd.JobNo;

            var containerNo = _context.TxnStuffingPlanHds
                .Where(s => s.JobNumber == strjobNo)
                .Select(s => s.ContainerNo)
                .FirstOrDefault();

            var tables = new InvoiceFCLViewModel
            {
                InvoiceFCLHdMulti = _context.TxnInvoiceFCLHds.Include(c => c.CustomerInvoiceFCLHd).Where(t => t.InvoiceNo == InvoiceNo),
                InvoiceFCLDtlMulti = _context.TxnInvoiceFCLDtls.Include(c => c.ChargeItemNavigation).Where(t => t.InvoiceNo == InvoiceNo),
                FCLJobMulti = _context.TxnFCLJobs
                .Include(t => t.ShippingLineFCLJobExportNavigation)
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

            //var InvoiceBokkingNo = txnInvoiceFCLHd.Blno;  // Contains Booking No. 
            //var txnBookingExp = await _context.TxnBookingExps
            //    .Include(t => t.FDNNavigation)
            //    .FirstOrDefaultAsync(m => m.BookingNo == InvoiceBokkingNo);
            //ViewData["HBL"] = txnBookingExp.Blno;
            //ViewData["CBM"] = txnBookingExp.Cbm;
            //ViewData["Weight"] = txnBookingExp.GrossWeight;
            //ViewData["NoofPkgs"] = txnBookingExp.NoofPackages;
            //ViewData["FinalDest"] = txnBookingExp.FDNNavigation.PortName;

            //DateTime etdPolDate = (DateTime)txnBookingExp.EtdPol;
            //string formattedDate = etdPolDate.ToString("dd/MM/yyyy");
            //ViewData["ETD"] = formattedDate;

            return View(tables);

        }




        public async Task<IActionResult> Approve(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var txnInvoiceFCLHd = await _context.TxnInvoiceFCLHds
                .FirstOrDefaultAsync(m => m.InvoiceNo == id);

            if (txnInvoiceFCLHd == null)
            {
                return NotFound();
            }

            jobNo = txnInvoiceFCLHd.JobNo; // Set the jobNo property

            var tables = new InvoiceFCLViewModel
            {
                InvoiceFCLHdMulti = _context.TxnInvoiceFCLHds.Where(t => t.InvoiceNo == id),
                InvoiceFCLDtlMulti = _context.TxnInvoiceFCLDtls.Where(t => t.InvoiceNo == id),
                FCLJobMulti = _context.TxnFCLJobs
                .Include(t => t.ShippingLineFCLJobExportNavigation)
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

            var txnInvoiceFCLHd = await _context.TxnInvoiceFCLHds
                .FirstOrDefaultAsync(m => m.InvoiceNo == id);

            if (txnInvoiceFCLHd == null)
            {
                return NotFound();
            }

            // Inserting Transaction Data to Acccounts 
            var txnInvoiceFCLDtls = await _context.TxnInvoiceFCLDtls
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
            if (txnInvoiceFCLDtls != null)
            {
                var SerialNo_AccTxn = 1;
                var AccTxnDescription = " Local Debtors INV: " + txnInvoiceFCLHd.InvoiceNo;
                // First Acc transaction 
                TxnTransactions NewRowAccTxnFirst = new TxnTransactions();
                NewRowAccTxnFirst.TxnNo = nextAccTxnNo;
                NewRowAccTxnFirst.TxnSNo = SerialNo_AccTxn;
                NewRowAccTxnFirst.Date = txnInvoiceFCLHd.Date; //  Jurnal Date 
                NewRowAccTxnFirst.Description = AccTxnDescription;
                NewRowAccTxnFirst.TxnAccCode = txnInvoiceFCLHd.DebitAcc;
                NewRowAccTxnFirst.Dr = (decimal)txnInvoiceFCLHd.TotalInvoiceAmountLkr;
                NewRowAccTxnFirst.Cr = (decimal)0;
                NewRowAccTxnFirst.RefNo = txnInvoiceFCLHd.InvoiceNo; // Invoice No
                NewRowAccTxnFirst.Note = "";
                NewRowAccTxnFirst.Reconciled = false;
                NewRowAccTxnFirst.DocType = "Invoice";
                NewRowAccTxnFirst.IsMonthEndDone = false;
                NewRowAccTxnFirst.CreatedBy = "Admin";
                NewRowAccTxnFirst.CreatedDateTime = DateTime.Now;
                NewRowAccTxnFirst.Canceled = false;

                _accountsContext.TxnTransactions.Add(NewRowAccTxnFirst);

                foreach (var item in txnInvoiceFCLDtls)
                {
                    // Transaction table Insert 
                    TxnTransactions NewRowAccTxn = new TxnTransactions();

                    SerialNo_AccTxn = SerialNo_AccTxn + 1;
                    NewRowAccTxn.TxnNo = nextAccTxnNo;
                    NewRowAccTxn.TxnSNo = SerialNo_AccTxn;
                    NewRowAccTxn.Date = txnInvoiceFCLHd.Date; //  Jurnal Date 
                    NewRowAccTxn.Description = item.ChargeItemNavigation.Description + "INV: " + item.InvoiceNo;
                    NewRowAccTxn.TxnAccCode = item.AccNo;
                    NewRowAccTxn.Dr = (decimal)0;
                    if (item.Currency == "USD")
                    {
                        var AmtLkr = item.Amount * txnInvoiceFCLHd.ExchangeRate; // convert to LKR
                        NewRowAccTxn.Cr = (decimal)AmtLkr;
                    }
                    else
                    {
                        NewRowAccTxn.Cr = (decimal)item.Amount;
                    }
                    NewRowAccTxn.RefNo = item.InvoiceNo; // Invoice No
                    NewRowAccTxn.Note = "";
                    NewRowAccTxn.Reconciled = false;
                    NewRowAccTxn.DocType = "InvoiceFCL";
                    NewRowAccTxn.IsMonthEndDone = false;
                    NewRowAccTxn.CreatedBy = "Admin";
                    NewRowAccTxn.CreatedDateTime = DateTime.Now;
                    NewRowAccTxn.Canceled = false;

                    _accountsContext.TxnTransactions.Add(NewRowAccTxn);
                }
            }
            // END Inserting Transaction Data to Acccounts 

            /// Update the Approved property based on the form submission
            txnInvoiceFCLHd.Approved = approved;
            txnInvoiceFCLHd.ApprovedBy = "CurrentUserName"; // Replace with the actual user name
            txnInvoiceFCLHd.ApprovedDateTime = DateTime.Now;

            _context.Update(txnInvoiceFCLHd);
            await _context.SaveChangesAsync();
            await _accountsContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirect to the appropriate action
        }









    }
}
