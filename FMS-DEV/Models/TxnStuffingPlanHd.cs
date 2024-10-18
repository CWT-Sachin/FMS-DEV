using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Txn_StuffingPlan_HD")]
public partial class TxnStuffingPlanHd
{
    [Key]
    [Column("PlanID")]
    [StringLength(20)]
    public string PlanId { get; set; } = null!;

    [Column(TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime PlaneDate { get; set; }

    [StringLength(20)]
    public string? PreparedBy { get; set; }

    [StringLength(20)]
    public string? Vessel { get; set; }

    [StringLength(20)]
    public string? Voyage { get; set; }

    [Column("FDN")]
    [StringLength(20)]
    public string? Fdn { get; set; }

    [Column("POD")]
    [StringLength(20)]
    public string? Pod { get; set; }

    [StringLength(100)]
    public string? ContainerNo { get; set; }

    [StringLength(20)]
    public string? ContainerSize { get; set; }

    [Column(TypeName = "numeric(18, 4)")]
    public decimal? ContainerRate { get; set; }

    [StringLength(100)]
    public string? SealNo { get; set; }

    [Column(TypeName = "numeric(18, 4)")]
    public decimal? ProfitLost { get; set; }

    [Column("FCLCutoffDate", TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? FclcutoffDate { get; set; }

    [Column("FCLCutoffTime")]
    public TimeSpan? FclcutoffTime { get; set; }

    [Column("DepotID")]
    [StringLength(20)]
    public string? DepotId { get; set; }

    [StringLength(250)]
    public string? AttnName { get; set; }

    [Column("ShippingLineID")]
    [StringLength(20)]
    public string? ShippingLineId { get; set; }

    [Column("AgentID")]
    [StringLength(20)]
    public string? AgentId { get; set; }

    [Column("ETA_CBM", TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? EtaCbm { get; set; }

    [Column("TerminalID")]
    [StringLength(20)]
    public string? TerminalId { get; set; }

    [Column("VOC")]
    [StringLength(100)]
    public string? Voc { get; set; }

    [Column("COC")]
    [StringLength(100)]
    public string? Coc { get; set; }

    [StringLength(100)]
    public string? ShipLineBookingNo { get; set; }

    [StringLength(500)]
    public string? RemarksForDocs { get; set; }

    [StringLength(500)]
    public string? RemarksForStuffing { get; set; }

    [Column("WHCutOffDate", TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? WhcutOffDate { get; set; }

    [Column("WHCutOffTime")]
    public TimeSpan? WhcutOffTime { get; set; }

    [Column("TotalCBM", TypeName = "numeric(18, 4)")]
    public decimal? TotalCbm { get; set; }

    [Column(TypeName = "numeric(18, 4)")]
    public decimal? TotalWeght { get; set; }

    [StringLength(20)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDateTime { get; set; }

    [StringLength(20)]
    public string? LastUpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdatedDateTime { get; set; }

    public bool? Canceled { get; set; }

    [StringLength(20)]
    public string? CanceledBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CanceledDateTime { get; set; }

    [StringLength(500)]
    public string? CanceledReason { get; set; }

    [Column("Stack")]
    [StringLength(20)]
    public string? Stack { get; set; }
    public bool ConsoleConfirmed { get; set; }

    [StringLength(20)]
    public string? ConsoleConfirmedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ConsoleConfirmedDateTime { get; set; }

    public bool? JobCretaed { get; set; }

    [StringLength(20)]
    public string? JobCretaedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? JobCretaedDateTime { get; set; }

    [StringLength(20)]
    public string? JobNumber { get; set; }

    public bool? PrePLdone { get; set; }

    [StringLength(20)]
    public string? PrePLNumber { get; set; }
    public bool IsCoLoad { get; set; }

    [StringLength(20)]
    public string? Coloader { get; set; }

    [ForeignKey("AgentId")]
    [InverseProperty("TxnStuffingPlanHds")]
    public virtual RefAgent? AgentStuffingPlanHd { get; set; }

    [ForeignKey("DepotId")]
    [InverseProperty("TxnStuffingPlanHds")]
    public virtual RefDepot? DepotStuffingPlanHd { get; set; }

    [ForeignKey("Fdn")]
    [InverseProperty("TxnStuffingPlanHdFdnNavigations")]
    public virtual RefPort? FdnStuffingNavigation { get; set; }

    [ForeignKey("Pod")]
    [InverseProperty("TxnStuffingPlanHdPodNavigations")]
    public virtual RefPort? PodStuffingNavigation { get; set; }

    [ForeignKey("ShippingLineId")]
    [InverseProperty("TxnStuffingPlanHds")]
    public virtual RefShippingLine? ShippingLine { get; set; }

    [ForeignKey("Coloader")]
    [InverseProperty("TxnStuffingPlanHdsCoLoad")]
    public virtual RefShippingLine? ColoaderNavigation { get; set; }

    [ForeignKey("TerminalId")]
    [InverseProperty("TxnStuffingPlanHds")]
    public virtual RefTerminal? Terminal { get; set; }

    [ForeignKey("Vessel")]
    [InverseProperty("TxnStuffingPlanHds")]
    public virtual RefVessel? VesselStuffingNavigation { get; set; }

    
    [ForeignKey("ContainerSize")]
    [InverseProperty("TxnStuffingPlanHds")]
    public virtual RefContainerSize? ContainerStuffingNavigation { get; set; }

    [ForeignKey("PreparedBy")]
    [InverseProperty("TxnStuffingPlanHds")]
    public virtual RefStaff? PreparedByStuffingNavigation { get; set; }
}
