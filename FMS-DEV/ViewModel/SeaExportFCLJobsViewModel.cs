using FMS_DEV.Models;

namespace FMS_DEV.ViewModel
{
    public class SeaExportFCLJobsViewModel
    {
        public IEnumerable<TxnFCLJob> FCLJobs { get; set; }
        public IEnumerable<TxnFCLJobContainers> FCLJobContainers { get; set; }

        //public IEnumerable<TxnExportJobHD> ExportJobHdMulti { get; set; }
        //public IEnumerable<TxnExportJobDtl> ExportJobDtlMulti { get; set; }
    }
}
