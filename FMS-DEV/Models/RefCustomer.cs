using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Ref_Customer")]
public partial class RefCustomer
{
    [Key]
    [StringLength(20)]
    public string CustomerId { get; set; } = null!;

    [StringLength(250)]
    public string Name { get; set; } = null!;

    [StringLength(20)]
    public string CustomerType { get; set; } = null!;

    [Column("CustomerClubID")]
    [StringLength(20)]
    public string? CustomerClubId { get; set; }

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
    public string? TelNo1 { get; set; }

    [StringLength(15)]
    public string? TelNo2 { get; set; }

    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(250)]
    public string? ContactPersonName { get; set; }

    [StringLength(250)]
    public string? Designation { get; set; }

    [StringLength(15)]
    public string? TelNo { get; set; }

    [StringLength(15)]
    public string? MobileNo { get; set; }

    [StringLength(15)]
    public string? ResidenceTel { get; set; }

    [StringLength(200)]
    public string? ContactPersonEmail { get; set; }

    [StringLength(100)]
    public string? VatReg { get; set; }

    [StringLength(100)]
    public string? SvatReg { get; set; }

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

    [InverseProperty("Shipper")]
    public virtual ICollection<TxnBookingExp> TxnBookingExps { get; } = new List<TxnBookingExp>();

    [InverseProperty("ColoaderNavigation")]
    public virtual ICollection<TxnBookingExp> TxnBookingExpsColoaderNavigation { get; } = new List<TxnBookingExp>();

    [InverseProperty("ShipperImportJobDtlNavigation")]
    public virtual ICollection<TxnImportJobDtl> TxnImportJobDtls { get; } = new List<TxnImportJobDtl>();

    [InverseProperty("CustomerInvoiceFCLHd")]
    public virtual ICollection<TxnInvoiceFCLHd> TxnInvoiceFCLHds { get; } = new List<TxnInvoiceFCLHd>();

    [InverseProperty("CustomerInvoiceExportHd")]
    public virtual ICollection<TxnInvoiceExportHd> TxnInvoiceExportHds { get; } = new List<TxnInvoiceExportHd>();

    [InverseProperty("CustomerInvoiceImportHd")]
    public virtual ICollection<TxnInvoiceImportHd> TxnInvoiceImportHds { get; } = new List<TxnInvoiceImportHd>();


}
