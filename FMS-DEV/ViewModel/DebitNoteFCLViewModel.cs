using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{
    public class DebitNoteFCLViewModel
    {
        public IEnumerable<TxnDebitNoteFCLHd> DebitNoteFCLHdMulti { get; set; }
        public IEnumerable<TxnDebitNoteFCLDtl> DebitNoteFCLDtlMulti { get; set; }

        public IEnumerable<TxnFCLJob> FCLJobMulti { get; set; }

        public IEnumerable<TxnExportJobHD> ExportJobHdMulti { get; set; }
        public IEnumerable<TxnExportJobDtl> ExportJobDtlMulti { get; set; }

    }

}
