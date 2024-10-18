using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models
{
    [PrimaryKey("PrePnLNo", "SNo")]
    [Table("Txn_PrePandLOtherIncomDtl")]
    public class TxnPrePandLOtherIncomDtl
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string PrePnLNo { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string SNo { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
    }
}
