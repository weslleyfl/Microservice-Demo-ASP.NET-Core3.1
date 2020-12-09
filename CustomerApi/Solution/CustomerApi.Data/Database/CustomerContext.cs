using CustomerApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace CustomerApi.Data.Database
{
    public class CustomerContext : DbContext
    {

        public DbSet<Customer> Customers { get; set; }
             
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
        {
           
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>((entity) =>
            {

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Birthday).HasColumnType("Date");
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();

            });

            // Customer data
            modelBuilder.Seed();
        }

    }
}
