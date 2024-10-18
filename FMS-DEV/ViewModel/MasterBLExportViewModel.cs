using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{
    public class MasterBLExportViewModel
    {
        public IEnumerable<TxnInvoiceExportHd> InvoiceHdMulti { get; set; }
        public IEnumerable<TxnInvoiceExportDtl> InvoiceDtMulti { get; set; }

        public IEnumerable<TxnImportJobHd> ImportJobHdMulti { get; set; }
        public IEnumerable<TxnImportJobDtl> ImportJobDtlMulti { get; set; }

        public IEnumerable<TxnExportJobHD> ExportJobHdMulti { get; set; }
        public IEnumerable<TxnExportJobDtl> ExportJobDtlMulti { get; set; }
            

        public IEnumerable<TxnExportMasterBLHD> ExportMasterBLHdMulti { get; set; }
        public IEnumerable<TxnExportMasterBLDtl> ExportMasterBLDtlMulti { get; set; }

        public IEnumerable<TxnBookingExp> BookingExportMulti { get; set; }

        public string ContainerNo { get; set; }

    }

}
