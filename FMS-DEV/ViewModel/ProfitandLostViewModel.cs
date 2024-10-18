using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{
    public class ProfitandLostViewModel
    {
        public IEnumerable<TxnPaymentVoucherExportHd> PayVoucherHdMulti { get; set; }
        public IEnumerable<TxnPaymentVoucherExportDtl> PayVoucherDtlMulti { get; set; }

        public IEnumerable<TxnInvoiceExportHd> InvoiceHdMulti { get; set; }
        public IEnumerable<TxnInvoiceExportDtl> InvoiceDtMulti { get; set; }

        public IEnumerable<TxnDebitNoteExportHd> DebitNoteHdMulti { get; set; }
        public IEnumerable<TxnDebitNoteExportDtl> DebitNoteDtMulti { get; set; }

        public IEnumerable<TxnCreditNoteFCLHd> CreditNoteHdMulti { get; set; }
        public IEnumerable<TxnCreditNoteFCLDtl> CreditNoteDtMulti { get; set; }

        public IEnumerable<TxnExportJobHD> ExportJobHdMulti { get; set; }
        public IEnumerable<TxnExportJobDtl> ExportJobDtlMulti { get; set; }

    }
}
