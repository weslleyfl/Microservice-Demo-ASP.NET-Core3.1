using Microsoft.EntityFrameworkCore;
using OrderApi.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderApi.Data.Database
{
    public class OrderContext : DbContext
    {

        public DbSet<Order> Orders { get; set; }

        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CustomerFullName).IsRequired();

                // Tratar o Enum
                // https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions
                entity.Property(e => e.OrderState)
                .HasConversion(
                    e => (int)e,
                    e => (OrderState)e);

                   //     entity.Property(e => e.MyEnumField)
                   // .HasMaxLength(50)
                   // .HasConversion(
                   //    v => v.ToString(),
                   //    v => (MyEnum)Enum.Parse(typeof(MyEnum), v))
                   //    .IsUnicode(false);

            });

            // Popular os dados dos pedidos iniciais
            modelBuilder.Seed();

        }
    }
}
