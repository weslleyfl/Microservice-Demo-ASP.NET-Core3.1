using Microsoft.EntityFrameworkCore;
using OrderApi.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderApi.Data.Database
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasData(

                new Order()
                {
                    Id = Guid.Parse("9f35b48d-cb87-4783-bfdb-21e36012930a"),
                    OrderState = OrderState.PaidOrder,
                    CustomerGuid = Guid.Parse("d3e3137e-ccc9-488c-9e89-50ba354738c2"),
                    CustomerFullName = "Wolfgang Ofner"
                },
                new Order
                {
                    Id = Guid.Parse("bffcf83a-0224-4a7c-a278-5aae00a02c1e"),
                    OrderState = OrderState.PaidOrder,
                    CustomerGuid = Guid.Parse("4a2f1e35-f527-4136-8b12-138a57e1ba08"),
                    CustomerFullName = "Darth Vader"
                },
                new Order
                {
                    Id = Guid.Parse("58e5cd7d-856b-4224-bdff-bd8f85bf5a6d"),
                    OrderState = OrderState.NotPaidOrder,
                    CustomerGuid = Guid.Parse("334feb16-d7bb-4ca9-ab56-f4fadeb88d21"),
                    CustomerFullName = "Son Goku"
                }

            );
        }
    }
}
