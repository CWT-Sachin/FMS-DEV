using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Ref_Vessel")]
public partial class RefVessel
{
    [Key]
    [Column("VesselID")]
    [StringLength(20)]
    public string VesselId { get; set; } = null!;

    [StringLength(300)]
    public string VesselName { get; set; } = null!;

    [StringLength(50)]
    public string? VesselCode { get; set; }

    [StringLength(150)]
    public string? Registration { get; set; }

    [Column("IMONumber")]
    [StringLength(50)]
    public string? Imonumber { get; set; }

    [StringLength(25)]
    public string? IsoCountryCode { get; set; }

    public bool IsActive { get; set; }

    [StringLength(20)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDateTime { get; set; }

    [StringLength(1)]
    public string? LastUpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdatedDateTime { get; set; }

    [InverseProperty("VesselNavigation")]
    public virtual ICollection<TxnBookingExp> TxnBookingExps { get; } = new List<TxnBookingExp>();

    [InverseProperty("TSVesselNavigation")]
    public virtual ICollection<TxnBookingExp> TxnBookingExpsTsVessel { get; } = new List<TxnBookingExp>();

    [InverseProperty("VesselStuffingNavigation")]
    public virtual ICollection<TxnStuffingPlanHd> TxnStuffingPlanHds { get; } = new List<TxnStuffingPlanHd>();


    [InverseProperty("VesselImportJobDtlNavigation")]
    public virtual ICollection<TxnImportJobHd> TxnImportJobHds { get; } = new List<TxnImportJobHd>();

    [InverseProperty("VesselExportJobDtlNavigation")]
    public virtual ICollection<TxnExportJobHD> TxnExportJobHds { get; } = new List<TxnExportJobHD>();

    [InverseProperty("IntendedVesselNavigation")]
    public virtual ICollection<TxnImportJobDtl> TxnImportJobDtls { get; } = new List<TxnImportJobDtl>();




}
