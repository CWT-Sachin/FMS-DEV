using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[PrimaryKey("PayVoucherNo", "SerialNo")]
[Table("Txn_PaymentVoucherDtl_Import")]
public partial class TxnPaymentVoucherImportDtl
{
    [Key]
    [StringLength(20)]
    public string PayVoucherNo { get; set; } = null!;

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
    [InverseProperty("TxnPaymentVoucherImportDtls")]
    public virtual RefChargeItemAcc? ChargeItemNavigation { get; set; }
}
