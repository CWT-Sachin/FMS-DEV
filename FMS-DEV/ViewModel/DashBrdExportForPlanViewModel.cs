using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{
    public class DashBrdExportForPlanViewModel
    {
        public IEnumerable<TxnBookingExp> BookingExpMulti { get; set; }
        public IEnumerable<TxnImportJobDtl> ImportJobDtlMulti { get; set; }
        public List<ResultsFDNWiseViewModel> ResultsFDNWiseTS { get; set; }
        public List<ResultsFDNWiseViewModel> ResultsFDNWiseLocal { get; set; }
        public List<ResultsFDNWiseViewModel> ResultsFDNWiseAll { get; set; }

    }

    public class ResultsFDNWiseViewModel
    {
        public string Fdn { get; set; }
        public decimal TotalCBM { get; set; }
        public decimal TotalWeight { get; set; }
        public int NumberOfShipments { get; set; }
    }
}
