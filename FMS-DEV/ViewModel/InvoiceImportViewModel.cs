using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{
    public class InvoiceImportViewModel
    {
        public IEnumerable<TxnInvoiceImportHd> InvoiceHdMulti { get; set; }
        public IEnumerable<TxnInvoiceImportDtl> InvoiceDtMulti { get; set; }

        public IEnumerable<TxnImportJobHd> ImportJobHdMulti { get; set; }
        public IEnumerable<TxnImportJobDtl> ImportJobDtlMulti { get; set; }

        public IEnumerable<TxnExportJobHD> ExportJobHdMulti { get; set; }
        public IEnumerable<TxnExportJobDtl> ExportJobDtlMulti { get; set; }
        public string ContainerNo { get; set; }

    }

}
