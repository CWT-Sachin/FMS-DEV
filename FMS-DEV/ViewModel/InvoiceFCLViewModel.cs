using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{
    public class InvoiceFCLViewModel
    {
        public IEnumerable<TxnInvoiceFCLHd> InvoiceFCLHdMulti { get; set; }
        public IEnumerable<TxnInvoiceFCLDtl> InvoiceFCLDtlMulti { get; set; }

        public IEnumerable<TxnFCLJob> FCLJobMulti { get; set; }
        public string ContainerNo { get; set; }

    }

}
