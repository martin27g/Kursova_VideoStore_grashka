using Kursova_VideoStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Videoteka.Models;

public partial class VideotekaContext : IdentityDbContext<IdentityUser>
{
    public VideotekaContext(DbContextOptions<VideotekaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Film> Films { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Film>(entity =>
        {
            entity.ToTable("Films");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");

            entity.HasOne(d => d.Customer)
                  .WithMany(p => p.Orders)
                  .HasForeignKey(d => d.CustomerID);

            entity.HasOne(d => d.Employee)
                  .WithMany(p => p.Orders)
                  .HasForeignKey(d => d.EmployeeID);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("OrderDetails");

            entity.HasOne(d => d.Order)
                  .WithMany(p => p.OrderDetails)
                  .HasForeignKey(d => d.OrderID);

            entity.HasOne(d => d.Film)
                  .WithMany(p => p.OrderDetails)
                  .HasForeignKey(d => d.FilmID);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
