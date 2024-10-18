using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Ref_Department")]
public partial class RefDepartment
{
    [Key]
    [Column("DeptID")]
    [StringLength(20)]
    public string DeptId { get; set; } = null!;

    [StringLength(250)]
    public string DeptName { get; set; } = null!;

    public bool? IsActive { get; set; }

    [StringLength(20)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDateTime { get; set; }

    [StringLength(1)]
    public string? LastUpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdatedDateTime { get; set; }

    [InverseProperty("StaffDeaprtmentNavigation")]
    public virtual ICollection<RefStaff> RefStaffDeaprtmentNavigation { get; } = new List<RefStaff>();
}
