using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FMS_DEV.ViewModel;
using System.ComponentModel;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FMS_DEV.Data;

namespace FMS_DEV.Views
{
    public class DashBrdExportsController : Controller
    {
        private readonly FtlcolombOperationContext _context;


        public DashBrdExportsController(FtlcolombOperationContext context)
        {
            _context = context;
        }


        // GET: TxnImportJobDtls- Transshipment Update
        public async Task<IActionResult> DashBrdExportForPlan(DateTime pFromDate, DateTime pToDate, int pg = 1)
        {
            var fromDate = DateTime.Now;
            var toDate = DateTime.Now;

            if (pFromDate == DateTime.MinValue)  // "01/01/000/"
            {
                 fromDate = DateTime.Today.AddDays(-28);
                toDate = DateTime.Today;
                ViewData["Period"] = "Last 28 days";
            }
            else
            {
                fromDate = pFromDate;
                toDate = pToDate;
                ViewData["Period"] = "Custom";
            }


            var tables = new DashBrdExportForPlanViewModel
            {
                ImportJobDtlMulti = (
                                from dtl in _context.TxnImportJobDtls
                                join hd in _context.TxnImportJobHds on dtl.JobNo equals hd.JobNo
                                where hd.JobDate >= fromDate && hd.JobDate <= toDate && dtl.Canceled != true && dtl.CargoType == "TS"
                                orderby dtl.RefNo descending
                                select dtl
                                ).Include(t => t.BlstatusNavigation).Include(t => t.Bltype).Include(t => t.PackageTypeImportJobNavigation).Include(t => t.SalesPersonImportJobdtlNavigation).Include(t => t.ShipperImportJobDtlNavigation).Include(t => t.TsblstatusNavigation).Include(t => t.TsbltypeNavigation).Include(t => t.TsdestinationNavigation)
                                .ToList(),
                BookingExpMulti = (
                                from dtl in _context.TxnBookingExps
                                where dtl.BookingDate >= fromDate && dtl.BookingDate <= toDate && dtl.Canceled != true
                                orderby dtl.BookingNo descending
                                select dtl
                                ).ToList(),
                
            };

            // imports
            var importJobDetails = tables.ImportJobDtlMulti;
            // Use the temporary variable in the ResultsFDNWiseTS query
            var resultsImportFDN = importJobDetails
                .GroupBy(detail => detail.TsdestinationNavigation.PortName)
                .Select(group => new ResultsFDNWiseViewModel
                {
                    Fdn = group.Key,
                    TotalCBM = (decimal)group.Sum(detail => detail.Cbm),
                    TotalWeight = (decimal)group.Sum(detail => detail.Weight),
                    NumberOfShipments = group.Count()
                })
                .OrderBy(t => t.Fdn).ToList();

            // Now you can assign the results to the ResultsFDNWiseTS property
            tables.ResultsFDNWiseTS = resultsImportFDN;

            ViewData["FromDate"] = fromDate;
            ViewData["ToDate"] = toDate;
            
 
            var TotalTSFDNsImport = tables.ResultsFDNWiseTS.Count();
            var TotalTSShipments = tables.ResultsFDNWiseTS.Sum(dtl => dtl.NumberOfShipments);
            var TotalTSCBM = tables.ImportJobDtlMulti.Sum(dtl => dtl.Cbm);

            //Import
            ViewData["TotalTSFDNsImport"] = TotalTSFDNsImport;
            ViewData["TotalTSShipments"] = TotalTSShipments;
            ViewData["TotalTSCBM"] = TotalTSCBM;

            //Local
            var BookingExportDetails = tables.BookingExpMulti;
            var resultsLocalFDN = BookingExportDetails
                .Where(detail => detail.FDNNavigation != null) // Filter out null FDNNavigation objects
                .GroupBy(detail => detail.FDNNavigation.PortName)
                .Select(group => new ResultsFDNWiseViewModel
                {
                    Fdn = group.Key,
                    TotalCBM = (decimal)group.Sum(detail => detail.Cbm),
                    TotalWeight = (decimal)group.Sum(detail => detail.GrossWeight),
                    NumberOfShipments = group.Count()
                })
                .OrderBy(t => t.Fdn)
                .ToList();

           
            tables.ResultsFDNWiseLocal = resultsLocalFDN;

            var TotalLocalFDNsExport = resultsLocalFDN.Count();
            var TotalLocalShipments = resultsLocalFDN.Sum(dtl => dtl.NumberOfShipments);
            var TotalLocalSCBM = resultsLocalFDN.Sum(dtl => dtl.TotalCBM);

            ViewData["TotalLocalFDNsExport"] = TotalLocalFDNsExport;
            ViewData["TotalLocalShipments"] = TotalLocalShipments;
            ViewData["TotalLocalSCBM"] = TotalLocalSCBM;
            var mergeResultFDNtotal = resultsLocalFDN.Concat(resultsImportFDN).ToList();

            tables.ResultsFDNWiseAll = mergeResultFDNtotal
                                .GroupBy(detail => detail.Fdn)
                .Select(group => new ResultsFDNWiseViewModel
                {
                    Fdn = group.Key,
                    TotalCBM = (decimal)group.Sum(detail => detail.TotalCBM),
                    TotalWeight = (decimal)group.Sum(detail => detail.TotalWeight),
                    NumberOfShipments = (int)group.Sum(detail => detail.NumberOfShipments)
                })
                .OrderBy(t => t.Fdn)
                .ToList();

            return View(tables);

            //var importJobDtl = (
            //                    from dtl in _context.TxnImportJobDtls
            //                    join hd in _context.TxnImportJobHds on dtl.JobNo equals hd.JobNo
            //                    where hd.JobDate >= fromDate && hd.JobDate <= toDate && dtl.Canceled != true && dtl.CargoType == "TS"
            //                    orderby dtl.RefNo descending
            //                    select dtl
            //                    //select new 
            //                    //{
            //                    //    dtl.RefNo,
            //                    //    dtl.JobNo,dtl.HouseBl,dtl.Freight,dtl.Tsblno,dtl.Cbm,dtl.Weight,dtl.NoofPackages,dtl.Tsdestination, dtl.ShippreName, dtl.ShipperCountry, dtl.ConsigneeName,
            //                    //    dtl.ConsigneeCountry, dtl.NotifyPartyName, dtl.NotifyPartyCountry, dtl.Blstatus, dtl.Bltype, dtl.PackageType, dtl.SalesPerson, dtl.Canceled, 
            //                    //    dtl.BlstatusNavigation,
            //                    //    dtl.PackageTypeImportJobNavigation,
            //                    //    dtl.SalesPersonImportJobdtlNavigation, dtl.ShipperImportJobDtlNavigation, dtl.TsblstatusNavigation, dtl.TsbltypeNavigation, dtl.TsdestinationNavigation,
            //                    //    hd.JobDate // Include the JobDate property from TxnImportJobHd
            //                    //}
            //                    ).Include(t => t.BlstatusNavigation).Include(t => t.Bltype).Include(t => t.PackageTypeImportJobNavigation).Include(t => t.SalesPersonImportJobdtlNavigation).Include(t => t.ShipperImportJobDtlNavigation).Include(t => t.TsblstatusNavigation).Include(t => t.TsbltypeNavigation).Include(t => t.TsdestinationNavigation)
            //                    .ToList();

            //const int pageSize = 20;
            //if (pg < 1)
            //    pg = 1;
            //int recsCount = importJobDtl.Count();
            //var pager = new Pager(recsCount, pg, pageSize);
            //int recSkip = (pg - 1) * pageSize;
            //var data = importJobDtl.Skip(recSkip).Take(pager.PageSize).ToList();

            //this.ViewBag.Pager = pager;







            //return View(await importJobDtl.ToListAsync());
        }

    }
}
