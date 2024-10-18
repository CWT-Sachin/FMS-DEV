using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Ref_ShippingLine")]
public partial class RefShippingLine
{
    [Key]
    [Column("ShippingLineID")]
    [StringLength(20)]
    public string ShippingLineId { get; set; } = null!;

    [StringLength(300)]
    public string Name { get; set; } = null!;

    [StringLength(300)]
    public string? AgentName { get; set; }

    [StringLength(250)]
    public string? Address1 { get; set; }

    [StringLength(250)]
    public string? Address2 { get; set; }

    [StringLength(250)]
    public string? Address3 { get; set; }

    [StringLength(250)]
    public string? City { get; set; }

    [StringLength(200)]
    public string? Country { get; set; }

    [StringLength(15)]
    public string? TelNo { get; set; }

    [StringLength(15)]
    public string? FaxNo { get; set; }

    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(300)]
    public string? Website { get; set; }

    [StringLength(100)]
    public string? CustomCode { get; set; }

    public bool IsActive { get; set; }

    [StringLength(20)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDateTime { get; set; }

    [StringLength(1)]
    public string? LastUpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdatedDateTime { get; set; }


    [InverseProperty("ShippingLineNavigation")]
    public virtual ICollection<TxnImportJobHd> TxnImportJobHds { get; } = new List<TxnImportJobHd>();

    [InverseProperty("ShippingLineExportNavigation")]
    public virtual ICollection<TxnExportJobHD> TxnExportJobHds { get; } = new List<TxnExportJobHD>();

    [InverseProperty("ColoaderExportNavigation")]
    public virtual ICollection<TxnExportJobHD> TxnExportJobHdsColoader { get; } = new List<TxnExportJobHD>();

    [InverseProperty("ShippingLine")]
    public virtual ICollection<TxnStuffingPlanHd> TxnStuffingPlanHds { get; } = new List<TxnStuffingPlanHd>();

    [InverseProperty("ColoaderNavigation")]
    public virtual ICollection<TxnStuffingPlanHd> TxnStuffingPlanHdsCoLoad { get; } = new List<TxnStuffingPlanHd>();


    [InverseProperty("ShippingLineDbitNoteExpNavigation")]
    public virtual ICollection<TxnDebitNoteExportHd> TxnDebitNoteExportHd { get; } = new List<TxnDebitNoteExportHd>();

    [InverseProperty("ShippingLineDbitNoteImpNavigation")]
    public virtual ICollection<TxnDebitNoteImportHd> TxnDebitNoteImportHd { get; } = new List<TxnDebitNoteImportHd>();

    //[InverseProperty("ShippingLineCreditNoteExpNavigation")]
    //public virtual ICollection<TxnCreditNoteFCLHd> TxnCreditNoteExportHd { get; } = new List<TxnCreditNoteFCLHd>();

    [InverseProperty("ShippingLineCreditNoteImpNavigation")]
    public virtual ICollection<TxnCreditNoteImportHd> TxnCreditNoteImportHd { get; } = new List<TxnCreditNoteImportHd>();

    [InverseProperty("ShippingLinePaymentVoucherExpNavigation")]
    public virtual ICollection<TxnPaymentVoucherExportHd> TxnPaymentVoucherExportHd { get; } = new List<TxnPaymentVoucherExportHd>();

    [InverseProperty("ShippingLinePaymentVoucherImpNavigation")]
    public virtual ICollection<TxnPaymentVoucherImportHd> TxnPaymentVoucherImportHd { get; } = new List<TxnPaymentVoucherImportHd>();

    [InverseProperty("ShippingLineFCLJobExportNavigation")]
    public virtual ICollection<TxnFCLJob> TxnFCLJob { get; } = new List<TxnFCLJob>();

    [InverseProperty("ShippingLineCreditNoteFCLNavigation")]
    public virtual ICollection<TxnCreditNoteFCLHd> TxnCreditNoteFCLHd { get; } = new List<TxnCreditNoteFCLHd>();

    [InverseProperty("ShippingLineDebitNoteFCLNavigation")]
    public virtual ICollection<TxnDebitNoteFCLHd> TxnDebitNoteFCLHd { get; } = new List<TxnDebitNoteFCLHd>();


    [InverseProperty("ShippingLinePaymentVoucherFCLNavigation")]
    public virtual ICollection<TxnPaymentVoucherFCLHd> TxnPaymentVoucherFCLHd { get; } = new List<TxnPaymentVoucherFCLHd>();


}
