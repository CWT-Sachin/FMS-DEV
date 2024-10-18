using System;
using System.Collections.Generic;
using FMS_DEV.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.Data;

public partial class FtlcolombOperationContext : DbContext
{
    public FtlcolombOperationContext()
    {

    }

    public FtlcolombOperationContext(DbContextOptions<FtlcolombOperationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RefAgent> RefAgents { get; set; }

    public virtual DbSet<RefContainerSize> RefContainerSizes { get; set; }

    public virtual DbSet<RefCustomer> RefCustomers { get; set; }

    public virtual DbSet<RefChargeItemAcc> RefChargeItemsAcc { get; set; }

    public virtual DbSet<RefDepartment> RefDepartments { get; set; }

    public virtual DbSet<RefDepot> RefDepots { get; set; }

    public virtual DbSet<RefDesignation> RefDesignations { get; set; }

    public virtual DbSet<RefPackage> RefPackages { get; set; }

    public virtual DbSet<RefPort> RefPorts { get; set; }

    public virtual DbSet<RefSalesPerson> RefSalesPeople { get; set; }

    public virtual DbSet<RefShippingLine> RefShippingLines { get; set; }

    public virtual DbSet<RefStaff> RefStaffs { get; set; }

    public virtual DbSet<RefVessel> RefVessels { get; set; }

    public virtual DbSet<RefAirPort> RefAirPorts { get; set; }
    public virtual DbSet<RefBlTypes> RefBLTypes { get; set; }
    public virtual DbSet<RefBLStatus> RefBLStatus { get; set; }
    public virtual DbSet<RefShipmentTerms> RefShipmentTerms { get; set; }

    public virtual DbSet<RefTerminal> RefTerminal { get; set; }

    public virtual DbSet<RefCountry> RefCountries { get; set; }
    public virtual DbSet<RefStatusMain> RefStatusMain { get; set; }

    public virtual DbSet<RefLastNumber> RefLastNumbers { get; set; }
    public virtual DbSet<TxnBookingExp> TxnBookingExps { get; set; }
    public virtual DbSet<TxnBookingExpCargoDtl> TxnBookingExpCargoDtls { get; set; }

    public virtual DbSet<TxnVoyageSchedul> TxnVoyageScheduls { get; set; }
    public virtual DbSet<TxnStuffingPlanHd> TxnStuffingPlanHds { get; set; }
    public virtual DbSet<TxnStuffingPlanDtl> TxnStuffingPlanDtls { get; set; }

    public virtual DbSet<TxnImportJobCnt> TxnImportJobCnts { get; set; }

    public virtual DbSet<TxnImportJobDtl> TxnImportJobDtls { get; set; }

    public virtual DbSet<TxnImportJobHd> TxnImportJobHds { get; set; }

    public virtual DbSet<TxnExportJobHD> TxnExportJobHds { get; set; }
    public virtual DbSet<TxnExportJobDtl> TxnExportJobDtls { get; set; }

    public virtual DbSet<TxnInvoiceExportDtl> TxnInvoiceExportDtls { get; set; }
    public virtual DbSet<TxnInvoiceExportHd> TxnInvoiceExportHds { get; set; }

    public virtual DbSet<TxnInvoiceFCLHd> TxnInvoiceFCLHds { get; set; }
    public virtual DbSet<TxnInvoiceFCLDtl> TxnInvoiceFCLDtls { get; set; }

    public virtual DbSet<TxnInvoiceImportDtl> TxnInvoiceImportDtls { get; set; }
    public virtual DbSet<TxnInvoiceImportHd> TxnInvoiceImportHds { get; set; }

    public virtual DbSet<TxnCreditNoteFCLDtl> TxnCreditNoteFCLDtls { get; set; }
    public virtual DbSet<TxnCreditNoteFCLHd> TxnCreditNoteFCLHds { get; set; }

    public virtual DbSet<TxnDebitNoteFCLDtl> TxnDebitNoteFCLDtls { get; set; }
    public virtual DbSet<TxnDebitNoteFCLHd> TxnDebitNoteFCLHds { get; set; }

    public virtual DbSet<TxnCreditNoteFCLDtl> TxnCreditNoteExportDtls { get; set; }
    public virtual DbSet<TxnCreditNoteFCLHd> TxnCreditNoteExportHds { get; set; }
    public virtual DbSet<TxnCreditNoteImportDtl> TxnCreditNoteImportDtls { get; set; }
    public virtual DbSet<TxnCreditNoteImportHd> TxnCreditNoteImportHds { get; set; }

    public virtual DbSet<TxnDebitNoteExportDtl> TxnDebitNoteExportDtls { get; set; }
    public virtual DbSet<TxnDebitNoteExportHd> TxnDebitNoteExportHds { get; set; }

    public virtual DbSet<TxnDebitNoteImportDtl> TxnDebitNoteImportDtls { get; set; }
    public virtual DbSet<TxnDebitNoteImportHd> TxnDebitNoteImportHds { get; set; }

    public virtual DbSet<TxnPaymentVoucherExportDtl> TxnPaymentVoucherExportDtls { get; set; }
    public virtual DbSet<TxnPaymentVoucherExportHd> TxnPaymentVoucherExportHds { get; set; }

    public virtual DbSet<TxnPaymentVoucherFCLDtl> TxnPaymentVoucherFCLDtls { get; set; }
    public virtual DbSet<TxnPaymentVoucherFCLHd> TxnPaymentVoucherFCLHds { get; set; }

    public virtual DbSet<TxnPaymentVoucherImportDtl> TxnPaymentVoucherImportDtls { get; set; }
    public virtual DbSet<TxnPaymentVoucherImportHd> TxnPaymentVoucherImportHds { get; set; }

    public DbSet<TxnFCLJob> TxnFCLJobs { get; set; }

    public DbSet<TxnFCLJobContainers> TxnFCLJobContainers { get; set; }

    public DbSet<TxnFCLBL> TxnFCLBLs { get; set; }

    public DbSet<TxnPrePandLHD> TxnPrePandLHds { get; set; }

    public DbSet<TxnPrePandLOtherExpenDtl> TxnPrePandLOtherExpenDtl { get; set; }

    public DbSet<TxnPrePandLOtherIncomDtl> TxnPrePandLOtherIncomDtl { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=constring");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<TxnImportJobDtl>(entity =>
        {
            entity.HasOne(d => d.BlstatusNavigation).WithMany(p => p.TxnImportJobDtlBlstatusNavigations).HasConstraintName("FK_Txn_ImportJob_Dtl_Ref_BLStatus");

            entity.HasOne(d => d.Bltype).WithMany(p => p.TxnImportJobDtlBltypes).HasConstraintName("FK_Txn_ImportJob_Dtl_Ref_BLTypes");

            entity.HasOne(d => d.PackageTypeImportJobNavigation).WithMany(p => p.TxnImportJobDtls).HasConstraintName("FK_Txn_ImportJob_Dtl_Ref_Package");

            entity.HasOne(d => d.SalesPersonImportJobdtlNavigation).WithMany(p => p.TxnImportJobDtls).HasConstraintName("FK_Txn_ImportJob_Dtl_Ref_Staff");

            entity.HasOne(d => d.ShipperImportJobDtlNavigation).WithMany(p => p.TxnImportJobDtls).HasConstraintName("FK_Txn_ImportJob_Dtl_Ref_Customer");

            entity.HasOne(d => d.TsblstatusNavigation).WithMany(p => p.TxnImportJobDtlTsblstatusNavigations).HasConstraintName("FK_Txn_ImportJob_Dtl_Ref_BLStatus_TSBL");

            entity.HasOne(d => d.TsdestinationNavigation).WithMany(p => p.TxnImportJobDtls).HasConstraintName("FK_Txn_ImportJob_Dtl_Ref_Ports");
        });

        modelBuilder.Entity<TxnImportJobHd>(entity =>
        {
            entity.HasOne(d => d.AgentNavigation).WithMany(p => p.TxnImportJobHds).HasConstraintName("FK_Txn_ImportJob_HD_Ref_Agent");

            entity.HasOne(d => d.HandlebyImportJobNavigation).WithMany(p => p.TxnImportJobHds).HasConstraintName("FK_Txn_ImportJob_HD_Ref_Staff_HandleBy");

            entity.HasOne(d => d.ShippingLineNavigation).WithMany(p => p.TxnImportJobHds).HasConstraintName("FK_Txn_ImportJob_HD_Ref_ShippingLine");

            entity.HasOne(d => d.TerminalNavigation).WithMany(p => p.TxnImportJobHds).HasConstraintName("FK_Txn_ImportJob_HD_Ref_Terminal");

            entity.HasOne(d => d.VesselImportJobDtlNavigation).WithMany(p => p.TxnImportJobHds).HasConstraintName("FK_Txn_ImportJob_HD_Ref_Vessel");
        });
        modelBuilder.Entity<TxnExportJobHD>(entity =>
        {
            entity.HasOne(d => d.AgentExportNavigation).WithMany(p => p.TxnExportJobHds).HasConstraintName("FK_Txn_ExportJobHD_Ref_Agent");

            entity.HasOne(d => d.HandlebyExportJobNavigation).WithMany(p => p.TxnExportJobHds).HasConstraintName("FK_Txn_ExportJobHD_Ref_Staff_HandlBy");
            entity.HasOne(d => d.CreatedByExportJobNavigation).WithMany(p => p.TxnExportJobHdsCreatedBy).HasConstraintName("FK_Txn_ExportJobHD_Ref_Staff_CreatedBy");

            entity.HasOne(d => d.ShippingLineExportNavigation).WithMany(p => p.TxnExportJobHds).HasConstraintName("FK_Txn_ExportJobHD_Ref_ShippingLine");
            entity.HasOne(d => d.VesselExportJobDtlNavigation).WithMany(p => p.TxnExportJobHds).HasConstraintName("FK_Txn_ExportJobHD_Ref_Vessel");

            entity.HasOne(d => d.PODExportJobNavigation).WithMany(p => p.TxnExportJobHdsPod).HasConstraintName("FK_Txn_ExportJobHD_Ref_Ports_POD");
            entity.HasOne(d => d.FDNExportJobNavigation).WithMany(p => p.TxnExportJobHds).HasConstraintName("FK_Txn_ExportJobHD_Ref_Ports_FDN");
            entity.HasOne(d => d.JobStatusExportJobNavigation).WithMany(p => p.TxnExportJobHds).HasConstraintName("FK_Txn_ExportJobHD_Ref_StatusMain_JobStatus");
        });


        modelBuilder.Entity<TxnStuffingPlanHd>(entity =>
        {


            entity.HasOne(d => d.AgentStuffingPlanHd).WithMany(p => p.TxnStuffingPlanHds).HasConstraintName("FK_Txn_StuffingPlan_HD_Ref_Agent");

            entity.HasOne(d => d.DepotStuffingPlanHd).WithMany(p => p.TxnStuffingPlanHds).HasConstraintName("FK_Txn_StuffingPlan_HD_Ref_Depot");

            entity.HasOne(d => d.ShippingLine).WithMany(p => p.TxnStuffingPlanHds).HasConstraintName("FK_Txn_StuffingPlan_HD_Ref_ShippingLine");

            entity.HasOne(d => d.Terminal).WithMany(p => p.TxnStuffingPlanHds).HasConstraintName("FK_Txn_StuffingPlan_HD_Ref_Terminal");

            entity.HasOne(d => d.FdnStuffingNavigation).WithMany(p => p.TxnStuffingPlanHdFdnNavigations).HasConstraintName("FK_Txn_StuffingPlan_HD_Ref_Ports_FDN");

            entity.HasOne(d => d.PodStuffingNavigation).WithMany(p => p.TxnStuffingPlanHdPodNavigations).HasConstraintName("FK_Txn_StuffingPlan_HD_Ref_Ports_POD");

            entity.HasOne(d => d.ContainerStuffingNavigation).WithMany(p => p.TxnStuffingPlanHds).HasConstraintName("FK_Txn_StuffingPlan_HD_Ref_ContainerSize");

            entity.HasOne(d => d.PreparedByStuffingNavigation).WithMany(p => p.TxnStuffingPlanHds).HasConstraintName("FK_Txn_StuffingPlan_HD_Ref_Staff_PreparedBy");
            entity.HasOne(d => d.ColoaderNavigation).WithMany(p => p.TxnStuffingPlanHdsCoLoad).HasConstraintName("FK_Txn_StuffingPlan_HD_Ref_ShippingLine_Coloader");

        });
        modelBuilder.Entity<RefStaff>(entity =>
        {
            entity.HasOne(d => d.StaffDeaprtmentNavigation).WithMany(p => p.RefStaffDeaprtmentNavigation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ref_Staff_Ref_Department");
            entity.HasOne(d => d.StaffDesignationNavigation).WithMany(p => p.RefStaffDesignationNavigation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ref_Staff_Ref_Designation");
        });
        modelBuilder.Entity<RefAgent>(entity =>
        {
            entity.HasOne(d => d.Port).WithMany(p => p.RefAgents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ref_Agent_Ref_Ports");
        });

        modelBuilder.Entity<TxnBookingExp>(entity =>
        {
            entity.HasOne(d => d.HandleByNavigation).WithMany(p => p.TxnBookingExpHandleByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Txn_Booking_HandlByExp_Ref_Staff");

            entity.HasOne(d => d.SalesPerson).WithMany(p => p.TxnBookingExpSalesPeople)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Txn_Booking_ExpSalesPerson_Ref_Staff");
            entity.HasOne(d => d.BLTypeIDsNavigation).WithMany(p => p.TxnBookingExps).HasConstraintName("FK_Txn_Booking_Exp_NewRef_BLTypes");
            entity.HasOne(d => d.RefBLStatusIDNavigation).WithMany(p => p.TxnBookingExps).HasConstraintName("FK_Txn_Booking_Exp_Ref_BLStatus");
            entity.HasOne(d => d.RefShipmentTermsNavigation).WithMany(p => p.TxnBookingExps).HasConstraintName("FK_Txn_Booking_Exp_Ref_ShipmentTerms");
            entity.HasOne(d => d.Shipper).WithMany(p => p.TxnBookingExps).HasConstraintName("FK_Txn_Booking_Exp_Ref_Customer");
            entity.HasOne(d => d.ColoaderNavigation).WithMany(p => p.TxnBookingExpsColoaderNavigation).HasConstraintName("FK_Txn_Booking_Exp_Ref_Customer_CoLoader");

            entity.HasOne(d => d.VesselNavigation).WithMany(p => p.TxnBookingExps).HasConstraintName("FK_Txn_Booking_Exp_Ref_Vessel");
            entity.HasOne(d => d.TSVesselNavigation).WithMany(p => p.TxnBookingExpsTsVessel).HasConstraintName("FK_Txn_Booking_Exp_Ref_Vessel_TS_Vessel");
            entity.HasOne(d => d.PackageNavigation).WithMany(p => p.TxnBookingExps).HasConstraintName("FK_Txn_Booking_Exp_Ref_Package");
            entity.HasOne(d => d.PODNavigation).WithMany(p => p.TxnBookingExpsPODNavigation).HasConstraintName("FK_Txn_Booking_POD_Exp_Ref_Ports");
            entity.HasOne(d => d.POLNavigation).WithMany(p => p.TxnBookingExpsPOLNavigation).HasConstraintName("FK_Txn_Booking_POL_Exp_Ref_Ports");
            entity.HasOne(d => d.FDNNavigation).WithMany(p => p.TxnBookingExpsFDNNavigation).HasConstraintName("FK_Txn_Booking_FDN_Exp_Ref_Ports");
            entity.HasOne(d => d.TSDestinationNavigation).WithMany(p => p.TxnBookingExpsTSDestinationNavigation).HasConstraintName("FK_Txn_Booking_Exp_Ref_Ports_TS_Destination");
            entity.HasOne(d => d.TsPolNavigation).WithMany(p => p.TxnBookingExpsTsPolNavigation).HasConstraintName("FK_Txn_Booking_Exp_Ref_Ports_TS_POL");

            entity.HasOne(d => d.AgentIDNominationNavigation).WithMany(p => p.TxnBookingExpsAgentIDNomination).HasConstraintName("FK_Txn_Booking_Exp_AgentNomination_Txn_Booking_Exp");
            //entity.HasOne(d => d.V).WithMany(p => p.TxnBookingExps).HasConstraintName("FK_Txn_Booking_Exp_Txn_VoyageSchedul");
        });

        modelBuilder.Entity<TxnBookingExpCargoDtl>(entity =>
        {
            entity.HasKey(e => new { e.BookingNo, e.PORefNumber });
            entity.HasOne(d => d.PackageNavigationExpBoookingCargoDtls).WithMany(p => p.TxnBookingExpCargoDtls).HasConstraintName("FK_Txn_Booking_Exp_CargoDtl_Ref_Package");
        });

        OnModelCreatingPartial(modelBuilder);
    }

	//protected override void OnModelCreating(ModelBuilder modelBuilder)
	//{
	//	base.OnModelCreating(modelBuilder);

	//	// Your other entity configurations...

	//	modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
	//	{
	//		entity.HasKey(e => e.UserId); // Assuming UserId is the primary key
	//	});
	//}  

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
