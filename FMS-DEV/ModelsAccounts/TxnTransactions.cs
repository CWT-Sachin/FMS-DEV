using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.ModelsAccounts;

[PrimaryKey("TxnNo", "TxnSNo")]
[Table("Txn_Transactions")]
public class TxnTransactions
{

    [Key]
    [StringLength(50)]
    public string TxnNo { get; set; } = null!;

    [Key]
    [Column(TypeName = "numeric(6, 0)")]
    public decimal TxnSNo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Date { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? TxnAccCode { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal Dr { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal Cr { get; set; }

    [StringLength(100)]
    public string? RefNo { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    public bool Reconciled { get; set; }

    [StringLength(100)]
    public string? DocType { get; set; }

    public bool IsMonthEndDone { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? MonthEndDoneDate { get; set; }

    [StringLength(50)]
    public string? MonthEndDoneBy { get; set; }

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
}
