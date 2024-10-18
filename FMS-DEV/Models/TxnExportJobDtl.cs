using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[PrimaryKey("JobNo", "StuffingPlnNo")]
[Table("Txn_ExportJobDtl")]
public partial class TxnExportJobDtl
{
    [Key]
    [StringLength(20)]
    public string JobNo { get; set; } = null!;

    [Key]
    [StringLength(20)]
    public string StuffingPlnNo { get; set; } = null!;

    [StringLength(20)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDateTime { get; set; }



}
