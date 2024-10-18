using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Ref_Ports")]
public partial class RefPort
{
    [Key]
    [StringLength(20)]
    public string PortCode { get; set; } = null!;

    [StringLength(300)]
    public string PortName { get; set; } = null!;

    [StringLength(250)]
    public string Country { get; set; } = null!;

    [StringLength(100)]
    public string? Custom { get; set; }

    public bool IsActive { get; set; }

    [StringLength(20)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDateTime { get; set; }

    [StringLength(1)]
    public string? LastUpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdatedDateTime { get; set; }

    [InverseProperty("Port")]
    public virtual ICollection<RefAgent> RefAgents { get; } = new List<RefAgent>();

    [InverseProperty("PODNavigation")]
    public virtual ICollection<TxnBookingExp> TxnBookingExpsPODNavigation { get; } = new List<TxnBookingExp>();
    
    [InverseProperty("POLNavigation")]
    public virtual ICollection<TxnBookingExp> TxnBookingExpsPOLNavigation { get; } = new List<TxnBookingExp>();

    [InverseProperty("FDNNavigation")]
    public virtual ICollection<TxnBookingExp> TxnBookingExpsFDNNavigation { get; } = new List<TxnBookingExp>();

    [InverseProperty("TSDestinationNavigation")]
    public virtual ICollection<TxnBookingExp> TxnBookingExpsTSDestinationNavigation { get; } = new List<TxnBookingExp>();

    [InverseProperty("TsPolNavigation")]
    public virtual ICollection<TxnBookingExp> TxnBookingExpsTsPolNavigation { get; } = new List<TxnBookingExp>();

  

    [InverseProperty("TsdestinationNavigation")]
    public virtual ICollection<TxnImportJobDtl> TxnImportJobDtls { get; } = new List<TxnImportJobDtl>();

    [InverseProperty("PodNavigation")]
    public virtual ICollection<TxnImportJobDtl> TxnImportJobDtlsPOD { get; } = new List<TxnImportJobDtl>();

    //[InverseProperty("DepartLastPortNavigation")]
    //public virtual ICollection<TxnImportJobHd> TxnImportJobHdDepartLastPortNavigations { get; } = new List<TxnImportJobHd>();

    [InverseProperty("LastPortNavigation")]
    public virtual ICollection<TxnImportJobHd> TxnImportJobHdLastPortNavigations { get; } = new List<TxnImportJobHd>();

    [InverseProperty("PolNavigation")]
    public virtual ICollection<TxnImportJobHd> TxnImportJobHdPolNavigations { get; } = new List<TxnImportJobHd>();

    [InverseProperty("FdnStuffingNavigation")]
    public virtual ICollection<TxnStuffingPlanHd> TxnStuffingPlanHdFdnNavigations { get; } = new List<TxnStuffingPlanHd>();

    [InverseProperty("PodStuffingNavigation")]
    public virtual ICollection<TxnStuffingPlanHd> TxnStuffingPlanHdPodNavigations { get; } = new List<TxnStuffingPlanHd>();

    [InverseProperty("PODExportJobNavigation")]
    public virtual ICollection<TxnExportJobHD> TxnExportJobHdsPod { get; } = new List<TxnExportJobHD>();

    [InverseProperty("FDNExportJobNavigation")]
    public virtual ICollection<TxnExportJobHD> TxnExportJobHds { get; } = new List<TxnExportJobHD>();


}
