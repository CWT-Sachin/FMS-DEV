using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Ref_SalesPerson")]
public partial class RefSalesPerson
{
    [Key]
    [Column("SalesPersonID")]
    [StringLength(20)]
    public string SalesPersonId { get; set; } = null!;

    [StringLength(250)]
    public string Name { get; set; } = null!;

    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(15)]
    public string? Mobile01 { get; set; }

    [StringLength(15)]
    public string? Mobile02 { get; set; }

    public string? Remarks { get; set; }

    [Column("PictURL")]
    [StringLength(350)]
    public string? PictUrl { get; set; }
}
