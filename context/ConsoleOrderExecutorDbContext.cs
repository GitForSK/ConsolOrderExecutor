using ConsoleOrderExecutor.Orders.Models;
using ConsoleOrderExecutor.Products.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleOrderExecutor.context;

public partial class ConsoleOrderExecutorDbContext : DbContext
{
    private string _connectionString;
    public ConsoleOrderExecutorDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public ConsoleOrderExecutorDbContext(string connectionString, DbContextOptions<ConsoleOrderExecutorDbContext> options)
        : base(options)
    {
        _connectionString = connectionString;
    }

    public virtual DbSet<AppOrder> AppOrders { get; set; }

    public virtual DbSet<AppProduct> AppProducts { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<PaymentOption> PaymentOptions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__App_Orde__46596229CFC5A0A8");

            entity.ToTable("App_Order");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.DeliveryAddress)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("delivery_address");
            entity.Property(e => e.IsCompany).HasColumnName("isCompany");
            entity.Property(e => e.PaymentOptionId).HasColumnName("payment_option_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.PaymentOption).WithMany(p => p.AppOrders)
                .HasForeignKey(d => d.PaymentOptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Payment_Option");

            entity.HasOne(d => d.Status).WithMany(p => p.AppOrders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Order_status");
        });

        modelBuilder.Entity<AppProduct>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__App_Prod__47027DF5CB98C2A4");

            entity.ToTable("App_Product");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductEan)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("product_ean");
            entity.Property(e => e.ProductName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("product_name");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("PK__Order_Pr__022945F64587C9EA");

            entity.ToTable("Order_Product");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(30, 2)")
                .HasColumnName("price");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Id");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Id");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Order_St__3683B531140FE2BC");

            entity.ToTable("Order_Status");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<PaymentOption>(entity =>
        {
            entity.HasKey(e => e.PaymentOptionId).HasName("PK__Payment___331ED1821D459358");

            entity.ToTable("Payment_Option");

            entity.Property(e => e.PaymentOptionId).HasColumnName("payment_option_id");
            entity.Property(e => e.OptionName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("option_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
