using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{
    public class PayVoucherFCLViewModel
    {
        public IEnumerable<TxnPaymentVoucherFCLHd> PayVoucherHdMulti { get; set; }
        public IEnumerable<TxnPaymentVoucherFCLDtl> PayVoucherDtlMulti { get; set; }

        public IEnumerable<TxnFCLJob> FCLJobMulti { get; set; }


    }
}
