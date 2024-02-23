using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RahulAI.Model;
using System;

namespace RahulAI.Data
{
    public class RahulAIContext : DbContext
    {
        protected override void OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=codezen1.database.windows.net;Initial Catalog=rahul-ai;Persist Security Info=True;user id=test;password=Test@123;Integrated Security=false;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasKey(a => a.CustomerId);
            modelBuilder.Entity<Order>().HasKey(a => a.OrderID);
            modelBuilder.Entity<OrderLine>().HasKey(a => a.OrderLineId);
            modelBuilder.Entity<Product>().HasKey(a => a.ProductId);
            modelBuilder.Entity<OrderStatus>().HasKey(a => a.OrderStatusId);
            modelBuilder.Entity<Country>().HasKey(a => a.Name);
            modelBuilder.Entity<Sales>().HasKey(a => a.SalesId);
            modelBuilder.Entity<Dictionary>().HasKey(a => a.Id);
            modelBuilder.Entity<Customer>().HasOne(a => a.CountryNameCountry).WithMany(b => b.Customers).HasForeignKey(c => c.CountryName);
            modelBuilder.Entity<Order>().HasOne(a => a.Customer).WithMany(b => b.Orders).HasForeignKey(c => c.CustomerId);
            modelBuilder.Entity<Order>().HasOne(a => a.OrderStatus).WithMany(b => b.Orders).HasForeignKey(c => c.OrderStatusId);
            modelBuilder.Entity<OrderLine>().HasOne(a => a.Order).WithMany(b => b.OrderLines).HasForeignKey(c => c.OrderId);
            modelBuilder.Entity<OrderLine>().HasOne(a => a.Product).WithMany(b => b.OrderLines).HasForeignKey(c => c.ProductId);
            modelBuilder.Entity<Sales>().HasOne(a => a.Product).WithMany(b => b.Saless).HasForeignKey(c => c.ProductId);
            modelBuilder.Entity<Dictionary>().HasOne(a => a.ParentIdDictionary).WithMany().HasForeignKey(c => c.ParentId);
        }

        public DbSet<Customer> Customer { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderLine> OrderLine { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<Dictionary> Dictionary { get; set; }
    }
}