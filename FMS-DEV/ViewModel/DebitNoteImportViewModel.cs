using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{
    public class DebitNoteImportViewModel 
    {
        public IEnumerable<TxnDebitNoteImportHd> DebitNoteHdMulti { get; set; }
        public IEnumerable<TxnDebitNoteImportDtl> DebitNoteDtlMulti { get; set; }

        public IEnumerable<TxnImportJobHd> ImportJobHdMulti { get; set; }
        public IEnumerable<TxnImportJobDtl> ImportJobDtlMulti { get; set; }

    }
}
