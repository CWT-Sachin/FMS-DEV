using FMS_DEV.Data;
using FMS_DEV.DataAccounts;
using FMS_DEV.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.CommonMethods
{
    public class CommonMethodAccClass
    {
        private readonly FtlcolombOperationContext _context;
     

        //public CommonMethodAccClass(FtlcolombOperationContext context)
        //{
        //    this.context = context;
        //}

        public CommonMethodAccClass(FtlcolombOperationContext context)
        {
            _context = context;
           
        }
        public string GetChargeItemAccNo(string chargeItemNo, string mode, string Type, string Vender)
        {
            var chargeItem =  _context.RefChargeItemsAcc.FirstOrDefault(x=>x.ChargeId == chargeItemNo);
            var AccNumber = "";
            

            if (mode == "Import")
            {
                if (Type == "Revenue")
                {
                    AccNumber = chargeItem?.AccNo_Revenue_Imp;
                }
                else // Expenses
                {
                    if (Vender == "Liner")
                    {
                        AccNumber = chargeItem?.AccNo_Expense_Imp_Liner;
                    }
                    else // Vender == "Agent"
                    {
                        AccNumber = chargeItem?.AccNo_Expense_Imp_Agent;
                    }
                }
            }
            else // Export
            {
                if (Type == "Revenue")
                {
                    AccNumber = chargeItem?.AccNo_Revenue_Exp;
                }
                else // Type == Expenses
                {
                    if (Vender == "Liner")
                    {
                        AccNumber = chargeItem?.AccNo_Expense_Exp_Liner;
                    }
                    else // Vender == "Agent"
                    {
                        AccNumber = chargeItem?.AccNo_Expense_Exp_Agent;
                    }
                }
            }

            return AccNumber;
        }

        public string insertTransaction()
        {
            var succsess = "False";
            return (succsess);
        }
    }
}
