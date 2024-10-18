using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[PrimaryKey("InvoiceNo", "SerialNo")]
[Table("Txn_InvoiceDtl_Export")]
public partial class TxnInvoiceExportDtl
{
    [Key]
    [StringLength(20)]
    public string InvoiceNo { get; set; } = null!;

    [Key]
    [Column(TypeName = "numeric(6, 0)")]
    public decimal SerialNo { get; set; }

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

    [Column(TypeName = "CreatedDate")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("ChargeItem")]
    [InverseProperty("TxnInvoiceDtls")]
    public virtual RefChargeItemAcc? ChargeItemNavigation { get; set; }
}
