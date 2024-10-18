using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Txn_ImportJob_HD")]
public partial class TxnImportJobHd
{
    [Key]
    [StringLength(20)]
    public string JobNo { get; set; } = null!;

    [Column(TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? JobDate { get; set; }

    [StringLength(20)]
    public string? Handleby { get; set; }

    [StringLength(20)]
    public string? Vessel { get; set; }

    [StringLength(20)]
    public string? Voyage { get; set; }

    [StringLength(20)]
    public string? Terminal { get; set; }

    [Column("ETA", TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
    public DateTime? Eta { get; set; }

    [Column("DepartLastPort", TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
    public DateTime? DepartLastPort { get; set; }

    [StringLength(20)]
    public string? LastPort { get; set; }

    [Column("CustomerROE", TypeName = "numeric(18, 4)")]
    public decimal? CustomerRoe { get; set; }

    [Column("LineROE", TypeName = "numeric(18, 4)")]
    public decimal? LineRoe { get; set; }

    [Column("AgentROE", TypeName = "numeric(18, 4)")]
    public decimal? AgentRoe { get; set; }

    [Column("DOSerial")]
    [StringLength(100)]
    public string? Doserial { get; set; }

    [Column("POL")]
    [StringLength(20)]
    public string? Pol { get; set; }

    [StringLength(20)]
    public string? Agent { get; set; }

    [StringLength(20)]
    public string? ShippingLine { get; set; }

    [Column("MasterBLNo")]
    [StringLength(100)]
    public string? MasterBlno { get; set; }

    [StringLength(50)]
    public string? Type { get; set; }

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

    [Column("ExchangeRate", TypeName = "numeric(10, 2)")]
    public decimal? ExchangeRate { get; set; }


    [ForeignKey("Agent")]
    [InverseProperty("TxnImportJobHds")]
    public virtual RefAgent? AgentNavigation { get; set; }

    //[ForeignKey("DepartLastPort")]
    //[InverseProperty("TxnImportJobHdDepartLastPortNavigations")]
    //public virtual RefPort? DepartLastPortNavigation { get; set; }

    [ForeignKey("Handleby")]
    [InverseProperty("TxnImportJobHds")]
    public virtual RefStaff? HandlebyImportJobNavigation { get; set; }

    [ForeignKey("LastPort")]
    [InverseProperty("TxnImportJobHdLastPortNavigations")]
    public virtual RefPort? LastPortNavigation { get; set; }

    [ForeignKey("Pol")]
    [InverseProperty("TxnImportJobHdPolNavigations")]
    public virtual RefPort? PolNavigation { get; set; }

    [ForeignKey("ShippingLine")]
    [InverseProperty("TxnImportJobHds")]
    public virtual RefShippingLine? ShippingLineNavigation { get; set; }

    [ForeignKey("Terminal")]
    [InverseProperty("TxnImportJobHds")]
    public virtual RefTerminal? TerminalNavigation { get; set; }

    [ForeignKey("Vessel")]
    [InverseProperty("TxnImportJobHds")]
    public virtual RefVessel? VesselImportJobDtlNavigation { get; set; }

}
