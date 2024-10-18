using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMS_DEV.Models
{
    [Table("Txn_FCLJob")]
    public partial class TxnFCLJob
    {
        [Key]
        [StringLength(20)]
        public string JobNo { get; set; } = null!;

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? Date { get; set; }

        [StringLength(50)]
        public string? JobType { get; set; }

        [Column(TypeName = "numeric(6, 2)")]
        public decimal? ExchangeRate { get; set; }

        [StringLength(20)]
        public string? HandleBy { get; set; }

        [StringLength(20)]
        public string? SalesPersonID { get; set; }

        [StringLength(20)]
        public string? ShipperID { get; set; }

        [StringLength(20)]
        public string? ShippingLineID { get; set; }

        [StringLength(50)]
        public string? MasterBL { get; set; }

        [StringLength(20)]
        public string? VesselId { get; set; }

        [StringLength(50)]
        public string? Voyage { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ETD { get; set; }

        [StringLength(20)]
        public string? POD { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ETA { get; set; }

        [StringLength(50)]
        public string? BLType { get; set; }

        [StringLength(20)]
        public string? AgentId { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        [StringLength(20)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateTime { get; set; }

        [StringLength(20)]
        public string? LastUpdatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDateTime { get; set; }

        public bool? Canceled { get; set; }

        [StringLength(20)]
        public string? CanceledBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CanceledDateTime { get; set; }

        [StringLength(350)]
        public string? CanceledReason { get; set; }

        //[ForeignKey("AgentId")]
        //[InverseProperty("TxnFCLJob")]
        //public virtual RefAgent? AgentFCLJobExportNavigation { get; set; }

        [ForeignKey("ShippingLineID")]
        [InverseProperty("TxnFCLJob")]
        public virtual RefShippingLine? ShippingLineFCLJobExportNavigation { get; set; }


    }
}
