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
namespace FMS_DEV.Controllers
{

    public class ProfitandLostController : Controller
    {
        private readonly FtlcolombOperationContext _context;
        private readonly FtlcolomboAccountsContext _accountsContext;
        public ProfitandLostController(FtlcolombOperationContext context)
        {
            _context = context;
        }

        // GET: txnCreditNoteHds
        public async Task<IActionResult> Index(string searchString, int pg = 1)
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
            //Job Number Data Come From When Job Number is Search 
            var jobData = _context.TxnExportJobHds
              .Include(t => t.AgentExportNavigation)
              .Include(t => t.HandlebyExportJobNavigation)
              .Include(t => t.CreatedByExportJobNavigation)
              .Include(t => t.ShippingLineExportNavigation)
              .Include(t => t.ColoaderExportNavigation)
              .Include(t => t.VesselExportJobDtlNavigation)
              .Include(t => t.PODExportJobNavigation)
              .Include(t => t.FDNExportJobNavigation)
              .Where(job => job.JobNo == jobNo)
              .ToList();
            //This is Doing Whne User search the Job Numebr and Display the data for Invoice 
            var invoiceHdData = _context.TxnInvoiceExportHds
                .Where(invoice => invoice.JobNo == jobNo)
                .ToList();

            var invoiceDtlData = _context.TxnInvoiceExportDtls
                .Include(t => t.ChargeItemNavigation)
                .Where(dtl => invoiceHdData.Select(hd => hd.InvoiceNo).Contains(dtl.InvoiceNo))
                .ToList();

            //This is Doing Whne User search the Job Numebr and Display the data for Debit Note
            var DebitNoteHdData = _context.TxnDebitNoteExportHds
                .Where(Debitnote => Debitnote.JobNo == jobNo)
                .ToList();

            var DebitNoteeDtlData = _context.TxnDebitNoteExportDtls
                .Include(t => t.ChargeItemNavigation)
                .Where(dtl => DebitNoteHdData.Select(hd => hd.DebitNoteNo).Contains(dtl.DebitNoteNo))
                .ToList();
            //This is Doing Whne User search the Job Numebr and Display the data for Credit Note
            var CreditNoteHdData = _context.TxnCreditNoteExportHds
                .Where(Creditnote => Creditnote.JobNo == jobNo)
                .ToList();

            var CreditNoteeDtlData = _context.TxnCreditNoteExportDtls
                .Include(t => t.ChargeItemNavigation)
                .Where(dtl => CreditNoteHdData.Select(hd => hd.CreditNoteNo).Contains(dtl.CreditNoteNo))
                .ToList();
            //This is Doing Whne User search the Job Numebr and Display the data for Payment Vocher
            var PaymentVocherHdData = _context.TxnPaymentVoucherExportHds
                .Where(PaymentVocherNo => PaymentVocherNo.JobNo == jobNo)
                .ToList();

            var PaymentVocherDtlData = _context.TxnPaymentVoucherExportDtls
                .Include(t => t.ChargeItemNavigation)
                .Where(dtl => PaymentVocherHdData.Select(hd => hd.PayVoucherNo).Contains(dtl.PayVoucherNo))
                .ToList();


            var viewModel = new ProfitandLostViewModel
            {
                //This is the Invoice Data Come 
                InvoiceHdMulti = invoiceHdData,
                InvoiceDtMulti = invoiceDtlData,
                //This is the Debit Note Data come
                DebitNoteHdMulti = DebitNoteHdData,
                DebitNoteDtMulti = DebitNoteeDtlData,
                //This is the Credit Note Data come
                CreditNoteHdMulti = CreditNoteHdData,
                CreditNoteDtMulti = CreditNoteeDtlData,
                ////This is the Payment Vocher Data come
                PayVoucherHdMulti = PaymentVocherHdData,
                PayVoucherDtlMulti = PaymentVocherDtlData,
                //This is the Job Data Come
                ExportJobHdMulti = jobData,


            };

            //Total Values Come From and Make it This is for Invoice 

            var sumTotalTotalInvoiceAmountLKR = _context.TxnInvoiceExportHds
            .Where(invoice => invoice.JobNo == jobNo)
            .Sum(invoice => invoice.TotalInvoiceAmountLkr);

            //Total Values Come From and Make it This is for Credit Note 

            var sumTotalCreditNoteAmountLKR = _context.TxnCreditNoteExportHds
            .Where(Creditnote => Creditnote.JobNo == jobNo)
            .Sum(Creditnote => Creditnote.TotalCreditAmountLkr);

            //Total Values Come From and Make it This is for Debit Note 

            var sumTotalDebitNoteAmountLKR = _context.TxnDebitNoteExportHds
            .Where(Debitnote => Debitnote.JobNo == jobNo)
            .Sum(Debitnote => Debitnote.TotalDebitAmountLkr);


            //Total Values Come From and Make it This is for Payment Vocher 

            var sumTotalPayemntVocherAmountLKR = _context.TxnPaymentVoucherExportHds
            .Where(PaymentVocherNo => PaymentVocherNo.JobNo == jobNo)
            .Sum(PaymentVocherNo => PaymentVocherNo.TotalPayVoucherAmountLkr);

            // Format and set ViewBag values for Invoice
            ViewBag.TotalInvoiceAmountLKR = ((decimal)sumTotalTotalInvoiceAmountLKR).ToString("#,##0.00");

            // Format and set ViewBag values for Credit Note
            ViewBag.TotalCreditNoteAmountLKR = ((decimal)sumTotalCreditNoteAmountLKR).ToString("#,##0.00");

            // Format and set ViewBag values for Debit Note
            ViewBag.TotalDebitNoteAmountLKR = ((decimal)sumTotalDebitNoteAmountLKR).ToString("#,##0.00");

            // Format and set ViewBag values for Payment Voucher
            ViewBag.TotalPaymentVoucherAmountLKR = ((decimal)sumTotalPayemntVocherAmountLKR).ToString("#,##0.00");


            // Calculate Total Income
            decimal totalIncome = (sumTotalTotalInvoiceAmountLKR ?? 0) + (sumTotalDebitNoteAmountLKR ?? 0);

            // Calculate Total Expenses
            decimal totalExpenses = (sumTotalCreditNoteAmountLKR ?? 0) + (sumTotalPayemntVocherAmountLKR ?? 0);

            // Calculate Profit/Loss
            decimal profitLoss = totalIncome - totalExpenses;

            // Format and set ViewBag values for Total Income
            ViewBag.TotalIncome = totalIncome.ToString("#,##0.00");

            // Format and set ViewBag values for Total Expenses
            ViewBag.TotalExpenses = totalExpenses.ToString("#,##0.00");

            // Format and set ViewBag value for Profit/Loss
            ViewBag.ProfitLoss = profitLoss.ToString("#,##0.00");


            return View(viewModel);
        }
     
    }
}


