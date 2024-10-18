using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Ref_ShipmentTerms")]
public partial class RefShipmentTerms
{
    [Key]
    [Column("TermID")]
    [StringLength(20)]
    public string TermID { get; set; } = null!;

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

    [InverseProperty("RefShipmentTermsNavigation")]
    public virtual ICollection<TxnBookingExp> TxnBookingExps { get; } = new List<TxnBookingExp>();
}
