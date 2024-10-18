using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Txn_ExportMasterBLHD")]
public class TxnExportMasterBLHD
{
    [Key]
    [Required]
    [StringLength(20)]
    public string MasterBLno { get; set; }

    [StringLength(20)]
    public string JobNo { get; set; }

    [StringLength(20)]
    public string CreatedBy { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    [StringLength(20)]
    public string LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedDateTime { get; set; }

    public bool? Canceled { get; set; }

    [StringLength(20)]
    public string CanceledBy { get; set; }

    public DateTime? CanceledDateTime { get; set; }

    [StringLength(350)]
    public string CanceledReason { get; set; }
}

