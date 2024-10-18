using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMS_DEV.Models
{
    [Table("Txn_PrePandLHD")]
    public class TxnPrePandLHD
    {
        [Key]
        [StringLength(20)]
        public string PrePnLNo { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? Date { get; set; }

        [StringLength(20)]
        public string? StufPlnNo { get; set; }

        [StringLength(20)]
        public string? VesselID { get; set; }

        [StringLength(20)]
        public string? PODId { get; set; }

        [StringLength(20)]
        public string? ShippingLineID { get; set; }

        [StringLength(20)]
        public string? Voyage { get; set; }

        [StringLength(20)]
        public string? FDNId { get; set; }

        [StringLength(20)]
        public string? AgentId { get; set; }

        [StringLength(20)]
        public string? ContainerSizeId { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? CBM { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? KG { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? LocalNoOfBL { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TSNoOfBL { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TotCBM { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? BoxRateUSD { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPCostACEStuffing { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPCostRework { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPCostPORTStuffing { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPCostSLPA { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPCostTSImpCost { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPCostMCC { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPCostAMJ { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPCostTotalExportCostColombo { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? INCFstTSRateRcvdFromPOLTS { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? INCFstLocalRateRcvdLocalShipper { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? INCFstFreightCollectAmtNominations { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? INCFstTSRateRcvdFromPOLTSBL { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPHndDestuffingCost { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPHndLinearCost { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPHndHandlingFee { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? INCHndDestinationChargesCollectionBL { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPRbtTSCost { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPRbtDoorCost { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPRbtDestinationCharges { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPRbtFirstLegCost { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPRbtTotalRebateSystemCost { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPTruckingOnCarriageCost { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? EXPOtherExpensesTotal { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? INCRbtRemateForCBM { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? INCRbtRemateForBL { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? INCRbtLess { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? INCRbtDiffOfDestinationCharges { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? INCRbtISF { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? INCRbtLSS { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? INCRbtPSS { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? INCOtherIncomeTotal { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TotalExpenses { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TotalIncome { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? PandL { get; set; }
    }
}
