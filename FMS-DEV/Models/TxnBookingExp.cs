using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Txn_Booking_Exp")]
public partial class TxnBookingExp
{
    [Key]
    [StringLength(20)]
    public string BookingNo { get; set; } = null!;

    [Column(TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
    public DateTime BookingDate { get; set; }

    
    [Column("CargoBookingType")]
    [StringLength(20)]
    public string CargoBookingType { get; set; } = null!;

    [Column("SalesPersonID")]
    [StringLength(20)]
    public string SalesPersonId { get; set; } = null!;

    //AgentIDNomination
    [Column("AgentIDNomination")]
    [StringLength(20)]
    public string AgentIDNomination { get; set; } = null!;

    [StringLength(20)]
    public string HandleBy { get; set; } = null!;

    [Column("BLNo")]
    [StringLength(100)]
    public string? Blno { get; set; }

    [Column("BLTypeID")]
    [StringLength(20)]
    public string? BLTypeID { get; set; }

    [Column("BLStatus")]
    [StringLength(20)]
    public string? BLStatus { get; set; }

    [StringLength(20)]
    public string? Vessel { get; set; }

    [StringLength(20)]
    public string? Voyage { get; set; }

    
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
    [Column("ETA_POL")]
    public DateTime? EtaPol { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
    [Column("ETD_POL")]
    public DateTime? EtdPol { get; set; }

    [Column("ETA_FDN")]
    [StringLength(20)]
    public string? EtaFdn { get; set; }

    [Column("POD")]
    [StringLength(20)]
    public string? Pod { get; set; }

    [Column("FDN")]
    [StringLength(20)]
    public string? Fdn { get; set; }

    [Column("POL")]
    [StringLength(20)]
    public string? Pol { get; set; }


    [Column(TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? FDN_ETA { get; set; }

    [Column(TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode =true)]
    public DateTime? CutoffDate { get; set; }

    public TimeSpan? CutoffTime { get; set; }

    [StringLength(50)]
    public string? CargoType { get; set; }

    [StringLength(20)]
    public string? CoLoader { get; set; }

    [StringLength(100)]
    public string? CargoMethod { get; set; }

    [Column("TS_POL")]
    [StringLength(20)]
    public string? TsPol { get; set; }

    [Column("TS_Destination")]
    [StringLength(20)]
    public string? TsDestination { get; set; }

    [Column("TS_BLNo")]
    [StringLength(100)]
    public string? TsBlno { get; set; }

    [Column("TS_Vessel")]
    [StringLength(20)]
    public string? TsVessel { get; set; }

    [Column("TS_Voyage")]
    [StringLength(20)]
    public string? TsVoyage { get; set; }

    [Column("TS_ETA_Colombo")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? TsETAColombo { get; set; }

    [Column("TS_ImportContainerNo")]
    [StringLength(100)]
    public string? TsImportContainerNo { get; set; }

    [Column("GrossWeight",TypeName = "numeric(18, 4)")]
   // [Required(ErrorMessage = "GrossWeight is required.")]
   // [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Valid Decimal number with maximum 2 decimal places.")]
    public decimal? GrossWeight { get; set; }

    [Column("NetWeight", TypeName = "numeric(18, 4)")]
     public decimal? NetWeight { get; set; }

    [Column("CBM", TypeName = "numeric(18, 4)")]
    [Required(ErrorMessage = "CBM is required.")]
  //  [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Valid Decimal number with maximum 2 decimal places.")]
    public decimal? Cbm { get; set; }

    [Column(TypeName = "numeric(18, 0)")]
    public decimal? NoofPackages { get; set; }

    [StringLength(20)]
    public string? PackageType { get; set; }

    [Column(TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? CargoRrecvDdate { get; set; }

    [StringLength(250)]
    public string? Commodity { get; set; }

    [Column("HSCode")]
    [StringLength(200)]
    public string? Hscode { get; set; }

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
    public string? NotifyPartyName { get; set; }

    [StringLength(250)]
    public string? NotifyPartyAddress1 { get; set; }

    [StringLength(250)]
    public string? NotifyPartyAddress2 { get; set; }

    [StringLength(250)]
    public string? NotifyPartyAddress3 { get; set; }

    [StringLength(250)]
    public string? NotifyPartyCity { get; set; }

    [StringLength(250)]
    public string? NotifyPartyCountry { get; set; }

    [StringLength(15)]
    public string? NotifyPartyTel { get; set; }

    [StringLength(250)]
    public string? NotifyPartyEmail { get; set; }

    [Column("SellingRate", TypeName = "numeric(18, 2)")]
    //[Required(ErrorMessage = "Selling Rate is required.")]
   // [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Valid Decimal number with maximum 2 decimal places.")]
    public decimal? SellingRate { get; set; }

    public string? MarksAndNos { get; set; }

    public string? Description { get; set; }
    public string? Freight { get; set; }
    public Boolean Forwading { get; set; }

    [StringLength(20)]
    public string? Forwader { get; set; }
    public Boolean LocalCharges { get; set; }

    public bool IsStuffingPlan { get; set; }

    [Column("StuffingPlanNo")]
    [StringLength(20)]
    public string? StuffingPlanNo { get; set; }

    [StringLength(20)]
    public string? ShipmentTerm { get; set; }

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
    public string? CanceledReson { get; set; }



    [ForeignKey("HandleBy")]
    [InverseProperty("TxnBookingExpHandleByNavigations")]
    public virtual RefStaff? HandleByNavigation { get; set; } = null!;

    [ForeignKey("SalesPersonId")]
    [InverseProperty("TxnBookingExpSalesPeople")]
    public virtual RefStaff? SalesPerson { get; set; } = null!;

    //
    [ForeignKey("BLTypeID")]
    [InverseProperty("TxnBookingExps")]
    public virtual RefBlTypes? BLTypeIDsNavigation { get; set; } = null!;

    [ForeignKey("BLStatus")]
    [InverseProperty("TxnBookingExps")]
    public virtual RefBLStatus? RefBLStatusIDNavigation { get; set; } = null!;

    [ForeignKey("ShipmentTerm")]
    [InverseProperty("TxnBookingExps")]
    public virtual RefShipmentTerms? RefShipmentTermsNavigation { get; set; } = null!;

   

    [ForeignKey("PackageType")]
    [InverseProperty("TxnBookingExps")]
    public virtual RefPackage? PackageNavigation { get; set; }

    [ForeignKey("Pod")]
    [InverseProperty("TxnBookingExpsPODNavigation")]
    public virtual RefPort? PODNavigation { get; set; }

    [ForeignKey("Pol")]
    [InverseProperty("TxnBookingExpsPOLNavigation")]
    public virtual RefPort? POLNavigation { get; set; }

    [ForeignKey("Fdn")]
    [InverseProperty("TxnBookingExpsFDNNavigation")]
    public virtual RefPort? FDNNavigation { get; set; }


    [ForeignKey("AgentIDNomination")]
    [InverseProperty("TxnBookingExpsAgentIDNomination")]
    public virtual RefAgent? AgentIDNominationNavigation { get; set; }

    [ForeignKey("ShipperId")]
    [InverseProperty("TxnBookingExps")]
    public virtual RefCustomer? Shipper { get; set; }

    [ForeignKey("CoLoader")]
    [InverseProperty("TxnBookingExpsColoaderNavigation")]
    public virtual RefCustomer? ColoaderNavigation { get; set; }

    [ForeignKey("Vessel")]
    [InverseProperty("TxnBookingExps")]
    public virtual RefVessel? VesselNavigation { get; set; }

    [ForeignKey("TsVessel")]
    [InverseProperty("TxnBookingExpsTsVessel")]
    public virtual RefVessel? TSVesselNavigation { get; set; }

    [ForeignKey("TsDestination")]
    [InverseProperty("TxnBookingExpsTSDestinationNavigation")]
    public virtual RefPort? TSDestinationNavigation { get; set; }

    [ForeignKey("TsPol")]
    [InverseProperty("TxnBookingExpsTsPolNavigation")]
    public virtual RefPort? TsPolNavigation { get; set; }

    //[ForeignKey("Vessel, Voyage")]
    //[InverseProperty("TxnBookingExps")]
    //public virtual TxnVoyageSchedul? V { get; set; }
}
