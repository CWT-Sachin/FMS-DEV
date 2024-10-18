using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[PrimaryKey("AgentId")]
[Table("Ref_Agent")]
public partial class RefAgent
{
    [Key]
    [Column("AgentID")]
    [StringLength(20)]
    public string AgentId { get; set; } = null!;

    [Key]
    [Column("PortID")]
    [StringLength(20)]
    public string PortId { get; set; } = null!;

    [StringLength(300)]
    public string AgentName { get; set; } = null!;

    [StringLength(250)]
    public string? Address1 { get; set; }

    [StringLength(250)]
    public string? Address2 { get; set; }

    [StringLength(250)]
    public string? Address3 { get; set; }

    [StringLength(250)]
    public string? City { get; set; }

    [StringLength(200)]
    public string? County { get; set; }

    [StringLength(15)]
    public string? TelNo { get; set; }

    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(350)]
    public string? Remarks { get; set; }

    public bool IsActive { get; set; }

    [StringLength(20)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDateTime { get; set; }

    [StringLength(20)]
    public string? LastUpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdatedDateTime { get; set; }

    [ForeignKey("PortId")]
    [InverseProperty("RefAgents")]
    public virtual RefPort Port { get; set; } = null!;

    [InverseProperty("AgentIDNominationNavigation")]
    public virtual ICollection<TxnBookingExp> TxnBookingExpsAgentIDNomination { get; } = new List<TxnBookingExp>();

    [InverseProperty("AgentNavigation")]
    public virtual ICollection<TxnImportJobHd> TxnImportJobHds { get; } = new List<TxnImportJobHd>();

    [InverseProperty("AgentStuffingPlanHd")]
    public virtual ICollection<TxnStuffingPlanHd> TxnStuffingPlanHds { get; } = new List<TxnStuffingPlanHd>();

    [InverseProperty("AgentExportNavigation")]
    public virtual ICollection<TxnExportJobHD> TxnExportJobHds { get; } = new List<TxnExportJobHD>();

    [InverseProperty("AgentDebitNoteExportNavigation")]
    public virtual ICollection<TxnDebitNoteExportHd> TxnDebitNoteExportHd { get; } = new List<TxnDebitNoteExportHd>();

    [InverseProperty("AgentDebitNoteImportNavigation")]
    public virtual ICollection<TxnDebitNoteImportHd> TxnDebitNoteImportHd { get; } = new List<TxnDebitNoteImportHd>();

    //[InverseProperty("AgentCreditNoteExportNavigation")]
    //public virtual ICollection<TxnCreditNoteFCLHd> TxnCreditNoteExportHd { get; } = new List<TxnCreditNoteFCLHd>();

    [InverseProperty("AgentCreditNoteImportNavigation")]
    public virtual ICollection<TxnCreditNoteImportHd> TxnCreditNoteImportHd { get; } = new List<TxnCreditNoteImportHd>();

    [InverseProperty("AgentPaymentVoucherExportNavigation")]
    public virtual ICollection<TxnPaymentVoucherExportHd> TxnPaymentVoucherExportHd { get; } = new List<TxnPaymentVoucherExportHd>();

    [InverseProperty("AgentPaymentVoucherImportNavigation")]
    public virtual ICollection<TxnPaymentVoucherImportHd> TxnPaymentVoucherImportHd { get; } = new List<TxnPaymentVoucherImportHd>();

    [InverseProperty("AgentCreditNoteFCLNavigation")]
    public virtual ICollection<TxnCreditNoteFCLHd> TxnCreditNoteFCLHd { get; } = new List<TxnCreditNoteFCLHd>();

    [InverseProperty("AgentDebitNoteFCLNavigation")]
    public virtual ICollection<TxnDebitNoteFCLHd> TxnDebitNoteFCLHd { get; } = new List<TxnDebitNoteFCLHd>();

    [InverseProperty("AgentPaymentVoucherFCLNavigation")]
    public virtual ICollection<TxnPaymentVoucherFCLHd> TxnPaymentVoucherFCLHd { get; } = new List<TxnPaymentVoucherFCLHd>();
}
