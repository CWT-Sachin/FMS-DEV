using System;
using System.Collections.Generic;
using FMS_DEV.Models;
using FMS_DEV.ModelsAccounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FMS_DEV.DataAccounts;

public partial class FtlcolomboAccountsContext : DbContext
{
    public FtlcolomboAccountsContext()
    {

    }

    public FtlcolomboAccountsContext(DbContextOptions<FtlcolomboAccountsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RefChartOfAcc> RefChartOfAccs { get; set; }

    public virtual DbSet<RefLastNumberAcc> RefLastNumberAcc { get; set; }
    public virtual DbSet<RefAccountTypes> RefAccountTypesACC { get; set; }
    public virtual DbSet<TxnTransactions> TxnTransactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=accountsConString");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        //modelBuilder.Entity<TxnImportJobDtl>(entity =>
        //{
        //    entity.HasOne(d => d.BlstatusNavigation).WithMany(p => p.TxnImportJobDtlBlstatusNavigations).HasConstraintName("FK_Txn_ImportJob_Dtl_Ref_BLStatus");

        //    entity.HasOne(d => d.Bltype).WithMany(p => p.TxnImportJobDtlBltypes).HasConstraintName("FK_Txn_ImportJob_Dtl_Ref_BLTypes");

        //    entity.HasOne(d => d.PackageTypeImportJobNavigation).WithMany(p => p.TxnImportJobDtls).HasConstraintName("FK_Txn_ImportJob_Dtl_Ref_Package");

        //    entity.HasOne(d => d.SalesPersonImportJobdtlNavigation).WithMany(p => p.TxnImportJobDtls).HasConstraintName("FK_Txn_ImportJob_Dtl_Ref_Staff");

        //    entity.HasOne(d => d.ShipperImportJobDtlNavigation).WithMany(p => p.TxnImportJobDtls).HasConstraintName("FK_Txn_ImportJob_Dtl_Ref_Customer");

        //    entity.HasOne(d => d.TsblstatusNavigation).WithMany(p => p.TxnImportJobDtlTsblstatusNavigations).HasConstraintName("FK_Txn_ImportJob_Dtl_Ref_BLStatus_TSBL");

        //    entity.HasOne(d => d.TsdestinationNavigation).WithMany(p => p.TxnImportJobDtls).HasConstraintName("FK_Txn_ImportJob_Dtl_Ref_Ports");
        //});





        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
