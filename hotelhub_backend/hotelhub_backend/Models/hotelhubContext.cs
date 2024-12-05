using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using hotelhub_backend.Models;

namespace hotelhub_backend.Models
{
    public partial class hotelhubContext : DbContext
    {
        public hotelhubContext()
        {
        }

        public hotelhubContext(DbContextOptions<hotelhubContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CancelBookingtb> CancelBookingtbs { get; set; } = null!;
        public virtual DbSet<Facilitytb> Facilitytbs { get; set; } = null!;
        public virtual DbSet<Featurestb> Featurestbs { get; set; } = null!;
        public virtual DbSet<Feedbacktb> Feedbacktbs { get; set; } = null!;
        public virtual DbSet<FestivalDiscount> FestivalDiscounts { get; set; } = null!;
        public virtual DbSet<Historytb> Historytbs { get; set; } = null!;
        public virtual DbSet<Hoteltb> Hoteltbs { get; set; } = null!;
        public virtual DbSet<Reservationtb> Reservationtbs { get; set; } = null!;
        public virtual DbSet<RoomCategorytb> RoomCategorytbs { get; set; } = null!;
        public virtual DbSet<RoomFacilitytb> RoomFacilitytbs { get; set; } = null!;
        public virtual DbSet<RoomFeaturetb> RoomFeaturetbs { get; set; } = null!;
        public virtual DbSet<RoomImagetb> RoomImagetbs { get; set; } = null!;
        public virtual DbSet<Roomtb> Roomtbs { get; set; } = null!;
        public virtual DbSet<Usertb> Usertbs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=127.0.0.1;database=hotelhub;user=root;password=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.32-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<CancelBookingtb>(entity =>
            {
                entity.ToTable("cancel_bookingtb");

                entity.HasIndex(e => e.Revid, "revid");

                entity.HasIndex(e => e.Uid, "uid");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Revid)
                    .HasColumnType("int(11)")
                    .HasColumnName("revid");

                entity.Property(e => e.Uid)
                    .HasColumnType("int(11)")
                    .HasColumnName("uid");

                entity.HasOne(d => d.Rev)
                    .WithMany(p => p.CancelBookingtbs)
                    .HasForeignKey(d => d.Revid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("cancel_bookingtb_ibfk_2");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.CancelBookingtbs)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("cancel_bookingtb_ibfk_1");
            });

            modelBuilder.Entity<Facilitytb>(entity =>
            {
                entity.ToTable("facilitytb");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.FacilityName)
                    .HasMaxLength(100)
                    .HasColumnName("facilityName");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .HasColumnName("image");
            });

