using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{
    public class SeaExportFCLBLViewModel
    {
        public IEnumerable<TxnFCLJob> FCLJobsMulti { get; set; }
        public IEnumerable<TxnFCLJobContainers> FCLJobContainersMulti { get; set; }

        public IEnumerable<TxnFCLBL> FCLBLsMulti { get; set; }
    }
}


