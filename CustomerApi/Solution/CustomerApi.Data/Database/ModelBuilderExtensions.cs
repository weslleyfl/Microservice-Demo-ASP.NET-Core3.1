using CustomerApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerApi.Data.Database
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasData(

               new Customer
               {
                   Id = Guid.Parse("d3e3137e-ccc9-488c-9e89-50ba354738c2"),
                   FirstName = "Wolfgang",
                   LastName = "Ofner",
                   Birthday = new DateTime(1989, 11, 23),
                   Age = 30
               },
                new Customer
                {
                    Id = Guid.Parse("4a2f1e35-f527-4136-8b12-138a57e1ba08"),
                    FirstName = "Darth",
                    LastName = "Vader",
                    Birthday = new DateTime(1977, 05, 25),
                    Age = 42
                },
                new Customer
                {
                    Id = Guid.Parse("334feb16-d7bb-4ca9-ab56-f4fadeb88d21"),
                    FirstName = "Son",
                    LastName = "Goku",
                    Birthday = new DateTime(737, 04, 16),
                    Age = 1282
                }

            );
        }

        public static IHost MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<CustomerContext>())
                {
                    try
                    {
                        appContext.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        //Log errors or do anything you think it's needed
                        throw;
                    }
                }
            }
            return host;
        }
    }
}
