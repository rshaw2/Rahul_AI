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
            optionsBuilder.UseSqlServer("Data Source=codezen1.database.windows.net;Initial Catalog=rahul-ai;Persist Security Info=True;user id=test;password=RahulAI@789;Integrated Security=false;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInRole>().HasKey(a => a.Id);
            modelBuilder.Entity<UserToken>().HasKey(a => a.Id);
            modelBuilder.Entity<Entity>().HasKey(a => a.Id);
            modelBuilder.Entity<Tenant>().HasKey(a => a.Id);
            modelBuilder.Entity<User>().HasKey(a => a.Id);
            modelBuilder.Entity<Role>().HasKey(a => a.Id);
            modelBuilder.Entity<Customer>().HasKey(a => a.CustomerId);
            modelBuilder.Entity<Order>().HasKey(a => a.OrderID);
            modelBuilder.Entity<OrderLine>().HasKey(a => a.OrderLineId);
            modelBuilder.Entity<Product>().HasKey(a => a.ProductId);
            modelBuilder.Entity<OrderStatus>().HasKey(a => a.OrderStatusId);
            modelBuilder.Entity<Country>().HasKey(a => a.Name);
            modelBuilder.Entity<Sales>().HasKey(a => a.SalesId);
            modelBuilder.Entity<Dictionary>().HasKey(a => a.Id);
            modelBuilder.Entity<UserInRole>().HasOne(a => a.Tenant).WithMany(b => b.UserInRoles).HasForeignKey(c => c.TenantId);
            modelBuilder.Entity<UserInRole>().HasOne(a => a.Role).WithMany(b => b.UserInRoles).HasForeignKey(c => c.RoleId);
            modelBuilder.Entity<UserInRole>().HasOne(a => a.User).WithMany(b => b.UserInRoles).HasForeignKey(c => c.UserId);
            modelBuilder.Entity<UserInRole>().HasOne(a => a.CreatedByUser).WithMany().HasForeignKey(c => c.CreatedBy);
            modelBuilder.Entity<UserInRole>().HasOne(a => a.UpdatedByUser).WithMany().HasForeignKey(c => c.UpdatedBy);
            modelBuilder.Entity<UserToken>().HasOne(a => a.Tenant).WithMany(b => b.UserTokens).HasForeignKey(c => c.TenantId);
            modelBuilder.Entity<UserToken>().HasOne(a => a.User).WithMany(b => b.UserTokens).HasForeignKey(c => c.UserId);
            modelBuilder.Entity<Entity>().HasOne(a => a.Tenant).WithMany(b => b.Entitys).HasForeignKey(c => c.TenantId);
            modelBuilder.Entity<Entity>().HasOne(a => a.CreatedByUser).WithMany(b => b.Entitys).HasForeignKey(c => c.CreatedBy);
            modelBuilder.Entity<Entity>().HasOne(a => a.UpdatedByUser).WithMany().HasForeignKey(c => c.UpdatedBy);
            modelBuilder.Entity<User>().HasOne(a => a.Tenant).WithMany(b => b.Users).HasForeignKey(c => c.TenantId);
            modelBuilder.Entity<Role>().HasOne(a => a.Tenant).WithMany(b => b.Roles).HasForeignKey(c => c.TenantId);
            modelBuilder.Entity<Role>().HasOne(a => a.CreatedByUser).WithMany(b => b.Roles).HasForeignKey(c => c.CreatedBy);
            modelBuilder.Entity<Role>().HasOne(a => a.UpdatedByUser).WithMany().HasForeignKey(c => c.UpdatedBy);
            modelBuilder.Entity<Customer>().HasOne(a => a.CountryNameCountry).WithMany(b => b.Customers).HasForeignKey(c => c.CountryName);
            modelBuilder.Entity<Order>().HasOne(a => a.Customer).WithMany(b => b.Orders).HasForeignKey(c => c.CustomerId);
            modelBuilder.Entity<Order>().HasOne(a => a.OrderStatus).WithMany(b => b.Orders).HasForeignKey(c => c.OrderStatusId);
            modelBuilder.Entity<OrderLine>().HasOne(a => a.Order).WithMany(b => b.OrderLines).HasForeignKey(c => c.OrderId);
            modelBuilder.Entity<OrderLine>().HasOne(a => a.Product).WithMany(b => b.OrderLines).HasForeignKey(c => c.ProductId);
            modelBuilder.Entity<Sales>().HasOne(a => a.Product).WithMany(b => b.Saless).HasForeignKey(c => c.ProductId);
            modelBuilder.Entity<Dictionary>().HasOne(a => a.ParentIdDictionary).WithMany().HasForeignKey(c => c.ParentId);
        }

        public DbSet<UserInRole> UserInRole { get; set; }
        public DbSet<UserToken> UserToken { get; set; }
        public DbSet<Entity> Entity { get; set; }
        public DbSet<Tenant> Tenant { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
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