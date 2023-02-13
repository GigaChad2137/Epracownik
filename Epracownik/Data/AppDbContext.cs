using System;
using System.IO;
using Epracownik.Models;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Epracownik.Data
{
    public partial class AppDbContext : DbContext
    {
        
        public AppDbContext()
        {
        }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<InformacjePersonalne> InformacjePersonalnes { get; set; }
        public virtual DbSet<Praca> Pracas { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserWnioski> UserWnioskis { get; set; }
        public virtual DbSet<Wiadomosci> Wiadomoscis { get; set; }
        public virtual DbSet<Wnioski> Wnioskis { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = Directory.GetCurrentDirectory();
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer($@"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={path}\baza_projekt.mdf;Integrated Security=True");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<InformacjePersonalne>(entity =>
            {
                entity.HasKey(e => e.IdPracownika)
                    .HasName("PK__informac__E9472F1CC7176887");

                entity.ToTable("informacje_personalne");

                entity.Property(e => e.IdPracownika)
                    .ValueGeneratedNever()
                    .HasColumnName("Id_pracownika");

                entity.Property(e => e.DataZatrudnienia)
                    .HasColumnType("date")
                    .HasColumnName("Data_zatrudnienia")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DniUrlopowe)
                    .HasColumnName("Dni_urlopowe")
                    .HasDefaultValueSql("((30))");

                entity.Property(e => e.Imie)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nazwisko)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zarobki).HasDefaultValueSql("((3000))");

                entity.HasOne(d => d.IdPracownikaNavigation)
                    .WithOne(p => p.InformacjePersonalne)
                    .HasForeignKey<InformacjePersonalne>(d => d.IdPracownika)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__informacj__Id_pr__0F624AF8");
            });

            modelBuilder.Entity<Praca>(entity =>
            {
                entity.ToTable("praca");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CzyPracuje)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Czy_pracuje")
                    .HasDefaultValueSql("('Poza Praca')");

                entity.Property(e => e.Data)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DataRozpoczecia)
                    .HasColumnType("datetime")
                    .HasColumnName("Data_rozpoczecia");

                entity.Property(e => e.DataZakonczenia)
                    .HasColumnType("datetime")
                    .HasColumnName("Data_zakonczenia");

                entity.Property(e => e.IdPracownika).HasColumnName("Id_pracownika");

                entity.HasOne(d => d.IdPracownikaNavigation)
                    .WithMany(p => p.Pracas)
                    .HasForeignKey(d => d.IdPracownika)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__praca__Id_pracow__04E4BC85");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.HasIndex(e => e.Role1, "UQ__roles__863D214820CB32A2")
                    .IsUnique();

                entity.Property(e => e.Role1)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("role");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.Username, "UQ__users__F3DBC57213899EFB")
                    .IsUnique();

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.IdUser)
                    .HasName("PK__user_rol__D2D1463798513419");

                entity.ToTable("user_roles");

                entity.Property(e => e.IdUser)
                    .ValueGeneratedNever()
                    .HasColumnName("id_user");

                entity.Property(e => e.IdRole).HasColumnName("id_role");

                entity.HasOne(d => d.IdRoleNavigation)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.IdRole)
                    .HasConstraintName("FK__user_role__id_ro__412EB0B6");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithOne(p => p.UserRole)
                    .HasForeignKey<UserRole>(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__user_role__id_us__403A8C7D");
            });

            modelBuilder.Entity<UserWnioski>(entity =>
            {
                entity.ToTable("user_wnioski");

                entity.Property(e => e.DataRozpoczecia)
                    .HasColumnType("date")
                    .HasColumnName("Data_rozpoczecia");

                entity.Property(e => e.DataZakonczenia)
                    .HasColumnType("date")
                    .HasColumnName("Data_zakonczenia");

                entity.Property(e => e.IdPracownika).HasColumnName("id_pracownika");

                entity.Property(e => e.IdWniosku).HasColumnName("id_wniosku");

                entity.Property(e => e.Kwota).HasColumnName("kwota");

                entity.Property(e => e.NotiC).HasColumnName("noti_c");

                entity.Property(e => e.Notka).IsUnicode(false);

                entity.Property(e => e.StatusWniosku).HasColumnName("Status_Wniosku");

                entity.HasOne(d => d.IdPracownikaNavigation)
                    .WithMany(p => p.UserWnioskis)
                    .HasForeignKey(d => d.IdPracownika)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__user_wnio__id_pr__625A9A57");

                entity.HasOne(d => d.IdWnioskuNavigation)
                    .WithMany(p => p.UserWnioskis)
                    .HasForeignKey(d => d.IdWniosku)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__user_wnio__id_wn__634EBE90");
            });

            modelBuilder.Entity<Wiadomosci>(entity =>
            {
                entity.ToTable("wiadomosci");

                entity.Property(e => e.CzyPrzeczytane)
                    .IsRequired()
                    .HasColumnName("czy_przeczytane")
                    .HasDefaultValueSql("('FALSE')");

                entity.Property(e => e.IdNadawcy).HasColumnName("id_nadawcy");

                entity.Property(e => e.IdOdbiorcy).HasColumnName("id_odbiorcy");

                entity.Property(e => e.Wiadomosc).IsUnicode(false);

                entity.HasOne(d => d.IdNadawcyNavigation)
                    .WithMany(p => p.WiadomosciIdNadawcyNavigations)
                    .HasForeignKey(d => d.IdNadawcy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__wiadomosc__id_na__3C34F16F");

                entity.HasOne(d => d.IdOdbiorcyNavigation)
                    .WithMany(p => p.WiadomosciIdOdbiorcyNavigations)
                    .HasForeignKey(d => d.IdOdbiorcy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__wiadomosc__id_od__3D2915A8");
            });

            modelBuilder.Entity<Wnioski>(entity =>
            {
                entity.ToTable("wnioski");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TypWniosku)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("typ_wniosku");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
