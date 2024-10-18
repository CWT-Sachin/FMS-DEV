using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.ModelsAccounts;

[Table("Ref_LastNumber")]
public partial class RefLastNumberAcc
{
    [Key]
    [Column("TableID")]
    [StringLength(50)]
    public string TableID { get; set; } = null!;

    [Column("LastNumber", TypeName = "numeric(18, 0)")]
    public decimal LastNumber { get; set; } 

  
}
