using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FMS_DEV.Models;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Txn_InvoiceHD_FCL")]
public partial class TxnInvoiceFCLHd
{
    [Key]
    [StringLength(20)]
    public string InvoiceNo { get; set; } = null!;

    [Column(TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? Date { get; set; }

    [StringLength(20)]
    public string? JobNo { get; set; }

    [Column("BLNo")]
    [StringLength(20)]
    public string? Blno { get; set; }

    [StringLength(20)]
    public string? Customer { get; set; }

    [Column(TypeName = "numeric(18, 3)")]
    public decimal? ExchangeRate { get; set; }

    [Column("TotalInvoiceAmountLKR", TypeName = "numeric(18, 3)")]
    public decimal? TotalInvoiceAmountLkr { get; set; }

    [Column("TotalInvoiceAmountUSD", TypeName = "numeric(18, 3)")]
    public decimal? TotalInvoiceAmountUsd { get; set; }

    [StringLength(450)]
    public string? Remarks { get; set; }

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

    [StringLength(350)]
    public string? CanceledReson { get; set; }

    public bool? Approved { get; set; }

    [StringLength(20)]
    public string? ApprovedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ApprovedDateTime { get; set; }


    [StringLength(50)]
    public string? DebitAcc { get; set; }

    [StringLength(50)]
    public string? CreditAcc { get; set; }


    [Column("AmountPaid", TypeName = "numeric(18, 3)")]
    public decimal? AmountPaid { get; set; }

    [Column("AmountToBePaid", TypeName = "numeric(18, 3)")]
    public decimal? AmountToBePaid { get; set; }

    [StringLength(50)]
    public string? BLNoTS { get; set; }
    [StringLength(50)]
    public string? BLNoType { get; set; }


    [ForeignKey("Customer")] // Assuming this is the correct foreign key property
    [InverseProperty("TxnInvoiceFCLHds")]
    public virtual RefCustomer? CustomerInvoiceFCLHd { get; set; }


}
