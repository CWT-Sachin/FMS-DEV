using Microsoft.AspNetCore.Mvc;
using System.Linq;
using FMS_DEV.Models; // Add the appropriate namespace for your model class and DbContext.
using FMS_DEV.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FMS_DEV.Data;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace FMS_DEV.Controllers
{
    public class TxnPrePandLImportHDsController : Controller
    {
        private readonly FtlcolombOperationContext _context;

        public TxnPrePandLImportHDsController(FtlcolombOperationContext context)
        {
            _context = context;
        }


        // GET: PnL
        public async Task<IActionResult> Index()
        {
            return View(await _context.TxnPrePandLHds.ToListAsync());
        }


        public IActionResult Create(string searchString)
        {
            var planNo = "xxx";      

            if (!string.IsNullOrEmpty(searchString))
            {
                planNo = searchString;
                if (_context.TxnStuffingPlanHds.Any(t => t.PlanId == planNo))
                {
                    ViewData["PlanFound"] = "";
                }
                else
                {
                    ViewData["PlanFound"] = "Plan Number: " + searchString + " not found";
                }
            }

            var tables = new PrePandLViewModel
            {
                TxnPrePandLOtherExpenDtlMulti = new List<TxnPrePandLOtherExpenDtl>(),
                TxnPrePandLOtherIncomDtlMulti = new List<TxnPrePandLOtherIncomDtl>(),
                txnPrePandLHDMulti = new List<TxnPrePandLHD>(),
                TxnStuffingPlanDtlMulti = _context.TxnStuffingPlanDtls
                .Where(t => t.PlanId == planNo)
                .ToList(),
                txnStuffingPlanHdMulti = _context.TxnStuffingPlanHds
                .Where(t => t.PlanId == planNo)
                .ToList()
            };

            return View(tables);
        }





        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(string dtlItemsList, [Bind("PrePnLNo,Date,StufPlnNo,VesselID,PODId,ShippingLineID,Voyage,FDNId,AgentId,ContainerSizeId,CBM,KG,LocalNoOfBL,TSNoOfBL,TotCBM,BoxRateUSD,EXPCostACEStuffing,EXPCostRework,EXPCostPORTStuffing,EXPCostSLPA,EXPCostTSImpCost,EXPCostMCC,EXPCostAMJ,EXPCostTotalExportCostColombo,INCFstTSRateRcvdFromPOLTS,INCFstLocalRateRcvdLocalShipper,INCFstFreightCollectAmtNominations,INCFstTSRateRcvdFromPOLTSBL,EXPHndDestuffingCost,EXPHndLinearCost,EXPHndHandlingFee,INCHndDestinationChargesCollectionBL,EXPRbtTSCost,EXPRbtDoorCost,EXPRbtDestinationCharges,EXPRbtFirstLegCost,EXPRbtTotalRebateSystemCost,EXPTruckingOnCarriageCost,EXPOtherExpensesTotal,INCRbtRemateForCBM,INCRbtRemateForBL,INCRbtLess,INCRbtDiffOfDestinationCharges,INCRbtISF,INCRbtLSS,INCRbtPSS,INCOtherIncomeTotal,TotalExpenses,TotalIncome,PandL")] TxnPrePandLHD txnPrePandLHD, PrePandLViewModel viewModel)
        //{
        //    // Handle ID generation
        //    var TableID = "Txn_PrePandLHD";
        //    var refLastNumber = await _context.RefLastNumbers.FindAsync(TableID);
        //    if (refLastNumber != null)
        //    {
        //        var nextNumber = refLastNumber.LastNumber + 1;
        //        refLastNumber.LastNumber = nextNumber;
        //        var IDNumber = "P" + nextNumber.ToString().PadLeft(4, '0');
        //        txnPrePandLHD.PrePnLNo = IDNumber;

        //        _context.Update(refLastNumber);
        //        await _context.SaveChangesAsync();
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("RefLastNumber", "Reference number not found.");
        //        Console.WriteLine("Reference number not found.");
        //        return View(viewModel);
        //    }

        //    // Remove certain properties from ModelState validation
        //    ModelState.Remove("PrePnLNo");
        //    ModelState.Remove("StufPlnNo");
        //    ModelState.Remove("VesselID");
        //    ModelState.Remove("PODId");
        //    ModelState.Remove("ShippingLineID");
        //    ModelState.Remove("Voyage");
        //    ModelState.Remove("FDNId");
        //    ModelState.Remove("AgentId");
        //    ModelState.Remove("ContainerSizeId");
        //    ModelState.Remove("CBM");
        //    ModelState.Remove("KG");
        //    ModelState.Remove("LocalNoOfBL");
        //    ModelState.Remove("TSNoOfBL");
        //    ModelState.Remove("TotCBM");
        //    ModelState.Remove("BoxRateUSD");
        //    ModelState.Remove("dtlItemsList");

        //    // Validate the model state and save the main record if valid
        //    if (ModelState.IsValid)
        //    {
        //        // Save the main record
        //        _context.Add(txnPrePandLHD);
        //        await _context.SaveChangesAsync();

        //        // Save the Other Expenses
        //        if (viewModel.TxnPrePandLOtherExpenDtlMulti != null && viewModel.TxnPrePandLOtherExpenDtlMulti.Any())
        //        {
        //            foreach (var expense in viewModel.TxnPrePandLOtherExpenDtlMulti)
        //            {
        //                expense.PrePnLNo = txnPrePandLHD.PrePnLNo;
        //                _context.TxnPrePandLOtherExpenDtls.Add(expense);
        //            }
        //        }

        //        // Save the Other Incomes
        //        if (viewModel.TxnPrePandLOtherIncomDtlMulti != null && viewModel.TxnPrePandLOtherIncomDtlMulti.Any())
        //        {
        //            foreach (var income in viewModel.TxnPrePandLOtherIncomDtlMulti)
        //            {
        //                income.PrePnLNo = txnPrePandLHD.PrePnLNo;
        //                _context.TxnPrePandLOtherIncomDtls.Add(income);
        //            }
        //        }

        //        // Save all changes
        //        await _context.SaveChangesAsync();

        //        Console.WriteLine("Data saved successfully.");
        //        return RedirectToAction(nameof(Index));
        //    }
        //    else
        //    {
        //        Console.WriteLine("Model state is invalid.");
        //    }

        //    // If model state is not valid, return the same view with the ViewModel
        //    return View(viewModel);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("PrePnLNo,Date,StufPlnNo,VesselID,PODId,ShippingLineID,Voyage,FDNId,AgentId,ContainerSizeId,CBM,KG,LocalNoOfBL,TSNoOfBL,TotCBM,BoxRateUSD,EXPCostACEStuffing,EXPCostRework,EXPCostPORTStuffing,EXPCostSLPA,EXPCostTSImpCost,EXPCostMCC,EXPCostAMJ,EXPCostTotalExportCostColombo,INCFstTSRateRcvdFromPOLTS,INCFstLocalRateRcvdLocalShipper,INCFstFreightCollectAmtNominations,INCFstTSRateRcvdFromPOLTSBL,EXPHndDestuffingCost,EXPHndLinearCost,EXPHndHandlingFee,INCHndDestinationChargesCollectionBL,EXPRbtTSCost,EXPRbtDoorCost,EXPRbtDestinationCharges,EXPRbtFirstLegCost,EXPRbtTotalRebateSystemCost,EXPTruckingOnCarriageCost,EXPOtherExpensesTotal,INCRbtRemateForCBM,INCRbtRemateForBL,INCRbtLess,INCRbtDiffOfDestinationCharges,INCRbtISF,INCRbtLSS,INCRbtPSS,INCOtherIncomeTotal,TotalExpenses,TotalIncome,PandL")] TxnPrePandLHD txnPrePandLHD,
            [Bind("TxnPrePandLOtherExpenDtlMulti,TxnPrePandLOtherIncomDtlMulti,ExpensesJson,IncomesJson")] PrePandLViewModel viewModel)
        {
            // Generate PrePnLNo
            var TableID = "Txn_PrePandLHD";
            var refLastNumber = await _context.RefLastNumbers.FindAsync(TableID);
            if (refLastNumber != null)
            {
                var nextNumber = refLastNumber.LastNumber + 1;
                refLastNumber.LastNumber = nextNumber;
                var IDNumber = "P" + nextNumber.ToString().PadLeft(4, '0');
                txnPrePandLHD.PrePnLNo = IDNumber;

                _context.Update(refLastNumber);
                await _context.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError("", "Error while generating ID.");
                return View("Create", viewModel); // Return the view with the viewModel
            }

            // Remove certain properties from ModelState validation
            ModelState.Remove("PrePnLNo");
            ModelState.Remove("StufPlnNo");
            ModelState.Remove("VesselID");
            ModelState.Remove("PODId");
            ModelState.Remove("ShippingLineID");
            ModelState.Remove("Voyage");
            ModelState.Remove("FDNId");
            ModelState.Remove("AgentId");
            ModelState.Remove("ContainerSizeId");
            ModelState.Remove("CBM");
            ModelState.Remove("KG");
            ModelState.Remove("LocalNoOfBL");
            ModelState.Remove("TSNoOfBL");
            ModelState.Remove("TotCBM");
            ModelState.Remove("BoxRateUSD");

            if (ModelState.IsValid)
            {
                _context.TxnPrePandLHds.Add(txnPrePandLHD);
                await _context.SaveChangesAsync();

                // Save Other Expenses
                if (!string.IsNullOrEmpty(viewModel.ExpensesJson))
                {
                    var expenses = JsonConvert.DeserializeObject<List<TxnPrePandLOtherExpenDtl>>(viewModel.ExpensesJson);
                    if (expenses != null)
                    {
                        foreach (var expense in expenses)
                        {
                            expense.PrePnLNo = txnPrePandLHD.PrePnLNo;
                            expense.SNo = (expenses.IndexOf(expense) + 1).ToString();
                            _context.TxnPrePandLOtherExpenDtl.Add(expense);
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                // Save Other Incomes
                if (!string.IsNullOrEmpty(viewModel.IncomesJson))
                {
                    var incomes = JsonConvert.DeserializeObject<List<TxnPrePandLOtherIncomDtl>>(viewModel.IncomesJson);
                    if (incomes != null)
                    {
                        foreach (var income in incomes)
                        {
                            income.PrePnLNo = txnPrePandLHD.PrePnLNo;
                            income.SNo = (incomes.IndexOf(income) + 1).ToString();
                            _context.TxnPrePandLOtherIncomDtl.Add(income);
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // In case of invalid model state, return the view with the viewModel
            return View("Create", viewModel);
        }

    }
}
                             