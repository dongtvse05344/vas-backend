using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VAS.Model;

namespace VAS.Data
{
    public class VASDbContext : IdentityDbContext<MyUser>
    {
        public VASDbContext() : base((new DbContextOptionsBuilder())
            .UseLazyLoadingProxies()
            .UseSqlServer(@"Server=202.78.227.89;Database=VASDB;user id=sa;password=an@0906782333;Trusted_Connection=True;Integrated Security=false;")
            .Options)
        {

        }

        public DbSet<SignalRConnection> signalRConnections { get; set; }
        public DbSet<DoctorBasic> DoctorBasics { get; set; }
        public DbSet<DoctorPro> DoctorPros { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Scheduling> Schedulings { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Family> Families { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region MyUser 
            builder.Entity<MyUser>()
               .HasOne(_ => _.Doctor).WithOne(_ => _.MyUser)
               .HasForeignKey<DoctorBasic>(_ => _.Id);
            builder.Entity<MyUser>()
                .HasOne(_ => _.Nurse).WithOne(_ => _.MyUser)
                .HasForeignKey<Nurse>(_ => _.Id);
            #endregion

            #region Doctor
            builder.Entity<DoctorBasic>()
               .HasOne(_ => _.DoctorPro).WithOne(_ => _.DoctorBasic)
               .HasForeignKey<DoctorPro>(_ => _.Id);
            builder.Entity<DoctorBasic>().
                    HasIndex(_ => _.HisCode)
                    .IsUnique();
            #endregion


            #region Room
            builder.Entity<Room>()
                .HasIndex(_ => _.Number)
                .IsUnique();
            builder.Entity<Room>()
                .HasIndex(_ => _.HisCode)
                .IsUnique();
			#endregion
			#region Block
			builder.Entity<Block>()
			.HasIndex(p => new { p.DoctorId, p.StartTime, p.Date}).IsUnique();
			builder.Entity<Block>()
			.HasIndex(p => new { p.RoomId, p.StartTime, p.Date}).IsUnique();
            #endregion
            #region Family
            builder.Entity<Family>().HasKey(sc => new { sc.CustomerId, sc.MyUserId });
            #endregion
            #region SpecialityDoctor
            builder.Entity<SpecialityDoctor>().HasKey(sc => new { sc.DoctorBasicId, sc.SpecialityId });
            #endregion

            base.OnModelCreating(builder);

        }

        public void Commit()
        {
            base.SaveChanges();
        }
    }
}
