using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Ref_BLStatus")]
public partial class RefBLStatus
{
    [Key]
    [Column("BLStatusID")]
    [StringLength(20)]
    public string BLStatusID { get; set; } = null!;

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

    [InverseProperty("RefBLStatusIDNavigation")]
    public virtual ICollection<TxnBookingExp> TxnBookingExps { get; } = new List<TxnBookingExp>();

    [InverseProperty("BlstatusNavigation")]
    public virtual ICollection<TxnImportJobDtl> TxnImportJobDtlBlstatusNavigations { get; } = new List<TxnImportJobDtl>();

    [InverseProperty("TsblstatusNavigation")]
    public virtual ICollection<TxnImportJobDtl> TxnImportJobDtlTsblstatusNavigations { get; } = new List<TxnImportJobDtl>();
}
