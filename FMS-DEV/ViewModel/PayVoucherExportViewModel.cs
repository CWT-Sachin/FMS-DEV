using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{ 
    public class PayVoucherExportViewModel
    {
        public IEnumerable<TxnPaymentVoucherExportHd> PayVoucherHdMulti { get; set; }
        public IEnumerable<TxnPaymentVoucherExportDtl> PayVoucherDtlMulti { get; set; }

        public IEnumerable<TxnExportJobHD> ExportJobHdMulti { get; set; }
        public IEnumerable<TxnExportJobDtl> ExportJobDtlMulti { get; set; }

    }
}
