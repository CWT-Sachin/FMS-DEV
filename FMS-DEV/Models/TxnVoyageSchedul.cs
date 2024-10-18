using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[PrimaryKey("VesselId", "VoyageId")]
[Table("Txn_VoyageSchedul")]
public partial class TxnVoyageSchedul
{
    [Key]
    [Column("VesselID")]
    [StringLength(20)]
    public string VesselId { get; set; } = null!;

    [Key]
    [Column("VoyageID")]
    [StringLength(20)]
    public string VoyageId { get; set; } = null!;

    [Column("ETA", TypeName = "date")]
    public DateTime? Eta { get; set; }

    [Column("ETD", TypeName = "date")]
    public DateTime? Etd { get; set; }

    [Column(TypeName = "date")]
    public DateTime? CutOffDate { get; set; }

    public TimeSpan? CutOffTime { get; set; }

    [StringLength(250)]
    public string? Depot { get; set; }

    [StringLength(20)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDateTime { get; set; }

    [StringLength(1)]
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

    //[InverseProperty("V")]
    //public virtual ICollection<TxnBookingExp> TxnBookingExps { get; } = new List<TxnBookingExp>();
}
