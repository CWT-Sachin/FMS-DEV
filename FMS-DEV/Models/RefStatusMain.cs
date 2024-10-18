using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Ref_StatusMain")]
public partial class RefStatusMain
{
    [Key]
    [Column("ID")]
    [StringLength(20)]
    public string Id { get; set; } = null!;

    [StringLength(250)]
    public string Description { get; set; } = null!;

    public bool? IsActive { get; set; }



    [InverseProperty("JobStatusExportJobNavigation")]
    public virtual ICollection<TxnExportJobHD> TxnExportJobHds { get; } = new List<TxnExportJobHD>();
}


