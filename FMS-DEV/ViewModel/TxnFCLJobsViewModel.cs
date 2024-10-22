using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{
    public class TxnFCLJobsViewModel
    {
        public IEnumerable<TxnFCLJob> FCLJobs { get; set; }
        public IEnumerable<TxnFCLJobContainers> FCLJobContainers { get; set; }

    }
}
