using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Txn_ExportJobHD")]
public partial class TxnExportJobHD
{
    [Key]
    [StringLength(20)]
    public string JobNo { get; set; } = null!;

    [Column(TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? JobDate { get; set; }

    [StringLength(20)]
    public string? HandledBy { get; set; }

    [StringLength(20)]
    public string? Vessel { get; set; }

    [StringLength(20)]
    public string? Voyage { get; set; }

    [StringLength(20)]
    public string? POD { get; set; }

    [StringLength(20)]
    public string? FDN { get; set; }

    [StringLength(20)]
    public string? Agent { get; set; }

    [StringLength(20)]
    public string? ShippinLine { get; set; }

    [StringLength(450)]
    public string? Remarks { get; set; }

    [StringLength(20)]
    public string? JobStatus { get; set; }

    public bool? Canceled { get; set; }

    [StringLength(20)]
    public string? CanceledBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CanceledDateTime { get; set; }

    [StringLength(350)]
    public string? CanceledReason { get; set; }

    [StringLength(20)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDateTime { get; set; }

    [StringLength(20)]
    public string? LastUpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdatedDateTime { get; set; }

    public bool? IsCoLoad { get; set; }

    [StringLength(20)]
    public string? Coloader { get; set; }


    [ForeignKey("Agent")]
    [InverseProperty("TxnExportJobHds")]
    public virtual RefAgent? AgentExportNavigation { get; set; }

    [ForeignKey("HandledBy")]
    [InverseProperty("TxnExportJobHds")]
    public virtual RefStaff? HandlebyExportJobNavigation { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("TxnExportJobHdsCreatedBy")]
    public virtual RefStaff? CreatedByExportJobNavigation { get; set; }

    [ForeignKey("ShippinLine")]
    [InverseProperty("TxnExportJobHds")]
    public virtual RefShippingLine? ShippingLineExportNavigation { get; set; }

    [ForeignKey("Coloader")]
    [InverseProperty("TxnExportJobHdsColoader")]
    public virtual RefShippingLine? ColoaderExportNavigation { get; set; }

    [ForeignKey("Vessel")]
    [InverseProperty("TxnExportJobHds")]
    public virtual RefVessel? VesselExportJobDtlNavigation { get; set; }

    [ForeignKey("POD")]
    [InverseProperty("TxnExportJobHdsPod")]
    public virtual RefPort? PODExportJobNavigation { get; set; }

    [ForeignKey("FDN")]
    [InverseProperty("TxnExportJobHds")]
    public virtual RefPort? FDNExportJobNavigation { get; set; }

    [ForeignKey("JobStatus")]
    [InverseProperty("TxnExportJobHds")]
    public virtual RefStatusMain? JobStatusExportJobNavigation { get; set; }



}
