//using System;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata;
//using Epracownik.Models;

//#nullable disable

//namespace Epracownik.Data
//{
//    public partial class AppDbContext : DbContext
//    {

//        public AppDbContext(DbContextOptions<AppDbContext> options)
//            : base(options)
//        {
//        }


//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//                optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Admin\\Desktop\\Projekt\\CarWise\\CarWise\\CarWiseDB.mdf;Integrated Security=True");
//            }
//        }


//    }
//}
