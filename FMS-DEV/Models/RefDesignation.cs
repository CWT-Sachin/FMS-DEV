using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Ref_Designation")]
public partial class RefDesignation
{
    [Key]
    [Column("DesigID")]
    [StringLength(20)]
    public string DesigId { get; set; } = null!;

    [StringLength(250)]
    public string DesigName { get; set; } = null!;

    public bool? IsActive { get; set; }

    [StringLength(20)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
    public DateTime? CreatedDateTime { get; set; }

    [StringLength(1)]
    public string? LastUpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
   
    public DateTime? LastUpdatedDateTime { get; set; }

    [InverseProperty("StaffDesignationNavigation")]
    public virtual ICollection<RefStaff> RefStaffDesignationNavigation { get; } = new List<RefStaff>();
}
