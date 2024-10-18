using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{
    public class DebitNoteExportViewModel 
    {
        public IEnumerable<TxnDebitNoteExportHd> DebitNoteHdMulti { get; set; }
        public IEnumerable<TxnDebitNoteExportDtl> DebitNoteDtlMulti { get; set; }

        public IEnumerable<TxnExportJobHD> ExportJobHdMulti { get; set; }
        public IEnumerable<TxnExportJobDtl> ExportJobDtlMulti { get; set; }

    }
}
