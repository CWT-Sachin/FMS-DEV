﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FMS_DEV.Models;
using FMS_DEV.ViewModel;
using Newtonsoft.Json;
using FMS_DEV.Data;
using FMS_DEV.DataAccounts;
using FMS_DEV.ModelsAccounts;
using FMS_DEV.CommonMethods;

namespace FMS_DEV.Controllers
{
    public class TxnPaymentVoucherExportHdsController : Controller
    {
        private readonly FtlcolombOperationContext _context;
        private readonly FtlcolomboAccountsContext _accountsContext;


        public string jobNo { get; private set; }


        public TxnPaymentVoucherExportHdsController(FtlcolombOperationContext context, FtlcolomboAccountsContext accountsContext)
        {
            _context = context;
            _accountsContext = accountsContext;
        }

        // GET: TxnPaymentVoucherHds
        public async Task<IActionResult> Index(string searchString, string searchType, int pg = 1)
        {
            var txnPaymentVoucherExportHds = _context.TxnPaymentVoucherExportHds.OrderByDescending(p => p.PayVoucherNo).Include(t => t.ShippingLinePaymentVoucherExpNavigation).Include(t => t.AgentPaymentVoucherExportNavigation);

            if (searchType == "Approve")
            {
                // Filter only not approved invoices
                txnPaymentVoucherExportHds = txnPaymentVoucherExportHds.Where(t => t.Approved != true).OrderByDescending(p => p.PayVoucherNo).Include(t => t.ShippingLinePaymentVoucherExpNavigation).Include(t => t.AgentPaymentVoucherExportNavigation);
            }
            else if (!String.IsNullOrEmpty(searchString))
            {
                if (searchType == "PaymentVoucherNote")
                {
                    txnPaymentVoucherExportHds = txnPaymentVoucherExportHds.Where(t => t.PayVoucherNo.Contains(searchString)).OrderByDescending(p => p.PayVoucherNo).Include(t => t.ShippingLinePaymentVoucherExpNavigation).Include(t => t.AgentPaymentVoucherExportNavigation);
                }
                else if (searchType == "Job")
                {
                    txnPaymentVoucherExportHds = txnPaymentVoucherExportHds.Where(t => t.JobNo.Contains(searchString)).OrderByDescending(p => p.PayVoucherNo).Include(t => t.ShippingLinePaymentVoucherExpNavigation).Include(t => t.AgentPaymentVoucherExportNavigation);
                }

                // Check if results are found
                if (txnPaymentVoucherExportHds.Any())
                {
                    ViewData["PayemntVocherFound"] = "";
                }
                else
                {
                    ViewData["PayemntVocherFound"] = $"{searchType} Number: {searchString} not found";
                }
            }

            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int recsCount = txnPaymentVoucherExportHds.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = txnPaymentVoucherExportHds.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;
            return View(data);
        }


