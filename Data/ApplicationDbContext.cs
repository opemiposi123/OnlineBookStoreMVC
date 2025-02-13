﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineBookStoreMVC.Entities;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ForgotPasswordCode> ForgotPasswordCodes { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Delivery> Deliveries { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        // Configure the one-to-many relationship between Address and Order
        modelBuilder.Entity<Order>()
          .HasOne(o => o.Address)
          .WithMany(a => a.Orders)
          .HasForeignKey(o => o.AddressId)
          .OnDelete(DeleteBehavior.Restrict);

    }
}

