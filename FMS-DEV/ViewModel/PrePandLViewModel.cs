using FMS_DEV.Models;
using System.Collections.Generic;

namespace FMS_DEV.ViewModel
{
    public class PrePandLViewModel
    {
        public PrePandLViewModel()
        {
            TxnPrePandLOtherExpenDtlMulti = new List<TxnPrePandLOtherExpenDtl>();
            TxnPrePandLOtherIncomDtlMulti = new List<TxnPrePandLOtherIncomDtl>();
            txnPrePandLHDMulti = new List<TxnPrePandLHD>();

        }

        public List<TxnPrePandLOtherExpenDtl> TxnPrePandLOtherExpenDtlMulti { get; set; }
        public List<TxnPrePandLOtherIncomDtl> TxnPrePandLOtherIncomDtlMulti { get; set; }
        public List<TxnPrePandLHD> txnPrePandLHDMulti { get; set; }

        public List<TxnStuffingPlanHd> txnStuffingPlanHdMulti { get; set; }

        //public List<TxnStuffingPlanDtl> TxnStuffingPlanDtlMulti { get; set; }

        public List<TxnStuffingPlanDtl> TxnStuffingPlanDtlMulti { get; set; } = new List<TxnStuffingPlanDtl>();

        public string ExpensesJson { get; set; }  // Add this property
        public string IncomesJson { get; set; }
    }
}



















//using FMS_DEV.Models;
//using System.Collections.Generic;

//namespace FMS_DEV.ViewModel
//{
//    public class PrePandLViewModel
//    {
//        public PrePandLViewModel()
//        {
//            TxnPrePandLOtherExpenDtlMulti = new List<TxnPrePandLOtherExpenDtl>();
//            TxnPrePandLOtherIncomDtlMulti = new List<TxnPrePandLOtherIncomDtl>();
//            txnPrePandLHDMulti = new List<TxnPrePandLHD>();
//        }

//        public IEnumerable<TxnPrePandLOtherExpenDtl> TxnPrePandLOtherExpenDtlMulti { get; set; }
//        public IEnumerable<TxnPrePandLOtherIncomDtl> TxnPrePandLOtherIncomDtlMulti { get; set; }
//        public IEnumerable<TxnPrePandLHD> txnPrePandLHDMulti { get; set; }
//    }
//}












//using FMS_DEV.Models;
//using Microsoft.EntityFrameworkCore;
//namespace FMS_DEV.ViewModel 
//{
//    public class PrePandLViewModel
//    {
//        public IEnumerable<TxnPrePandLOtherExpenDtl> TxnPrePandLOtherExpenDtlMulti { get; set; }
//        public IEnumerable<TxnPrePandLOtherIncomDtl> TxnPrePandLOtherIncomDtlMulti { get; set; }

//        public IEnumerable<TxnPrePandLHD> txnPrePandLHDMulti { get; set; }



//    }
//}
