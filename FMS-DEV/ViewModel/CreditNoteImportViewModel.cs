using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{
    public class CreditNoteImportViewModel
    {
        public IEnumerable<TxnCreditNoteImportHd> CreditNoteHdMulti { get; set; }
        public IEnumerable<TxnCreditNoteImportDtl> CreditNoteDtlMulti { get; set; }

        public IEnumerable<TxnImportJobHd> ImportJobHdMulti { get; set; }
        public IEnumerable<TxnImportJobDtl> ImportJobDtlMulti { get; set; }

        public IEnumerable<TxnExportJobHD> ExportJobHdMulti { get; set; }
        public IEnumerable<TxnExportJobDtl> ExportJobDtlMulti { get; set; }

    }

}
