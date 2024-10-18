using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{ 
    public class PayVoucherImportViewModel
    {
        public IEnumerable<TxnPaymentVoucherImportHd> PayVoucherHdMulti { get; set; }
        public IEnumerable<TxnPaymentVoucherImportDtl> PayVoucherDtlMulti { get; set; }

        public IEnumerable<TxnImportJobHd> ImportJobHdMulti { get; set; }
        public IEnumerable<TxnImportJobDtl> ImportJobDtlMulti { get; set; }

    }
}
