using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
//using PowerAcc.ModelsOperation;
using FMS_DEV.Models;

namespace FMS_DEV.Models;

[Table("Ref_ChargeItemAcc")]
public partial class RefChargeItemAcc
{
    [Key]
    [Column("ChargeID")]
    [StringLength(20)]
    public string ChargeId { get; set; } = null!;

    [StringLength(300)]
    public string Description { get; set; } = null!;

    public bool? IsActive { get; set; }

    [StringLength(20)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDateTime { get; set; }

    [StringLength(1)]
    public string? LastUpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdatedDateTime { get; set; }

    [StringLength(50)]
    public string? AccNo { get; set; }

    [StringLength(50)]
    public string? AccNo_Revenue_Exp { get; set; }

    [StringLength(50)]
    public string? AccNo_Revenue_Imp { get; set; }

    [StringLength(50)]
    public string? AccNo_Expense_Imp_Liner { get; set; }

    [StringLength(50)]
    public string? AccNo_Expense_Imp_Agent { get; set; }

    [StringLength(50)]
    public string? AccNo_Expense_Exp_Liner { get; set; }
    [StringLength(50)]
    public string? AccNo_Expense_Exp_Agent { get; set; }





    [InverseProperty("ChargeItemNavigation")]
    public virtual ICollection<TxnInvoiceExportDtl> TxnInvoiceDtls { get; } = new List<TxnInvoiceExportDtl>();


    [InverseProperty("ChargeItemNavigation")]
    public virtual ICollection<TxnInvoiceImportDtl> TxnInvoiceImportDtls { get; } = new List<TxnInvoiceImportDtl>();

    [InverseProperty("ChargeItemNavigation")]
    public virtual ICollection<TxnDebitNoteExportDtl> TxnDebitNoteExportDtls { get; } = new List<TxnDebitNoteExportDtl>();

    [InverseProperty("ChargeItemNavigation")]
    public virtual ICollection<TxnDebitNoteImportDtl> TxnDebitNoteImportDtls { get; } = new List<TxnDebitNoteImportDtl>();

   

    //[InverseProperty("ChargeItemNavigation")]
    //public virtual ICollection<TxnCreditNoteFCLDtl> TxnCreditNoteExportDtls { get; } = new List<TxnCreditNoteFCLDtl>();

    [InverseProperty("ChargeItemNavigation")]
    public virtual ICollection<TxnCreditNoteImportDtl> TxnCreditNoteImportDtls { get; } = new List<TxnCreditNoteImportDtl>();

    [InverseProperty("ChargeItemNavigation")]
    public virtual ICollection<TxnPaymentVoucherExportDtl> TxnPaymentVoucherExportDtls { get; } = new List<TxnPaymentVoucherExportDtl>();

    [InverseProperty("ChargeItemNavigation")]
    public virtual ICollection<TxnPaymentVoucherImportDtl> TxnPaymentVoucherImportDtls { get; } = new List<TxnPaymentVoucherImportDtl>();

    [InverseProperty("ChargeItemNavigation")]
    public virtual ICollection<TxnInvoiceFCLDtl> TxnInvoiceFCLDtls { get; } = new List<TxnInvoiceFCLDtl>();

    [InverseProperty("ChargeItemNavigation")]
    public virtual ICollection<TxnCreditNoteFCLDtl> TxnCreditNoteFCLDtls { get; } = new List<TxnCreditNoteFCLDtl>();

    [InverseProperty("ChargeItemNavigation")]
    public virtual ICollection<TxnDebitNoteFCLDtl> TxnDebitNoteFCLDtls { get; } = new List<TxnDebitNoteFCLDtl>();

    [InverseProperty("ChargeItemNavigation")]
    public virtual ICollection<TxnPaymentVoucherFCLDtl> TxnPaymentVoucherFCLDtls { get; } = new List<TxnPaymentVoucherFCLDtl>();
}
