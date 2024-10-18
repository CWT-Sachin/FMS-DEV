using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Txn_ExportMasterBLDtl")]
public class TxnExportMasterBLDtl
{
    [Key]
    [Required]
    [StringLength(20)]
    public string MasterBLno { get; set; }

    [StringLength(20)]
    public string BookingNo { get; set; }

    public DateTime? CreatedDate { get; set; }
}
