using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{
    public class InvoiceExportViewModel
    {
        public IEnumerable<TxnInvoiceExportHd> InvoiceHdMulti { get; set; }
        public IEnumerable<TxnInvoiceExportDtl> InvoiceDtMulti { get; set; }

        public IEnumerable<TxnImportJobHd> ImportJobHdMulti { get; set; }
        public IEnumerable<TxnImportJobDtl> ImportJobDtlMulti { get; set; }

        public IEnumerable<TxnExportJobHD> ExportJobHdMulti { get; set; }
        public IEnumerable<TxnExportJobDtl> ExportJobDtlMulti { get; set; }
        public string ContainerNo { get; set; }

    }

}
