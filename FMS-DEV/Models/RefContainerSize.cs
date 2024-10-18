using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Ref_ContainerSize")]
public partial class RefContainerSize
{
    [Key]
    [Column("ContainerID")]
    [StringLength(20)]
    public string ContainerId { get; set; } = null!;

    [StringLength(250)]
    public string Description { get; set; } = null!;

    [Column(TypeName = "numeric(8, 0)")]
    public decimal? Size { get; set; }

    [StringLength(20)]
    public string? Type { get; set; }

    [StringLength(100)]
    public string? CustomCode { get; set; }

    [InverseProperty("SizeNavigation")]
    public virtual ICollection<TxnImportJobCnt> TxnImportJobCnts { get; } = new List<TxnImportJobCnt>();

    [InverseProperty("ContainerStuffingNavigation")]
    public virtual ICollection<TxnStuffingPlanHd> TxnStuffingPlanHds { get; } = new List<TxnStuffingPlanHd>();
}
