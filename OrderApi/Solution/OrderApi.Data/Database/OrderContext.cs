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
            //var orders = new[]
            //{
            //    new Order()
            //    {
            //        Id = Guid.Parse("9f35b48d-cb87-4783-bfdb-21e36012930a"),
            //        OrderState = OrderState.PaidOrder,
            //        CustomerGuid = Guid.Parse("d3e3137e-ccc9-488c-9e89-50ba354738c2"),
            //        CustomerFullName = "Wolfgang Ofner"
            //    },
            //    new Order
            //    {
            //        Id = Guid.Parse("bffcf83a-0224-4a7c-a278-5aae00a02c1e"),
            //        OrderState = OrderState.PaidOrder,
            //        CustomerGuid = Guid.Parse("4a2f1e35-f527-4136-8b12-138a57e1ba08"),
            //        CustomerFullName = "Darth Vader"
            //    },
            //    new Order
            //    {
            //        Id = Guid.Parse("58e5cd7d-856b-4224-bdff-bd8f85bf5a6d"),
            //        OrderState = OrderState.NotPaidOrder,
            //        CustomerGuid = Guid.Parse("334feb16-d7bb-4ca9-ab56-f4fadeb88d21"),
            //        CustomerFullName = "Son Goku"
            //    }
            //};

            //Orders.AddRange(orders);
            //SaveChanges();

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