        public async Task<IActionResult> Approve(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var txnPaymentVoucherExportHds = await _context.TxnPaymentVoucherExportHds
                .FirstOrDefaultAsync(m => m.PayVoucherNo == id);

            if (txnPaymentVoucherExportHds == null)
            {
                return NotFound();
            }

            jobNo = txnPaymentVoucherExportHds.JobNo; // Set the jobNo property

            var tables = new PayVoucherExportViewModel
            {
                PayVoucherHdMulti = _context.TxnPaymentVoucherExportHds.Where(t => t.PayVoucherNo == id),
                PayVoucherDtlMulti = _context.TxnPaymentVoucherExportDtls.Where(t => t.PayVoucherNo == id),
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

            var txnPaymentVoucherHds = await _context.TxnPaymentVoucherExportHds
                .FirstOrDefaultAsync(m => m.PayVoucherNo == id);

            if (txnPaymentVoucherHds == null)
            {
                return NotFound();
            }

            // Inserting Transaction Data to Acccounts 
            var txnPaymentVoucherExportDtls = await _context.TxnPaymentVoucherExportDtls
                .Include(t => t.ChargeItemNavigation)
                .Where(m => m.PayVoucherNo == id)
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
            if (txnPaymentVoucherExportDtls != null)
            {
                var SerialNo_AccTxn = 1;
                var AccTxnDescription = " Local Creditor CREDT: " + txnPaymentVoucherHds.PayVoucherNo;
                // First Acc transaction 
                TxnTransactions NewRowAccTxnFirst = new TxnTransactions();
                NewRowAccTxnFirst.TxnNo = nextAccTxnNo;
                NewRowAccTxnFirst.TxnSNo = SerialNo_AccTxn;
                NewRowAccTxnFirst.Date = txnPaymentVoucherHds.Date; //  Jurnal Date 
                NewRowAccTxnFirst.Description = AccTxnDescription;
                NewRowAccTxnFirst.TxnAccCode = txnPaymentVoucherHds.CreditAcc;
                NewRowAccTxnFirst.Dr = (decimal)0;
                NewRowAccTxnFirst.Cr = (decimal)txnPaymentVoucherHds.TotalPayVoucherAmountLkr;
                NewRowAccTxnFirst.RefNo = txnPaymentVoucherHds.PayVoucherNo; // Debit Note No
                NewRowAccTxnFirst.Note = "";
                NewRowAccTxnFirst.Reconciled = false;
                NewRowAccTxnFirst.DocType = "PayVCExport";
                NewRowAccTxnFirst.IsMonthEndDone = false;
                NewRowAccTxnFirst.CreatedBy = "Admin";
                NewRowAccTxnFirst.CreatedDateTime = DateTime.Now;
                NewRowAccTxnFirst.Canceled = false;

                _accountsContext.TxnTransactions.Add(NewRowAccTxnFirst);

                foreach (var item in txnPaymentVoucherExportDtls)
                {
                    // Transaction table Insert 
                    TxnTransactions NewRowAccTxn = new TxnTransactions();

                    SerialNo_AccTxn = SerialNo_AccTxn + 1;
                    NewRowAccTxn.TxnNo = nextAccTxnNo;
                    NewRowAccTxn.TxnSNo = SerialNo_AccTxn;
                    NewRowAccTxn.Date = txnPaymentVoucherHds.Date; //  Jurnal Date 
                    NewRowAccTxn.Description = item.ChargeItemNavigation.Description + "PAYVC: " + item.PayVoucherNo;
                    NewRowAccTxn.TxnAccCode = item.AccNo;
                    NewRowAccTxn.Cr = (decimal)0;
                    if (item.Currency == "USD")
                    {
                        var AmtLkr = item.Amount * txnPaymentVoucherHds.ExchangeRate; // convert to LKR
                        NewRowAccTxn.Dr = (decimal)AmtLkr;
                    }
                    else
                    {
                        NewRowAccTxn.Dr = (decimal)item.Amount;
                    }
                    NewRowAccTxn.RefNo = item.PayVoucherNo; // Invoice No
                    NewRowAccTxn.Note = "";
                    NewRowAccTxn.Reconciled = false;
                    NewRowAccTxn.DocType = "PayVCExport";
                    NewRowAccTxn.IsMonthEndDone = false;
                    NewRowAccTxn.CreatedBy = "Admin";
                    NewRowAccTxn.CreatedDateTime = DateTime.Now;
                    NewRowAccTxn.Canceled = false;

                    _accountsContext.TxnTransactions.Add(NewRowAccTxn);
                }
            }
            // END Inserting Transaction Data to Acccounts 


            /// Update the Approved property based on the form submission
            txnPaymentVoucherHds.Approved = approved;
            txnPaymentVoucherHds.ApprovedBy = "CurrentUserName"; // Replace with the actual user name
            txnPaymentVoucherHds.ApprovedDateTime = DateTime.Now;

            _context.Update(txnPaymentVoucherHds);
            await _context.SaveChangesAsync();
            await _accountsContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirect to the appropriate action
        }


        // GET: TxnPaymentVoucherHds/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TxnPaymentVoucherExportHds == null)
            {
                return NotFound();
            }

            var txnPaymentVoucherHd = await _context.TxnPaymentVoucherExportHds
                .FirstOrDefaultAsync(m => m.PayVoucherNo == id);
            if (txnPaymentVoucherHd == null)
            {
                return NotFound();
            }
            var jobNo = txnPaymentVoucherHd.JobNo;
            var tables = new PayVoucherExportViewModel
            {
                PayVoucherHdMulti = _context.TxnPaymentVoucherExportHds.Where(t => t.PayVoucherNo == id),
                PayVoucherDtlMulti = _context.TxnPaymentVoucherExportDtls.Where(t => t.PayVoucherNo == id),
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
            List<SelectListItem> paymentcurrency = new List<SelectListItem>
                {
                    new SelectListItem { Value = "USD Payment", Text = "USD Payment" },
                    new SelectListItem { Value = "LKR Payment", Text = "LKR Payment" }
                };
            ViewData["paymentcurrencyList"] = new SelectList(paymentcurrency, "Value", "Text", "paymentcurrency");
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


        //// GET: TxnDebitNoteHds/Create
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
            var tables = new PayVoucherExportViewModel
            {
                PayVoucherHdMulti = _context.TxnPaymentVoucherExportHds.Where(t => t.PayVoucherNo == "xxx"),
                PayVoucherDtlMulti = _context.TxnPaymentVoucherExportDtls.Where(t => t.PayVoucherNo == "xxx"),
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

            List<SelectListItem> paymentcurrency = new List<SelectListItem>
                {
                    new SelectListItem { Value = "USD Payment", Text = "USD Payment" },
                    new SelectListItem { Value = "LKR Payment", Text = "LKR Payment" }
                };
            ViewData["paymentcurrencyList"] = new SelectList(paymentcurrency, "Value", "Text", "paymentcurrency");

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


        // POST: TxnPaymentVoucherHds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string dtlItemsList, [Bind("PayVoucherNo,Date,JobNo,Customer,ExchangeRate,PayCurrency,TotalPayVoucherAmountLkr,TotalPayVoucherAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,Approved,Type, AmountPaid,AmountToBePaid, AgentID")] TxnPaymentVoucherExportHd txnPaymentVoucherHd)
        {

            // Next RefNumber for txnImportJobDtl
            var nextRefNo = "";
            var TableID = "Txn_PaymentVoucherHD_Export";
            var refLastNumber = await _context.RefLastNumbers.FindAsync(TableID);

            if (refLastNumber != null)
            {
                var nextNumber = refLastNumber.LastNumber + 1;
                refLastNumber.LastNumber = nextNumber;
                nextRefNo = "EPV" + DateTime.Now.Year.ToString() + nextNumber.ToString().PadLeft(5, '0');
                txnPaymentVoucherHd.PayVoucherNo = nextRefNo;

                // _context.RefLastNumbers.Remove(refLastNumber);
            }
            else
            {
                return NotFound();
                //return View(tables);
            }

            txnPaymentVoucherHd.Canceled = false;
            txnPaymentVoucherHd.Approved = false;

            txnPaymentVoucherHd.CreatedBy = "Admin";
            txnPaymentVoucherHd.CreatedDateTime = DateTime.Now;

            var LocalOverseas = txnPaymentVoucherHd.Type;
            var TransDocMode = "Export";
            var TransDocType = "Expenses";
            var TransDocVender = "";

            // Temparory solution : 
            // Assuming txnPaymentVoucherHd.Type is a string
             LocalOverseas = string.IsNullOrEmpty(txnPaymentVoucherHd.Type) ? "Overseas" : txnPaymentVoucherHd.Type;

            if (LocalOverseas == "Local")
            {
                txnPaymentVoucherHd.CreditAcc = "ACC0067"; // LOCAL CREDITORS  1200-02	
                TransDocVender = "Liner";
            }
            else // Oversease
            {
                txnPaymentVoucherHd.CreditAcc = "ACC0068"; // OVERSEAS CREDITORS	  1200-03	
                TransDocVender = "Agent";
                txnPaymentVoucherHd.Type = "Overseas"; // There is an error in the create if the Selection is "Overseae" Type not pass. This is  a temporary solution. 

            }

            txnPaymentVoucherHd.DebitAcc = null;




            ModelState.Remove("PayVoucherNo");  // Auto genrated
            if (ModelState.IsValid)
            {
                // Adding CargoBreakDown records
                if (!string.IsNullOrWhiteSpace(dtlItemsList))
                {
                    var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnPaymentVoucherExportDtl>>(dtlItemsList);
                    if (DtailItemdataTable != null)
                    {
                        var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the common method  class
                        foreach (var item in DtailItemdataTable)
                        {
                            TxnPaymentVoucherExportDtl DetailItem = new TxnPaymentVoucherExportDtl();
                            DetailItem.PayVoucherNo = nextRefNo; // New PayVoucherNo Number
                            DetailItem.SerialNo = item.SerialNo;
                            DetailItem.ChargeItem = item.ChargeItem;
                            DetailItem.Unit = item.Unit ?? "DefaultUnit"; // Set a default value if 'Unit' is null
                            DetailItem.Rate = item.Rate;
                            DetailItem.Currency = item.Currency;
                            DetailItem.Qty = item.Qty;
                            DetailItem.Amount = item.Amount;
                            DetailItem.BlContainerNo = item.BlContainerNo;
                            DetailItem.AccNo = CommonMethodAccClass.GetChargeItemAccNo(item.ChargeItem, TransDocMode, TransDocType, TransDocVender);
                            _context.TxnPaymentVoucherExportDtls.Add(DetailItem);
                        }
                    }
                }
                _context.Add(txnPaymentVoucherHd);
                _context.RefLastNumbers.Update(refLastNumber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var tables = new PayVoucherExportViewModel
            {
                PayVoucherHdMulti = _context.TxnPaymentVoucherExportHds.Where(t => t.PayVoucherNo == "xxx"),
                PayVoucherDtlMulti = _context.TxnPaymentVoucherExportDtls.Where(t => t.PayVoucherNo == "xxx"),

                ExportJobHdMulti = _context.TxnExportJobHds.Where(t => t.JobNo == "xxx"),
                ExportJobDtlMulti = _context.TxnExportJobDtls.Where(t => t.JobNo == "xxx"),
            };

            List<SelectListItem> Currency = new List<SelectListItem>
                {
                    new SelectListItem { Value = "USD", Text = "USD" },
                    new SelectListItem { Value = "LKR", Text = "LKR" }
                };
            ViewData["CurrencyList"] = new SelectList(Currency, "Value", "Text", "CurrencyList");
            List<SelectListItem> paymentcurrency = new List<SelectListItem>
                {
                    new SelectListItem { Value = "USD Payment", Text = "USD Payment" },
                    new SelectListItem { Value = "LKR Payment", Text = "LKR Payment" }
                };
            ViewData["paymentcurrencyList"] = new SelectList(paymentcurrency, "Value", "Text", "paymentcurrency");
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



      


        // GET: Print Payment Voucher
        public async Task<IActionResult> RepPrintPaymentVoucher(string PayVoucherNo)
        {
            if (PayVoucherNo == null || _context.TxnPaymentVoucherExportHds == null)
            {
                return NotFound();
            }

            var txnPaymentVoucherHd = await _context.TxnPaymentVoucherExportHds
                .FirstOrDefaultAsync(m => m.PayVoucherNo == PayVoucherNo);

            if (txnPaymentVoucherHd == null)
            {
                return NotFound();
            }
            var strjobNo = txnPaymentVoucherHd.JobNo;
            var tables = new PayVoucherExportViewModel
            {
                PayVoucherHdMulti = _context.TxnPaymentVoucherExportHds.Where(t => t.PayVoucherNo == PayVoucherNo)
                 .Include(t => t.AgentPaymentVoucherExportNavigation)
                  .Include(t => t.ShippingLinePaymentVoucherExpNavigation),
                PayVoucherDtlMulti = _context.TxnPaymentVoucherExportDtls.Where(t => t.PayVoucherNo == PayVoucherNo)
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
            List<SelectListItem> paymentcurrency = new List<SelectListItem>
                {
                    new SelectListItem { Value = "USD Payment", Text = "USD Payment" },
                    new SelectListItem { Value = "LKR Payment", Text = "LKR Payment" }
                };
            ViewData["paymentcurrencyList"] = new SelectList(paymentcurrency, "Value", "Text", "paymentcurrency");
            List<SelectListItem> Unit = new List<SelectListItem>
                {
                    new SelectListItem { Value = "CBM", Text = "CBM" },
                    new SelectListItem { Value = "BL", Text = "BL" },
                    new SelectListItem { Value = "CNT", Text = "CNT" }
                };
            ViewData["UnitList"] = new SelectList(Unit, "Value", "Text", "UnitList");
            ViewData["ChargeItemsList"] = new SelectList(_context.Set<RefChargeItemAcc>().Where(a => a.IsActive.Equals(true)).OrderBy(p => p.Description), "ChargeId", "Description", "ChargeId");
            ViewData["ShippingLines"] = new SelectList(_context.RefShippingLines.OrderBy(c => c.Name), "ShippingLineId", "Name", "ShippingLineId");
            return View(tables);

        }


        // GET: TxnPaymentVoucherHds/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.TxnPaymentVoucherExportHds == null)
            {
                return NotFound();
            }

            var txnPaymentVoucherExportHd = await _context.TxnPaymentVoucherExportHds.FindAsync(id);
            if (txnPaymentVoucherExportHd == null)
            {
                return NotFound();
            }
            var PayVoucherNo = id;
            var JobNo = txnPaymentVoucherExportHd.JobNo;

            var tables = new PayVoucherExportViewModel
            {
                PayVoucherHdMulti = _context.TxnPaymentVoucherExportHds.Where(t => t.PayVoucherNo == PayVoucherNo),
                PayVoucherDtlMulti = _context.TxnPaymentVoucherExportDtls.Where(t => t.PayVoucherNo == PayVoucherNo),
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
            List<SelectListItem> paymentcurrency = new List<SelectListItem>
                {
                    new SelectListItem { Value = "USD Payment", Text = "USD Payment" },
                    new SelectListItem { Value = "LKR Payment", Text = "LKR Payment" }
                };
            ViewData["ChargeItemsList"] = new SelectList(_context.Set<RefChargeItemAcc>().Where(a => a.IsActive.Equals(true)).OrderBy(p => p.Description), "ChargeId", "Description", "ChargeId");
            ViewData["paymentcurrencyList"] = new SelectList(paymentcurrency, "Value", "Text", "paymentcurrency");
            ViewData["UnitList"] = new SelectList(Unit, "Value", "Text", "UnitList");
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

        // POST: TxnPaymentVoucherHds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string dtlItemsList, [Bind("PayVoucherNo,Date,JobNo,Customer,ShippingLine,ExchangeRate,PayCurrency,TotalPayVoucherAmountLkr,TotalPayVoucherAmountUsd,Remarks,CreatedBy,CreatedDateTime,LastUpdatedBy,LastUpdatedDateTime,Canceled,Approved,Type, AmountPaid,AmountToBePaid, AgentID")] TxnPaymentVoucherExportHd txnPaymentVoucherHd)
        {
            if (id != txnPaymentVoucherHd.PayVoucherNo)
            {
                return NotFound();
            }

            txnPaymentVoucherHd.LastUpdatedBy = "Admin";
            txnPaymentVoucherHd.LastUpdatedDateTime = DateTime.Now;

            txnPaymentVoucherHd.AmountPaid = 0;
            txnPaymentVoucherHd.AmountToBePaid = txnPaymentVoucherHd.TotalPayVoucherAmountLkr;

            var LocalOverseas = txnPaymentVoucherHd.Type;
            var TransDocMode = "Export";
            var TransDocType = "Expenses";
            var TransDocVender = "";

            if (LocalOverseas == "Local")
            {
                txnPaymentVoucherHd.CreditAcc = "ACC0067"; // LOCAL CREDITORS  1200-02	
                TransDocVender = "Liner";

            }
            else // Oversease
            {
                txnPaymentVoucherHd.CreditAcc = "ACC0068"; // OVERSEAS CREDITORS	  1200-03	
                TransDocVender = "Agent";
                txnPaymentVoucherHd.Type = "Overseas"; // There is an error in the create if the Selection is "Overseae" Type not pass. This is  a temporary solution. 

            }

            txnPaymentVoucherHd.DebitAcc = null;

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(dtlItemsList))
                    {
                        var rowsToDelete = _context.TxnPaymentVoucherExportDtls.Where(t => t.PayVoucherNo == id);
                        if (rowsToDelete != null || rowsToDelete.Any())
                        {
                            // Remove the rows from the database context
                            _context.TxnPaymentVoucherExportDtls.RemoveRange(rowsToDelete);
                        }
                        var DtailItemdataTable = JsonConvert.DeserializeObject<List<TxnPaymentVoucherExportDtl>>(dtlItemsList);
                        if (DtailItemdataTable != null)
                        {
                            var CommonMethodAccClass = new CommonMethodAccClass(_context); // Instantiate the common method  class

                            foreach (var item in DtailItemdataTable)
                            {
                                TxnPaymentVoucherExportDtl DetailItem = new TxnPaymentVoucherExportDtl();
                                DetailItem.PayVoucherNo = id; // New CreditNote Number
                                DetailItem.SerialNo = item.SerialNo;
                                DetailItem.ChargeItem = item.ChargeItem;
                                DetailItem.Unit = item.Unit;
                                //// Add a null check for 'Unit'
                                //DetailItem.Unit = item.Unit ?? "DefaultUnit"; // Use a default value if 'Unit' is null

                                DetailItem.Rate = item.Rate;
                                DetailItem.Currency = item.Currency;
                                DetailItem.Qty = item.Qty;
                                DetailItem.Amount = item.Amount;
                                DetailItem.BlContainerNo = item.BlContainerNo;
                                DetailItem.AccNo = DetailItem.AccNo = CommonMethodAccClass.GetChargeItemAccNo(item.ChargeItem, TransDocMode, TransDocType, TransDocVender);

                                _context.TxnPaymentVoucherExportDtls.Add(DetailItem);
                            }

                        }
                    }
                    _context.Update(txnPaymentVoucherHd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TxnPaymentVoucherHdExists(txnPaymentVoucherHd.PayVoucherNo))
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
            return View(txnPaymentVoucherHd);
        }

        // GET: TxnPaymentVoucherHds/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.TxnPaymentVoucherExportHds == null)
            {
                return NotFound();
            }

            var txnPaymentVoucherHd = await _context.TxnPaymentVoucherExportHds
                .FirstOrDefaultAsync(m => m.PayVoucherNo == id);
            if (txnPaymentVoucherHd == null)
            {
                return NotFound();
            }

            return View(txnPaymentVoucherHd);
        }

        // POST: TxnPaymentVoucherHds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.TxnPaymentVoucherExportHds == null)
            {
                return Problem("Entity set 'FtlcolombOperationContext.TxnPaymentVoucherHds'  is null.");
            }
            var txnPaymentVoucherHd = await _context.TxnPaymentVoucherExportHds.FindAsync(id);
            if (txnPaymentVoucherHd != null)
            {
                _context.TxnPaymentVoucherExportHds.Remove(txnPaymentVoucherHd);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TxnPaymentVoucherHdExists(string id)
        {
          return (_context.TxnPaymentVoucherExportHds?.Any(e => e.PayVoucherNo == id)).GetValueOrDefault();
        }
    }
}