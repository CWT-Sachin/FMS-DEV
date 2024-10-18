using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Ref_Staff")]
public partial class RefStaff
{
    [Key]
    [Column("SatffID")]
    [StringLength(20)]
    public string SatffId { get; set; } = null!;

    [StringLength(250)]
    public string StaffName { get; set; } = null!;

    [StringLength(250)]
    public string FullName { get; set; } = null!;

    [StringLength(20)]
    public string? DeaprtmentID { get; set; }

    [StringLength(20)]
    public string? DesignationID { get; set; }

    [Column("Email")]
    [StringLength(300)]
    public string? Email { get; set; }

    [Column("Mobile01")]
    [StringLength(15)]
    public string? Mobile01 { get; set; }

    [Column("Mobile02")]
    [StringLength(15)]
    public string? Mobile02 { get; set; }

    [Column("Remarks")]
    [StringLength(500)]
    public string? Remarks { get; set; }

    [Column("PictURL")]
    [StringLength(350)]
    public string? PictURL { get; set; }

    [UIHint("YesNo")]
    [Column("IsActive")]
    public bool IsActive { get; set; }

    [UIHint("YesNo")]
    public bool IsSalesPerson { get; set; }

    [StringLength(20)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDateTime { get; set; }

    [StringLength(1)]
    public string? LastUpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdatedDateTime { get; set; }

    [ForeignKey("DeaprtmentID")]
    [InverseProperty("RefStaffDeaprtmentNavigation")]
    public virtual RefDepartment?  StaffDeaprtmentNavigation{ get; set; }

    [ForeignKey("DesignationID")]
    [InverseProperty("RefStaffDesignationNavigation")]
    public virtual RefDesignation? StaffDesignationNavigation { get; set; }

    [InverseProperty("HandleByNavigation")]
    public virtual ICollection<TxnBookingExp> TxnBookingExpHandleByNavigations { get; } = new List<TxnBookingExp>();

    [InverseProperty("SalesPerson")]
    public virtual ICollection<TxnBookingExp> TxnBookingExpSalesPeople { get; } = new List<TxnBookingExp>();

    [InverseProperty("SalesPersonImportJobdtlNavigation")]
    public virtual ICollection<TxnImportJobDtl> TxnImportJobDtls { get; } = new List<TxnImportJobDtl>();

    [InverseProperty("HandlebyImportJobNavigation")]
    public virtual ICollection<TxnImportJobHd> TxnImportJobHds { get; } = new List<TxnImportJobHd>();

    [InverseProperty("PreparedByStuffingNavigation")]
    public virtual ICollection<TxnStuffingPlanHd> TxnStuffingPlanHds { get; } = new List<TxnStuffingPlanHd>();

    [InverseProperty("HandlebyExportJobNavigation")]
    public virtual ICollection<TxnExportJobHD> TxnExportJobHds { get; } = new List<TxnExportJobHD>();

    [InverseProperty("CreatedByExportJobNavigation")]
    public virtual ICollection<TxnExportJobHD> TxnExportJobHdsCreatedBy { get; } = new List<TxnExportJobHD>();




}



