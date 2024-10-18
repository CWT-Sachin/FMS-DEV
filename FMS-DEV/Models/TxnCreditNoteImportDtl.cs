using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using FMS_DEV.ModelsAccounts;

namespace FMS_DEV.Models;

[PrimaryKey("CreditNoteNo", "SerialNo")]
[Table("Txn_CreditNoteDtl_Import")]
public partial class TxnCreditNoteImportDtl
{
    [Key]
    [StringLength(20)]
    public string CreditNoteNo { get; set; } = null!;

    [Key]
    [Column(TypeName = "numeric(6, 0)")]
    public decimal SerialNo { get; set; }

    [StringLength(100)]
    public string? BlContainerNo { get; set; }

    [StringLength(20)]
    public string? ChargeItem { get; set; }

    [StringLength(50)]
    public string? AccNo { get; set; }

    [StringLength(20)]
    public string? Unit { get; set; }

    [Column(TypeName = "numeric(18, 3)")]
    public decimal? Rate { get; set; }

    [StringLength(20)]
    public string? Currency { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Qty { get; set; }

    [Column(TypeName = "numeric(18, 3)")]
    public decimal? Amount { get; set; }

    [ForeignKey("ChargeItem")]
    [InverseProperty("TxnCreditNoteImportDtls")]
    public virtual RefChargeItemAcc? ChargeItemNavigation { get; set; }
}