            modelBuilder.Entity<Featurestb>(entity =>
            {
                entity.ToTable("featurestb");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.FeatureName)
                    .HasMaxLength(100)
                    .HasColumnName("featureName");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .HasColumnName("image");
            });

            modelBuilder.Entity<Feedbacktb>(entity =>
            {
                entity.ToTable("feedbacktb");

                entity.HasIndex(e => e.Hid, "hid");

                entity.HasIndex(e => e.RoomId, "roomId");

                entity.HasIndex(e => e.UserId, "userId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Comments)
                    .HasColumnType("text")
                    .HasColumnName("comments");

                entity.Property(e => e.Hid)
                    .HasColumnType("int(11)")
                    .HasColumnName("hid");

                entity.Property(e => e.Rating)
                    .HasColumnType("int(11)")
                    .HasColumnName("rating");

                entity.Property(e => e.ReadStatus)
                    .HasColumnType("int(11)")
                    .HasColumnName("read_status")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.RoomId)
                    .HasColumnType("int(11)")
                    .HasColumnName("roomId");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("userId");

                entity.HasOne(d => d.HidNavigation)
                    .WithMany(p => p.Feedbacktbs)
                    .HasForeignKey(d => d.Hid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("feedbacktb_ibfk_1");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Feedbacktbs)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("feedbacktb_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Feedbacktbs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("feedbacktb_ibfk_3");
            });

            modelBuilder.Entity<FestivalDiscount>(entity =>
            {
                entity.ToTable("festival_discount");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Discount)
                    .HasColumnType("int(11)")
                    .HasColumnName("discount");

                entity.Property(e => e.Fesdate).HasColumnType("date").HasColumnName("fesdate");

                entity.Property(e => e.Festname)
                    .HasMaxLength(100)
                    .HasColumnName("festname");
            });

            modelBuilder.Entity<Historytb>(entity =>
            {
                entity.ToTable("historytb");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(100)
                    .HasColumnName("category_name");

                entity.Property(e => e.CheckIn).HasColumnType("date").HasColumnName("checkIn");

                entity.Property(e => e.CheckOut).HasColumnType("date").HasColumnName("checkOut");

                entity.Property(e => e.HotelName)
                    .HasMaxLength(100)
                    .HasColumnName("hotel_name");

                entity.Property(e => e.Rent)
                    .HasColumnType("int(11)")
                    .HasColumnName("rent");

                entity.Property(e => e.UserName)
                    .HasMaxLength(100)
                    .HasColumnName("user_name");
            });

            modelBuilder.Entity<HotelCitytb>(entity =>
            {
                entity.ToTable("hotel_citytb");

                entity.HasIndex(e => e.Hid, "fk-hid");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.City)
                    .HasMaxLength(255)
                    .HasColumnName("city");

                entity.Property(e => e.Hid)
                    .HasColumnType("int(11)")
                    .HasColumnName("hid");

                entity.HasOne(d => d.HidNavigation)
                    .WithMany(p => p.HotelCitytbs)
                    .HasForeignKey(d => d.Hid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk-hid");
            });

            modelBuilder.Entity<Hoteltb>(entity =>
            {
                entity.ToTable("hoteltb");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.City)
                    .HasMaxLength(100)
                    .HasColumnName("city");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.Hname)
                    .HasMaxLength(255)
                    .HasColumnName("hname");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .HasColumnName("image");

                entity.Property(e => e.IsApproved)
                    .HasColumnType("int(11)")
                    .HasColumnName("is_approved");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");
            });

            modelBuilder.Entity<Reservationtb>(entity =>
            {
                entity.ToTable("reservationtb");

                entity.HasIndex(e => e.Hid, "hid");

                entity.HasIndex(e => e.RoomId, "roomId");

                entity.HasIndex(e => e.UserId, "userId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.BookingStatus)
                   .HasColumnType("int(11)")
                   .HasColumnName("booking_status");

                entity.Property(e => e.CheckIn).HasColumnType("date").HasColumnName("checkIn");

                entity.Property(e => e.CheckOut).HasColumnType("date").HasColumnName("checkOut");

                entity.Property(e => e.Hid)
                    .HasColumnType("int(11)")
                    .HasColumnName("hid");

                entity.Property(e => e.Rent)
                    .HasColumnType("int(11)")
                    .HasColumnName("rent");

                entity.Property(e => e.RoomId)
                    .HasColumnType("int(11)")
                    .HasColumnName("roomId");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("userId");

                entity.HasOne(d => d.HidNavigation)
                    .WithMany(p => p.Reservationtbs)
                    .HasForeignKey(d => d.Hid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("reservationtb_ibfk_1");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Reservationtbs)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("reservationtb_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reservationtbs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("reservationtb_ibfk_3");
            });

            modelBuilder.Entity<RoomCategorytb>(entity =>
            {
                entity.ToTable("room_categorytb");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(100)
                    .HasColumnName("categoryName");
            });

            modelBuilder.Entity<RoomFacilitytb>(entity =>
            {
                entity.ToTable("room_facilitytb");

                entity.HasIndex(e => e.FacilityId, "facilityId");

                entity.HasIndex(e => e.RoomId, "roomId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.FacilityId)
                    .HasColumnType("int(11)")
                    .HasColumnName("facilityId");

                entity.Property(e => e.RoomId)
                    .HasColumnType("int(11)")
                    .HasColumnName("roomId");

                entity.HasOne(d => d.Facility)
                    .WithMany(p => p.RoomFacilitytbs)
                    .HasForeignKey(d => d.FacilityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("room_facilitytb_ibfk_1");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomFacilitytbs)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("room_facilitytb_ibfk_2");
            });

            modelBuilder.Entity<RoomFeaturetb>(entity =>
            {
                entity.ToTable("room_featuretb");

                entity.HasIndex(e => e.FeatureId, "featureId");

                entity.HasIndex(e => e.RoomId, "roomId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.FeatureId)
                    .HasColumnType("int(11)")
                    .HasColumnName("featureId");

                entity.Property(e => e.RoomId)
                    .HasColumnType("int(11)")
                    .HasColumnName("roomId");

                entity.HasOne(d => d.Feature)
                    .WithMany(p => p.RoomFeaturetbs)
                    .HasForeignKey(d => d.FeatureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("room_featuretb_ibfk_1");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomFeaturetbs)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("room_featuretb_ibfk_2");
            });

            modelBuilder.Entity<RoomImagetb>(entity =>
            {
                entity.ToTable("room_imagetb");

                entity.HasIndex(e => e.Roomid, "roomid");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .HasColumnName("image");

                entity.Property(e => e.Roomid)
                    .HasColumnType("int(11)")
                    .HasColumnName("roomid");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomImagetbs)
                    .HasForeignKey(d => d.Roomid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("room_imagetb_ibfk_1");
            });

            modelBuilder.Entity<Roomtb>(entity =>
            {
                entity.ToTable("roomtb");

                entity.HasIndex(e => e.FestivalId, "festivalId");

                entity.HasIndex(e => e.Hid, "hid");

                entity.HasIndex(e => e.Roomcategoryid, "roomcategoryid");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.ActiveStatus)
                    .HasColumnType("int(11)")
                    .HasColumnName("active_status");

                entity.Property(e => e.AdultCapacity)
                    .HasColumnType("int(11)")
                    .HasColumnName("adult_capacity");

                entity.Property(e => e.ChildrenCapacity)
                    .HasColumnType("int(11)")
                    .HasColumnName("children_capacity");

                entity.Property(e => e.Discount)
                    .HasColumnType("int(11)")
                    .HasColumnName("discount");

                entity.Property(e => e.FestivalId)
                    .HasColumnType("int(11)")
                    .HasColumnName("festivalId");

                entity.Property(e => e.Hid)
                    .HasColumnType("int(11)")
                    .HasColumnName("hid");

                entity.Property(e => e.Quantity)
                    .HasColumnType("int(11)")
                    .HasColumnName("quantity");

                entity.Property(e => e.Rent)
                    .HasColumnType("int(11)")
                    .HasColumnName("rent");

                entity.Property(e => e.Roomcategoryid)
                    .HasColumnType("int(11)")
                    .HasColumnName("roomcategoryid");

                entity.HasOne(d => d.Festival)
                    .WithMany(p => p.Roomtbs)
                    .HasForeignKey(d => d.FestivalId)
                    .HasConstraintName("roomtb_ibfk_3");

                entity.HasOne(d => d.HidNavigation)
                    .WithMany(p => p.Roomtbs)
                    .HasForeignKey(d => d.Hid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("roomtb_ibfk_2");

                entity.HasOne(d => d.Roomcategory)
                    .WithMany(p => p.Roomtbs)
                    .HasForeignKey(d => d.Roomcategoryid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("roomtb_ibfk_1");
            });

            modelBuilder.Entity<Usertb>(entity =>
            {
                entity.ToTable("usertb");

                entity.HasIndex(e => e.Email, "email")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.City)
                    .HasMaxLength(100)
                    .HasColumnName("city");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.Gender)
                    .HasMaxLength(255)
                    .HasColumnName("gender");

                entity.Property(e => e.Mno)
                    .HasMaxLength(15)
                    .HasColumnName("mno");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public DbSet<hotelhub_backend.Models.HotelCitytb>? HotelCitytb { get; set; }
    }
}
