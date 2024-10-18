namespace FMS_DEV.Models
{
    public class CombinedTxnImportJobDtlAndTxnImportJobCntsModel
    {
        public TxnImportJobDtl ImportDtl { get; set; }
        public string CntNo { get; set; }
        public string CntSeal { get; set;}
    }
}
