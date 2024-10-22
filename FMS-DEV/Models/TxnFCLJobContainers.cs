using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMS_DEV.Models
{
    [PrimaryKey("JobNo", "SerialNo")]
    [Table("Txn_FCLJobContainers")]
    public partial class TxnFCLJobContainers
    {
        [Key]
        [StringLength(20)]
        public string JobNo { get; set; } = null!;

        [Key]
        public int SerialNo { get; set; }

        [StringLength(50)]
        public string? ContainerNo { get; set; }

        [StringLength(50)]
        public string? Size { get; set; }

        [StringLength(20)]
        public string? Seal { get; set; }

        [StringLength(20)]
        public string? BLNumber { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateTime { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDateTime { get; set; }

        // Navigation property back to TxnFCLJob
        public virtual TxnFCLJob TxnFCLJob { get; set; }
    }
}

