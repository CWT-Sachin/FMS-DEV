using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMS_DEV.Models
{
    [Table("Txn_FCLBL")]
    public partial class TxnFCLBL
    {
        [Key, Column(Order = 0)]
        [StringLength(20)]
        public string JobNo { get; set; } = null!;

        //[Key, Column(Order = 1)]
        [StringLength(100)]
        public string BLNo { get; set; } = null!;

        [StringLength(20)]
        public string? BLTypeID { get; set; }

        [StringLength(20)]
        public string? BLStatus { get; set; }

        [StringLength(20)]
        public string? ShipmentTerm { get; set; }

        [Column(TypeName = "numeric(18, 4)")]
        public decimal? GrossWeight { get; set; }

        [Column(TypeName = "numeric(18, 4)")]
        public decimal? CBM { get; set; }

        [Column(TypeName = "numeric(18, 4)")]
        public decimal? NetWeight { get; set; }

        [Column(TypeName = "numeric(18, 0)")]
        public decimal? NoofPackages { get; set; }

        [StringLength(20)]
        public string? PackageType { get; set; }

        [StringLength(250)]
        public string? Commodity { get; set; }

        
        [StringLength(200)]
        public string? HSCode { get; set; }

        [Column("ShipperID")]
        [StringLength(20)]
        public string? ShipperId { get; set; }

        [StringLength(300)]
        public string? ShippreName { get; set; }

        [StringLength(250)]
        public string? ShippreAddress1 { get; set; }

        [StringLength(250)]
        public string? ShippreAddress2 { get; set; }

        [StringLength(250)]
        public string? ShippreAddress3 { get; set; }

        [StringLength(250)]
        public string? ShipperCity { get; set; }

        [StringLength(250)]
        public string? ShipperCountry { get; set; }

        [StringLength(15)]
        public string? ShipperTel { get; set; }

        [StringLength(250)]
        public string? ShipperEmail { get; set; }

        [StringLength(300)]
        public string? ConsigneeName { get; set; }

        [StringLength(250)]
        public string? ConsigneeAddress1 { get; set; }

        [StringLength(250)]
        public string? ConsigneeAddress2 { get; set; }

        [StringLength(250)]
        public string? ConsigneeAddress3 { get; set; }

        [StringLength(250)]
        public string? ConsigneeCity { get; set; }

        [StringLength(250)]
        public string? ConsigneeCountry { get; set; }

        [StringLength(15)]
        public string? ConsigneeTel { get; set; }

        [StringLength(250)]
        public string? ConsigneeEmail { get; set; }

        [StringLength(300)]
        public string? NotifyPartyName { get; set; } // Notify Party Name

        [StringLength(250)]
        public string? NotifyPartyAddress1 { get; set; } // Notify Party Address 1

        [StringLength(250)]
        public string? NotifyPartyAddress2 { get; set; } // Notify Party Address 2

        [StringLength(250)]
        public string? NotifyPartyAddress3 { get; set; } // Notify Party Address 3

        [StringLength(250)]
        public string? NotifyPartyCity { get; set; } // Notify Party City

        [StringLength(250)]
        public string? NotifyPartyCountry { get; set; } // Notify Party Country

        [StringLength(15)]
        public string? NotifyPartyTel { get; set; } // Notify Party Telephone

        [StringLength(250)]
        public string? NotifyPartyEmail { get; set; } // Notify Party Email

        [StringLength(50)]
        public string? Freight { get; set; } // Pre-paid, Collect

        public bool? Forwading { get; set; } // Forwarding

        [StringLength(20)]
        public string? Forwader { get; set; } // Forwarder "Local" (FK Ref_Customer)

        public bool? LocalCharges { get; set; } // BL Printing Option "Local Charges"



        public string? MarksAndNos { get; set; } // Marks and Nos

        public string? Description { get; set; } // Description

        [StringLength(20)]
        public string? CreatedBy { get; set; } // Created By

        public DateTime? CreatedDateTime { get; set; } // Created DateTime

        [StringLength(20)]
        public string? LastUpdatedBy { get; set; } // Last Updated By

        public DateTime? LastUpdatedDateTime { get; set; } // Last Updated DateTime

        public bool? Canceled { get; set; } // Canceled

        [StringLength(20)]
        public string? CanceledBy { get; set; } // Canceled By

        public DateTime? CanceledDateTime { get; set; } // Canceled DateTime

        [StringLength(350)]
        public string? CanceledReson { get; set; } // Canceled Reason

    }
}

