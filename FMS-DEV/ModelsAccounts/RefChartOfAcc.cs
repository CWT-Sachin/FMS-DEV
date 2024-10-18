using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMS_DEV.ModelsAccounts
{
    [Table("Ref_ChartOfAcc")]
    public class RefChartOfAcc
    {
        [Key]
        [StringLength(50)]
        public string AccNo { get; set; }

        [StringLength(50)]
        public string? ParentNo { get; set; }

        [StringLength(500)]
        public string? AccName { get; set; }

        [StringLength(50)]
        public string? AccCode { get; set; }

        [StringLength(50)]
        public string? AccType { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [MaxLength]
        public string? Note { get; set; }

        [StringLength(20)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateTime { get; set; }

        [StringLength(20)]
        public string? LastUpdatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDateTime { get; set; }

        public bool? IsInactive { get; set; }

        [StringLength(20)]
        public string? InactivatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? InactivatedDateTime { get; set; }

        [StringLength(350)]
        public string? InactivatedReason { get; set; }


        [ForeignKey("AccType")]
        [InverseProperty("RefChartOfAcc")]
        public virtual RefAccountTypes? AccountsTypes { get; set; }

        // Navigation property to represent the parent node
        [ForeignKey("ParentNo")]
        public virtual RefChartOfAcc Parent { get; set; }

        public virtual ICollection<RefChartOfAcc> Children { get; set; }

    }
}
