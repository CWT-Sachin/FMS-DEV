using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Models;

[Table("Txn_Booking_Exp_CargoDtl")]
public partial class TxnBookingExpCargoDtl
{
   
    [StringLength(20)]
    public string BookingNo { get; set; } = null!;

    
    [StringLength(20)]
    public string PORefNumber { get; set; } = null!;

    

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

    

    [ForeignKey("PackageType")]
    [InverseProperty("TxnBookingExpCargoDtls")]
    public virtual RefPackage? PackageNavigationExpBoookingCargoDtls { get; set; }

}
