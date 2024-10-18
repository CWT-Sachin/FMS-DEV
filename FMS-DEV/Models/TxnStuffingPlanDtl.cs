using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[PrimaryKey("PlanId", "BookingRefNo")]
[Table("Txn_StuffingPlan_Dtl")]
public partial class TxnStuffingPlanDtl
{
    [Key]
    [Column("PlanID")]
    [StringLength(20)]
    public string PlanId { get; set; } = null!;

    [Key]
    [StringLength(20)]
    public string BookingRefNo { get; set; } = null!;

    [StringLength(20)]
    public string CargoType { get; set; } = null!;
}
