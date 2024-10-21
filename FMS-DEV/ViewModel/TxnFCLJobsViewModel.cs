using FMS_DEV.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FMS_DEV.ViewModel
{
    public class TxnFCLJobsViewModel
    {
        //public IEnumerable<TxnFCLJob> FCLJobs { get; set; }
        //public IEnumerable<TxnFCLJobContainers> FCLJobContainers { get; set; }

        public TxnFCLJob TxnFCLJob { get; set; } // The main job model for editing
        public IEnumerable<TxnFCLJob> FCLJobs { get; set; } // Collection of all jobs
        public IEnumerable<TxnFCLJobContainers> FCLJobContainers { get; set; } // Collection of job containers
        public SelectList ContainerSizes { get; set; }

    }
}
