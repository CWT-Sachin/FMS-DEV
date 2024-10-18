using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Ref_Terminal")]
public partial class RefTerminal
{
    [Key]
    [Column("TerminalID")]
    [StringLength(20)]
    public string TerminalId { get; set; } = null!;

    [StringLength(250)]
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

    [InverseProperty("TerminalNavigation")]
    public virtual ICollection<TxnImportJobHd> TxnImportJobHds { get; } = new List<TxnImportJobHd>();

    [InverseProperty("Terminal")]
    public virtual ICollection<TxnStuffingPlanHd> TxnStuffingPlanHds { get; } = new List<TxnStuffingPlanHd>();
}
