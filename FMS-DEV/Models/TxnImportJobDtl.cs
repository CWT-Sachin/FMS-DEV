using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[PrimaryKey("JobNo", "RefNo")]
[Table("Txn_ImportJob_Dtl")]
public partial class TxnImportJobDtl
{
    [Key]
    [StringLength(20)]
    public string JobNo { get; set; } = null!;

    [Key]
    [StringLength(20)]
    public string RefNo { get; set; } = null!;


    [Column("HouseBL")]
    [StringLength(50)]
    public string? HouseBl { get; set; }

    [Column("BLTypeID")]
    [StringLength(20)]
    public string? BltypeId { get; set; }

    [Column("BLStatus")]
    [StringLength(20)]
    public string? Blstatus { get; set; }

    [StringLength(20)]
    public string? SalesPerson { get; set; }

    [StringLength(20)]
    public string? Freight { get; set; }

    
    [Column("IsTBL")]
    public bool IsTBL { get; set; }

    [Column("TSBLNo")]
    [StringLength(50)]
    public string? Tsblno { get; set; }

    [Column("TSBLStatus")]
    [StringLength(20)]
    public string? Tsblstatus { get; set; }

    [Column("TSBLType")]
    [StringLength(20)]
    public string? Tsbltype { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Weight { get; set; }

    [Column("CBM", TypeName = "numeric(18, 3)")]
    [DisplayFormat(DataFormatString = "{0:0.000}")]
    public decimal? Cbm { get; set; }

    [Column(TypeName = "numeric(18, 0)")]
    public decimal? NoofPackages { get; set; }

    [StringLength(20)]
    public string? PackageType { get; set; }

    [StringLength(20)]
    public string? CargoType { get; set; }

    [Column("TSDestination")]
    [StringLength(20)]
    public string? Tsdestination { get; set; }

    public string? MarksAndNumbers { get; set; }

    public string? Description { get; set; }

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

    [StringLength(250)]
    public string? ShipperTel { get; set; }

    [StringLength(500)]
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

    [StringLength(250)]
    public string? ConsigneeTel { get; set; }

    [StringLength(500)]
    public string? ConsigneeEmail { get; set; }

    [StringLength(250)]
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

    [StringLength(250)]
    public string? NotifyPartyTel { get; set; }

    [StringLength(300)]
    public string? NotifyPartyEmail { get; set; }

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

    [Column("ShipperID")]
    [StringLength(20)]
    public string? ShipperId { get; set; }


    [Column("PrintParty")]
    [StringLength(20)]
    public string? PrintParty { get; set; }

    [Column("DemurrageFreedays", TypeName = "numeric(15, 2)")]
    public decimal? DemurrageFreedays { get; set; }
     
    
    [Column("IntendedVessel")]
    [StringLength(20)]
    public string? IntendedVessel { get; set; }

    [Column("IntendedVoyage")]
    [StringLength(50)]
    public string? IntendedVoyage { get; set; }

    [Column("StuffingPlanNo")]
    [StringLength(20)]
    public string? StuffingPlanNo { get; set; }

    public Boolean IsStuffingPlan { get; set; }

    [Column(TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? ETDColombo { get; set; }

    [Column("POD")]
    [StringLength(20)]
    public string? Pod { get; set; }

    [Column(TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? ETAPod { get; set; }

    [Column(TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? FDNETA { get; set; }

    [Column("SerialNoBL")]
    [StringLength(20)]
    public string? SerialNoBL { get; set; }

    [Column("DOApproveBy")]
    [StringLength(20)]
    public string? DOApproveBy { get; set; }

    public Boolean? IsDOApproved { get; set; }

    [Column(TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? DODate { get; set; }

    public Boolean? IsDOprint { get; set; }

    [StringLength(300)]
    public string? PackagsInword { get; set; }

    [Column(TypeName = "numeric(10, 2)")]
    public decimal? ExchangeRate { get; set; }

    [StringLength(300)]
    public string? FreightPayMethod { get; set; }

    [StringLength(20)]
    public string? DeleveryAgent { get; set; }
    public string? RealeseNote { get; set; }

    [StringLength(50)]
    public string? AMSNo { get; set; }
    public string? MarksAndNumbersExport { get; set; }
    public string? DescriptionExport { get; set; }

    [StringLength(20)]
    public string? ConsigneeID { get; set; }

    [StringLength(20)]
    public string? NotifyPartyID { get; set; }


    // Forign Key
    [ForeignKey("Blstatus")]
    [InverseProperty("TxnImportJobDtlBlstatusNavigations")]
    public virtual RefBLStatus? BlstatusNavigation { get; set; }

    [ForeignKey("BltypeId")]
    [InverseProperty("TxnImportJobDtlBltypes")]
    public virtual RefBlTypes? Bltype { get; set; }

    [ForeignKey("PackageType")]
    [InverseProperty("TxnImportJobDtls")]
    public virtual RefPackage? PackageTypeImportJobNavigation { get; set; }

    [ForeignKey("SalesPerson")]
    [InverseProperty("TxnImportJobDtls")]
    public virtual RefStaff? SalesPersonImportJobdtlNavigation { get; set; }

    [ForeignKey("ShipperId")]
    [InverseProperty("TxnImportJobDtls")]
    public virtual RefCustomer? ShipperImportJobDtlNavigation { get; set; }

    [ForeignKey("Tsblstatus")]
    [InverseProperty("TxnImportJobDtlTsblstatusNavigations")]
    public virtual RefBLStatus? TsblstatusNavigation { get; set; }

    [ForeignKey("Tsbltype")]
    [InverseProperty("TxnImportJobDtlTsbltypeNavigations")]
    public virtual RefBlTypes? TsbltypeNavigation { get; set; }

    [ForeignKey("Tsdestination")]
    [InverseProperty("TxnImportJobDtls")]
    public virtual RefPort? TsdestinationNavigation { get; set; }

    [ForeignKey("Pod")]
    [InverseProperty("TxnImportJobDtlsPOD")]
    public virtual RefPort? PodNavigation { get; set; }

    [ForeignKey("IntendedVessel")]
    [InverseProperty("TxnImportJobDtls")]
    public virtual RefVessel? IntendedVesselNavigation { get; set; }
}
