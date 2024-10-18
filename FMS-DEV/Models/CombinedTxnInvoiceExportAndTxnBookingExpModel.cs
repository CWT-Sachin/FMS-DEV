namespace FMS_DEV.Models
{
    public class CombinedTxnInvoiceExportAndTxnBookingExpModel
    {
        public TxnInvoiceExportHd Invoice { get; set; }
        public string BKBlNo { get; set; }
        public string HouseBl { get; set; } // Add this property

    }
}
